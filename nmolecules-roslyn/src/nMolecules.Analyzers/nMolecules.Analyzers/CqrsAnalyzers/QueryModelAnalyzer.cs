using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class QueryModelAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.QueryModelsMustBeReadOnlyRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
        }

        private static void AnalyzeField(SymbolAnalysisContext context)
        {
            var field = (IFieldSymbol) context.Symbol;
            if (!field.ContainingType.IsQueryModel() || field.IsStatic || field.IsConst || field.IsReadOnly)
            {
                return;
            }

            context.ReportDiagnostic(field.ViolatesReadOnlyRule());
        }

        private static void AnalyzeProperty(SymbolAnalysisContext context)
        {
            var property = (IPropertySymbol) context.Symbol;
            if (!property.ContainingType.IsQueryModel() || property.IsStatic || property.SetMethod is null)
            {
                return;
            }

            if (property.SetMethod.DeclaredAccessibility == Accessibility.Private)
            {
                return;
            }

            context.ReportDiagnostic(property.ViolatesReadOnlyRule());
        }
    }
}
