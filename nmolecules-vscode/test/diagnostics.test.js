'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');

const {
  parseDiagnosticLine,
  parseDiagnosticsFromBuildOutput,
  refreshDiagnostics,
  summarizeDiagnosticsByRule,
  getLastDiagnosticsReport
} = require('../src/diagnostics');

test('parseDiagnosticLine reads nMolecules analyzer output', () => {
  const line = 'c:\\repo\\src\\Banking.Domain\\BrokenDomainModel.cs(9,6): error XMoleculesValueObject0006: Value object should not declare identity members [c:\\repo\\src\\Banking.Domain\\Banking.Domain.csproj]';
  const diagnostic = parseDiagnosticLine(line);

  assert.equal(diagnostic.filePath, 'c:\\repo\\src\\Banking.Domain\\BrokenDomainModel.cs');
  assert.equal(diagnostic.line, 9);
  assert.equal(diagnostic.column, 6);
  assert.equal(diagnostic.severity, 'error');
  assert.equal(diagnostic.code, 'XMoleculesValueObject0006');
});

test('parseDiagnosticsFromBuildOutput keeps warnings and errors', () => {
  const diagnostics = parseDiagnosticsFromBuildOutput([
    'c:\\repo\\Domain.cs(10,3): warning XMoleculesService0001: Service should use a specific role marker [c:\\repo\\Domain.csproj]',
    'c:\\repo\\Application.cs(4,2): error XMoleculesApplicationService0001: Application service should not also be a domain building block [c:\\repo\\Application.csproj]'
  ].join('\n'));

  assert.equal(diagnostics.length, 2);
  assert.deepEqual(
    diagnostics.map((entry) => entry.code),
    ['XMoleculesService0001', 'XMoleculesApplicationService0001']
  );
});

test('summarizeDiagnosticsByRule groups diagnostics per rule ID', () => {
  const summary = summarizeDiagnosticsByRule([
    { code: 'XMoleculesValueObject0006' },
    { code: 'XMoleculesValueObject0006' },
    { code: 'XMoleculesService0001' }
  ]);

  assert.deepEqual(summary, [
    { code: 'XMoleculesValueObject0006', count: 2 },
    { code: 'XMoleculesService0001', count: 1 }
  ]);
});

test('refreshDiagnostics publishes diagnostics into the collection', async () => {
  const infoMessages = [];
  const warningMessages = [];
  const output = [];
  const sets = [];
  const diagnosticCollection = {
    clear() {
      sets.push({ type: 'clear' });
    },
    set(uri, diagnostics) {
      sets.push({ type: 'set', path: uri.fsPath, diagnostics });
    }
  };
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

            if (name === 'diagnosticsBuildArguments') {
              return ['-v', 'minimal'];
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
      showInformationMessage(message) {
        infoMessages.push(message);
        return Promise.resolve(message);
      },
      showWarningMessage(message) {
        warningMessages.push(message);
        return Promise.resolve(message);
      }
    }
  };
  const outputChannel = {
    appendLine(line) {
      output.push(line);
    }
  };

  const result = await refreshDiagnostics(vscode, outputChannel, diagnosticCollection, {
    runBuild: async () => ({
      exitCode: 1,
      output: 'c:\\repo\\src\\BrokenDomainModel.cs(9,6): error XMoleculesValueObject0006: Value object should not declare identity members [c:\\repo\\src\\Banking.Domain\\Banking.Domain.csproj]\n'
        + 'c:\\repo\\src\\LegacyBillingService.cs(4,2): warning XMoleculesService0001: Service should use a specific role marker [c:\\repo\\src\\Banking.Domain\\Banking.Domain.csproj]'
    })
  });

  assert.equal(result.totalDiagnostics, 2);
  assert.equal(result.errors, 1);
  assert.equal(result.warnings, 1);
  assert.equal(warningMessages.length, 0);
  assert.deepEqual(infoMessages, ['nMolecules published 2 diagnostic(s) from Banking.Sample.Violations.sln.']);
  assert.equal(sets[0].type, 'clear');
  assert.equal(sets.filter((entry) => entry.type === 'set').length, 2);
  assert.match(output.join('\n'), /Refreshing nMolecules diagnostics/);
  assert.match(output.join('\n'), /nMolecules diagnostics by rule/);
  assert.deepEqual(getLastDiagnosticsReport().diagnosticsByRule, [
    { code: 'XMoleculesService0001', count: 1 },
    { code: 'XMoleculesValueObject0006', count: 1 }
  ]);
});
