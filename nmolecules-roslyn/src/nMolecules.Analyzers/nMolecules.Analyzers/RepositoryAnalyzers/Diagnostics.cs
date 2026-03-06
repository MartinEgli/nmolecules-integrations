using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.RepositoryAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
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

            if (type.IsRepository() && !symbol.AllowsRepositoryComposition())
            {
                yield return symbol.ViolatesRepositoryUsage(type);
            }

            if (type.IsRepository() && symbol.AllowsRepositoryComposition() && type is INamedTypeSymbol { TypeKind: not TypeKind.Interface })
            {
                yield return symbol.ViolatesRepositoryCompositionContract(type);
            }

            if (type.IsFactory())
            {
                yield return symbol.ViolatesFactoryUsage(type);
            }
        }

        private static Diagnostic ViolatesDomainServiceUsage(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(Rules.RepositoriesShouldNotUseDomainServicesRule, repository, type.DisplayName(), symbol.Name);
        }

        private static Diagnostic ViolatesApplicationServiceUsage(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(Rules.RepositoriesShouldNotUseApplicationServicesRule, repository, type.DisplayName(), symbol.Name);
        }

        private static Diagnostic ViolatesLegacyServiceUsage(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(Rules.RepositoriesShouldNotUseLegacyServicesRule, repository, type.DisplayName(), symbol.Name);
        }
        private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(Rules.RepositoriesShouldNotDependOnRepositoriesRule, repository, type.DisplayName(), symbol.Name);
        }

        private static Diagnostic ViolatesRepositoryCompositionContract(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(
                Rules.ApprovedRepositoryCompositionShouldUseContractsRule,
                repository,
                symbol.Name,
                type.DisplayName());
        }

        private static Diagnostic ViolatesFactoryUsage(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(
                Rules.RepositoriesShouldNotDependOnFactoriesRule,
                repository,
                type.DisplayName(),
                symbol.Name);
        }

        public static Diagnostic ViolatesInfrastructureSignature(this ISymbol symbol, ITypeSymbol type)
        {
            var repository = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(
                Rules.RepositoriesShouldNotExposeInfrastructureSignaturesRule,
                repository,
                type.DisplayName(),
                symbol.Name);
        }
    }
}
