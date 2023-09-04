using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.Shared.Analyzers;

namespace NMolecules.DDD.Analyzers.AggregateRootAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AggregateRootAnalyzer : Analyzer<AggregateRootAttribute>
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Rules.AggregateRootsShouldNotUseRepositoriesRule,
        Rules.AggregateRootsShouldNotUseServicesRule,
        Rules.AggregateRootsShouldHaveIdRule);

    protected override void Initialize(AnalysisContext<AggregateRootAttribute> context)
    {
        var fieldAnalyzer = new FieldAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
        var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
        var propertyAnalyzer = new PropertyAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
        var namedTypeAnalyzer = new NamedTypeAnalyzer(it => it.AnalyzeEntityForId(typeSymbol => typeSymbol.ViolatesMandatoryId()));

        context.RegisterSymbolAction(fieldAnalyzer);
        context.RegisterSymbolAction(methodAnalyzer);
        context.RegisterSymbolAction(propertyAnalyzer);
        context.RegisterSymbolAction(namedTypeAnalyzer);
        context.RegisterSyntaxNodeAction(methodAnalyzer);
    }
}