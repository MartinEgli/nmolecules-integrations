using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.FactoryAnalyzers
{
    public static class Rules
    {
        public const string FactoriesShouldNotUseApplicationServicesId = "XMoleculesFactory0001";
        public const string FactoriesShouldNotUseRepositoriesId = "XMoleculesFactory0002";
        public const string FactoriesShouldNotAlsoBeDomainBuildingBlocksId = "XMoleculesFactory0003";
        public const string FactoriesShouldNotUseFactoriesId = "XMoleculesFactory0004";

        public static readonly DiagnosticDescriptor FactoriesShouldNotUseApplicationServicesRule = new(
            FactoriesShouldNotUseApplicationServicesId,
            "Factories must not depend on application services",
            "Factory '{0}' must not depend on application service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A factory reaches into an application-service collaborator while constructing model objects.",
                "Factories are creation abstractions and must not depend on use-case orchestration components.",
                "Keep the factory focused on valid object creation and move orchestration into an application service that calls the factory."));

        public static readonly DiagnosticDescriptor FactoriesShouldNotUseRepositoriesRule = new(
            FactoriesShouldNotUseRepositoriesId,
            "Factories must not depend on repositories",
            "Factory '{0}' must not depend on repository '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The factory needs repository access to perform loading or retrieval while creating objects.",
                "Factories should build new domain objects, while repositories own retrieval and persistence responsibilities.",
                "Load required state before invoking the factory or move lookup behavior into a repository or application-layer orchestration step."));

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeDomainBuildingBlocksRule = new(
            FactoriesShouldNotAlsoBeDomainBuildingBlocksId,
            "Factories must not also be domain building blocks",
            "Factory '{0}' must not also be marked as '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The same type is declared as both a factory and another domain building block role.",
                "A factory has one responsibility: creation. It must not also model identity, state, persistence, or orchestration roles.",
                "Split the type so creation stays in the factory and the other responsibility lives in a dedicated domain building block."));

        public static readonly DiagnosticDescriptor FactoriesShouldNotUseFactoriesRule = new(
            FactoriesShouldNotUseFactoriesId,
            "Factories must not depend on other factories",
            "Factory '{0}' must not depend on factory '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "One factory delegates object creation to another factory directly.",
                "Factory logic should stay locally focused so object creation paths remain explicit and bounded.",
                "Inline the required creation logic, extract a lower-level constructor abstraction, or let an application/domain service coordinate multiple factories if that composition is intentional."));
    }
}
