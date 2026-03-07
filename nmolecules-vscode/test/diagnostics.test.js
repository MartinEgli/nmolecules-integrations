'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const { Buffer } = require('node:buffer');

const {
  parseDiagnosticLine,
  parseDiagnosticsFromBuildOutput,
  resolveBuildTarget,
  refreshDiagnostics,
  summarizeDiagnosticsByRule,
  getLastDiagnosticsReport
} = require('../src/diagnostics');

function createUriApi() {
  return {
    file(fsPath) {
      return { fsPath };
    },
    joinPath(base, ...parts) {
      return {
        fsPath: [base.fsPath, ...parts].join('\\')
      };
    }
  };
}

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
    Uri: createUriApi(),
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
  assert.match(output.join('\n'), /nMolecules target source: configured target\./);
  assert.match(output.join('\n'), /Refreshing nMolecules diagnostics/);
  assert.match(output.join('\n'), /nMolecules diagnostics by rule/);
  assert.deepEqual(getLastDiagnosticsReport().diagnosticsByRule, [
    { code: 'XMoleculesService0001', count: 1 },
    { code: 'XMoleculesValueObject0006', count: 1 }
  ]);
});

test('refreshDiagnostics logs active-file fallback reasoning when no exact include exists', async () => {
  const output = [];
  const infoMessages = [];
  const diagnosticCollection = {
    clear() {},
    set() {}
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
    Uri: createUriApi(),
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      getConfiguration() {
        return {
          get(name, fallback) {
            if (name === 'diagnosticsBuildArguments') {
              return ['-v', 'minimal'];
            }

            return fallback;
          }
        };
      },
      fs: {
        readFile() {
          return Promise.resolve(Buffer.from('<Project><ItemGroup><Compile Remove="**\\Bad*.cs" /></ItemGroup></Project>', 'utf8'));
        },
        stat() {
          return Promise.reject(new Error('not found'));
        }
      },
      findFiles(pattern) {
        if (pattern === 'samples/06-analyzer-workbench/scenarios/good/checkout/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/good/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/*.{csproj,fsproj}') {
          return Promise.resolve([
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerWorkbench.csproj' }
          ]);
        }

        return Promise.resolve([]);
      }
    },
    window: {
      activeTextEditor: {
        document: {
          uri: {
            fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\scenarios\\good\\checkout\\GoodExample.cs'
          }
        }
      },
      showInformationMessage(message) {
        infoMessages.push(message);
        return Promise.resolve(message);
      },
      showWarningMessage() {
        return Promise.resolve();
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
      exitCode: 0,
      output: ''
    })
  });

  assert.equal(result.totalDiagnostics, 0);
  assert.deepEqual(infoMessages, ['nMolecules found no analyzer diagnostics in Samples.Block06.AnalyzerWorkbench.csproj.']);
  assert.match(output.join('\n'), /no exact project include matched .*GoodExample\.cs; using active-file nearest project\./);
  assert.match(output.join('\n'), /nMolecules target source: active-file nearest project\./);
});

test('resolveBuildTarget prefers an exact active-file compile include over configured target', async () => {
  const findCalls = [];
  const projectContents = {
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.csproj': '<Project><ItemGroup><Compile Remove="**\\Good*.cs" /></ItemGroup></Project>',
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.BoundedContextConsistency.csproj': '<Project><ItemGroup><Compile Include="scenarios\\violations\\metadata\\BadBoundedContextConsistency.cs" /></ItemGroup></Project>',
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.Ddd.csproj': '<Project><ItemGroup><Compile Include="scenarios\\violations\\ddd\\BadDddRules.cs" /></ItemGroup></Project>'
  };
  const vscode = {
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        readFile(uri) {
          return Promise.resolve(Buffer.from(projectContents[uri.fsPath] ?? '', 'utf8'));
        },
        stat() {
          return Promise.resolve({});
        }
      },
      findFiles(pattern, exclude) {
        findCalls.push({ pattern, exclude });

        if (pattern === 'samples/06-analyzer-workbench/scenarios/violations/ddd/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/violations/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/*.{csproj,fsproj}') {
          return Promise.resolve([
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.csproj' },
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.BoundedContextConsistency.csproj' },
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.Ddd.csproj' }
          ]);
        }

        return Promise.resolve([]);
      }
    },
    window: {
      activeTextEditor: {
        document: {
          uri: {
            fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\scenarios\\violations\\ddd\\BadDddRules.cs'
          }
        }
      }
    }
  };

  const target = await resolveBuildTarget(vscode, 'fallback\\workspace.sln');

  assert.equal(target, 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.Ddd.csproj');
  assert.equal(findCalls[0].exclude, '**/{.git,.tmp,node_modules,.vscode-test,bin,obj}/**');
});

test('resolveBuildTarget prefers the isolated metadata project when it explicitly includes the active file', async () => {
  const projectContents = {
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.csproj': '<Project><ItemGroup><Compile Remove="**\\Good*.cs" /></ItemGroup></Project>',
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.BoundedContextConsistency.csproj': '<Project><ItemGroup><Compile Include="scenarios\\violations\\metadata\\BadBoundedContextConsistency.cs" /></ItemGroup></Project>',
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.Ddd.csproj': '<Project><ItemGroup><Compile Include="scenarios\\violations\\ddd\\BadDddRules.cs" /></ItemGroup></Project>'
  };
  const vscode = {
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        readFile(uri) {
          return Promise.resolve(Buffer.from(projectContents[uri.fsPath] ?? '', 'utf8'));
        },
        stat() {
          return Promise.reject(new Error('not found'));
        }
      },
      findFiles(pattern) {
        if (pattern === 'samples/06-analyzer-workbench/scenarios/violations/metadata/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/violations/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/*.{csproj,fsproj}') {
          return Promise.resolve([
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.csproj' },
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.BoundedContextConsistency.csproj' },
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.Ddd.csproj' }
          ]);
        }

        return Promise.resolve([]);
      }
    },
    window: {
      activeTextEditor: {
        document: {
          uri: {
            fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\scenarios\\violations\\metadata\\BadBoundedContextConsistency.cs'
          }
        }
      }
    }
  };

  const target = await resolveBuildTarget(vscode, '');

  assert.equal(target, 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerViolations.BoundedContextConsistency.csproj');
});

test('resolveBuildTarget falls back to the nearest project when no exact include exists', async () => {
  const findCalls = [];
  const projectContents = {
    'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerWorkbench.csproj': '<Project><ItemGroup><Compile Remove="**\\Bad*.cs" /></ItemGroup></Project>'
  };
  const vscode = {
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        readFile(uri) {
          return Promise.resolve(Buffer.from(projectContents[uri.fsPath] ?? '', 'utf8'));
        },
        stat() {
          return Promise.reject(new Error('not found'));
        }
      },
      findFiles(pattern, exclude) {
        findCalls.push({ pattern, exclude });

        if (pattern === 'samples/06-analyzer-workbench/scenarios/good/checkout/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/good/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/scenarios/*.{csproj,fsproj}') {
          return Promise.resolve([]);
        }

        if (pattern === 'samples/06-analyzer-workbench/*.{csproj,fsproj}') {
          return Promise.resolve([
            { fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerWorkbench.csproj' }
          ]);
        }

        return Promise.resolve([]);
      }
    },
    window: {
      activeTextEditor: {
        document: {
          uri: {
            fsPath: 'c:\\repo\\samples\\06-analyzer-workbench\\scenarios\\good\\checkout\\GoodExample.cs'
          }
        }
      }
    }
  };

  const target = await resolveBuildTarget(vscode, '');

  assert.equal(target, 'c:\\repo\\samples\\06-analyzer-workbench\\Samples.Block06.AnalyzerWorkbench.csproj');
  assert.equal(findCalls[0].exclude, '**/{.git,.tmp,node_modules,.vscode-test,bin,obj}/**');
});

test('resolveBuildTarget uses the configured target when no active editor is available', async () => {
  const vscode = {
    Uri: createUriApi(),
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        stat() {
          return Promise.resolve({});
        }
      }
    },
    window: {}
  };

  const target = await resolveBuildTarget(vscode, 'sample-violations\\Banking.Sample.Violations.sln');

  assert.equal(target, 'c:\\repo\\sample-violations\\Banking.Sample.Violations.sln');
});

test('resolveBuildTarget ignores temporary folders and prefers shallow fallback solutions', async () => {
  const findCalls = [];
  const vscode = {
    Uri: createUriApi(),
    workspace: {
      workspaceFolders: [{ uri: { fsPath: 'c:\\repo' } }],
      fs: {
        stat() {
          return Promise.reject(new Error('not found'));
        }
      },
      findFiles(pattern, exclude) {
        findCalls.push({ pattern, exclude });

        if (pattern === '**/*.sln') {
          return Promise.resolve([
            { fsPath: 'c:\\repo\\nmolecules.examples\\Molecules.Examples.All.sln' },
            { fsPath: 'c:\\repo\\nmolecules-integrations\\nmolecules-vscode\\sample-workspace\\Banking.Sample.sln' }
          ]);
        }

        if (pattern === '**/*.{csproj,fsproj}') {
          return Promise.resolve([
            { fsPath: 'c:\\repo\\nmolecules\\src\\nMolecules.DDD\\nMolecules.DDD.csproj' }
          ]);
        }

        return Promise.resolve([]);
      }
    },
    window: {}
  };

  const target = await resolveBuildTarget(vscode, 'missing\\workspace.sln');

  assert.equal(target, 'c:\\repo\\nmolecules.examples\\Molecules.Examples.All.sln');
  assert.deepEqual(findCalls, [
    { pattern: '**/*.sln', exclude: '**/{.git,.tmp,node_modules,.vscode-test,bin,obj}/**' },
    { pattern: '**/*.{csproj,fsproj}', exclude: '**/{.git,.tmp,node_modules,.vscode-test,bin,obj}/**' }
  ]);
});
