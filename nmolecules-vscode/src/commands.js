'use strict';

const { TextDecoder } = require('util');
const { refreshDiagnostics, getLastDiagnosticsReport } = require('./diagnostics');
const { buildWorkspaceReport, formatWorkspaceReport } = require('./inspectWorkspace');
const { getDocumentationCandidates, getRuleCatalogCandidates } = require('./docs');

const COMMANDS = {
  inspectWorkspace: 'nmolecules.inspectWorkspace',
  refreshDiagnostics: 'nmolecules.refreshDiagnostics',
  openWorkspaceDocs: 'nmolecules.openWorkspaceDocs',
  openRuleCatalog: 'nmolecules.openRuleCatalog',
  showDiagnosticsSummary: 'nmolecules.showDiagnosticsSummary'
};

async function readWorkspaceEntries(vscodeApi) {
  const [solutionUris, projectUris] = await Promise.all([
    vscodeApi.workspace.findFiles('**/*.sln'),
    vscodeApi.workspace.findFiles('**/*.{csproj,fsproj}')
  ]);
  const decoder = new TextDecoder('utf8');

  const projectEntries = await Promise.all(projectUris.map(async (uri) => ({
    path: uri.fsPath,
    content: decoder.decode(await vscodeApi.workspace.fs.readFile(uri))
  })));

  const solutionEntries = solutionUris.map((uri) => ({ path: uri.fsPath }));
  return [...solutionEntries, ...projectEntries];
}

async function inspectWorkspace(vscodeApi, outputChannel) {
  const entries = await readWorkspaceEntries(vscodeApi);
  const report = buildWorkspaceReport(entries);
  const formatted = formatWorkspaceReport(report);

  outputChannel.clear();
  outputChannel.appendLine(formatted);
  outputChannel.show(true);

  const summary = `nMolecules inspected ${report.totalProjects} project(s) and ${report.totalSolutions} solution(s).`;
  await vscodeApi.window.showInformationMessage(summary);

  return report;
}

async function openFirstDocument(vscodeApi, candidates, warningMessage) {
  const folders = vscodeApi.workspace.workspaceFolders ?? [];

  for (const folder of folders) {
    for (const relativePath of candidates) {
      const candidate = vscodeApi.Uri.joinPath(folder.uri, ...relativePath.split('/'));

      try {
        await vscodeApi.workspace.fs.stat(candidate);
        const document = await vscodeApi.workspace.openTextDocument(candidate);
        await vscodeApi.window.showTextDocument(document, { preview: false });
        return candidate;
      } catch {
        // Try the next candidate.
      }
    }
  }

  await vscodeApi.window.showWarningMessage(warningMessage);
  return undefined;
}

async function openWorkspaceDocs(vscodeApi, docsRoot) {
  return openFirstDocument(
    vscodeApi,
    getDocumentationCandidates(docsRoot),
    'No nMolecules documentation file was found in the current workspace.'
  );
}

async function openRuleCatalog(vscodeApi, docsRoot) {
  return openFirstDocument(
    vscodeApi,
    getRuleCatalogCandidates(docsRoot),
    'No nMolecules rule catalog document was found in the current workspace.'
  );
}

async function showDiagnosticsSummary(vscodeApi, outputChannel) {
  const report = getLastDiagnosticsReport();
  if (!report) {
    await vscodeApi.window.showWarningMessage('No nMolecules diagnostics report is available yet. Run "nMolecules: Refresh Diagnostics" first.');
    return undefined;
  }

  outputChannel.appendLine('nMolecules last diagnostics summary');
  outputChannel.appendLine(`- Target: ${report.buildTarget}`);
  outputChannel.appendLine(`- Total: ${report.totalDiagnostics}, Errors: ${report.errors}, Warnings: ${report.warnings}`);

  if (report.diagnosticsByRule?.length > 0) {
    outputChannel.appendLine('- Rules:');
    for (const entry of report.diagnosticsByRule) {
      outputChannel.appendLine(`  - ${entry.code}: ${entry.count}`);
    }
  }

  outputChannel.show(true);
  return report;
}

function getTraceLevel(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('traceLevel', 'basic');
}

function getDocsRoot(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('docsRoot', 'docs');
}

function getDiagnosticsTarget(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('diagnosticsTarget', '');
}

function getDiagnosticsBuildArguments(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('diagnosticsBuildArguments', ['-v', 'minimal']);
}

function shouldAutoInspect(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('autoInspectOnStartup', false);
}

function shouldRefreshDiagnosticsOnStartup(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('refreshDiagnosticsOnStartup', false);
}

function registerCommands(vscodeApi, context, outputChannel, diagnosticCollection) {
  const subscriptions = [
    vscodeApi.commands.registerCommand(COMMANDS.inspectWorkspace, () => inspectWorkspace(vscodeApi, outputChannel)),
    vscodeApi.commands.registerCommand(COMMANDS.refreshDiagnostics, () => refreshDiagnostics(vscodeApi, outputChannel, diagnosticCollection)),
    vscodeApi.commands.registerCommand(COMMANDS.openWorkspaceDocs, () => openWorkspaceDocs(vscodeApi, getDocsRoot(vscodeApi))),
    vscodeApi.commands.registerCommand(COMMANDS.openRuleCatalog, () => openRuleCatalog(vscodeApi, getDocsRoot(vscodeApi))),
    vscodeApi.commands.registerCommand(COMMANDS.showDiagnosticsSummary, () => showDiagnosticsSummary(vscodeApi, outputChannel))
  ];

  context.subscriptions.push(...subscriptions, outputChannel);
  return subscriptions;
}

module.exports = {
  COMMANDS,
  readWorkspaceEntries,
  inspectWorkspace,
  openFirstDocument,
  openWorkspaceDocs,
  openRuleCatalog,
  showDiagnosticsSummary,
  registerCommands,
  getTraceLevel,
  getDocsRoot,
  getDiagnosticsTarget,
  getDiagnosticsBuildArguments,
  shouldAutoInspect,
  shouldRefreshDiagnosticsOnStartup
};
