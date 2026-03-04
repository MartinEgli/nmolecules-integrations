using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.RepositoryAnalyzers
{
    public static class Rules
    {
        public const string RepositoriesShouldNotUseServicesId = "XMoleculesRepository0001";
        public const string RepositoriesShouldNotExposeInfrastructureSignaturesId = "XMoleculesRepository0002";
        
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
    }
}
