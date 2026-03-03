'use strict';

const path = require('node:path');
const { spawn } = require('node:child_process');

const NMOLECULES_DIAGNOSTIC_PATTERN = /^(.+?)\((\d+),(\d+)(?:,\d+,\d+)?\):\s(error|warning|fehler|warnung)\s(XMolecules[A-Za-z0-9]+):\s(.+?)(?:\s\[(.+)\])?$/i;

function getDiagnosticsTarget(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('diagnosticsTarget', '');
}

function getDiagnosticsBuildArguments(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('diagnosticsBuildArguments', ['-v', 'minimal']);
}

async function resolveBuildTarget(vscodeApi, configuredTarget) {
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

  const [solutionUris, projectUris] = await Promise.all([
    vscodeApi.workspace.findFiles('**/*.sln'),
    vscodeApi.workspace.findFiles('**/*.{csproj,fsproj}')
  ]);

  const orderedSolutions = [...solutionUris].sort((left, right) => left.fsPath.localeCompare(right.fsPath));
  if (orderedSolutions.length > 0) {
    return orderedSolutions[0].fsPath;
  }

  const orderedProjects = [...projectUris].sort((left, right) => left.fsPath.localeCompare(right.fsPath));
  return orderedProjects[0]?.fsPath;
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

async function refreshDiagnostics(vscodeApi, outputChannel, diagnosticCollection, options = {}) {
  const buildTarget = await resolveBuildTarget(vscodeApi, options.buildTarget ?? getDiagnosticsTarget(vscodeApi));

  if (!buildTarget) {
    const message = 'No solution or project file was found for nMolecules diagnostics.';
    diagnosticCollection.clear();
    await vscodeApi.window.showWarningMessage(message);
    outputChannel.appendLine(message);
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

  outputChannel.appendLine(`Refreshing nMolecules diagnostics for ${buildTarget}`);
  const buildResult = await runBuild(buildTarget, buildArguments, workspaceFolderPath);
  const diagnostics = parseDiagnosticsFromBuildOutput(buildResult.output);
  applyDiagnostics(vscodeApi, diagnosticCollection, diagnostics);

  const summary = summarizeDiagnostics(buildTarget, diagnostics, buildResult.exitCode);
  outputChannel.appendLine(`nMolecules diagnostics: ${summary.totalDiagnostics} issue(s), ${summary.errors} error(s), ${summary.warnings} warning(s).`);

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
  runDotnetBuild,
  parseDiagnosticLine,
  parseDiagnosticsFromBuildOutput,
  applyDiagnostics,
  summarizeDiagnostics,
  refreshDiagnostics
};
