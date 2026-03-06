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

            if (type.IsDomainService())
            {
                yield return symbol.ViolatesDomainServiceUsage(type);
            }

            if (type.IsApplicationService())
            {
                yield return symbol.ViolatesApplicationServiceUsage(type);
            }

            if (type.IsLegacyService())
            {
                yield return symbol.ViolatesLegacyServiceUsage(type);
            }

            if (type.IsFactory())
            {
                yield return symbol.ViolatesFactoryUsage(type);
            }
        }

        private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseRepositoriesRule);
        private static Diagnostic ViolatesAggregateRootUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseAggregateRootsRule);
        private static Diagnostic ViolatesDomainServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(Rules.EntitiesShouldNotUseDomainServicesRule, symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(), type.DisplayName(), symbol.Name);
        private static Diagnostic ViolatesApplicationServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(Rules.EntitiesShouldNotUseApplicationServicesRule, symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(), type.DisplayName(), symbol.Name);
        private static Diagnostic ViolatesLegacyServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(Rules.EntitiesShouldNotUseLegacyServicesRule, symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(), type.DisplayName(), symbol.Name);
        private static Diagnostic ViolatesFactoryUsage(this ISymbol symbol, ITypeSymbol type) => symbol.Diagnostic(Rules.EntitiesShouldNotUseFactoriesRule, symbol.DisplayName(), type.DisplayName());
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
