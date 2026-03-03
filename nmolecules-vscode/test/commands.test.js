'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');

const { COMMANDS, getDocsRoot, getTraceLevel, shouldAutoInspect, registerCommands } = require('../src/commands');

test('registerCommands registers both extension commands', () => {
  const registered = [];
  const context = { subscriptions: [] };
  const outputChannel = { appendLine() {}, clear() {}, show() {} };
  const vscode = {
    commands: {
      registerCommand(id, handler) {
        registered.push({ id, handler });
        return { dispose() {} };
      }
    },
    workspace: {
      getConfiguration() {
        return {
          get() {
            return undefined;
          }
        };
      }
    }
  };

  registerCommands(vscode, context, outputChannel);

  assert.deepEqual(
    registered.map((entry) => entry.id),
    [COMMANDS.inspectWorkspace, COMMANDS.openWorkspaceDocs]
  );
  assert.equal(context.subscriptions.length, 3);
});

test('configuration helpers read extension settings with defaults', () => {
  const values = {
    docsRoot: 'workspace-docs',
    traceLevel: 'verbose',
    autoInspectOnStartup: true
  };
  const vscode = {
    workspace: {
      getConfiguration(section) {
        assert.equal(section, 'nmolecules');
        return {
          get(name, fallback) {
            return Object.prototype.hasOwnProperty.call(values, name) ? values[name] : fallback;
          }
        };
      }
    }
  };

  assert.equal(getDocsRoot(vscode), 'workspace-docs');
  assert.equal(getTraceLevel(vscode), 'verbose');
  assert.equal(shouldAutoInspect(vscode), true);
});
