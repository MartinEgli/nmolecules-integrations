using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace NMolecules.Analyzers.Test.AnalyzerQualityTests
{
    public class RuleCatalogQuality
    {
        private static readonly Regex RuleIdDefinitionRegex = new(
            "public const string\\s+(?<name>\\w+Id)\\s*=\\s*\"(?<id>XMolecules[A-Za-z]+\\d{4})\";",
            RegexOptions.Compiled);

        private static readonly Regex RuleDescriptorRegex = new(
            "public static readonly DiagnosticDescriptor\\s+\\w+\\s*=\\s*new\\(\\s*(?<idConst>\\w+Id)\\s*,",
            RegexOptions.Compiled);

        private static readonly Regex RuleIdRegex = new(
            "XMolecules[A-Za-z]+\\d{4}",
            RegexOptions.Compiled);

        [Fact]
        public void RuleIds_AreUniqueAcrossRulesFiles()
        {
            var ruleDefinitions = LoadRuleDefinitions();
            var duplicates = ruleDefinitions
                .GroupBy(it => it.RuleId)
                .Where(group => group.Count() > 1)
                .ToArray();

            Assert.True(
                duplicates.Length == 0,
                $"Duplicate rule IDs detected:{Environment.NewLine}{FormatGroups(duplicates)}");
        }

        [Fact]
        public void RuleDescriptors_ReferenceKnownRuleIds()
        {
            var ruleDefinitions = LoadRuleDefinitions()
                .GroupBy(it => it.ConstantName)
                .ToDictionary(group => group.Key, group => group.First());

            var unknownDescriptorReferences = LoadRuleFiles()
                .SelectMany(file =>
                    RuleDescriptorRegex.Matches(File.ReadAllText(file))
                        .Select(match => (File: file, IdConstant: match.Groups["idConst"].Value)))
                .Where(it => !ruleDefinitions.ContainsKey(it.IdConstant))
                .ToArray();

            Assert.True(
                unknownDescriptorReferences.Length == 0,
                $"Diagnostic descriptors referencing unknown ID constants:{Environment.NewLine}{string.Join(Environment.NewLine, unknownDescriptorReferences.Select(it => $"- {it.IdConstant} in {it.File}"))}");
        }

        [Fact]
        public void RuleIds_AreCoveredByAnalyzerReleaseCatalog()
        {
            var ruleIds = LoadRuleDefinitions().Select(it => it.RuleId).Distinct().ToHashSet(StringComparer.Ordinal);
            var releaseIds = LoadReleaseCatalogIds().ToHashSet(StringComparer.Ordinal);

            var missingInRelease = ruleIds
                .Where(id => !releaseIds.Contains(id))
                .OrderBy(it => it)
                .ToArray();

            Assert.True(
                missingInRelease.Length == 0,
                $"Rule IDs missing in AnalyzerReleases.*.md:{Environment.NewLine}{string.Join(Environment.NewLine, missingInRelease.Select(it => $"- {it}"))}");
        }

        [Fact]
        public void RuleIds_AreReferencedByAtLeastOneAnalyzerTest()
        {
            var definitions = LoadRuleDefinitions();
            var testContent = LoadAnalyzerTestSourceContent();

            var missingCoverage = definitions
                .Where(definition =>
                    !testContent.Contains(definition.RuleId, StringComparison.Ordinal) &&
                    !testContent.Contains($"Rules.{definition.ConstantName}", StringComparison.Ordinal))
                .OrderBy(it => it.RuleId)
                .ToArray();

            Assert.True(
                missingCoverage.Length == 0,
                $"Rule IDs without test reference:{Environment.NewLine}{string.Join(Environment.NewLine, missingCoverage.Select(it => $"- {it.RuleId} ({it.ConstantName})"))}");
        }

        private static string LoadAnalyzerTestSourceContent()
        {
            var testRoot = Path.Combine(FindRoslynRoot(), "test", "nMolecules.Analyzers.Test");
            var files = Directory.EnumerateFiles(testRoot, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}Verifiers{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}AnalyzerQualityTests{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));

            return string.Join(Environment.NewLine, files.Select(File.ReadAllText));
        }

        private static string[] LoadRuleFiles()
        {
            var analyzerRoot = Path.Combine(FindRoslynRoot(), "src", "nMolecules.Analyzers", "nMolecules.Analyzers");
            return Directory.EnumerateFiles(analyzerRoot, "Rules.cs", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static IReadOnlyList<RuleDefinition> LoadRuleDefinitions()
        {
            return LoadRuleFiles()
                .SelectMany(file =>
                    RuleIdDefinitionRegex.Matches(File.ReadAllText(file))
                        .Select(match => new RuleDefinition(
                            match.Groups["name"].Value,
                            match.Groups["id"].Value,
                            file)))
                .ToArray();
        }

        private static IEnumerable<string> LoadReleaseCatalogIds()
        {
            var root = FindRoslynRoot();
            var files = new[]
            {
                Path.Combine(root, "src", "nMolecules.Analyzers", "nMolecules.Analyzers", "AnalyzerReleases.Shipped.md"),
                Path.Combine(root, "src", "nMolecules.Analyzers", "nMolecules.Analyzers", "AnalyzerReleases.Unshipped.md")
            };

            return files
                .SelectMany(file => RuleIdRegex.Matches(File.ReadAllText(file)).Select(match => match.Value))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal);
        }

        private static string FindRoslynRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current is not null)
            {
                var hasAnalyzerSource = Directory.Exists(Path.Combine(current.FullName, "src", "nMolecules.Analyzers"));
                var hasAnalyzerTests = Directory.Exists(Path.Combine(current.FullName, "test", "nMolecules.Analyzers.Test"));
                if (hasAnalyzerSource && hasAnalyzerTests)
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Could not locate nMolecules.Roslyn repository root.");
        }

        private static string FormatGroups(IEnumerable<IGrouping<string, RuleDefinition>> groups) =>
            string.Join(
                Environment.NewLine,
                groups.Select(group =>
                    $"- {group.Key}: {string.Join(", ", group.Select(it => $"{it.ConstantName} ({it.SourceFile})"))}"));

        private sealed class RuleDefinition
        {
            public RuleDefinition(string constantName, string ruleId, string sourceFile)
            {
                ConstantName = constantName;
                RuleId = ruleId;
                SourceFile = sourceFile;
            }

            public string ConstantName { get; }
            public string RuleId { get; }
            public string SourceFile { get; }
        }
    }
}
