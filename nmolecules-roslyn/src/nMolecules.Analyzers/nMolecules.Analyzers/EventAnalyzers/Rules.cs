using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.EventAnalyzers
{
    public static class Rules
    {
        public const string DomainEventsMustNotReferenceEntitiesId = "XMoleculesDomainEvent0001";
        public const string DomainEventsMustNotReferenceAggregateRootsId = "XMoleculesDomainEvent0002";
        public const string DomainEventsMustNotReferenceRepositoriesId = "XMoleculesDomainEvent0003";
        public const string DomainEventsMustNotReferenceServicesId = "XMoleculesDomainEvent0004";
        public const string DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesId = "XMoleculesDomainEvent0005";
        public const string RepositoriesAndFactoriesMustNotPublishDomainEventsId = "XMoleculesDomainEvent0006";
        public const string DomainEventHandlersMustConsumeDomainEventsId = "XMoleculesDomainEvent0007";
        public const string DomainEventPublishersShouldExposeDomainEventPayloadsId = "XMoleculesDomainEvent0008";
        public const string DomainEventHandlersShouldHandleSingleDomainEventPayloadId = "XMoleculesDomainEvent0009";

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceEntitiesRule = new(
            DomainEventsMustNotReferenceEntitiesId,
            "Domain events must not reference entities",
            "Domain event '{0}' must not reference entity '{1}' in member '{2}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The domain event payload captures an entity reference.",
                "Domain events should carry transport-friendly data, not live entity objects with identity and behavior.",
                "Publish the entity identity or a value snapshot instead of the entity reference itself."));

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceAggregateRootsRule = new(
            DomainEventsMustNotReferenceAggregateRootsId,
            "Domain events must not reference aggregate roots",
            "Domain event '{0}' must not reference aggregate root '{1}' in member '{2}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The domain event payload captures an aggregate root reference.",
                "Domain events should describe something that happened, not retain a live aggregate boundary.",
                "Publish aggregate identity or copied event data instead of a direct aggregate root reference."));

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceRepositoriesRule = new(
            DomainEventsMustNotReferenceRepositoriesId,
            "Domain events must not reference repositories",
            "Domain event '{0}' must not reference repository '{1}' in member '{2}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The domain event payload references a repository type.",
                "Domain event payloads are facts and must not embed retrieval or persistence abstractions.",
                "Remove the repository dependency and publish only the data consumers need to react to the event."));

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceServicesRule = new(
            DomainEventsMustNotReferenceServicesId,
            "Domain events must not reference services",
            "Domain event '{0}' must not reference {1} '{2}' in member '{3}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The domain event payload references a service role such as a domain service, application service, or legacy service.",
                "Domain events should remain stable payloads and must not carry orchestration or service abstractions.",
                "Publish pure event data and let consumers resolve their own collaborators outside the event type."));

        public static readonly DiagnosticDescriptor DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesRule = new(
            DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesId,
            "Domain event publishers should prefer aggregate roots or application services",
            "{0} '{1}' publishes domain events from host '{2}'; prefer AggregateRoot or ApplicationService as the default source",
            Category.Events,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A domain event publisher is hosted on a type that is neither an aggregate root nor an application service.",
                "Domain event publication is usually owned by aggregate roots or application services because they naturally model state changes and use-case orchestration.",
                "Move publication to an aggregate root or application service unless the alternative host is an intentional, documented exception."));

        public static readonly DiagnosticDescriptor RepositoriesAndFactoriesMustNotPublishDomainEventsRule = new(
            RepositoriesAndFactoriesMustNotPublishDomainEventsId,
            "Repositories and factories must not publish domain events directly",
            "{0} '{1}' must not be marked as a domain event publisher",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A repository or factory is marked as a domain event publisher.",
                "Repositories and factories must not publish domain events because persistence and creation concerns are not event-source responsibilities.",
                "Publish the event from the aggregate root or the application service that owns the state change instead."));

        public static readonly DiagnosticDescriptor DomainEventHandlersMustConsumeDomainEventsRule = new(
            DomainEventHandlersMustConsumeDomainEventsId,
            "Domain event handlers must consume domain events",
            "Domain event handler '{0}' must declare at least one parameter of a [DomainEvent] type",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain event handler is marked as a handler but has no [DomainEvent] payload parameter.",
                "Handlers must explicitly consume an event payload so their event contract is visible and analyzable.",
                "Add exactly one domain-event parameter or remove the handler marker if the method is not an event handler."));

        public static readonly DiagnosticDescriptor DomainEventPublishersShouldExposeDomainEventPayloadsRule = new(
            DomainEventPublishersShouldExposeDomainEventPayloadsId,
            "Domain event publishers should expose domain event payloads explicitly",
            "Domain event publisher '{0}' should declare a [DomainEvent] parameter or return type",
            Category.Events,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A method-level domain event publisher does not expose a [DomainEvent] payload in its parameters or return type.",
                "Publisher methods should make the published event contract explicit instead of hiding it behind side effects.",
                "Add a domain-event parameter or return type so the publication contract is visible in the API."));

        public static readonly DiagnosticDescriptor DomainEventHandlersShouldHandleSingleDomainEventPayloadRule = new(
            DomainEventHandlersShouldHandleSingleDomainEventPayloadId,
            "Domain event handlers should handle exactly one domain event payload",
            "Domain event handler '{0}' declares multiple [DomainEvent] parameters",
            Category.Events,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A domain event handler declares multiple [DomainEvent] payload parameters.",
                "Handlers should focus on one event contract at a time so event-flow responsibilities stay explicit.",
                "Split the logic into separate handlers or reduce the signature to one domain-event payload."));
    }
}
