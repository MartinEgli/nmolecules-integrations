using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using static NMolecules.Analyzers.IdAnalyzer;

namespace NMolecules.Analyzers.EntityAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsRepository())
            {
                yield return symbol.ViolatesRepositoryUsage();
            }

            if (type.IsAggregateRoot())
            {
                yield return symbol.ViolatesAggregateRootUsage();
            }

            if (type.IsService())
            {
                yield return symbol.ViolatesServiceUsage();
            }
        }

        private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseRepositoriesRule);
        private static Diagnostic ViolatesAggregateRootUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseAggregateRootsRule);
        private static Diagnostic ViolatesServiceUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseServicesRule);
        public static Diagnostic ViolatesMandatoryId(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldHaveIdRule);

        public static Diagnostic ViolatesMultipleIdentities(this INamedTypeSymbol symbol)
        {
            var identities = GetIdentityMembers(symbol);
            return symbol.Diagnostic(
                Rules.EntitiesShouldHaveSingleIdRule,
                symbol.DisplayName(),
                identities.Count,
                string.Join(", ", identities.Select(it => it.Name)));
        }
    }
}
