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
            "Event Storming actor '{0}' must not depend on aggregate '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An Event Storming actor directly depends on an aggregate.",
                "Actors represent external initiators and should not couple to aggregate internals.",
                "Express the interaction through commands, application services, or another boundary contract instead of a direct aggregate dependency."));

        public static readonly DiagnosticDescriptor CommandsShouldDependOnAggregatesRule = new(
            CommandsShouldDependOnAggregatesId,
            "Event Storming commands should target aggregates",
            "Event Storming command '{0}' should depend on at least one aggregate",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "An Event Storming command does not target any aggregate.",
                "Commands should express intent toward aggregate behavior in the model.",
                "Let the command reference or target the responsible aggregate, or remodel it if it is not really a command."));

        public static readonly DiagnosticDescriptor PoliciesShouldDependOnDomainEventsRule = new(
            PoliciesShouldDependOnDomainEventsId,
            "Event Storming policies should react to domain events",
            "Event Storming policy '{0}' should depend on at least one domain event",
            Category.Architecture,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "An Event Storming policy is modeled without depending on any domain event.",
                "Policies in Event Storming typically react to domain events and coordinate follow-up behavior.",
                "Drive the policy from one or more domain events or remodel it as another artifact if event reaction is not its purpose."));

        public static readonly DiagnosticDescriptor ReadModelsShouldNotDependOnAggregatesRule = new(
            ReadModelsShouldNotDependOnAggregatesId,
            "Event Storming read models must not depend on aggregates",
            "Event Storming read model '{0}' must not depend on aggregate '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An Event Storming read model depends directly on an aggregate.",
                "Read models belong to the read side and should remain decoupled from aggregate write-side behavior.",
                "Project aggregate changes into a dedicated read model instead of coupling the read model to aggregate internals."));

        public static readonly DiagnosticDescriptor ExternalSystemsShouldNotDependOnAggregatesRule = new(
            ExternalSystemsShouldNotDependOnAggregatesId,
            "Event Storming external systems must not depend on aggregates",
            "Event Storming external system '{0}' must not depend on aggregate '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An Event Storming external system depends directly on an aggregate.",
                "External systems should integrate through contracts, commands, or events rather than aggregate internals.",
                "Introduce an integration boundary and remove the direct aggregate dependency from the external system."));
    }
}
