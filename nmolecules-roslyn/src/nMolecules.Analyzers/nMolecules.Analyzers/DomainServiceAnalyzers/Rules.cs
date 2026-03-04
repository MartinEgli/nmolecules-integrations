using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.DomainServiceAnalyzers
{
    public static class Rules
    {
        public const string DomainServicesShouldNotUseApplicationServicesId = "XMoleculesDomainService0001";
        public const string DomainServicesShouldNotExposeInfrastructureSignaturesId = "XMoleculesDomainService0003";

        public static readonly DiagnosticDescriptor DomainServicesShouldNotUseApplicationServicesRule = new(
            DomainServicesShouldNotUseApplicationServicesId,
            "Domain service should not use application services",
            "Domain service '{0}' must not depend on application service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Domain services belong to the domain model and should not depend on application service orchestration.");

        public static readonly DiagnosticDescriptor DomainServicesShouldNotExposeInfrastructureSignaturesRule = new(
            DomainServicesShouldNotExposeInfrastructureSignaturesId,
            "Domain services should not expose infrastructure-layer types in public signatures",
            "Domain service '{0}' must not expose infrastructure-layer type '{1}' in public member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Domain services should keep their public API independent from infrastructure-layer types.");
    }
}
