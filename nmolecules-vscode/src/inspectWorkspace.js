'use strict';

const ANALYZER_REFERENCES = ['NMolecules.Analyzers', 'nMolecules.Analyzers'];
const CORE_REFERENCES = ['NMolecules.DDD', 'nMolecules.DDD', 'NMolecules.Architecture', 'nMolecules.Architecture'];

function inspectProjectFile(projectPath, content) {
  const packageReferences = [...content.matchAll(/<PackageReference\s+Include="([^"]+)"/g)].map((match) => match[1]);
  const projectReferences = [...content.matchAll(/<ProjectReference\s+Include="([^"]+)"/g)].map((match) => match[1]);

  const usesAnalyzerPackage = packageReferences.some((name) => ANALYZER_REFERENCES.includes(name));
  const usesCorePackage = packageReferences.some((name) => CORE_REFERENCES.includes(name));
  const usesAnalyzerProject = projectReferences.some((path) => /nMolecules\.Analyzers/i.test(path));
  const usesCoreProject = projectReferences.some((path) => /nMolecules\.(DDD|Architecture)/i.test(path));

  return {
    projectPath,
    packageReferences,
    projectReferences,
    usesAnalyzerPackage,
    usesCorePackage,
    usesAnalyzerProject,
    usesCoreProject
  };
}

function buildWorkspaceReport(fileEntries) {
  const solutionFiles = fileEntries.filter((entry) => entry.path.toLowerCase().endsWith('.sln'));
  const projectFiles = fileEntries
    .filter((entry) => /\.(csproj|fsproj)$/i.test(entry.path))
    .map((entry) => inspectProjectFile(entry.path, entry.content ?? ''));

  const analyzerPackageProjects = projectFiles.filter((project) => project.usesAnalyzerPackage);
  const analyzerProjectReferenceProjects = projectFiles.filter((project) => project.usesAnalyzerProject);
  const coreReferenceProjects = projectFiles.filter((project) => project.usesCorePackage || project.usesCoreProject);

  return {
    totalSolutions: solutionFiles.length,
    totalProjects: projectFiles.length,
    analyzerPackageProjects: analyzerPackageProjects.length,
    analyzerProjectReferenceProjects: analyzerProjectReferenceProjects.length,
    coreReferenceProjects: coreReferenceProjects.length,
    projects: projectFiles,
    recommendations: buildRecommendations({
      totalSolutions: solutionFiles.length,
      totalProjects: projectFiles.length,
      analyzerPackageProjects: analyzerPackageProjects.length,
      analyzerProjectReferenceProjects: analyzerProjectReferenceProjects.length,
      coreReferenceProjects: coreReferenceProjects.length
    })
  };
}

function buildRecommendations(report) {
  const recommendations = [];

  if (report.totalProjects === 0) {
    recommendations.push('No C# project files were found in the current workspace.');
    return recommendations;
  }

  if (report.analyzerPackageProjects === 0 && report.analyzerProjectReferenceProjects === 0) {
    recommendations.push('No nMolecules analyzer reference was found. Add either the analyzer package or a local analyzer project reference.');
  }

  if (report.analyzerPackageProjects > 0 && report.analyzerProjectReferenceProjects > 0) {
    recommendations.push('Mixed analyzer reference strategies were detected. Prefer either package references or local project references per workspace.');
  }

  if (report.coreReferenceProjects === 0) {
    recommendations.push('No nMolecules core package or project reference was found. The extension can inspect the workspace, but DDD markers are not wired into any project yet.');
  }

  if (report.totalSolutions === 0) {
    recommendations.push('No solution file was found. Consider keeping at least one solution file for editor onboarding and analyzer validation.');
  }

  if (recommendations.length === 0) {
    recommendations.push('The workspace already exposes nMolecules references. Refresh diagnostics to populate the VS Code Problems view.');
  }

  return recommendations;
}

function formatWorkspaceReport(report) {
  const lines = [
    'nMolecules workspace inspection',
    '',
    `Solutions: ${report.totalSolutions}`,
    `Projects: ${report.totalProjects}`,
    `Projects using analyzer package refs: ${report.analyzerPackageProjects}`,
    `Projects using analyzer project refs: ${report.analyzerProjectReferenceProjects}`,
    `Projects using nMolecules core refs: ${report.coreReferenceProjects}`,
    '',
    'Recommendations:'
  ];

  for (const recommendation of report.recommendations) {
    lines.push(`- ${recommendation}`);
  }

  return lines.join('\n');
}

module.exports = {
  ANALYZER_REFERENCES,
  CORE_REFERENCES,
  inspectProjectFile,
  buildWorkspaceReport,
  buildRecommendations,
  formatWorkspaceReport
};
