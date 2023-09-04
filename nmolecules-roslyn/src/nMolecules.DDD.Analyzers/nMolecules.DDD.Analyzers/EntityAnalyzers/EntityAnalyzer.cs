using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.Shared.Analyzers;

namespace NMolecules.DDD.Analyzers.EntityAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EntityAnalyzer : Analyzer<EntityAttribute>
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rules.EntitiesShouldNotUseRepositoriesRule,
        Rules.EntitiesShouldNotUseAggregateRootsRule,
        Rules.EntitiesShouldNotUseServicesRule,
        Rules.EntitiesShouldHaveIdRule);

    protected override void Initialize(AnalysisContext<EntityAttribute> context)
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