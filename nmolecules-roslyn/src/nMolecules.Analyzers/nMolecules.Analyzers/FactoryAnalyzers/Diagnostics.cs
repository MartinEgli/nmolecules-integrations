using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.FactoryAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsApplicationService())
            {
                yield return symbol.Diagnostic(Rules.FactoriesShouldNotUseApplicationServicesRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }

            if (type.IsRepository())
            {
                yield return symbol.Diagnostic(Rules.FactoriesShouldNotUseRepositoriesRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }
        }
    }
}
