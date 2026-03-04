using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NMolecules.DDD;

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

            if (type.IsFactory() &&
                !SymbolEqualityComparer.Default.Equals(symbol.ContainingType, type))
            {
                yield return symbol.Diagnostic(Rules.FactoriesShouldNotUseFactoriesRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }
        }

        public static IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol type)
        {
            if (type.IsEntity())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(EntityAttribute).Replace("Attribute", ""));
            }

            if (type.IsAggregateRoot())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(AggregateRootAttribute).Replace("Attribute", ""));
            }

            if (type.Is<ValueObjectAttribute>())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(ValueObjectAttribute).Replace("Attribute", ""));
            }

            if (type.IsRepository())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(RepositoryAttribute).Replace("Attribute", ""));
            }

            if (type.IsLegacyService())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(ServiceAttribute).Replace("Attribute", ""));
            }

            if (type.IsDomainService())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), "DomainService");
            }

            if (type.IsApplicationService())
            {
                yield return type.Diagnostic(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), "ApplicationService");
            }
        }
    }
}
