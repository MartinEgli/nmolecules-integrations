using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.LayerAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LayerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Rules.DomainLayersShouldNotUseApplicationLayersRule,
            Rules.DomainLayersShouldNotUseUserInterfaceLayersRule,
            Rules.DomainLayersShouldNotUseInfrastructureLayersRule,
            Rules.InterfaceLayersShouldNotUseDomainLayersRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            var fieldAnalyzer = new FieldAnalyzer(it => Diagnostics.AnalyzeTypeInSymbol(it, it.Type));
            var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
            var propertyAnalyzer = new PropertyAnalyzer(it => Diagnostics.AnalyzeTypeInSymbol(it, it.Type));

            context.RegisterSymbolAction(fieldAnalyzer.AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(methodAnalyzer.AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(propertyAnalyzer.AnalyzeProperty, SymbolKind.Property);
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(methodAnalyzer.AnalyzeDeclarations, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol) context.Symbol;

            if (!type.IsLayer())
            {
                return;
            }

            if (type.BaseType is { SpecialType: not SpecialType.System_Object } baseType)
            {
                context.ReportDiagnostics(Diagnostics.AnalyzeTypeInSymbol(type, baseType));
            }

            foreach (var @interface in type.Interfaces)
            {
                context.ReportDiagnostics(Diagnostics.AnalyzeTypeInSymbol(type, @interface));
            }
        }
    }
}
