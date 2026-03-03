'use strict';

const {
  registerCommands,
  inspectWorkspace,
  shouldAutoInspect,
  shouldRefreshDiagnosticsOnStartup,
  getTraceLevel
} = require('./commands');
const { refreshDiagnostics } = require('./diagnostics');

function activate(context) {
  const vscode = require('vscode');
  const outputChannel = vscode.window.createOutputChannel('nMolecules');
  const diagnosticCollection = vscode.languages.createDiagnosticCollection('nmolecules');

  if (getTraceLevel(vscode) !== 'off') {
    outputChannel.appendLine('nMolecules extension activated.');
  }

  context.subscriptions.push(diagnosticCollection);
  registerCommands(vscode, context, outputChannel, diagnosticCollection);

  if (shouldAutoInspect(vscode)) {
    void inspectWorkspace(vscode, outputChannel);
  }

  if (shouldRefreshDiagnosticsOnStartup(vscode)) {
    void refreshDiagnostics(vscode, outputChannel, diagnosticCollection);
  }
}

function deactivate() {}

module.exports = {
  activate,
  deactivate
};
