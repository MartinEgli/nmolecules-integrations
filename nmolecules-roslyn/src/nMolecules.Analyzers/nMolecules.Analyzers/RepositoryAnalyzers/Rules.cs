using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.RepositoryAnalyzers
{
    public static class Rules
    {
        public const string RepositoriesShouldNotUseDomainServicesId = "XMoleculesRepository0001";
        public const string RepositoriesShouldNotExposeInfrastructureSignaturesId = "XMoleculesRepository0002";
        public const string RepositoriesShouldNotDependOnRepositoriesId = "XMoleculesRepository0003";
        public const string ApprovedRepositoryCompositionShouldUseContractsId = "XMoleculesRepository0004";
        public const string RepositoriesShouldNotDependOnFactoriesId = "XMoleculesRepository0005";
        public const string RepositoriesShouldNotUseApplicationServicesId = "XMoleculesRepository0006";
        public const string RepositoriesShouldNotUseLegacyServicesId = "XMoleculesRepository0007";
        
        public static readonly DiagnosticDescriptor RepositoriesShouldNotUseDomainServicesRule = new(
            RepositoriesShouldNotUseDomainServicesId,
            "Repositories must not depend on domain services",
            "Repository '{0}' must not depend on domain service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Repositories should focus on retrieval and persistence responsibilities instead of depending on domain-service behavior.");

        public static readonly DiagnosticDescriptor RepositoriesShouldNotExposeInfrastructureSignaturesRule = new(
            RepositoriesShouldNotExposeInfrastructureSignaturesId,
            "Repositories should not expose infrastructure-layer types in public signatures",
            "Repository '{0}' must not expose infrastructure-layer type '{1}' in public member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Repository contracts should not expose persistence-specific or infrastructure-layer types in their public API.");

        public static readonly DiagnosticDescriptor RepositoriesShouldNotDependOnRepositoriesRule = new(
            RepositoriesShouldNotDependOnRepositoriesId,
            "Repositories should not depend on other repositories directly",
            "Repository '{0}' should not depend on repository '{1}' directly in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Repository-to-repository dependencies are only acceptable for explicitly approved technical composition patterns.");

        public static readonly DiagnosticDescriptor ApprovedRepositoryCompositionShouldUseContractsRule = new(
            ApprovedRepositoryCompositionShouldUseContractsId,
            "Approved repository composition should depend on repository contracts only",
            "Repository '{0}' uses approved composition in member '{1}' but depends on concrete repository '{2}' instead of a contract",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "When repository composition is explicitly approved, dependencies should still target repository contracts (interfaces), not concrete repository implementations.");

        public static readonly DiagnosticDescriptor RepositoriesShouldNotDependOnFactoriesRule = new(
            RepositoriesShouldNotDependOnFactoriesId,
            "Repositories should not depend on factories",
            "Repository '{0}' should not depend on factory '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Repository concerns should focus on retrieval/persistence contracts and avoid direct dependencies on object-construction orchestration.");

        public static readonly DiagnosticDescriptor RepositoriesShouldNotUseApplicationServicesRule = new(
            RepositoriesShouldNotUseApplicationServicesId,
            "Repositories should not depend on application services",
            "Repository '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Repositories must not depend on application-service orchestration.");

        public static readonly DiagnosticDescriptor RepositoriesShouldNotUseLegacyServicesRule = new(
            RepositoriesShouldNotUseLegacyServicesId,
            "Repositories must not depend on legacy services",
            "Repository '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Repositories should not depend on the legacy Service marker. Use explicit DomainService or ApplicationService roles instead.");
    }
}
