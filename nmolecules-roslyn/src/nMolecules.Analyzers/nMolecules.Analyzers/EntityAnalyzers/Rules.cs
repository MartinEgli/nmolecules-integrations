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
            new LocalizableResourceString(nameof(Resources.EntityShouldNotUseRepositoryDescription),
                Resources.ResourceManager,
                typeof(Resources)));

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
            new LocalizableResourceString(nameof(Resources.EntityShouldNotUseAggregateRootDescription),
                Resources.ResourceManager,
                typeof(Resources)));
        
        public static readonly DiagnosticDescriptor EntitiesShouldNotUseDomainServicesRule = new(
            EntitiesShouldNotUseDomainServicesId,
            "Entities must not depend on domain services",
            "Entity '{0}' must not depend on domain service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Entities should encapsulate domain state and behavior directly instead of depending on separate domain-service collaborators.");
        
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
            new LocalizableResourceString(nameof(Resources.EntityShouldHaveIdDescription),
                Resources.ResourceManager,
                typeof(Resources)));

        public static readonly DiagnosticDescriptor EntitiesShouldHaveSingleIdRule = new(
            EntitiesShouldHaveSingleIdRuleId,
            "Entities must declare exactly one identity",
            "Entity '{0}' declares {1} [Identity] members: {2}. Exactly one is allowed",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Entities represent continuity through a single model identity.");

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseFactoriesRule = new(
            EntitiesShouldNotUseFactoriesId,
            "Entities must not depend on factories",
            "Entity '{0}' must not depend on factory '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Entities should own behavior and identity state, but object creation orchestration belongs to dedicated Factory components.");

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseApplicationServicesRule = new(
            EntitiesShouldNotUseApplicationServicesId,
            "Entities must not depend on application services",
            "Entity '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Entities are domain building blocks and must not depend on application-service orchestration.");

        public static readonly DiagnosticDescriptor EntitiesShouldNotUseLegacyServicesRule = new(
            EntitiesShouldNotUseLegacyServicesId,
            "Entities must not depend on legacy services",
            "Entity '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Entities should not depend on the legacy Service marker. Use explicit DomainService or ApplicationService roles instead.");
    }
}
