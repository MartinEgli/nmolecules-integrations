using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.BricksAnalyzers
{
    internal static class BricksDiagnosticProperties
    {
        public const string ViolationKind = "BrickViolationKind";
        public const string ConfigurationKind = "BrickConfigurationKind";
        public const string RuleId = "RuleId";
        public const string SourceRole = "SourceRole";
        public const string TargetRole = "TargetRole";
        public const string Source = "Source";
        public const string Target = "Target";
        public const string ContractKind = "ContractKind";

        public static Diagnostic Create(
            DiagnosticDescriptor descriptor,
            Location location,
            string message,
            string? violationKind = null,
            string? configurationKind = null,
            string? ruleId = null,
            string? sourceRole = null,
            string? targetRole = null,
            string? source = null,
            string? target = null,
            string? contractKind = null)
        {
            return Diagnostic.Create(
                descriptor,
                location,
                additionalLocations: null,
                properties: CreateProperties(
                    violationKind,
                    configurationKind,
                    ruleId,
                    sourceRole,
                    targetRole,
                    source,
                    target,
                    contractKind),
                messageArgs: new object[] { message });
        }

        private static ImmutableDictionary<string, string?> CreateProperties(
            string? violationKind,
            string? configurationKind,
            string? ruleId,
            string? sourceRole,
            string? targetRole,
            string? source,
            string? target,
            string? contractKind)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string?>();
            Add(builder, ViolationKind, violationKind);
            Add(builder, ConfigurationKind, configurationKind);
            Add(builder, RuleId, ruleId);
            Add(builder, SourceRole, sourceRole);
            Add(builder, TargetRole, targetRole);
            Add(builder, Source, source);
            Add(builder, Target, target);
            Add(builder, ContractKind, contractKind);
            return builder.ToImmutable();
        }

        private static void Add(ImmutableDictionary<string, string?>.Builder builder, string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                builder[key] = value;
            }
        }
    }
}
