using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.Shared.Analyzers;
using static NMolecules.DDD.Analyzers.ValueObjectAnalyzers.Rules;

namespace NMolecules.DDD.Analyzers.ValueObjectAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValueObjectAnalyzer : Analyzer<ValueObjectAttribute>
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ValueObjectMustNotUseEntityRule,
        ValueObjectMustNotUseServiceRule,
        ValueObjectMustNotUseRepositoryRule,
        ValueObjectMustNotUseAggregateRootRule,
        ValueObjectShouldBeImmutableRule,
        ValueObjectMustImplementIEquatableRule,
        ValueObjectShouldBeSealedRule);

    protected override void Initialize(AnalysisContext<ValueObjectAttribute> context)
    {
        var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeUsageInSymbol);
        var propertyAnalyzer = new ValueObjectPropertyAnalyzer();
        var valueObjectFieldAnalyzer = new ValueObjectFieldAnalyzer();
        var namedTypeAnalyzer = new NamedTypeAnalyzer(ClassSymbolAnalyzer.AnalyzeType);
        
        context.RegisterSymbolAction(methodAnalyzer);
        context.RegisterSymbolAction(propertyAnalyzer);
        context.RegisterSymbolAction(namedTypeAnalyzer);
        context.RegisterSymbolAction(valueObjectFieldAnalyzer);
        context.RegisterSyntaxNodeAction(methodAnalyzer);
    }
}