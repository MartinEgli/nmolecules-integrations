using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.LayerAnalyzers
{
    internal static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            var owner = symbol as ITypeSymbol ?? symbol.ContainingType;

            if (owner is null)
            {
                yield break;
            }

            if (owner.IsDomainLayer())
            {
                if (type.IsApplicationLayer())
                {
                    yield return symbol.Diagnostic(Rules.DomainLayersShouldNotUseApplicationLayersRule, symbol.DiagnosticTargetName(), type.DisplayName());
                }

                if (type.IsInfrastructureLayer())
                {
                    yield return symbol.Diagnostic(Rules.DomainLayersShouldNotUseInfrastructureLayersRule, symbol.DiagnosticTargetName(), type.DisplayName());
                }

                if (type.IsUserInterfaceLayer())
                {
                    yield return symbol.Diagnostic(Rules.DomainLayersShouldNotUseUserInterfaceLayersRule, symbol.DiagnosticTargetName(), type.DisplayName());
                }
            }

            if (owner.IsUserInterfaceLayer() && type.IsDomainLayer())
            {
                yield return symbol.Diagnostic(Rules.InterfaceLayersShouldNotUseDomainLayersRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }

            if (owner.IsApplicationLayer() && type.IsInfrastructureLayer())
            {
                yield return symbol.Diagnostic(Rules.ApplicationLayersShouldLimitInfrastructureDependenciesRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }

            if (owner.IsInfrastructureLayer() && type.IsApplicationLayer())
            {
                yield return symbol.Diagnostic(Rules.InfrastructureLayersShouldUseApplicationLayersForWiringOnlyRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }
        }
    }
}
