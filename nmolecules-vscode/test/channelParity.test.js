'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const vscodeRoot = path.join(__dirname, '..');
const integrationsRoot = path.join(vscodeRoot, '..');

function read(relativePathFromIntegrationsRoot) {
  return fs.readFileSync(path.join(integrationsRoot, relativePathFromIntegrationsRoot), 'utf8');
}

test('channel parity checklist exists and covers both IDE channels', () => {
  const content = read(path.join('docs', 'ide-channel-parity-checklist.md'));

  assert.match(content, /Visual Studio/);
  assert.match(content, /VS Code/);
  assert.match(content, /sample-workspace/);
  assert.match(content, /sample-violations/);
  assert.match(content, /nMolecules\.Analyzers\.VisualStudio\.vsix/);
  assert.match(content, /nMolecules\.VSCode\.vsix/);
});

test('sample workspace readmes document both VS Code and Visual Studio entry points', () => {
  const sampleWorkspaceReadme = read(path.join('nmolecules-vscode', 'sample-workspace', 'README.md'));
  const sampleViolationsReadme = read(path.join('nmolecules-vscode', 'sample-violations', 'README.md'));

  assert.match(sampleWorkspaceReadme, /Open In VS Code/);
  assert.match(sampleWorkspaceReadme, /Open In Visual Studio/);
  assert.match(sampleWorkspaceReadme, /nmolecules-sample\.code-workspace/);
  assert.match(sampleWorkspaceReadme, /Banking\.Sample\.sln/);

  assert.match(sampleViolationsReadme, /Open In VS Code/);
  assert.match(sampleViolationsReadme, /Open In Visual Studio/);
  assert.match(sampleViolationsReadme, /nmolecules-violations\.code-workspace/);
  assert.match(sampleViolationsReadme, /Banking\.Sample\.Violations\.sln/);
});

test('packaging flow preserves stable installer artifact names for both channels', () => {
  const packagingScript = read(path.join('tools', 'packaging', 'build-ide-installers.ps1'));

  assert.match(packagingScript, /nMolecules\.Analyzers\.VisualStudio\.vsix/);
  assert.match(packagingScript, /nMolecules\.VSCode\.vsix/);
  assert.match(packagingScript, /install-visual-studio-extension\.cmd/);
  assert.match(packagingScript, /install-vscode-extension\.cmd/);
  assert.match(packagingScript, /Visual Studio setup:/);
  assert.match(packagingScript, /VS Code setup:/);
});

test('setup executables keep the documented CLI contracts and default artifact names', () => {
  const visualStudioSetupProgram = read(path.join('tools', 'installers', 'nMolecules.Setup.VisualStudio', 'Program.cs'));
  const vsCodeSetupProgram = read(path.join('tools', 'installers', 'nMolecules.Setup.VSCode', 'Program.cs'));

  assert.match(visualStudioSetupProgram, /nMolecules\.Analyzers\.VisualStudio\.vsix/);
  assert.match(visualStudioSetupProgram, /\[--vsix <path>\] \[--installer <path>\] \[--quiet\]/);

  assert.match(vsCodeSetupProgram, /nMolecules\.VSCode\.vsix/);
  assert.match(vsCodeSetupProgram, /\[--vsix <path>\] \[--code <path>\] \[--no-force\]/);
});
