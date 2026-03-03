'use strict';

const { registerCommands, inspectWorkspace, shouldAutoInspect, getTraceLevel } = require('./commands');

function activate(context) {
  const vscode = require('vscode');
  const outputChannel = vscode.window.createOutputChannel('nMolecules');

  if (getTraceLevel(vscode) !== 'off') {
    outputChannel.appendLine('nMolecules extension activated.');
  }

  registerCommands(vscode, context, outputChannel);

  if (shouldAutoInspect(vscode)) {
    void inspectWorkspace(vscode, outputChannel);
  }
}

function deactivate() {}

module.exports = {
  activate,
  deactivate
};
