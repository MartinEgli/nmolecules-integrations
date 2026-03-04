using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ValueObjectAnalyzers
{
    public static class Rules
    {
        public const string NoEntitiesInValueObjectsId = "XMoleculesValueObject0001";
        public const string NoServicesInValueObjectsId = "XMoleculesValueObject0002";
        public const string NoRepositoriesInValueObjectsId = "XMoleculesValueObject0003";
        public const string NoAggregateRootsInValueObjectsId = "XMoleculesValueObject0004";
        public const string ValueObjectsShouldBeImmutableId = "XMoleculesValueObject0005";
        public const string ValueObjectsMustNotDeclareIdentityId = "XMoleculesValueObject0006";
        public const string NoFactoriesInValueObjectsId = "XMoleculesValueObject0007";
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
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesEntityDescription),
                Resources.ResourceManager,
                typeof(Resources)));

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseServiceRule = new(NoServicesInValueObjectsId,
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesServiceTitle),
                Resources.ResourceManager,
                typeof(Resources)),
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesServiceMessageFormat),
                Resources.ResourceManager,
                typeof(Resources)),
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesServiceDescription),
                Resources.ResourceManager,
                typeof(Resources)));

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
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesRepositoryDescription),
                Resources.ResourceManager,
                typeof(Resources)));

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
            new LocalizableResourceString(nameof(Resources.ValueObjectUsesAggregateRootDescription),
                Resources.ResourceManager,
                typeof(Resources)));

        public static readonly DiagnosticDescriptor ValueObjectShouldBeImmutableRule = new(ValueObjectsShouldBeImmutableId,
            "Value objects must be immutable",
            "Value object '{0}' must be immutable; member '{1}' is writable",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Value objects must not expose writable fields or properties.");

        public static readonly DiagnosticDescriptor ValueObjectMustNotDeclareIdentityRule = new(ValueObjectsMustNotDeclareIdentityId,
            "Value objects must not declare identities",
            "Value object '{0}' must not declare [Identity] member '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Value objects are defined by value and must not introduce entity-style identities.");

        public static readonly DiagnosticDescriptor ValueObjectMustNotUseFactoryRule = new(NoFactoriesInValueObjectsId,
            "Value objects must not depend on factories",
            "Value object '{0}' must not depend on factory '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Value objects should not depend on factory abstractions or implementations.");

        public static readonly DiagnosticDescriptor ValueObjectMustImplementIEquatableRule = new(ValueObjectsMustImplementIEquatableId,
            "Value objects must implement IEquatable<T>",
            "Value object '{0}' must implement IEquatable<{0}>",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Value objects should implement type-specific equality semantics.");

        public static readonly DiagnosticDescriptor ValueObjectShouldBeSealedRule = new(ValueObjectsShouldBeSealedId,
            "Value objects should be sealed",
            "Value object '{0}' should be sealed or declared as a value type",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Reference-type value objects should be sealed to protect value-based semantics.");
    }
}
