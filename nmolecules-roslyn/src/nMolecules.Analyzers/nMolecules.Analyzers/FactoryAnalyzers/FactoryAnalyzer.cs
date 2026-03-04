using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.DDD;

namespace NMolecules.Analyzers.FactoryAnalyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FactoryAnalyzer : Analyzer<FactoryAttribute>
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Rules.FactoriesShouldNotUseApplicationServicesRule,
                Rules.FactoriesShouldNotUseRepositoriesRule,
                Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksRule);

        protected override void Initialize(AnalysisContext<FactoryAttribute> context)
        {
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
            var fieldAnalyzer = new FieldAnalyzer(it => Diagnostics.AnalyzeTypeInSymbol(it, it.Type));
            var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
            var propertyAnalyzer = new PropertyAnalyzer(it => Diagnostics.AnalyzeTypeInSymbol(it, it.Type));
            context.RegisterSymbolAction(fieldAnalyzer.AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(methodAnalyzer.AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(propertyAnalyzer.AnalyzeProperty, SymbolKind.Property);
            context.RegisterSyntaxNodeAction(methodAnalyzer.AnalyzeDeclarations, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            context.ReportDiagnostics(Diagnostics.AnalyzeType(type));
        }
    }
}
