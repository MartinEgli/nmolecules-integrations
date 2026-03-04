using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.RepositoryAnalyzers
{
    public static class Rules
    {
        public const string RepositoriesShouldNotUseServicesId = "XMoleculesRepository0001";
        public const string RepositoriesShouldNotExposeInfrastructureSignaturesId = "XMoleculesRepository0002";
        public const string RepositoriesShouldNotDependOnRepositoriesId = "XMoleculesRepository0003";
        public const string ApprovedRepositoryCompositionShouldUseContractsId = "XMoleculesRepository0004";
        
        public static readonly DiagnosticDescriptor RepositoriesShouldNotUseServicesRule = new(
            RepositoriesShouldNotUseServicesId,
            new LocalizableResourceString(nameof(Resources.RepositoryShouldNotUseServiceTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.RepositoryShouldNotUseServiceFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            new LocalizableResourceString(nameof(Resources.RepositoryShouldNotUseServiceDescription),
                Resources.ResourceManager,
                typeof(Resources)));

        public static readonly DiagnosticDescriptor RepositoriesShouldNotExposeInfrastructureSignaturesRule = new(
            RepositoriesShouldNotExposeInfrastructureSignaturesId,
            "Repositories must not expose infrastructure-layer types in public signatures",
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
    }
}
