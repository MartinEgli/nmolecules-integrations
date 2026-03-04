using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.DDD;
using static NMolecules.Analyzers.ValueObjectAnalyzers.Rules;

namespace NMolecules.Analyzers.ValueObjectAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValueObjectAnalyzer : Analyzer<ValueObjectAttribute>
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ValueObjectMustNotUseEntityRule,
            ValueObjectMustNotUseServiceRule,
            ValueObjectMustNotUseRepositoryRule,
            ValueObjectMustNotUseAggregateRootRule,
            ValueObjectShouldBeImmutableRule,
            ValueObjectMustNotDeclareIdentityRule,
            ValueObjectMustNotUseFactoryRule,
            ValueObjectMustImplementIEquatableRule,
            ValueObjectShouldBeSealedRule);

        protected override void Initialize(AnalysisContext<ValueObjectAttribute> context)
        {
            var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeUsageInSymbol);
            var propertyAnalyzer = new ValueObjectPropertyAnalyzer();
            var valueObjectFieldAnalyzer = new ValueObjectFieldAnalyzer();
            context.RegisterSymbolAction(methodAnalyzer.AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(propertyAnalyzer.AnalyzeProperty, SymbolKind.Property);
            context.RegisterSymbolAction(ClassSymbolAnalyzer.AnalyzeType, SymbolKind.NamedType);
            context.RegisterSymbolAction(valueObjectFieldAnalyzer.AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeIdentityMember, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeIdentityMember, SymbolKind.Property);
            context.RegisterSyntaxNodeAction(methodAnalyzer.AnalyzeDeclarations, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeIdentityMember(SymbolAnalysisContext context)
        {
            var diagnostic = Diagnostics.AnalyzeIdentityDeclaration(context.Symbol);
            if (diagnostic is not null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
