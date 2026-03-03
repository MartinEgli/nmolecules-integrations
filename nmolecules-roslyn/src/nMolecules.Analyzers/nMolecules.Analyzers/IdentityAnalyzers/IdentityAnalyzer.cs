using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.IdentityAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IdentityAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.IdentityMustBelongToEntityOrAggregateRootRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Field, SymbolKind.Property);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = context.Symbol;
            if (!symbol.IsIdentity())
            {
                return;
            }

            var containingType = symbol.ContainingType;
            if (containingType.IsEntity() || containingType.IsAggregateRoot())
            {
                return;
            }

            context.ReportDiagnostic(symbol.Diagnostic(Rules.IdentityMustBelongToEntityOrAggregateRootRule));
        }
    }
}
