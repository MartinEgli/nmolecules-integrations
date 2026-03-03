'use strict';

const test = require('node:test');
const assert = require('node:assert/strict');

const { inspectProjectFile, buildWorkspaceReport, formatWorkspaceReport } = require('../src/inspectWorkspace');
const { getDocumentationCandidates } = require('../src/docs');

test('inspectProjectFile detects package and project references', () => {
  const content = `
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="NMolecules.Analyzers" Version="0.0.0.6" />
    <PackageReference Include="NMolecules.DDD" Version="0.2.2" />
    <ProjectReference Include="..\\..\\nmolecules-roslyn\\src\\nMolecules.Analyzers\\nMolecules.Analyzers.csproj" />
  </ItemGroup>
</Project>`;

  const inspection = inspectProjectFile('src/Banking/Banking.csproj', content);

  assert.equal(inspection.usesAnalyzerPackage, true);
  assert.equal(inspection.usesCorePackage, true);
  assert.equal(inspection.usesAnalyzerProject, true);
  assert.equal(inspection.usesCoreProject, false);
});

test('buildWorkspaceReport summarizes mixed workspace strategies', () => {
  const report = buildWorkspaceReport([
    { path: 'nMolecules.Roslyn.sln' },
    {
      path: 'src/Banking/Banking.csproj',
      content: '<Project><ItemGroup><PackageReference Include="NMolecules.Analyzers" Version="0.0.0.6" /><PackageReference Include="NMolecules.DDD" Version="0.2.2" /></ItemGroup></Project>'
    },
    {
      path: 'src/Examples/Examples.csproj',
      content: '<Project><ItemGroup><ProjectReference Include="..\\..\\nmolecules-roslyn\\src\\nMolecules.Analyzers\\nMolecules.Analyzers.csproj" /><ProjectReference Include="..\\..\\nmolecules\\src\\nMolecules.DDD\\nMolecules.DDD.csproj" /></ItemGroup></Project>'
    }
  ]);

  assert.equal(report.totalSolutions, 1);
  assert.equal(report.totalProjects, 2);
  assert.equal(report.analyzerPackageProjects, 1);
  assert.equal(report.analyzerProjectReferenceProjects, 1);
  assert.equal(report.coreReferenceProjects, 2);
  assert.match(report.recommendations.join('\n'), /Mixed analyzer reference strategies/);
});

test('formatWorkspaceReport returns a readable summary', () => {
  const formatted = formatWorkspaceReport({
    totalSolutions: 1,
    totalProjects: 3,
    analyzerPackageProjects: 1,
    analyzerProjectReferenceProjects: 2,
    coreReferenceProjects: 3,
    recommendations: ['Use a single reference strategy per workspace.']
  });

  assert.match(formatted, /nMolecules workspace inspection/);
  assert.match(formatted, /Projects: 3/);
  assert.match(formatted, /Use a single reference strategy per workspace/);
});

test('getDocumentationCandidates normalizes the docs root', () => {
  const candidates = getDocumentationCandidates('docs/');

  assert.deepEqual(candidates, ['docs/architecture.md', 'docs/layer-matrix.md', 'README.md']);
});
