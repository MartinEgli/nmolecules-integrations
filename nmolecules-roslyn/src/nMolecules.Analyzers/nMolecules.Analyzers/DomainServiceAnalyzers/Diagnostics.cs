using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.DomainServiceAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsApplicationService())
            {
                yield return symbol.Diagnostic(Rules.DomainServicesShouldNotUseApplicationServicesRule);
            }
        }
    }
}
