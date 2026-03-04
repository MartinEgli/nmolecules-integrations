'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');

const manifest = require('../package.json');

test('package manifest exposes the expected commands and activation events', () => {
  const commands = manifest.contributes.commands.map((command) => command.command);

  assert.deepEqual(commands, [
    'nmolecules.inspectWorkspace',
    'nmolecules.refreshDiagnostics',
    'nmolecules.openWorkspaceDocs',
    'nmolecules.openRuleCatalog',
    'nmolecules.showDiagnosticsSummary'
  ]);
  assert.ok(manifest.activationEvents.includes('onLanguage:csharp'));
  assert.ok(manifest.activationEvents.includes('onCommand:nmolecules.refreshDiagnostics'));
  assert.ok(manifest.activationEvents.includes('onCommand:nmolecules.openRuleCatalog'));
  assert.ok(manifest.activationEvents.includes('onCommand:nmolecules.showDiagnosticsSummary'));
  assert.equal(manifest.main, './src/extension.js');
});

test('package manifest exposes configuration and scripts for testing', () => {
  assert.equal(manifest.engines.vscode, '^1.96.0');
  assert.equal(manifest.scripts.test, 'node --test ./test/*.test.js');
  assert.equal(manifest.scripts.smoke, 'node ./test/package-manifest.test.js');
  assert.equal(manifest.scripts['test:host'], 'node ./test-host/runTest.js');
  assert.equal(manifest.contributes.configuration.title, 'nMolecules');
  assert.equal(manifest.contributes.configuration.properties['nmolecules.autoInspectOnStartup'].default, false);
  assert.equal(manifest.contributes.configuration.properties['nmolecules.refreshDiagnosticsOnStartup'].default, false);
  assert.equal(manifest.contributes.configuration.properties['nmolecules.diagnosticsTarget'].default, '');
  assert.deepEqual(manifest.contributes.configuration.properties['nmolecules.diagnosticsBuildArguments'].default, ['-v', 'minimal']);
  assert.equal(manifest.contributes.configuration.properties['nmolecules.docsRoot'].default, 'docs');
  assert.equal(manifest.contributes.configuration.properties['nmolecules.traceLevel'].default, 'basic');
  assert.equal(manifest.devDependencies['@vscode/test-electron'], '^2.5.2');
});
