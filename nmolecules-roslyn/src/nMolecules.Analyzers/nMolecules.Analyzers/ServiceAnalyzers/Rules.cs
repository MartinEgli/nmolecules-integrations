using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ServiceAnalyzers
{
    public static class Rules
    {
        public const string LegacyServicesShouldUseSpecificRoleId = "XMoleculesService0001";

        public static readonly DiagnosticDescriptor LegacyServicesShouldUseSpecificRoleRule = new(
            LegacyServicesShouldUseSpecificRoleId,
            "Legacy services should use a specific role",
            "Service should use [DomainService] or [ApplicationService] instead of legacy [Service]",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "The legacy [Service] marker is ambiguous. Prefer an explicit [DomainService] or [ApplicationService] role.");
    }
}
