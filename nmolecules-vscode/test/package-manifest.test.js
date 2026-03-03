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
