using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.OnionAnalyzers
{
    public static class Rules
    {
        public const string OnionDependenciesMustPointInwardId = "XMoleculesOnion0001";
        public const string DomainModelRingMustNotDependOnOuterRingsId = "XMoleculesOnion0002";
        public const string DomainServiceRingMustNotDependOnOuterRingsId = "XMoleculesOnion0003";
        public const string ApplicationServiceRingMustNotDependOnInfrastructureRingId = "XMoleculesOnion0004";
        public const string ClassicAndSimplifiedOnionStylesShouldNotMixId = "XMoleculesOnion0005";

        public static readonly DiagnosticDescriptor OnionDependenciesMustPointInwardRule = new(
            OnionDependenciesMustPointInwardId,
            "Onion dependencies must point inward only",
            "Onion ring '{0}' must not depend on outer ring '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An inner onion ring depends outward on a more external ring.",
                "Onion architecture requires all dependencies to point inward toward the domain core.",
                "Invert the dependency through an inward-facing abstraction and keep outer implementation details from leaking inward."));

        public static readonly DiagnosticDescriptor DomainModelRingMustNotDependOnOuterRingsRule = new(
            DomainModelRingMustNotDependOnOuterRingsId,
            "Domain model rings must not depend on outer rings",
            "Domain model ring '{0}' must not depend on outer ring '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain-model ring type depends on a more external onion ring.",
                "The domain model ring is the innermost core and must stay free of service, application, and infrastructure concerns.",
                "Move the dependency into an outer ring or introduce a domain-owned abstraction that the outer ring implements."));

        public static readonly DiagnosticDescriptor DomainServiceRingMustNotDependOnOuterRingsRule = new(
            DomainServiceRingMustNotDependOnOuterRingsId,
            "Domain service rings must not depend on outer rings",
            "Domain service ring '{0}' must not depend on outer ring '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A domain-service ring type depends on an outer onion ring.",
                "Domain services belong close to the model and must not depend on application or infrastructure rings.",
                "Keep the domain-service ring focused on domain policies and push outward dependencies behind inward-facing abstractions."));

        public static readonly DiagnosticDescriptor ApplicationServiceRingMustNotDependOnInfrastructureRingRule = new(
            ApplicationServiceRingMustNotDependOnInfrastructureRingId,
            "Application service rings must not depend on infrastructure rings",
            "Application service ring '{0}' must not depend on infrastructure ring '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An application-service ring type depends directly on an infrastructure ring type.",
                "Application services orchestrate the domain and should not be coupled directly to infrastructure implementations in classic onion architecture.",
                "Depend on an inward-facing port and let the infrastructure ring implement that port."));

        public static readonly DiagnosticDescriptor ClassicAndSimplifiedOnionStylesShouldNotMixRule = new(
            ClassicAndSimplifiedOnionStylesShouldNotMixId,
            "Classic and simplified onion styles must not mix in the same compilation",
            "Onion declaration on {0} mixes classic and simplified markers in the same compilation",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The same compilation mixes classic onion markers and simplified onion markers.",
                "One compilation should follow one onion style so ring semantics and dependency expectations remain coherent.",
                "Choose either classic onion or simplified onion markers for the compilation and remove the conflicting style markers."));
    }
}
