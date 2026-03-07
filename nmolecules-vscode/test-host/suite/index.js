'use strict';

const assert = require('node:assert/strict');
const path = require('node:path');
const vscode = require('vscode');

function endsWithPath(actualPath, expectedSuffix) {
  return actualPath.endsWith(expectedSuffix.replaceAll('/', path.sep))
    || actualPath.endsWith(expectedSuffix.replaceAll('\\', path.sep));
}

async function openWorkspace(relativeWorkspacePath, name) {
  const workspacePath = path.resolve(__dirname, '..', '..', relativeWorkspacePath);
  const existingFolders = vscode.workspace.workspaceFolders ?? [];

  if (existingFolders.length > 0) {
    vscode.workspace.updateWorkspaceFolders(0, existingFolders.length);
  }

  const added = vscode.workspace.updateWorkspaceFolders(0, 0, {
    uri: vscode.Uri.file(workspacePath),
    name
  });
  assert.equal(added, true, `The ${name} workspace should be added successfully.`);

  await new Promise((resolve) => setTimeout(resolve, 500));
}

async function smokeHealthyWorkspace() {
  await openWorkspace('sample-workspace', 'sample-workspace');

  const inspectReport = await vscode.commands.executeCommand('nmolecules.inspectWorkspace');
  assert.ok(inspectReport, 'The inspect command should return a workspace report.');
  assert.equal(inspectReport.totalProjects, 4, 'The healthy sample should expose the four documented projects.');

  const diagnosticsSummary = await vscode.commands.executeCommand('nmolecules.refreshDiagnostics');
  assert.equal(diagnosticsSummary.totalDiagnostics, 0, 'The healthy sample should stay free of analyzer diagnostics.');

  const byRuleSummary = await vscode.commands.executeCommand('nmolecules.showDiagnosticsSummary');
  assert.equal(byRuleSummary.totalDiagnostics, 0, 'The healthy diagnostics summary should remain empty.');

  const docsUri = await vscode.commands.executeCommand('nmolecules.openWorkspaceDocs');
  assert.ok(endsWithPath(docsUri.fsPath, 'docs/architecture.md'));

  const ruleCatalogUri = await vscode.commands.executeCommand('nmolecules.openRuleCatalog');
  assert.ok(endsWithPath(ruleCatalogUri.fsPath, 'docs/architecture/analyzer-rule-map.md'));
}

async function smokeViolationsWorkspace() {
  await openWorkspace('sample-violations', 'sample-violations');

  const inspectReport = await vscode.commands.executeCommand('nmolecules.inspectWorkspace');
  assert.ok(inspectReport, 'The inspect command should return a workspace report.');
  assert.equal(inspectReport.totalProjects, 7, 'The violations sample should expose the seven documented projects.');

  const diagnosticsSummary = await vscode.commands.executeCommand('nmolecules.refreshDiagnostics');
  assert.ok(diagnosticsSummary.totalDiagnostics >= 4, 'The violations sample should emit multiple analyzer diagnostics.');

  const byRuleSummary = await vscode.commands.executeCommand('nmolecules.showDiagnosticsSummary');
  assert.ok(byRuleSummary.totalDiagnostics >= 4, 'The diagnostics summary command should reflect the latest refresh run.');

  const docsUri = await vscode.commands.executeCommand('nmolecules.openWorkspaceDocs');
  assert.ok(endsWithPath(docsUri.fsPath, 'docs/architecture.md'));

  const ruleCatalogUri = await vscode.commands.executeCommand('nmolecules.openRuleCatalog');
  assert.ok(endsWithPath(ruleCatalogUri.fsPath, 'docs/architecture/analyzer-rule-map.md'));
}

async function run() {
  const extension = vscode.extensions.getExtension('xmolecules.nmolecules');
  assert.ok(extension, 'The nMolecules extension must be available to the test host.');

  await extension.activate();
  await smokeHealthyWorkspace();
  await smokeViolationsWorkspace();
}

module.exports = {
  run
};
