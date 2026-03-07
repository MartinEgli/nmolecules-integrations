using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ValueObjectAnalyzers
{
    public static class Rules
    {
        public const string NoEntitiesInValueObjectsId = "XMoleculesValueObject0001";
        public const string NoDomainServicesInValueObjectsId = "XMoleculesValueObject0002";
        public const string NoRepositoriesInValueObjectsId = "XMoleculesValueObject0003";
        public const string NoAggregateRootsInValueObjectsId = "XMoleculesValueObject0004";
        public const string ValueObjectsShouldBeImmutableId = "XMoleculesValueObject0005";
        public const string ValueObjectsMustNotDeclareIdentityId = "XMoleculesValueObject0006";
        public const string NoFactoriesInValueObjectsId = "XMoleculesValueObject0007";
        public const string NoApplicationServicesInValueObjectsId = "XMoleculesValueObject0008";
        public const string NoLegacyServicesInValueObjectsId = "XMoleculesValueObject0009";
        public const string ValueObjectsMustImplementIEquatableId = "XMoleculesValueObject1001";
        public const string ValueObjectsShouldBeSealedId = "XMoleculesValueObject1002";

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseEntityRule = new(NoEntitiesInValueObjectsId,
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesEntityTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesEntityMessageFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object holds or references an entity collaborator.",
                "Value objects are defined purely by value and must not depend on identity-bearing model elements.",
                "Replace the entity reference with copied scalar data, another value object, or the entity identity if only correlation is needed."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseDomainServiceRule = new(NoDomainServicesInValueObjectsId,
            "Value objects must not depend on domain services",
            "Value object '{0}' must not depend on domain service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object depends on a domain service to compute or store part of its behavior.",
                "Value objects should remain self-contained value semantics without service collaborations.",
                "Move the behavior into the value object if it depends only on its own state, or invoke a domain service outside the value object."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseRepositoryRule = new(NoRepositoriesInValueObjectsId,
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesRepositoryTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesRepositoryMessageFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object reaches into a repository dependency for loading or lookup.",
                "Value objects must stay persistence-agnostic and must not depend on retrieval infrastructure.",
                "Load required state before constructing the value object and keep repository access outside the value-object type."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseAggregateRootRule = new(NoAggregateRootsInValueObjectsId,
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesAggregateRootTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesAggregateRootMessageFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object depends on an aggregate root or stores it directly.",
                "Value objects should remain pure value semantics and must not capture aggregate boundaries or lifecycle ownership.",
                "Copy only the required value data or store the aggregate identity externally instead of referencing the aggregate root."));

        public static readonly DiagnosticDescriptor ValueObjectShouldBeImmutableRule = new(ValueObjectsShouldBeImmutableId,
            "Value objects must be immutable",
            "Value object '{0}' must be immutable; member '{1}' is writable",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object exposes writable state through a field or property.",
                "Value semantics rely on immutability so the meaning of a value object cannot change after creation.",
                "Make all state read-only and create a new value object instance for any value change instead of mutating the existing one."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotDeclareIdentityRule = new(ValueObjectsMustNotDeclareIdentityId,
            "Value objects must not declare identities",
            "Value object '{0}' must not declare [Identity] member '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object declares an [Identity] member and therefore models entity-style identity.",
                "Value objects are equal by their contained values, not by a persistent identity.",
                "Remove the [Identity] member or remodel the type as an entity or aggregate root if identity is truly required."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseFactoryRule = new(NoFactoriesInValueObjectsId,
            "Value objects must not depend on factories",
            "Value object '{0}' must not depend on factory '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object depends on a factory to establish or update its state.",
                "Value objects should be complete and self-contained once created and must not rely on external creation services afterward.",
                "Create the value object through a factory before use if needed, but keep factory dependencies out of the value-object type itself."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseApplicationServiceRule = new(NoApplicationServicesInValueObjectsId,
            "Value objects must not depend on application services",
            "Value object '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object depends on an application-service collaborator.",
                "Value objects belong entirely to the domain model and must stay independent from use-case orchestration.",
                "Move orchestration out of the value object and keep it as a pure domain type that operates only on its own data."));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseLegacyServiceRule = new(NoLegacyServicesInValueObjectsId,
            "Value objects must not depend on legacy services",
            "Value object '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object depends on a collaborator marked only with the ambiguous legacy [Service] attribute.",
                "Value objects should remain free of ambiguous service dependencies so their architectural role stays purely value-based.",
                "Replace the legacy marker with an explicit service role and keep the value object itself independent from service collaborators."));

        public static readonly DiagnosticDescriptor ValueObjectMustImplementIEquatableRule = new(ValueObjectsMustImplementIEquatableId,
            "Value objects must implement IEquatable<T>",
            "Value object '{0}' must implement IEquatable<{0}>",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The value object relies on reference equality or incomplete equality semantics.",
                "Value objects are compared by their values, so .NET-specific equality contracts should make that semantics explicit.",
                "Implement IEquatable<T> with value-based equality across the members that define the value object's meaning."));

        public static readonly DiagnosticDescriptor ValueObjectShouldBeSealedRule = new(ValueObjectsShouldBeSealedId,
            "Value objects should be sealed",
            "Value object '{0}' should be sealed or declared as a value type",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The reference-type value object can be subclassed, which can weaken equality and immutability assumptions.",
                "Reference-type value objects should keep one closed value semantics definition.",
                "Seal the value-object type or remodel it as a value type if inheritance is not part of the intended design."));
    }
}
