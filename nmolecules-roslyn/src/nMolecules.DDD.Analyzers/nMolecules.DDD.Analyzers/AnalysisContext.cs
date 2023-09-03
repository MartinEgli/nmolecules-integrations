﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.DDD.Analyzers;

public class AnalysisContext<TAttribute> where TAttribute : Attribute
{
    private readonly AnalysisContext analysisContext;

    public AnalysisContext(AnalysisContext analysisContext)
    {
        this.analysisContext = analysisContext;
    }

    public void RegisterSymbolAction(Action<SymbolAnalysisContext> action, SymbolKind symbolKind)
    {
        analysisContext.RegisterSymbolAction(IfCorrectType(action, symbolKind), symbolKind);
    }

    private static Action<SymbolAnalysisContext> IfCorrectType(
        Action<SymbolAnalysisContext> analyze,
        SymbolKind symbolKind)
    {
        return symbolKind switch
        {
            SymbolKind.NamedType => it =>
            {
                var classType = (INamedTypeSymbol)it.Symbol;
                if (classType.Is<TAttribute>())
                {
                    analyze(it);
                }
            }
            ,
            SymbolKind.Namespace => analyze,
            _ => it =>
            {
                var classType = it.Symbol.ContainingType;
                if (classType.Is<TAttribute>())
                {
                    analyze(it);
                }
            }
        };
    }

    public void RegisterSyntaxNodeAction(Action<SyntaxNodeAnalysisContext> analysis, SyntaxKind syntaxKind)
    {
        analysisContext.RegisterSyntaxNodeAction(IfCorrectType(analysis), syntaxKind);
    }

    private static Action<SyntaxNodeAnalysisContext> IfCorrectType(Action<SyntaxNodeAnalysisContext> analyze)
    {
        return it =>
        {
            if (it.ContainingSymbol is { ContainingType: ITypeSymbol classSymbol } && classSymbol.Is<TAttribute>())
            {
                analyze(it);
            }
        };
    }
}