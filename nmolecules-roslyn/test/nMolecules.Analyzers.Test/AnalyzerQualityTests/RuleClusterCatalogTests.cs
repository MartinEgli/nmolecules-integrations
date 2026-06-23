using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NMolecules.Analyzers;
using Xunit;

namespace NMolecules.Analyzers.Test.AnalyzerQualityTests
{
    public class RuleClusterCatalogTests
    {
        private static readonly Regex RuleIdDefinitionRegex = new(
            "public const string\\s+\\w+Id\\s*=\\s*\"(?<id>XMolecules[A-Za-z]+\\d{4})\";",
            RegexOptions.Compiled);

        [Fact]
        public void RuleClusterCopiesPrefixesAndMatchesRuleIds()
        {
            var prefixes = new[] { "XMoleculesBricks", "XMoleculesBricksRuntime" };
            var cluster = new RuleCluster("Bricks", "Bricks", "Architecture", prefixes);
            prefixes[0] = "Mutated";

            Assert.Equal("Bricks", cluster.Id);
            Assert.Equal("Bricks", cluster.DisplayName);
            Assert.Equal("Architecture", cluster.Category);
            Assert.Equal(new[] { "XMoleculesBricks", "XMoleculesBricksRuntime" }, cluster.RuleIdPrefixes.ToArray());
            Assert.True(cluster.ContainsRuleId("XMoleculesBricks0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesBricksRuntime0700"));
            Assert.False(cluster.ContainsRuleId("XMoleculesDDD0001"));
        }

        [Fact]
        public void RuleClusterNormalizesNullValues()
        {
            var cluster = new RuleCluster(null, null, null, null);

            Assert.Equal(string.Empty, cluster.Id);
            Assert.Equal(string.Empty, cluster.DisplayName);
            Assert.Equal(string.Empty, cluster.Category);
            Assert.Empty(cluster.RuleIdPrefixes);
            Assert.False(cluster.ContainsRuleId(null));
        }

        [Fact]
        public void BuiltInClustersAreReturnedInStableOrderAndCopied()
        {
            var clusters = AnalyzerRuleClusters.All.ToArray();
            clusters[0] = new RuleCluster("Other", "Other", "Other", null);

            Assert.Equal(new[] { "DDD", "Events", "Architecture", "Bricks" }, AnalyzerRuleClusters.All.Select(cluster => cluster.Id).ToArray());
            Assert.Same(AnalyzerRuleClusters.Bricks, AnalyzerRuleClusters.FindClusterForRuleId("XMoleculesBricks0001"));
            Assert.Null(AnalyzerRuleClusters.FindClusterForRuleId("Unknown0001"));
        }

        [Fact]
        public void EveryAnalyzerRuleIdIsAssignedToExactlyOneCluster()
        {
            var unassigned = LoadRuleIds()
                .Where(ruleId => AnalyzerRuleClusters.All.Count(cluster => cluster.ContainsRuleId(ruleId)) != 1)
                .OrderBy(ruleId => ruleId, StringComparer.Ordinal)
                .ToArray();

            Assert.True(
                unassigned.Length == 0,
                $"Rule IDs without exactly one cluster:{Environment.NewLine}{string.Join(Environment.NewLine, unassigned.Select(ruleId => $"- {ruleId}"))}");
        }

        [Fact]
        public void ArchitectureClusterIncludesStyleAndOperationalAnalyzers()
        {
            var cluster = AnalyzerRuleClusters.Architecture;

            Assert.True(cluster.ContainsRuleId("XMoleculesLayered0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesOnion0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesHexagonal0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesCQRS0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesCrossStyle0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesEventStorming0001"));
            Assert.True(cluster.ContainsRuleId("XMoleculesMicroservices0001"));
        }

        private static string[] LoadRuleIds()
        {
            var analyzerRoot = Path.Combine(FindRoslynRoot(), "src", "nMolecules.Analyzers", "nMolecules.Analyzers");
            return Directory.EnumerateFiles(analyzerRoot, "Rules.cs", SearchOption.AllDirectories)
                .SelectMany(file => RuleIdDefinitionRegex.Matches(File.ReadAllText(file)).Select(match => match.Groups["id"].Value))
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }

        private static string FindRoslynRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current is not null)
            {
                if (Directory.Exists(Path.Combine(current.FullName, "src", "nMolecules.Analyzers"))
                    && Directory.Exists(Path.Combine(current.FullName, "test", "nMolecules.Analyzers.Test")))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new InvalidOperationException("Could not locate nMolecules.Roslyn repository root.");
        }
    }
}
