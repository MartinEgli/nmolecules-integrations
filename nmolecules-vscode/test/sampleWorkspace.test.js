'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const { buildWorkspaceReport } = require('../src/inspectWorkspace');

const sampleRoot = path.join(__dirname, '..', 'sample-workspace');
const violationsRoot = path.join(__dirname, '..', 'sample-violations');

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
  assert.equal(fs.existsSync(path.join(sampleRoot, 'docs', 'constellation-catalog.md')), true);
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
    'The workspace already exposes nMolecules references. Refresh diagnostics to populate the VS Code Problems view.'
  ]);
});

test('sample workspace projects are all wired to the local analyzer project', () => {
  const report = buildWorkspaceReport(enumerateFiles(sampleRoot));

  const nonAnalyzerProjects = report.projects.filter((project) => !project.usesAnalyzerProject).map((project) => project.projectPath);
  assert.deepEqual(nonAnalyzerProjects, []);
});

test('violations workspace contains dedicated extension usage documentation', () => {
  assert.equal(fs.existsSync(path.join(violationsRoot, 'README.md')), true);
  assert.equal(fs.existsSync(path.join(violationsRoot, 'docs', 'architecture.md')), true);
  assert.equal(fs.existsSync(path.join(violationsRoot, 'docs', 'constellation-catalog.md')), true);
  assert.equal(fs.existsSync(path.join(violationsRoot, 'docs', 'expected-diagnostics.md')), true);
  assert.equal(fs.existsSync(path.join(violationsRoot, 'docs', 'exact-diagnostic-details.md')), true);
  assert.equal(fs.existsSync(path.join(violationsRoot, 'docs', 'using-the-extension.md')), true);
  assert.equal(fs.existsSync(path.join(violationsRoot, 'nmolecules-violations.code-workspace')), true);
});

test('violations diagnostics catalog uses current layered rule IDs', () => {
  const expectedDiagnosticsPath = path.join(violationsRoot, 'docs', 'expected-diagnostics.md');
  const content = fs.readFileSync(expectedDiagnosticsPath, 'utf8');

  assert.match(content, /XMoleculesLayered0001/);
  assert.match(content, /XMoleculesLayered0002/);
  assert.match(content, /XMoleculesLayered0003/);
  assert.doesNotMatch(content, /XMoleculesDomainLayer0001/);
  assert.doesNotMatch(content, /XMoleculesDomainLayer0002/);
  assert.doesNotMatch(content, /XMoleculesDomainLayer0003/);
  assert.doesNotMatch(content, /XMoleculesApplicationLayer0003/);
});

test('violations workspace uses local analyzer project references in every project', () => {
  const report = buildWorkspaceReport(enumerateFiles(violationsRoot));

  assert.equal(report.totalSolutions, 1);
  assert.equal(report.totalProjects, 7);
  assert.equal(report.analyzerPackageProjects, 0);
  assert.equal(report.analyzerProjectReferenceProjects, 7);
  assert.equal(report.coreReferenceProjects, 7);
});

test('sample catalogs include multiple valid and invalid example files', () => {
  const validExampleFiles = [
    path.join(sampleRoot, 'src', 'Banking.Domain', 'ConstellationExamples.cs'),
    path.join(sampleRoot, 'src', 'Banking.Application', 'ApplicationConstellationExamples.cs'),
    path.join(sampleRoot, 'src', 'Banking.Infrastructure', 'InfrastructureConstellationExamples.cs'),
    path.join(sampleRoot, 'src', 'Banking.Api', 'UserInterfaceConstellationExamples.cs')
  ];
  const invalidExampleFiles = [
    path.join(violationsRoot, 'src', 'Banking.Violations.Domain', 'IsolatedValueObjectViolations.cs'),
    path.join(violationsRoot, 'src', 'Banking.Violations.Domain', 'IdentityAndServiceViolations.cs'),
    path.join(violationsRoot, 'src', 'Banking.Violations.Application', 'ApplicationViolationCatalog.cs'),
    path.join(violationsRoot, 'src', 'Banking.Violations.Infrastructure', 'FactoryAndLayerViolations.cs'),
    path.join(violationsRoot, 'src', 'Banking.Violations.Infrastructure', 'CombinedViolationScenarios.cs')
  ];

  for (const filePath of [...validExampleFiles, ...invalidExampleFiles]) {
    assert.equal(fs.existsSync(filePath), true, filePath);
  }
});
