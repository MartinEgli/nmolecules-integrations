using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.EntityAnalyzers
{
    public static class Rules
    {
        public const string EntitiesShouldNotUseRepositoriesId = "XMoleculesEntity0001";
        public const string EntitiesShouldNotUseAggregateRootsId = "XMoleculesEntity0002";
        public const string EntitiesShouldNotUseDomainServicesId = "XMoleculesEntity0003";
        public const string EntitiesShouldHaveIdRuleId = "XMoleculesEntity0004";
        public const string EntitiesShouldHaveSingleIdRuleId = "XMoleculesEntity0005";
        public const string EntitiesShouldNotUseFactoriesId = "XMoleculesEntity0006";
        public const string EntitiesShouldNotUseApplicationServicesId = "XMoleculesEntity0007";
        public const string EntitiesShouldNotUseLegacyServicesId = "XMoleculesEntity0008";

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseRepositoriesRule = new(
            EntitiesShouldNotUseRepositoriesId,
            new LocalizableResourceString(nameof(Resources.EntityShouldNotUseRepositoryTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.EntityShouldNotUseRepositoryFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity directly references a repository dependency from its model logic or state.",
                "Entities must protect domain behavior and identity, not reach into persistence access concerns.",
                "Move repository access into an application service, domain service, or repository boundary and keep the entity persistence-agnostic."));

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseAggregateRootsRule = new(
            EntitiesShouldNotUseAggregateRootsId,
            new LocalizableResourceString(nameof(Resources.EntityShouldNotUseAggregateRootTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.EntityShouldNotUseAggregateRootFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity references another aggregate root directly instead of through its identity or a boundary contract.",
                "Aggregate boundaries must stay explicit so one aggregate does not silently control or couple to another aggregate's consistency rules.",
                "Store the other aggregate's identity, publish an event, or coordinate the interaction through an application/domain service."));
        
        public static readonly DiagnosticDescriptor EntitiesShouldNotUseDomainServicesRule = new(
            EntitiesShouldNotUseDomainServicesId,
            "Entities must not depend on domain services",
            "Entity '{0}' must not depend on domain service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity calls or stores a domain service collaborator to perform part of its behavior.",
                "Entities should encapsulate state-local behavior directly instead of delegating core model behavior to service dependencies.",
                "Move the behavior into the entity when it belongs to its own invariants, or orchestrate the interaction from a dedicated domain service outside the entity."));
        
        public static readonly DiagnosticDescriptor EntitiesShouldHaveIdRule = new(
            EntitiesShouldHaveIdRuleId,
            new LocalizableResourceString(nameof(Resources.EntityShouldHaveIdTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.EntityShouldHaveIdFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity has been modeled as an identity-bearing concept but no [Identity] member is declared.",
                "Entities are distinguished by continuity over time, so they require an explicit identity in the model.",
                "Add exactly one stable [Identity] member that represents the entity's domain identity."));

        public static readonly DiagnosticDescriptor EntitiesShouldHaveSingleIdRule = new(
            EntitiesShouldHaveSingleIdRuleId,
            "Entities must declare exactly one identity",
            "Entity '{0}' declares {1} [Identity] members: {2}. Exactly one is allowed",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity declares multiple members as [Identity] and therefore exposes more than one model identity.",
                "An entity has one identity concept that defines its continuity across state changes.",
                "Keep one [Identity] member as the canonical identifier and remodel the remaining fields as normal attributes or invariants."));

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseFactoriesRule = new(
            EntitiesShouldNotUseFactoriesId,
            "Entities must not depend on factories",
            "Entity '{0}' must not depend on factory '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity depends on a factory to continue its own behavior or lifecycle.",
                "Factories own object creation, while entities own domain state and invariant-preserving behavior after creation.",
                "Move creation concerns out of the entity and let an application service or factory create entities before domain behavior begins."));

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseApplicationServicesRule = new(
            EntitiesShouldNotUseApplicationServicesId,
            "Entities must not depend on application services",
            "Entity '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity reaches out to an application-service collaborator from inside the domain model.",
                "Entities must stay inside the domain layer and must not depend on use-case orchestration.",
                "Move the orchestration into the application layer and let the entity expose domain operations without application-service dependencies."));

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseLegacyServicesRule = new(
            EntitiesShouldNotUseLegacyServicesId,
            "Entities must not depend on legacy services",
            "Entity '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The entity depends on a type still marked with the ambiguous legacy [Service] attribute.",
                "Entity dependencies should expose clear architectural roles so the model does not accidentally couple to orchestration concerns.",
                "Replace the legacy service marker with an explicit role and keep the entity independent from application-service style collaborators."));
    }
}
