using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Shared.Analyzers;

public class FieldAnalyzer : IFieldAnalyzer
{
    private readonly Func<IFieldSymbol, IEnumerable<Diagnostic>> analyzeField;

    public FieldAnalyzer(Func<IFieldSymbol, IEnumerable<Diagnostic>> analyzeField)
    {
        this.analyzeField = analyzeField;
    }

    public void AnalyzeField(SymbolAnalysisContext context)
    {
        var fieldSymbol = (IFieldSymbol)context.Symbol;
        var violations = analyzeField(fieldSymbol);
        context.ReportDiagnostics(violations);
    }
}

public interface IFieldAnalyzer
{
    void AnalyzeField(SymbolAnalysisContext context);
}