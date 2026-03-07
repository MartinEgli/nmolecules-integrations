using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ServiceAnalyzers
{
    public static class Rules
    {
        public const string LegacyServicesShouldUseSpecificRoleId = "XMoleculesService0001";

        public static readonly DiagnosticDescriptor LegacyServicesShouldUseSpecificRoleRule = new(
            LegacyServicesShouldUseSpecificRoleId,
            "Legacy services should use explicit role markers",
            "Type '{0}' uses legacy [Service]; replace it with [DomainService] or [ApplicationService]",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The type still uses the legacy [Service] marker, which does not reveal whether it belongs to the domain layer or application layer.",
                "Service roles should be explicit so dependency direction, analyzer semantics, and architectural intent stay unambiguous.",
                "Replace [Service] with [DomainService] or [ApplicationService] based on the type's real responsibility."));
    }
}
