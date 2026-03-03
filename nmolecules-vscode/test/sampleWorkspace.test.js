'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const { buildWorkspaceReport } = require('../src/inspectWorkspace');

const sampleRoot = path.join(__dirname, '..', 'sample-workspace');

function enumerateFiles(root) {
  const entries = [];

  function walk(currentPath) {
    for (const entry of fs.readdirSync(currentPath, { withFileTypes: true })) {
      const fullPath = path.join(currentPath, entry.name);

      if (entry.isDirectory()) {
        walk(fullPath);
        continue;
      }

      const relativePath = path.relative(sampleRoot, fullPath).replace(/\\/g, '/');
      const content = /\.(csproj|fsproj|sln)$/i.test(entry.name) ? fs.readFileSync(fullPath, 'utf8') : undefined;
      entries.push({
        path: relativePath,
        content
      });
    }
  }

  walk(root);
  return entries;
}

test('sample workspace contains the expected documentation entry points', () => {
  assert.equal(fs.existsSync(path.join(sampleRoot, 'README.md')), true);
  assert.equal(fs.existsSync(path.join(sampleRoot, 'docs', 'architecture.md')), true);
  assert.equal(fs.existsSync(path.join(sampleRoot, 'docs', 'layer-matrix.md')), true);
  assert.equal(fs.existsSync(path.join(sampleRoot, 'docs', 'expected-inspection-output.md')), true);
  assert.equal(fs.existsSync(path.join(sampleRoot, 'nmolecules-sample.code-workspace')), true);
});

test('sample workspace inspection report matches the documented expectation', () => {
  const report = buildWorkspaceReport(enumerateFiles(sampleRoot));

  assert.equal(report.totalSolutions, 1);
  assert.equal(report.totalProjects, 4);
  assert.equal(report.analyzerPackageProjects, 0);
  assert.equal(report.analyzerProjectReferenceProjects, 4);
  assert.equal(report.coreReferenceProjects, 4);
  assert.deepEqual(report.recommendations, [
    'The workspace already exposes nMolecules references. The next useful step is wiring diagnostics into the VS Code Problems view.'
  ]);
});

test('sample workspace projects are all wired to the local analyzer project', () => {
  const report = buildWorkspaceReport(enumerateFiles(sampleRoot));

  const nonAnalyzerProjects = report.projects.filter((project) => !project.usesAnalyzerProject).map((project) => project.projectPath);
  assert.deepEqual(nonAnalyzerProjects, []);
});
