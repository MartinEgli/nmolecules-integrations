using System;
using System.Collections.Generic;
using System.Linq;

namespace NMolecules.Analyzers
{
    public sealed class RuleCluster
    {
        public RuleCluster(string? id, string? displayName, string? category, IEnumerable<string>? ruleIdPrefixes)
        {
            Id = id ?? string.Empty;
            DisplayName = displayName ?? string.Empty;
            Category = category ?? string.Empty;
            RuleIdPrefixes = (ruleIdPrefixes ?? Enumerable.Empty<string>()).ToArray();
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string Category { get; }
        public IReadOnlyList<string> RuleIdPrefixes { get; }

        public bool ContainsRuleId(string? ruleId) =>
            ruleId is not null
            && RuleIdPrefixes.Any(prefix => ruleId.StartsWith(prefix, StringComparison.Ordinal));
    }

    public static class AnalyzerRuleClusters
    {
        public static RuleCluster Ddd { get; } = new(
            "DDD",
            "Domain-Driven Design",
            "DDD",
            new[]
            {
                "XMoleculesAggregateRoot",
                "XMoleculesApplicationService",
                "XMoleculesBoundedContext",
                "XMoleculesDomainService",
                "XMoleculesEntity",
                "XMoleculesFactory",
                "XMoleculesIdentity",
                "XMoleculesModule",
                "XMoleculesRepository",
                "XMoleculesService",
                "XMoleculesValueObject"
            });

        public static RuleCluster Events { get; } = new(
            "Events",
            "Events",
            "Events",
            new[] { "XMoleculesDomainEvent" });

        public static RuleCluster Architecture { get; } = new(
            "Architecture",
            "Architecture Styles",
            "Architecture",
            new[]
            {
                "XMoleculesCQRS",
                "XMoleculesCrossStyle",
                "XMoleculesEventStorming",
                "XMoleculesHexagonal",
                "XMoleculesLayered",
                "XMoleculesMicroservices",
                "XMoleculesOnion"
            });

        public static RuleCluster Bricks { get; } = new(
            "Bricks",
            "Bricks",
            "Architecture",
            new[] { "XMoleculesBricks" });

        public static IReadOnlyList<RuleCluster> All => new[] { Ddd, Events, Architecture, Bricks };

        public static RuleCluster? FindClusterForRuleId(string? ruleId) =>
            All.FirstOrDefault(cluster => cluster.ContainsRuleId(ruleId));
    }
}
