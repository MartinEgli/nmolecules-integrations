'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');

const manifest = require('../package.json');

test('package manifest exposes the expected commands and activation events', () => {
  const commands = manifest.contributes.commands.map((command) => command.command);

  assert.deepEqual(commands, ['nmolecules.inspectWorkspace', 'nmolecules.openWorkspaceDocs']);
  assert.ok(manifest.activationEvents.includes('onLanguage:csharp'));
  assert.equal(manifest.main, './src/extension.js');
});

test('package manifest exposes configuration and scripts for testing', () => {
  assert.equal(manifest.engines.vscode, '^1.96.0');
  assert.equal(manifest.scripts.test, 'node --test ./test/*.test.js');
  assert.equal(manifest.scripts.smoke, 'node ./test/package-manifest.test.js');
  assert.equal(manifest.contributes.configuration.title, 'nMolecules');
  assert.equal(manifest.contributes.configuration.properties['nmolecules.autoInspectOnStartup'].default, false);
  assert.equal(manifest.contributes.configuration.properties['nmolecules.docsRoot'].default, 'docs');
  assert.equal(manifest.contributes.configuration.properties['nmolecules.traceLevel'].default, 'basic');
});
