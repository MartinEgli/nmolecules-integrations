﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NMolecules.Shared.Analyzers;

namespace NMolecules.DDD.Analyzers.AggregateRootAnalyzers;

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

        if (type.IsService())
        {
            yield return symbol.ViolatesServiceUsage();
        }
    }

    public static Diagnostic ViolatesMandatoryId(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldHaveIdRule);

    private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldNotUseRepositoriesRule);
    private static Diagnostic ViolatesServiceUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.AggregateRootsShouldNotUseServicesRule);
}