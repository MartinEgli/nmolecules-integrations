﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.DDD.Analyzers;

public class PropertyAnalyzer
{
    private readonly Func<IPropertySymbol, IEnumerable<Diagnostic>> analyzeProperty;

    public PropertyAnalyzer(Func<IPropertySymbol, IEnumerable<Diagnostic>> analyzeProperty)
    {
        this.analyzeProperty = analyzeProperty;
    }

    public void AnalyzeProperty(SymbolAnalysisContext context)
    {
        var propertySymbol = (IPropertySymbol)context.Symbol;
        var violations = analyzeProperty(propertySymbol);
        context.ReportDiagnostics(violations);
    }
}