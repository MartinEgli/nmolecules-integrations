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
            "Aggregate roots should declare a single identity",
            "Aggregate root should declare exactly one [Identity] member",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate roots should expose a single model identity for their consistency boundary.");

        public static readonly DiagnosticDescriptor AggregateRootsShouldNotUseAggregateRootsRule = new(
            AggregateRootsShouldNotUseAggregateRootsRuleId,
            "Aggregate roots should not reference aggregate roots",
            "Aggregate root should not reference other aggregate roots directly",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Aggregate boundaries should be crossed through identity, not direct object references.");
    }
}
