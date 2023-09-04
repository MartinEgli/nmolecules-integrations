using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.Shared.Analyzers;

namespace NMolecules.DDD.Analyzers.RepositoryAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RepositoryAnalyzer : Analyzer<RepositoryAttribute>
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rules.RepositoriesShouldNotUseServicesRule);

    protected override void Initialize(AnalysisContext<RepositoryAttribute> context)
    {
        var fieldAnalyzer = new FieldAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
        var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
        var propertyAnalyzer = new PropertyAnalyzer(Diagnostics.AnalyzeTypeInSymbol);

        context.RegisterSymbolAction(fieldAnalyzer);
        context.RegisterSymbolAction(methodAnalyzer);
        context.RegisterSymbolAction(propertyAnalyzer);
        context.RegisterSyntaxNodeAction(methodAnalyzer);
    }
}