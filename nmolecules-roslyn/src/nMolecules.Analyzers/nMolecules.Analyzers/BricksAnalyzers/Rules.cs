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
            DiagnosticDescriptions.Create(
                "A dependency or usage pattern violates a declared brick rule between custom architectural roles.",
                "Brick rules define explicit role-to-role dependency policies that extend the built-in architectural styles.",
                "Adjust the dependency direction, retag the participating types, or relax the brick rule only if the architecture intentionally allows that relationship."));

        public static readonly DiagnosticDescriptor BrickRuleConfigurationRule = new(
            BrickRuleConfigurationId,
            "Brick rule configuration must be valid",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A brick rule declaration is incomplete, inconsistent, or references invalid metadata.",
                "Analyzer-enforced brick policies must be fully declared so the rule engine can evaluate them deterministically.",
                "Fix the rule metadata, referenced roles, filters, or message configuration until the rule declaration becomes internally valid."));
    }
}
