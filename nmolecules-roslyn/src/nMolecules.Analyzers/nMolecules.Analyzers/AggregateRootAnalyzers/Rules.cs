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
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldNotUseRepositoryDescription),
                Resources.ResourceManager,
                typeof(Resources)));
        
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
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldNotUseServiceDescription),
                Resources.ResourceManager,
                typeof(Resources)));
        
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
            new LocalizableResourceString(nameof(Resources.AggregateRootShouldHaveIdDescription),
                Resources.ResourceManager,
                typeof(Resources)));

        public static readonly DiagnosticDescriptor AggregateRootsShouldHaveSingleIdRule = new(
            AggregateRootsShouldHaveSingleIdRuleId,
            "Aggregate roots must declare exactly one identity",
            "Aggregate root '{0}' declares {1} [Identity] members: {2}. Exactly one is allowed",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate roots should expose a single model identity for their consistency boundary.");

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseAggregateRootsRule = new(
            AggregateRootsShouldNotUseAggregateRootsRuleId,
            "Aggregate roots must not reference other aggregate roots directly",
            "Aggregate root '{0}' must not reference aggregate root '{1}' directly",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate boundaries should be crossed through identity, not direct object references.");

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseApplicationServicesRule = new(
            AggregateRootsShouldNotUseApplicationServicesRuleId,
            "Aggregate roots must not depend on application services",
            "Aggregate root '{0}' must not depend on application service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate roots belong to the domain model and must not depend on application-service orchestration.");

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseFactoriesRule = new(
            AggregateRootsShouldNotUseFactoriesRuleId,
            "Aggregate roots must not depend on factories",
            "Aggregate root '{0}' must not depend on factory '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate roots should enforce their own consistency boundary and should not depend on external factory components by default.");

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseLegacyServicesRule = new(
            AggregateRootsShouldNotUseLegacyServicesRuleId,
            "Aggregate roots must not depend on legacy services",
            "Aggregate root '{0}' must not depend on legacy service '{1}' in member '{2}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate roots should not depend on the legacy Service marker. Use explicit DomainService or ApplicationService roles instead.");
    }
}
