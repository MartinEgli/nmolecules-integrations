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
            DiagnosticDescriptions.Create(
                "The repository depends on a domain service to complete its persistence or retrieval work.",
                "Repositories should own persistence access only and must not absorb domain-rule execution responsibilities.",
                "Move domain logic into the domain service and keep the repository limited to loading and storing aggregates or entities."));

        public static readonly DiagnosticDescriptor RepositoriesShouldNotExposeInfrastructureSignaturesRule = new(
            RepositoriesShouldNotExposeInfrastructureSignaturesId,
            "Repositories should not expose infrastructure-layer types in public signatures",
            "Repository '{0}' must not expose infrastructure-layer type '{1}' in public member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The repository contract leaks an infrastructure-specific type through a public member signature.",
                "Repository APIs should remain domain-facing contracts instead of exposing persistence technology or adapter details.",
                "Return domain objects or dedicated abstractions and keep persistence-specific types behind the repository implementation."));

        public static readonly DiagnosticDescriptor RepositoriesShouldNotDependOnRepositoriesRule = new(
            RepositoriesShouldNotDependOnRepositoriesId,
            "Repositories should not depend on other repositories directly",
            "Repository '{0}' should not depend on repository '{1}' directly in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "One repository directly uses another repository to complete its own work.",
                "Repositories should stay focused on one persistence boundary; cross-repository composition is a special-case technical pattern, not the default.",
                "Remove the direct repository dependency or mark and constrain the composition explicitly through the approved repository-composition pattern."));

        public static readonly DiagnosticDescriptor ApprovedRepositoryCompositionShouldUseContractsRule = new(
            ApprovedRepositoryCompositionShouldUseContractsId,
            "Approved repository composition should depend on repository contracts only",
            "Repository '{0}' uses approved composition in member '{1}' but depends on concrete repository '{2}' instead of a contract",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "Approved repository composition is present, but it depends on a concrete repository implementation.",
                "Even approved composition must preserve inversion of dependencies by targeting repository contracts instead of concrete technical classes.",
                "Depend on an interface or abstract repository contract and keep concrete repository implementations in the infrastructure wiring layer."));

        public static readonly DiagnosticDescriptor RepositoriesShouldNotDependOnFactoriesRule = new(
            RepositoriesShouldNotDependOnFactoriesId,
            "Repositories should not depend on factories",
            "Repository '{0}' should not depend on factory '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The repository uses a factory to drive part of its persistence or loading flow.",
                "Repositories should focus on retrieval and storage boundaries, not on object-creation orchestration.",
                "Build the object graph in a dedicated factory or mapper and keep the repository contract centered on persistence operations."));

        public static readonly DiagnosticDescriptor RepositoriesShouldNotUseApplicationServicesRule = new(
            RepositoriesShouldNotUseApplicationServicesId,
            "Repositories should not depend on application services",
            "Repository '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The repository reaches upward into an application-service collaborator.",
                "Repositories are low-level domain or infrastructure boundaries and must not depend on use-case orchestration components.",
                "Move orchestration into the application service and let the repository stay a passive persistence boundary."));

        public static readonly DiagnosticDescriptor RepositoriesShouldNotUseLegacyServicesRule = new(
            RepositoriesShouldNotUseLegacyServicesId,
            "Repositories must not depend on legacy services",
            "Repository '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The repository depends on a collaborator marked with the ambiguous legacy [Service] attribute.",
                "Repository boundaries should depend only on explicitly typed collaborators so architectural intent stays analyzable.",
                "Retype the dependency as [DomainService] or [ApplicationService] and keep repository boundaries free from ambiguous service roles."));
    }
}
