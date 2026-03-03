'use strict';

const { TextDecoder } = require('util');
const { buildWorkspaceReport, formatWorkspaceReport } = require('./inspectWorkspace');
const { getDocumentationCandidates } = require('./docs');

const COMMANDS = {
  inspectWorkspace: 'nmolecules.inspectWorkspace',
  openWorkspaceDocs: 'nmolecules.openWorkspaceDocs'
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

async function openWorkspaceDocs(vscodeApi, docsRoot) {
  const folders = vscodeApi.workspace.workspaceFolders ?? [];

  for (const folder of folders) {
    for (const relativePath of getDocumentationCandidates(docsRoot)) {
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

  await vscodeApi.window.showWarningMessage('No nMolecules documentation file was found in the current workspace.');
  return undefined;
}

function getTraceLevel(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('traceLevel', 'basic');
}

function getDocsRoot(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('docsRoot', 'docs');
}

function shouldAutoInspect(vscodeApi) {
  return vscodeApi.workspace.getConfiguration('nmolecules').get('autoInspectOnStartup', false);
}

function registerCommands(vscodeApi, context, outputChannel) {
  const subscriptions = [
    vscodeApi.commands.registerCommand(COMMANDS.inspectWorkspace, () => inspectWorkspace(vscodeApi, outputChannel)),
    vscodeApi.commands.registerCommand(COMMANDS.openWorkspaceDocs, () => openWorkspaceDocs(vscodeApi, getDocsRoot(vscodeApi)))
  ];

  context.subscriptions.push(...subscriptions, outputChannel);
  return subscriptions;
}

module.exports = {
  COMMANDS,
  inspectWorkspace,
  openWorkspaceDocs,
  registerCommands,
  getTraceLevel,
  getDocsRoot,
  shouldAutoInspect
};
