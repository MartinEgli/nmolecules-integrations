'use strict';

const assert = require('node:assert/strict');
const path = require('node:path');
const vscode = require('vscode');

async function run() {
  const extension = vscode.extensions.getExtension('xmolecules.nmolecules');
  assert.ok(extension, 'The nMolecules extension must be available to the test host.');

  await extension.activate();

  const workspacePath = path.resolve(__dirname, '..', '..', 'sample-violations');
  const existingFolders = vscode.workspace.workspaceFolders ?? [];

  if (existingFolders.length > 0) {
    vscode.workspace.updateWorkspaceFolders(0, existingFolders.length);
  }

  const added = vscode.workspace.updateWorkspaceFolders(0, 0, {
    uri: vscode.Uri.file(workspacePath),
    name: 'sample-violations'
  });
  assert.equal(added, true, 'The test workspace should be added successfully.');

  await new Promise((resolve) => setTimeout(resolve, 250));

  const inspectReport = await vscode.commands.executeCommand('nmolecules.inspectWorkspace');
  assert.ok(inspectReport, 'The inspect command should return a workspace report.');
  assert.ok(inspectReport.totalProjects >= 3, 'The violations sample should expose several projects.');

  const diagnosticsSummary = await vscode.commands.executeCommand('nmolecules.refreshDiagnostics');
  assert.ok(diagnosticsSummary.totalDiagnostics >= 4, 'The violations sample should emit multiple analyzer diagnostics.');

  const byRuleSummary = await vscode.commands.executeCommand('nmolecules.showDiagnosticsSummary');
  assert.ok(byRuleSummary.totalDiagnostics >= 4, 'The diagnostics summary command should reflect the latest refresh run.');

  const docsUri = await vscode.commands.executeCommand('nmolecules.openWorkspaceDocs');
  assert.ok(docsUri.fsPath.endsWith('docs\\architecture.md') || docsUri.fsPath.endsWith('docs/architecture.md'));

  const ruleCatalogUri = await vscode.commands.executeCommand('nmolecules.openRuleCatalog');
  assert.ok(ruleCatalogUri.fsPath.endsWith('docs\\architecture\\analyzer-rule-map.md') || ruleCatalogUri.fsPath.endsWith('docs/architecture/analyzer-rule-map.md'));
}

module.exports = {
  run
};
