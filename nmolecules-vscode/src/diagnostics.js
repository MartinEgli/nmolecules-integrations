'use strict';

const path = require('node:path');
const { spawn } = require('node:child_process');
const { TextDecoder } = require('util');

const NMOLECULES_DIAGNOSTIC_PATTERN = /^(.+?)\((\d+),(\d+)(?:,\d+,\d+)?\):\s(error|warning|fehler|warnung)\s(XMolecules[A-Za-z0-9]+):\s(.+?)(?:\s\[(.+)\])?$/i;
const SEARCH_EXCLUDE_GLOB = '**/{.git,.tmp,node_modules,.vscode-test,bin,obj}/**';
const PROJECT_INCLUDE_PATTERN = /<Compile\b[^>]*\bInclude\s*=\s*"([^"]+)"/gi;
const SUPPORTED_SOURCE_EXTENSIONS = new Set(['.cs', '.fs']);
let lastDiagnosticsReport;

function getDiagnosticsTarget(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('diagnosticsTarget', '');
}

function getDiagnosticsBuildArguments(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('diagnosticsBuildArguments', ['-v', 'minimal']);
}

function getWorkspaceFolderForPath(vscodeApi, filePath) {
  const folders = vscodeApi.workspace.workspaceFolders ?? [];

  return folders
    .filter((folder) => filePath.localeCompare(folder.uri.fsPath, undefined, { sensitivity: 'accent' }) === 0
      || filePath.startsWith(`${folder.uri.fsPath}${path.sep}`))
    .sort((left, right) => right.uri.fsPath.length - left.uri.fsPath.length)[0];
}

function sortTargetsBySpecificity(targetUris) {
  return [...targetUris].sort((left, right) => {
    const leftDepth = left.fsPath.split(/[\\/]+/).length;
    const rightDepth = right.fsPath.split(/[\\/]+/).length;

    if (leftDepth !== rightDepth) {
      return leftDepth - rightDepth;
    }

    return left.fsPath.localeCompare(right.fsPath);
  });
}

function getActiveSourceDocumentPath(vscodeApi) {
  const document = vscodeApi.window?.activeTextEditor?.document;
  const filePath = document?.uri?.fsPath;

  if (!filePath) {
    return undefined;
  }

  if (document.languageId === 'csharp' || document.languageId === 'fsharp') {
    return filePath;
  }

  return SUPPORTED_SOURCE_EXTENSIONS.has(path.extname(filePath).toLowerCase())
    ? filePath
    : undefined;
}

function normalizeRelativeProjectPath(relativePath) {
  return path.posix
    .normalize(relativePath.replace(/[\\/]+/g, '/'))
    .replace(/^\.\//, '')
    .toLowerCase();
}

async function readWorkspaceFile(vscodeApi, uri) {
  return new TextDecoder('utf8').decode(await vscodeApi.workspace.fs.readFile(uri));
}

function projectExplicitlyIncludesFile(projectContents, projectFilePath, filePath) {
  const expectedIncludePath = normalizeRelativeProjectPath(path.relative(path.dirname(projectFilePath), filePath));

  for (const match of projectContents.matchAll(PROJECT_INCLUDE_PATTERN)) {
    if (normalizeRelativeProjectPath(match[1]) === expectedIncludePath) {
      return true;
    }
  }

  return false;
}

async function findExplicitProjectTargetForFile(vscodeApi, filePath) {
  const workspaceFolder = getWorkspaceFolderForPath(vscodeApi, filePath);

  if (!workspaceFolder) {
    return undefined;
  }

  const workspaceRoot = workspaceFolder.uri.fsPath;
  let currentDirectory = path.dirname(filePath);

  while (currentDirectory.startsWith(workspaceRoot)) {
    const relativeDirectory = path.relative(workspaceRoot, currentDirectory).split(path.sep).join('/');
    const searchPrefix = relativeDirectory ? `${relativeDirectory}/` : '';
    const projectUris = await vscodeApi.workspace.findFiles(
      `${searchPrefix}*.{csproj,fsproj}`,
      SEARCH_EXCLUDE_GLOB
    );

    const matchingProjects = [];

    for (const projectUri of projectUris) {
      try {
        const projectContents = await readWorkspaceFile(vscodeApi, projectUri);
        if (projectExplicitlyIncludesFile(projectContents, projectUri.fsPath, filePath)) {
          matchingProjects.push(projectUri);
        }
      } catch {
        // Skip unreadable project files and continue searching.
      }
    }

    if (matchingProjects.length > 0) {
      return [...matchingProjects]
        .sort((left, right) => left.fsPath.localeCompare(right.fsPath))[0]
        .fsPath;
    }

    if (currentDirectory === workspaceRoot) {
      break;
    }

    currentDirectory = path.dirname(currentDirectory);
  }

  return undefined;
}

async function findNearestProjectTarget(vscodeApi, filePath) {
  const workspaceFolder = getWorkspaceFolderForPath(vscodeApi, filePath);

  if (!workspaceFolder) {
    return undefined;
  }

  const workspaceRoot = workspaceFolder.uri.fsPath;
  let currentDirectory = path.dirname(filePath);

  while (currentDirectory.startsWith(workspaceRoot)) {
    const relativeDirectory = path.relative(workspaceRoot, currentDirectory).split(path.sep).join('/');
    const searchPrefix = relativeDirectory ? `${relativeDirectory}/` : '';
    const projectUris = await vscodeApi.workspace.findFiles(
      `${searchPrefix}*.{csproj,fsproj}`,
      SEARCH_EXCLUDE_GLOB
    );

    if (projectUris.length > 0) {
      return sortTargetsBySpecificity(projectUris)[0].fsPath;
    }

    if (currentDirectory === workspaceRoot) {
      break;
    }

    currentDirectory = path.dirname(currentDirectory);
  }

  return undefined;
}

async function resolveConfiguredBuildTarget(vscodeApi, configuredTarget) {
  const folders = vscodeApi.workspace.workspaceFolders ?? [];

  if (configuredTarget && configuredTarget.trim()) {
    for (const folder of folders) {
      const trimmedTarget = configuredTarget.trim();
      const candidate = path.isAbsolute(trimmedTarget)
        ? vscodeApi.Uri.file(trimmedTarget)
        : vscodeApi.Uri.joinPath(folder.uri, ...trimmedTarget.split(/[\\/]+/));

      try {
        await vscodeApi.workspace.fs.stat(candidate);
        return candidate.fsPath;
      } catch {
        // Try the next folder or fallback search.
      }
    }
  }

  return undefined;
}

async function resolveWorkspaceFallbackTarget(vscodeApi) {
  const [solutionUris, projectUris] = await Promise.all([
    vscodeApi.workspace.findFiles('**/*.sln', SEARCH_EXCLUDE_GLOB),
    vscodeApi.workspace.findFiles('**/*.{csproj,fsproj}', SEARCH_EXCLUDE_GLOB)
  ]);

  const orderedSolutions = sortTargetsBySpecificity(solutionUris);
  if (orderedSolutions.length > 0) {
    return orderedSolutions[0].fsPath;
  }

  const orderedProjects = sortTargetsBySpecificity(projectUris);
  return orderedProjects[0]?.fsPath;
}

async function resolveBuildTargetDetails(vscodeApi, configuredTarget) {
  const activeDocumentPath = getActiveSourceDocumentPath(vscodeApi);

  if (activeDocumentPath) {
    const explicitProjectTarget = await findExplicitProjectTargetForFile(vscodeApi, activeDocumentPath);
    if (explicitProjectTarget) {
      return {
        buildTarget: explicitProjectTarget,
        source: 'active-file exact include',
        activeDocumentPath,
        exactIncludeMatched: true
      };
    }

    const nearestProjectTarget = await findNearestProjectTarget(vscodeApi, activeDocumentPath);
    if (nearestProjectTarget) {
      return {
        buildTarget: nearestProjectTarget,
        source: 'active-file nearest project',
        activeDocumentPath,
        exactIncludeMatched: false
      };
    }
  }

  const configuredBuildTarget = await resolveConfiguredBuildTarget(vscodeApi, configuredTarget);
  if (configuredBuildTarget) {
    return {
      buildTarget: configuredBuildTarget,
      source: 'configured target',
      activeDocumentPath,
      exactIncludeMatched: activeDocumentPath ? false : undefined
    };
  }

  const fallbackBuildTarget = await resolveWorkspaceFallbackTarget(vscodeApi);
  return {
    buildTarget: fallbackBuildTarget,
    source: 'workspace fallback',
    activeDocumentPath,
    exactIncludeMatched: activeDocumentPath ? false : undefined
  };
}

async function resolveBuildTarget(vscodeApi, configuredTarget) {
  const resolution = await resolveBuildTargetDetails(vscodeApi, configuredTarget);
  return resolution.buildTarget;
}

function runDotnetBuild(buildTarget, buildArguments, cwd) {
  return new Promise((resolve, reject) => {
    const processArguments = ['build', buildTarget, ...buildArguments];
    const child = spawn('dotnet', processArguments, {
      cwd,
      windowsHide: true
    });
    let output = '';

    child.stdout.on('data', (chunk) => {
      output += chunk.toString();
    });

    child.stderr.on('data', (chunk) => {
      output += chunk.toString();
    });

    child.on('error', reject);
    child.on('close', (exitCode) => {
      resolve({
        exitCode: exitCode ?? -1,
        command: ['dotnet', ...processArguments].join(' '),
        output
      });
    });
  });
}

function parseDiagnosticLine(line) {
  const match = NMOLECULES_DIAGNOSTIC_PATTERN.exec(line.trim());

  if (!match) {
    return undefined;
  }

  const [, filePath, lineNumber, columnNumber, severity, code, message, projectPath] = match;
  return {
    filePath,
    line: Number.parseInt(lineNumber, 10),
    column: Number.parseInt(columnNumber, 10),
    severity: severity.toLowerCase(),
    code,
    message,
    projectPath
  };
}

function parseDiagnosticsFromBuildOutput(output) {
  return output
    .split(/\r?\n/)
    .map(parseDiagnosticLine)
    .filter((entry) => entry !== undefined);
}

function toDiagnosticSeverity(vscodeApi, severity) {
  return /^warn/i.test(severity)
    ? vscodeApi.DiagnosticSeverity.Warning
    : vscodeApi.DiagnosticSeverity.Error;
}

function applyDiagnostics(vscodeApi, diagnosticCollection, diagnostics) {
  diagnosticCollection.clear();

  const grouped = new Map();

  for (const entry of diagnostics) {
    const uri = vscodeApi.Uri.file(entry.filePath);
    const current = grouped.get(uri.fsPath) ?? [];
    const range = new vscodeApi.Range(
      Math.max(entry.line - 1, 0),
      Math.max(entry.column - 1, 0),
      Math.max(entry.line - 1, 0),
      Math.max(entry.column, 1)
    );
    const diagnostic = new vscodeApi.Diagnostic(range, entry.message, toDiagnosticSeverity(vscodeApi, entry.severity));
    diagnostic.source = 'nMolecules';
    diagnostic.code = entry.code;
    current.push(diagnostic);
    grouped.set(uri.fsPath, current);
  }

  for (const [filePath, fileDiagnostics] of grouped.entries()) {
    diagnosticCollection.set(vscodeApi.Uri.file(filePath), fileDiagnostics);
  }

  return grouped;
}

function summarizeDiagnostics(buildTarget, diagnostics, exitCode) {
  const warnings = diagnostics.filter((entry) => /^warn/i.test(entry.severity)).length;
  const errors = diagnostics.length - warnings;

  return {
    buildTarget,
    exitCode,
    totalDiagnostics: diagnostics.length,
    warnings,
    errors
  };
}

function summarizeDiagnosticsByRule(diagnostics) {
  const counts = new Map();

  for (const diagnostic of diagnostics) {
    counts.set(diagnostic.code, (counts.get(diagnostic.code) ?? 0) + 1);
  }

  return [...counts.entries()]
    .map(([code, count]) => ({ code, count }))
    .sort((left, right) => {
      if (left.count !== right.count) {
        return right.count - left.count;
      }

      return left.code.localeCompare(right.code);
    });
}

function getLastDiagnosticsReport() {
  return lastDiagnosticsReport;
}

async function refreshDiagnostics(vscodeApi, outputChannel, diagnosticCollection, options = {}) {
  const resolution = await resolveBuildTargetDetails(vscodeApi, options.buildTarget ?? getDiagnosticsTarget(vscodeApi));
  const buildTarget = resolution.buildTarget;

  if (!buildTarget) {
    const message = 'No solution or project file was found for nMolecules diagnostics.';
    diagnosticCollection.clear();
    await vscodeApi.window.showWarningMessage(message);
    outputChannel.appendLine(message);
    lastDiagnosticsReport = undefined;
    return {
      buildTarget: undefined,
      totalDiagnostics: 0,
      warnings: 0,
      errors: 0,
      exitCode: -1
    };
  }

  const buildArguments = options.buildArguments ?? getDiagnosticsBuildArguments(vscodeApi);
  const runBuild = options.runBuild ?? runDotnetBuild;
  const workspaceFolderPath = vscodeApi.workspace.workspaceFolders?.[0]?.uri.fsPath ?? path.dirname(buildTarget);

  if (resolution.activeDocumentPath && resolution.exactIncludeMatched === false) {
    outputChannel.appendLine(
      `nMolecules target resolution: no exact project include matched ${resolution.activeDocumentPath}; using ${resolution.source}.`
    );
  }
  outputChannel.appendLine(`nMolecules target source: ${resolution.source}.`);
  outputChannel.appendLine(`Refreshing nMolecules diagnostics for ${buildTarget}`);
  const buildResult = await runBuild(buildTarget, buildArguments, workspaceFolderPath);
  const diagnostics = parseDiagnosticsFromBuildOutput(buildResult.output);
  applyDiagnostics(vscodeApi, diagnosticCollection, diagnostics);

  const summary = summarizeDiagnostics(buildTarget, diagnostics, buildResult.exitCode);
  const diagnosticsByRule = summarizeDiagnosticsByRule(diagnostics);
  outputChannel.appendLine(`nMolecules diagnostics: ${summary.totalDiagnostics} issue(s), ${summary.errors} error(s), ${summary.warnings} warning(s).`);
  if (diagnosticsByRule.length > 0) {
    outputChannel.appendLine('nMolecules diagnostics by rule:');
    for (const entry of diagnosticsByRule.slice(0, 10)) {
      outputChannel.appendLine(`- ${entry.code}: ${entry.count}`);
    }
  }

  lastDiagnosticsReport = {
    ...summary,
    diagnosticsByRule
  };

  if (summary.totalDiagnostics === 0) {
    await vscodeApi.window.showInformationMessage(`nMolecules found no analyzer diagnostics in ${path.basename(buildTarget)}.`);
  } else {
    await vscodeApi.window.showInformationMessage(
      `nMolecules published ${summary.totalDiagnostics} diagnostic(s) from ${path.basename(buildTarget)}.`
    );
  }

  return summary;
}

module.exports = {
  NMOLECULES_DIAGNOSTIC_PATTERN,
  getDiagnosticsTarget,
  getDiagnosticsBuildArguments,
  resolveBuildTarget,
  resolveBuildTargetDetails,
  runDotnetBuild,
  parseDiagnosticLine,
  parseDiagnosticsFromBuildOutput,
  applyDiagnostics,
  summarizeDiagnostics,
  summarizeDiagnosticsByRule,
  findExplicitProjectTargetForFile,
  findNearestProjectTarget,
  getLastDiagnosticsReport,
  refreshDiagnostics
};
