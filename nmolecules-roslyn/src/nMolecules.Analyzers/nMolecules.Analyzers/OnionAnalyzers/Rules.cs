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
            "Onion ring symbol '{0}' must not depend on outer ring type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Inner onion rings must stay independent from outer implementation rings.");

        public static readonly DiagnosticDescriptor DomainModelRingMustNotDependOnOuterRingsRule = new(
            DomainModelRingMustNotDependOnOuterRingsId,
            "Domain model rings must not depend on outer rings",
            "Domain model ring symbol '{0}' must not depend on outer ring type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "The domain model ring is the innermost classic onion ring and must stay independent from service and infrastructure rings.");

        public static readonly DiagnosticDescriptor DomainServiceRingMustNotDependOnOuterRingsRule = new(
            DomainServiceRingMustNotDependOnOuterRingsId,
            "Domain service rings must not depend on outer rings",
            "Domain service ring symbol '{0}' must not depend on outer ring type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Classic domain service rings must not depend on application or infrastructure rings.");

        public static readonly DiagnosticDescriptor ApplicationServiceRingMustNotDependOnInfrastructureRingRule = new(
            ApplicationServiceRingMustNotDependOnInfrastructureRingId,
            "Application service rings must not depend on infrastructure rings",
            "Application service ring symbol '{0}' must not depend on infrastructure ring type '{1}'",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Classic application service rings should orchestrate domain behavior and remain independent from infrastructure ring implementations.");

        public static readonly DiagnosticDescriptor ClassicAndSimplifiedOnionStylesShouldNotMixRule = new(
            ClassicAndSimplifiedOnionStylesShouldNotMixId,
            "Classic and simplified Onion styles must not be mixed in one compilation",
            "Onion declaration on {0} mixes classic and simplified markers in the same compilation",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Use either classic or simplified onion markers in one compilation unit, not both.");
    }
}
