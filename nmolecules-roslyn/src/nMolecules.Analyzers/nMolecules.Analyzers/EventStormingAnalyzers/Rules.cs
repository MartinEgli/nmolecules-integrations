using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.EventStormingAnalyzers
{
    public static class Rules
    {
        public const string ActorsShouldNotDependOnAggregatesId = "XMoleculesEventStorming0001";
        public const string CommandsShouldDependOnAggregatesId = "XMoleculesEventStorming0002";
        public const string PoliciesShouldDependOnDomainEventsId = "XMoleculesEventStorming0003";
        public const string ReadModelsShouldNotDependOnAggregatesId = "XMoleculesEventStorming0004";
        public const string ExternalSystemsShouldNotDependOnAggregatesId = "XMoleculesEventStorming0005";

        public static readonly DiagnosticDescriptor ActorsShouldNotDependOnAggregatesRule = new(
            ActorsShouldNotDependOnAggregatesId,
            "Event Storming actors must not depend on aggregates",
            "Event Storming actor symbol '{0}' must not depend on aggregate '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Actors represent external initiators and should not directly depend on aggregate internals.");

        public static readonly DiagnosticDescriptor CommandsShouldDependOnAggregatesRule = new(
            CommandsShouldDependOnAggregatesId,
            "Event Storming commands should target aggregates",
            "Event Storming command '{0}' should depend on at least one aggregate",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Commands should be explicit intent messages for aggregate behavior.");

        public static readonly DiagnosticDescriptor PoliciesShouldDependOnDomainEventsRule = new(
            PoliciesShouldDependOnDomainEventsId,
            "Event Storming policies should react to domain events",
            "Event Storming policy '{0}' should depend on at least one domain event",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            "Policies are typically triggered by domain events in Event Storming models.");

        public static readonly DiagnosticDescriptor ReadModelsShouldNotDependOnAggregatesRule = new(
            ReadModelsShouldNotDependOnAggregatesId,
            "Event Storming read models must not depend on aggregates",
            "Event Storming read model symbol '{0}' must not depend on aggregate '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Read models should stay decoupled from aggregate write-side behavior.");

        public static readonly DiagnosticDescriptor ExternalSystemsShouldNotDependOnAggregatesRule = new(
            ExternalSystemsShouldNotDependOnAggregatesId,
            "Event Storming external systems must not depend on aggregates",
            "Event Storming external system symbol '{0}' must not depend on aggregate '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "External systems should interact via contracts/events instead of aggregate internals.");
    }
}
