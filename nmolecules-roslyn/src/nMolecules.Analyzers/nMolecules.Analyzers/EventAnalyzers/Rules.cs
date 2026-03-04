using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.EventAnalyzers
{
    public static class Rules
    {
        public const string DomainEventsMustNotReferenceEntitiesId = "XMoleculesDomainEvent0001";
        public const string DomainEventsMustNotReferenceAggregateRootsId = "XMoleculesDomainEvent0002";
        public const string DomainEventsMustNotReferenceRepositoriesId = "XMoleculesDomainEvent0003";
        public const string DomainEventsMustNotReferenceServicesId = "XMoleculesDomainEvent0004";
        public const string RepositoriesAndFactoriesMustNotPublishDomainEventsId = "XMoleculesDomainEvent0006";

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceEntitiesRule = new(
            DomainEventsMustNotReferenceEntitiesId,
            "Domain events must not reference entities",
            "Domain event '{0}' must not reference entity '{1}' in member '{2}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            "Domain event payloads should remain transport-friendly and must not capture live entity references.");

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceAggregateRootsRule = new(
            DomainEventsMustNotReferenceAggregateRootsId,
            "Domain events must not reference aggregate roots",
            "Domain event '{0}' must not reference aggregate root '{1}' in member '{2}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            "Domain event payloads should remain transport-friendly and must not capture live aggregate root references.");

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceRepositoriesRule = new(
            DomainEventsMustNotReferenceRepositoriesId,
            "Domain events must not reference repositories",
            "Domain event '{0}' must not reference repository '{1}' in member '{2}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            "Domain event payloads must not carry repository abstractions.");

        public static readonly DiagnosticDescriptor DomainEventsMustNotReferenceServicesRule = new(
            DomainEventsMustNotReferenceServicesId,
            "Domain events must not reference services",
            "Domain event '{0}' must not reference {1} '{2}' in member '{3}'",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            "Domain event payloads must not carry service abstractions or orchestration roles.");

        public static readonly DiagnosticDescriptor RepositoriesAndFactoriesMustNotPublishDomainEventsRule = new(
            RepositoriesAndFactoriesMustNotPublishDomainEventsId,
            "Repositories and factories must not publish domain events directly",
            "{0} '{1}' must not be marked as a domain event publisher",
            Category.Events,
            DiagnosticSeverity.Error,
            true,
            "Repository and factory components are forbidden default sources for domain event publication.");
    }
}
