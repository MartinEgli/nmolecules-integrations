using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using static NMolecules.Analyzers.IdAnalyzer;

namespace NMolecules.Analyzers.AggregateRootAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsAggregateRoot())
            {
                yield return symbol.ViolatesAggregateRootUsage();
            }

            if (type.IsRepository())
            {
                yield return symbol.ViolatesRepositoryUsage();
            }

            if (type.IsService() && !type.IsApplicationService())
            {
                yield return symbol.ViolatesServiceUsage();
            }

            if (type.IsApplicationService())
            {
                yield return symbol.ViolatesApplicationServiceUsage(type);
            }

            if (type.IsFactory())
            {
                yield return symbol.ViolatesFactoryUsage(type);
            }
        }

        private static Diagnostic ViolatesAggregateRootUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldNotUseAggregateRootsRule);
        private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldNotUseRepositoriesRule);
        private static Diagnostic ViolatesServiceUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldNotUseServicesRule);
        private static Diagnostic ViolatesApplicationServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(
                Rules.AggregateRootsShouldNotUseApplicationServicesRule,
                symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(),
                type.DisplayName(),
                symbol.Name);
        private static Diagnostic ViolatesFactoryUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(
                Rules.AggregateRootsShouldNotUseFactoriesRule,
                symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(),
                type.DisplayName(),
                symbol.Name);
        public static Diagnostic ViolatesMandatoryId(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldHaveIdRule);

        public static Diagnostic ViolatesMultipleIdentities(this INamedTypeSymbol symbol)
        {
            var identities = GetIdentityMembers(symbol);
            return symbol.Diagnostic(
                Rules.AggregateRootsShouldHaveSingleIdRule,
                symbol.DisplayName(),
                identities.Count,
                string.Join(", ", identities.Select(it => it.Name)));
        }
    }
}
