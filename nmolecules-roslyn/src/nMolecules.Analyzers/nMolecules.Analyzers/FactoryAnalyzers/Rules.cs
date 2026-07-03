using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.FactoryAnalyzers
{
    public static class Rules
    {
        public const string FactoriesShouldNotUseApplicationServicesId = "XMoleculesFactory0001";
        public const string FactoriesShouldNotUseRepositoriesId = "XMoleculesFactory0002";
        public const string FactoriesShouldNotAlsoBeEntitiesId = "XMoleculesFactory0003";
        public const string FactoriesShouldNotAlsoBeDomainBuildingBlocksId = FactoriesShouldNotAlsoBeEntitiesId;
        public const string FactoriesShouldNotUseFactoriesId = "XMoleculesFactory0004";
        public const string FactoriesShouldNotAlsoBeAggregateRootsId = "XMoleculesFactory0005";
        public const string FactoriesShouldNotAlsoBeValueObjectsId = "XMoleculesFactory0006";
        public const string FactoriesShouldNotAlsoBeRepositoriesId = "XMoleculesFactory0007";
        public const string FactoriesShouldNotAlsoBeLegacyServicesId = "XMoleculesFactory0008";
        public const string FactoriesShouldNotAlsoBeDomainServicesId = "XMoleculesFactory0009";
        public const string FactoriesShouldNotAlsoBeApplicationServicesId = "XMoleculesFactory0010";

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

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeEntitiesRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeEntitiesId,
            "Factories must not also be entities",
            "an entity");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeDomainBuildingBlocksRule = FactoriesShouldNotAlsoBeEntitiesRule;

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

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeAggregateRootsRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeAggregateRootsId,
            "Factories must not also be aggregate roots",
            "an aggregate-root");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeValueObjectsRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeValueObjectsId,
            "Factories must not also be value objects",
            "a value-object");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeRepositoriesRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeRepositoriesId,
            "Factories must not also be repositories",
            "a repository");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeLegacyServicesRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeLegacyServicesId,
            "Factories must not also be legacy services",
            "a legacy-service");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeDomainServicesRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeDomainServicesId,
            "Factories must not also be domain services",
            "a domain-service");

        public static readonly DiagnosticDescriptor FactoriesShouldNotAlsoBeApplicationServicesRule = CreateRoleConflictRule(
            FactoriesShouldNotAlsoBeApplicationServicesId,
            "Factories must not also be application services",
            "an application-service");

        private static DiagnosticDescriptor CreateRoleConflictRule(
            string id,
            string title,
            string roleDescription)
        {
            return new DiagnosticDescriptor(
                id,
                title,
                "Factory '{0}' must not also be marked as '{1}'",
                Category.DDD,
                DiagnosticSeverity.Error,
                true,
                DiagnosticDescriptions.Create(
                    $"The same type is declared as both a factory and {roleDescription} role.",
                    "A factory has one responsibility: creation. It must not also model identity, state, persistence, or orchestration roles.",
                    "Split the type so creation stays in the factory and the other responsibility lives in a dedicated domain building block."));
        }
    }
}
