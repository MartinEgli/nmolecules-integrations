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
                    yield return symbol.Diagnostic(Rules.DomainLayersShouldNotUseApplicationLayersRule);
                }

                if (type.IsUserInterfaceLayer())
                {
                    yield return symbol.Diagnostic(Rules.DomainLayersShouldNotUseUserInterfaceLayersRule);
                }

                if (type.IsInfrastructureLayer())
                {
                    yield return symbol.Diagnostic(Rules.DomainLayersShouldNotUseInfrastructureLayersRule);
                }
            }

            if (owner.IsApplicationLayer() && type.IsUserInterfaceLayer())
            {
                yield return symbol.Diagnostic(Rules.ApplicationLayersShouldNotUseUserInterfaceLayersRule);
            }
        }
    }
}
