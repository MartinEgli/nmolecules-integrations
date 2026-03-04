using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ApplicationServiceAnalyzers
{
    internal static class TypeDependencyAnalyzer
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsLegacyService() && !type.IsDomainService() && !type.IsApplicationService())
            {
                yield return symbol.Diagnostic(Rules.ApplicationServicesShouldNotUseLegacyServicesRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }
        }
    }
}
