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
            "The legacy [Service] marker is ambiguous. Prefer an explicit [DomainService] or [ApplicationService] role.");
    }
}
