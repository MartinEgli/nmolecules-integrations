﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NMolecules.Shared.Analyzers;

namespace NMolecules.DDD.Analyzers.RepositoryAnalyzers;

public static class Diagnostics
{
    public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(IFieldSymbol symbol) => AnalyzeTypeInSymbol(symbol, symbol.Type);
    public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(IPropertySymbol symbol) => AnalyzeTypeInSymbol(symbol, symbol.Type);


    public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
    {
        if (type.IsService())
        {
            yield return symbol.ViolatesServiceUsage();
        }
    }

    private static Diagnostic ViolatesServiceUsage(this ISymbol symbol) => symbol.Diagnostic(Rules.RepositoriesShouldNotUseServicesRule);
}