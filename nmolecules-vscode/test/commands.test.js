'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const { Buffer } = require('node:buffer');

const {
  COMMANDS,
  getDiagnosticsBuildArguments,
  getDiagnosticsTarget,
  getDocsRoot,
  getTraceLevel,
  shouldAutoInspect,
  shouldRefreshDiagnosticsOnStartup,
  registerCommands,
  readWorkspaceEntries,
  inspectWorkspace,
  openWorkspaceDocs,
  openRuleCatalog,
  showDiagnosticsSummary
} = require('../src/commands');
const { refreshDiagnostics } = require('../src/diagnostics');

test('registerCommands registers all extension commands', () => {
  const registered = [];
  const context = { subscriptions: [] };
  const outputChannel = { appendLine() {}, clear() {}, show() {} };
  const diagnostics = { clear() {}, set() {} };
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

  registerCommands(vscode, context, outputChannel, diagnostics);

  assert.deepEqual(
    registered.map((entry) => entry.id),
    [COMMANDS.inspectWorkspace, COMMANDS.refreshDiagnostics, COMMANDS.openWorkspaceDocs, COMMANDS.openRuleCatalog, COMMANDS.showDiagnosticsSummary]
  );
  assert.equal(context.subscriptions.length, 6);
});

test('configuration helpers read extension settings with defaults', () => {
  const values = {
    docsRoot: 'workspace-docs',
    traceLevel: 'verbose',
    autoInspectOnStartup: true,
    diagnosticsTarget: 'sample-violations/Banking.Sample.Violations.sln',
    diagnosticsBuildArguments: ['-v', 'normal'],
    refreshDiagnosticsOnStartup: true
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
  assert.equal(getDiagnosticsTarget(vscode), 'sample-violations/Banking.Sample.Violations.sln');
  assert.deepEqual(getDiagnosticsBuildArguments(vscode), ['-v', 'normal']);
  assert.equal(shouldAutoInspect(vscode), true);
  assert.equal(shouldRefreshDiagnosticsOnStartup(vscode), true);
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

test('openRuleCatalog opens analyzer rule map when available', async () => {
  const opened = [];
  const knownFiles = new Set(['c:\\repo\\docs\\architecture\\analyzer-rule-map.md']);
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

  const openedUri = await openRuleCatalog(vscode, 'docs');

  assert.equal(openedUri.fsPath, 'c:\\repo\\docs\\architecture\\analyzer-rule-map.md');
  assert.deepEqual(opened, ['c:\\repo\\docs\\architecture\\analyzer-rule-map.md', 'preview:false']);
});

test('showDiagnosticsSummary prints last diagnostics report', async () => {
  const output = [];
  const diagnosticCollection = { clear() {}, set() {} };
  const vscode = {
    DiagnosticSeverity: {
      Error: 'error',
      Warning: 'warning'
    },
    Range: class Range {
      constructor(startLine, startCharacter, endLine, endCharacter) {
        this.start = { line: startLine, character: startCharacter };
        this.end = { line: endLine, character: endCharacter };
      }
    },
    Diagnostic: class Diagnostic {
      constructor(range, message, severity) {
        this.range = range;
        this.message = message;
        this.severity = severity;
      }
    },
    Uri: {
      file(fsPath) {
        return { fsPath };
      },
      joinPath(base, ...parts) {
        return {
          fsPath: [base.fsPath, ...parts].join('\\')
        };
      }
    },
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      getConfiguration() {
        return {
          get(name, fallback) {
            if (name === 'diagnosticsTarget') {
              return 'sample-violations\\Banking.Sample.Violations.sln';
            }

            return fallback;
          }
        };
      },
      fs: {
        stat() {
          return Promise.resolve({});
        }
      },
      findFiles() {
        return Promise.resolve([]);
      }
    },
    window: {
      showInformationMessage() {
        return Promise.resolve();
      },
      showWarningMessage() {
        return Promise.resolve();
      }
    }
  };
  const outputChannel = {
    appendLine(line) {
      output.push(line);
    },
    show() {
      output.push('[show]');
    }
  };

  await refreshDiagnostics(vscode, outputChannel, diagnosticCollection, {
    runBuild: async () => ({
      exitCode: 1,
      output: 'c:\\repo\\src\\BrokenDomainModel.cs(9,6): error XMoleculesValueObject0006: Value object should not declare identity members [c:\\repo\\src\\Banking.Domain\\Banking.Domain.csproj]'
    })
  });

  const summary = await showDiagnosticsSummary(vscode, outputChannel);

  assert.equal(summary.totalDiagnostics, 1);
  assert.match(output.join('\n'), /nMolecules last diagnostics summary/);
  assert.match(output.join('\n'), /XMoleculesValueObject0006: 1/);
});
