using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.BricksAnalyzers
{
    public static class Rules
    {
        public const string BrickRuleViolationId = "XMoleculesBricks0001";
        public const string BrickRuleConfigurationId = "XMoleculesBricks0002";

        public static readonly DiagnosticDescriptor BrickRuleViolationRule = new(
            BrickRuleViolationId,
            "Brick rules must be honored",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Generic brick rules define dependency policies between custom architectural roles.");

        public static readonly DiagnosticDescriptor BrickRuleConfigurationRule = new(
            BrickRuleConfigurationId,
            "Brick rule configuration must be valid",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "A brick rule declaration is incomplete or invalid and cannot be evaluated safely.");
    }
}
