using Microsoft.CodeAnalysis;
using NMolecules.Shared.Analyzers;

namespace NMolecules.DDD.Analyzers.EntityAnalyzers;

public static class Diagnostics
{
    public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(IFieldSymbol symbol) => AnalyzeTypeInSymbol(symbol, symbol.Type);
    public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(IPropertySymbol symbol) => AnalyzeTypeInSymbol(symbol, symbol.Type);

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

    public static Diagnostic ViolatesMandatoryId(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldHaveIdRule);

    private static Diagnostic ViolatesAggregateRootUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseAggregateRootsRule);

    private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseRepositoriesRule);
    private static Diagnostic ViolatesServiceUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.EntitiesShouldNotUseServicesRule);
}