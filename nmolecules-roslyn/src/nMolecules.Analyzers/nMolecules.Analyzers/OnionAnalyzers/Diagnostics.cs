using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.OnionAnalyzers
{
    internal static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            var owner = symbol as ITypeSymbol ?? symbol.ContainingType;
            if (owner is null || !owner.IsOnionRing())
            {
                yield break;
            }

            if (owner.IsClassicDomainModelRing() &&
                (type.IsClassicDomainServiceRing() || type.IsClassicApplicationServiceRing() || type.IsOnionInfrastructureRing()))
            {
                yield return symbol.ViolatesInwardDependency(type, Rules.DomainModelRingMustNotDependOnOuterRingsRule);
            }

            if (owner.IsClassicDomainServiceRing() &&
                (type.IsClassicApplicationServiceRing() || type.IsOnionInfrastructureRing()))
            {
                yield return symbol.ViolatesInwardDependency(type, Rules.DomainServiceRingMustNotDependOnOuterRingsRule);
            }

            if (owner.IsClassicApplicationServiceRing() &&
                type.IsOnionInfrastructureRing())
            {
                yield return symbol.ViolatesInwardDependency(type, Rules.ApplicationServiceRingMustNotDependOnInfrastructureRingRule);
            }

            if (owner.IsSimplifiedDomainRing() &&
                (type.IsSimplifiedApplicationRing() || type.IsOnionInfrastructureRing()))
            {
                yield return symbol.ViolatesInwardDependency(type, Rules.OnionDependenciesMustPointInwardRule);
            }

            if (owner.IsSimplifiedApplicationRing() &&
                type.IsOnionInfrastructureRing())
            {
                yield return symbol.ViolatesInwardDependency(type, Rules.OnionDependenciesMustPointInwardRule);
            }
        }

        private static Diagnostic ViolatesInwardDependency(this ISymbol symbol, ITypeSymbol dependency, DiagnosticDescriptor rule) =>
            symbol.Diagnostic(rule, symbol.DiagnosticTargetName(), dependency.DisplayName());

        internal static bool IsOnionRing(this ITypeSymbol type) =>
            type.IsClassicDomainModelRing() ||
            type.IsClassicDomainServiceRing() ||
            type.IsClassicApplicationServiceRing() ||
            type.IsSimplifiedDomainRing() ||
            type.IsSimplifiedApplicationRing() ||
            type.IsOnionInfrastructureRing();

        internal static bool IsClassicDomainModelRing(this ITypeSymbol type) =>
            type.HasAttributeNamed("DomainModelRingAttribute");

        internal static bool IsClassicDomainServiceRing(this ITypeSymbol type) =>
            type.HasAttributeNamed("DomainServiceRingAttribute");

        internal static bool IsClassicApplicationServiceRing(this ITypeSymbol type) =>
            type.HasAttributeNamed("ApplicationServiceRingAttribute");

        internal static bool IsSimplifiedDomainRing(this ITypeSymbol type) =>
            type.HasAttributeNamed("DomainRingAttribute");

        internal static bool IsSimplifiedApplicationRing(this ITypeSymbol type) =>
            type.HasAttributeNamed("ApplicationRingAttribute");

        internal static bool IsOnionInfrastructureRing(this ITypeSymbol type) =>
            type.HasAttributeNamed("InfrastructureRingAttribute");
    }
}
