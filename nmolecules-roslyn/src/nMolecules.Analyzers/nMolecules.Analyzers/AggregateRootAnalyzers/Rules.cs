using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.AggregateRootAnalyzers
{
    public static class Rules
    {
        public const string AggregateRootsShouldNotUseRepositoriesRuleId = "XMoleculesAggregateRoot0001";
        public const string AggregateRootsShouldNotUseServicesRuleId = "XMoleculesAggregateRoot0002";
        public const string AggregateRootsShouldHaveIdRuleId = "XMoleculesAggregateRoot0003";
        public const string AggregateRootsShouldHaveSingleIdRuleId = "XMoleculesAggregateRoot0004";
        public const string AggregateRootsShouldNotUseAggregateRootsRuleId = "XMoleculesAggregateRoot0005";
        public const string AggregateRootsShouldNotUseApplicationServicesRuleId = "XMoleculesAggregateRoot0006";
        public const string AggregateRootsShouldNotUseFactoriesRuleId = "XMoleculesAggregateRoot0007";
        public const string AggregateRootsShouldNotUseLegacyServicesRuleId = "XMoleculesAggregateRoot0008";

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseRepositoriesRule = new(
            AggregateRootsShouldNotUseRepositoriesRuleId,
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldNotUseRepositoryTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldNotUseRepositoryFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root reaches directly into a repository dependency from inside its consistency boundary.",
                "Aggregate roots protect domain invariants and must not depend on persistence access concerns.",
                "Move repository usage into an application service or domain service that loads the aggregate before invoking aggregate behavior."));
        
        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseServicesRule = new(
            AggregateRootsShouldNotUseServicesRuleId,
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldNotUseServiceTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldNotUseServiceFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root depends on a domain service to execute behavior inside the aggregate boundary.",
                "Aggregate roots should enforce their own invariants instead of delegating core boundary behavior to external service collaborators.",
                "Move invariant-relevant behavior into the aggregate root, or let a domain service coordinate behavior around aggregates from outside the boundary."));
        
        public static readonly DiagnosticDescriptor AggregateRootsShouldHaveIdRule = new(
            AggregateRootsShouldHaveIdRuleId,
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldHaveIdTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldHaveIdFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root is declared without any [Identity] member.",
                "Aggregate roots anchor aggregate references and lifecycle through one stable model identity.",
                "Add exactly one [Identity] member that uniquely represents the aggregate root across transactions."));

        public static readonly DiagnosticDescriptor AggregateRootsShouldHaveSingleIdRule = new(
            AggregateRootsShouldHaveSingleIdRuleId,
            "Aggregate roots must declare exactly one identity",
            "Aggregate root '{0}' declares {1} [Identity] members: {2}. Exactly one is allowed",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root declares multiple [Identity] members and therefore multiple competing aggregate identifiers.",
                "An aggregate root must expose one canonical identity for its boundary and references.",
                "Keep a single [Identity] member as the aggregate identifier and remodel the rest as ordinary domain attributes."));

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseAggregateRootsRule = new(
            AggregateRootsShouldNotUseAggregateRootsRuleId,
            "Aggregate roots must not reference other aggregate roots directly",
            "Aggregate root '{0}' must not reference aggregate root '{1}' directly",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "One aggregate root directly references another aggregate root as a collaborator or field.",
                "Aggregate boundaries should communicate through identity or higher-level coordination, not direct object graph coupling.",
                "Replace the direct reference with the other aggregate's identity, a domain event, or application-layer coordination."));

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseApplicationServicesRule = new(
            AggregateRootsShouldNotUseApplicationServicesRuleId,
            "Aggregate roots must not depend on application services",
            "Aggregate root '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root depends on an application-service collaborator from inside the domain model.",
                "Aggregate roots belong to the domain layer and must stay independent from use-case orchestration concerns.",
                "Move orchestration back into the application layer and keep the aggregate root focused on invariant-protecting domain behavior."));

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseFactoriesRule = new(
            AggregateRootsShouldNotUseFactoriesRuleId,
            "Aggregate roots must not depend on factories",
            "Aggregate root '{0}' must not depend on factory '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root depends on a factory to perform behavior inside its lifecycle.",
                "Factory responsibilities end after creation; aggregate behavior inside the consistency boundary must stay with the aggregate.",
                "Use the factory only for creation and keep subsequent state transitions and invariant checks inside the aggregate root."));

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseLegacyServicesRule = new(
            AggregateRootsShouldNotUseLegacyServicesRuleId,
            "Aggregate roots must not depend on legacy services",
            "Aggregate root '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The aggregate root depends on a type still marked with the legacy [Service] attribute.",
                "Aggregate boundaries should only depend on clearly typed domain collaborators, not ambiguous service roles.",
                "Replace the legacy marker with an explicit role and keep the aggregate root independent from application-service style collaborators."));
    }
}
