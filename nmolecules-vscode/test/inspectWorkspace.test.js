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

test('inspectProjectFile treats project references to core libraries as core usage', () => {
  const content = `
<Project>
  <ItemGroup>
    <ProjectReference Include="..\\..\\nmolecules\\src\\nMolecules.Architecture\\nMolecules.Architecture.csproj" />
  </ItemGroup>
</Project>`;

  const inspection = inspectProjectFile('src/Architecture/Architecture.csproj', content);

  assert.equal(inspection.usesAnalyzerPackage, false);
  assert.equal(inspection.usesAnalyzerProject, false);
  assert.equal(inspection.usesCorePackage, false);
  assert.equal(inspection.usesCoreProject, true);
});

test('buildWorkspaceReport emits missing reference recommendations when nothing is wired', () => {
  const report = buildWorkspaceReport([
    {
      path: 'src/Empty/Empty.csproj',
      content: '<Project Sdk="Microsoft.NET.Sdk"></Project>'
    }
  ]);

  assert.equal(report.totalSolutions, 0);
  assert.equal(report.totalProjects, 1);
  assert.equal(report.analyzerPackageProjects, 0);
  assert.equal(report.analyzerProjectReferenceProjects, 0);
  assert.equal(report.coreReferenceProjects, 0);
  assert.match(report.recommendations.join('\n'), /No nMolecules analyzer reference was found/);
  assert.match(report.recommendations.join('\n'), /No nMolecules core package or project reference was found/);
  assert.match(report.recommendations.join('\n'), /No solution file was found/);
});

test('buildWorkspaceReport returns early when no project files exist', () => {
  const report = buildWorkspaceReport([{ path: 'workspace.sln' }]);

  assert.equal(report.totalSolutions, 1);
  assert.equal(report.totalProjects, 0);
  assert.deepEqual(report.recommendations, ['No C# project files were found in the current workspace.']);
});

test('buildWorkspaceReport handles larger workspaces consistently', () => {
  const entries = [{ path: 'workspace.sln' }];

  for (let index = 0; index < 24; index += 1) {
    entries.push({
      path: `src/PackageProject${index}/PackageProject${index}.csproj`,
      content: '<Project><ItemGroup><PackageReference Include="NMolecules.Analyzers" Version="0.0.0.6" /><PackageReference Include="NMolecules.DDD" Version="0.2.2" /></ItemGroup></Project>'
    });
  }

  for (let index = 0; index < 18; index += 1) {
    entries.push({
      path: `src/ProjectReference${index}/ProjectReference${index}.csproj`,
      content: '<Project><ItemGroup><ProjectReference Include="..\\..\\nmolecules-roslyn\\src\\nMolecules.Analyzers\\nMolecules.Analyzers.csproj" /><ProjectReference Include="..\\..\\nmolecules\\src\\nMolecules.DDD\\nMolecules.DDD.csproj" /></ItemGroup></Project>'
    });
  }

  for (let index = 0; index < 7; index += 1) {
    entries.push({
      path: `src/PlainProject${index}/PlainProject${index}.csproj`,
      content: '<Project Sdk="Microsoft.NET.Sdk"></Project>'
    });
  }

  const report = buildWorkspaceReport(entries);

  assert.equal(report.totalSolutions, 1);
  assert.equal(report.totalProjects, 49);
  assert.equal(report.analyzerPackageProjects, 24);
  assert.equal(report.analyzerProjectReferenceProjects, 18);
  assert.equal(report.coreReferenceProjects, 42);
  assert.match(formatWorkspaceReport(report), /Projects: 49/);
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

test('formatWorkspaceReport preserves recommendation ordering', () => {
  const recommendations = [
    'First recommendation.',
    'Second recommendation.',
    'Third recommendation.'
  ];
  const formatted = formatWorkspaceReport({
    totalSolutions: 2,
    totalProjects: 4,
    analyzerPackageProjects: 0,
    analyzerProjectReferenceProjects: 1,
    coreReferenceProjects: 2,
    recommendations
  });

  const firstIndex = formatted.indexOf(recommendations[0]);
  const secondIndex = formatted.indexOf(recommendations[1]);
  const thirdIndex = formatted.indexOf(recommendations[2]);

  assert.ok(firstIndex < secondIndex);
  assert.ok(secondIndex < thirdIndex);
});

test('getDocumentationCandidates normalizes the docs root', () => {
  const candidates = getDocumentationCandidates('docs/');

  assert.deepEqual(candidates, ['docs/architecture.md', 'docs/layer-matrix.md', 'README.md']);
});

test('getDocumentationCandidates falls back to docs for blank roots', () => {
  assert.deepEqual(getDocumentationCandidates('   '), ['docs/architecture.md', 'docs/layer-matrix.md', 'README.md']);
});
