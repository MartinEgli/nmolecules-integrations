using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Shared.Analyzers;

public class PropertyAnalyzer : IPropertyAnalyzer
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

public interface IPropertyAnalyzer
{
    void AnalyzeProperty(SymbolAnalysisContext context);
}

public class NamedTypeAnalyzer : INamedTypeAnalyzer
{
    private readonly Action<SymbolAnalysisContext> analyze;

    public NamedTypeAnalyzer(Action<SymbolAnalysisContext> analyze)
    {
        this.analyze = analyze;
    }

    public void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        analyze(context);
    }
}

public interface INamedTypeAnalyzer
{
    void AnalyzeNamedType(SymbolAnalysisContext context);
}