using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Shared.Analyzers;

public static class ContextExtensions
{
    public static void ReportDiagnostics(this SymbolAnalysisContext context, IEnumerable<Diagnostic> violations)
    {
        foreach (var violation in violations)
        {
            context.ReportDiagnostic(violation);
        }
    }

    public static void ReportDiagnostics(this SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> violations)
    {
        foreach (var violation in violations)
        {
            context.ReportDiagnostic(violation);
        }
    }

    public static void RegisterSymbolAction<T>(this AnalysisContext<T> context, IFieldAnalyzer analyzer) where T : Attribute
    {
        context.RegisterSymbolAction(analyzer.AnalyzeField, SymbolKind.Field);
    }

    public static void RegisterSymbolAction<T>(this AnalysisContext<T> context, IMethodAnalyzer analyzer) where T : Attribute
    {
        context.RegisterSymbolAction(analyzer.AnalyzeMethod, SymbolKind.Method);
    }

    public static void RegisterSymbolAction<T>(this AnalysisContext<T> context, IPropertyAnalyzer analyzer) where T : Attribute
    {
        context.RegisterSymbolAction(analyzer.AnalyzeProperty, SymbolKind.Property);
    }
    public static void RegisterSymbolAction<T>(this AnalysisContext<T> context, INamedTypeAnalyzer analyzer) where T : Attribute
    {
        context.RegisterSymbolAction(analyzer.AnalyzeNamedType, SymbolKind.NamedType);
    }
    
    public static void RegisterSyntaxNodeAction<T>(this AnalysisContext<T> context, IMethodAnalyzer analyzer) where T : Attribute
    {
        context.RegisterSyntaxNodeAction(analyzer.AnalyzeDeclarations, SyntaxKind.LocalDeclarationStatement);
    }
}