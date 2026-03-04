using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.FactoryAnalyzers
{
    public static class Rules
    {
        public const string FactoriesShouldNotUseApplicationServicesId = "XMoleculesFactory0001";
        public const string FactoriesShouldNotUseRepositoriesId = "XMoleculesFactory0002";
        public const string FactoriesShouldNotAlsoBeDomainBuildingBlocksId = "XMoleculesFactory0003";

        public static readonly DiagnosticDescriptor FactoriesShouldNotUseApplicationServicesRule = new(
            FactoriesShouldNotUseApplicationServicesId,
            "Factories should not use application services",
            "Factory '{0}' must not depend on application service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Factories create valid model objects and should not depend on application-level orchestration.");

        public static readonly DiagnosticDescriptor FactoriesShouldNotUseRepositoriesRule = new(
            FactoriesShouldNotUseRepositoriesId,
            "Factories should not use repositories",
            "Factory '{0}' must not depend on repository '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Factories should construct model objects and must not take on repository-style loading responsibilities.");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeDomainBuildingBlocksRule = new(
            FactoriesShouldNotAlsoBeDomainBuildingBlocksId,
            "Factories must not also be domain building blocks",
            "Factory '{0}' must not also be marked as '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "A factory should stay a creation abstraction and must not also be modeled as an entity, aggregate root, value object, repository, or service role.");
    }
}
