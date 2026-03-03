'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const { Buffer } = require('node:buffer');

const {
  COMMANDS,
  getDocsRoot,
  getTraceLevel,
  shouldAutoInspect,
  registerCommands,
  readWorkspaceEntries,
  inspectWorkspace,
  openWorkspaceDocs
} = require('../src/commands');

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

test('readWorkspaceEntries reads solution and project files from the workspace', async () => {
  const projectContents = {
    'c:\\repo\\src\\Banking\\Banking.csproj': '<Project><ItemGroup><PackageReference Include="NMolecules.DDD" Version="0.2.2" /></ItemGroup></Project>',
    'c:\\repo\\src\\Api\\Api.fsproj': '<Project></Project>'
  };
  const vscode = {
    workspace: {
      findFiles(pattern) {
        if (pattern === '**/*.sln') {
          return Promise.resolve([{ fsPath: 'c:\\repo\\workspace.sln' }]);
        }

        return Promise.resolve(Object.keys(projectContents).map((fsPath) => ({ fsPath })));
      },
      fs: {
        readFile(uri) {
          return Promise.resolve(Buffer.from(projectContents[uri.fsPath], 'utf8'));
        }
      }
    }
  };

  const entries = await readWorkspaceEntries(vscode);

  assert.equal(entries.length, 3);
  assert.equal(entries[0].path, 'c:\\repo\\workspace.sln');
  assert.match(entries[1].content, /NMolecules\.DDD/);
});

test('inspectWorkspace writes a formatted report and shows a summary message', async () => {
  const output = [];
  const infoMessages = [];
  const outputChannel = {
    clear() {
      output.push('[clear]');
    },
    appendLine(line) {
      output.push(line);
    },
    show(preserveFocus) {
      output.push(`[show:${String(preserveFocus)}]`);
    }
  };
  const vscode = {
    workspace: {
      findFiles(pattern) {
        if (pattern === '**/*.sln') {
          return Promise.resolve([{ fsPath: 'c:\\repo\\workspace.sln' }]);
        }

        return Promise.resolve([
          { fsPath: 'c:\\repo\\src\\Banking\\Banking.csproj' }
        ]);
      },
      fs: {
        readFile() {
          return Promise.resolve(Buffer.from('<Project><ItemGroup><PackageReference Include="NMolecules.Analyzers" Version="0.0.0.6" /><PackageReference Include="NMolecules.DDD" Version="0.2.2" /></ItemGroup></Project>', 'utf8'));
        }
      }
    },
    window: {
      showInformationMessage(message) {
        infoMessages.push(message);
        return Promise.resolve(message);
      }
    }
  };

  const report = await inspectWorkspace(vscode, outputChannel);

  assert.equal(report.totalProjects, 1);
  assert.equal(report.totalSolutions, 1);
  assert.equal(report.analyzerPackageProjects, 1);
  assert.equal(report.coreReferenceProjects, 1);
  assert.deepEqual(infoMessages, ['nMolecules inspected 1 project(s) and 1 solution(s).']);
  assert.equal(output[0], '[clear]');
  assert.match(output[1], /nMolecules workspace inspection/);
  assert.equal(output.at(-1), '[show:true]');
});

test('openWorkspaceDocs opens the first available documentation candidate', async () => {
  const opened = [];
  const knownFiles = new Set(['c:\\repo\\docs\\layer-matrix.md']);
  const vscode = {
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        stat(uri) {
          if (!knownFiles.has(uri.fsPath)) {
            return Promise.reject(new Error('not found'));
          }

          return Promise.resolve({});
        }
      },
      openTextDocument(uri) {
        opened.push(uri.fsPath);
        return Promise.resolve({ uri });
      }
    },
    window: {
      showTextDocument(document, options) {
        opened.push(options.preview === false ? 'preview:false' : 'preview:true');
        return Promise.resolve(document);
      },
      showWarningMessage() {
        throw new Error('warning should not be shown');
      }
    },
    Uri: {
      joinPath(base, ...parts) {
        return {
          fsPath: [base.fsPath, ...parts].join('\\')
        };
      }
    }
  };

  const openedUri = await openWorkspaceDocs(vscode, 'docs');

  assert.equal(openedUri.fsPath, 'c:\\repo\\docs\\layer-matrix.md');
  assert.deepEqual(opened, ['c:\\repo\\docs\\layer-matrix.md', 'preview:false']);
});

test('openWorkspaceDocs warns when no documentation exists', async () => {
  const warnings = [];
  const vscode = {
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        stat() {
          return Promise.reject(new Error('not found'));
        }
      },
      openTextDocument() {
        throw new Error('document should not be opened');
      }
    },
    window: {
      showTextDocument() {
        throw new Error('editor should not be opened');
      },
      showWarningMessage(message) {
        warnings.push(message);
        return Promise.resolve(message);
      }
    },
    Uri: {
      joinPath(base, ...parts) {
        return {
          fsPath: [base.fsPath, ...parts].join('\\')
        };
      }
    }
  };

  const result = await openWorkspaceDocs(vscode, 'docs');

  assert.equal(result, undefined);
  assert.deepEqual(warnings, ['No nMolecules documentation file was found in the current workspace.']);
});
