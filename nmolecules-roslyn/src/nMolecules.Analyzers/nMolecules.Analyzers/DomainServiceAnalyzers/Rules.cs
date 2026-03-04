using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.DomainServiceAnalyzers
{
    public static class Rules
    {
        public const string DomainServicesShouldNotUseApplicationServicesId = "XMoleculesDomainService0001";

        public static readonly DiagnosticDescriptor DomainServicesShouldNotUseApplicationServicesRule = new(
            DomainServicesShouldNotUseApplicationServicesId,
            "Domain service should not use application services",
            "Domain service '{0}' must not depend on application service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Domain services belong to the domain model and should not depend on application service orchestration.");
    }
}
