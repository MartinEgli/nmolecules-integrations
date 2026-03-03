using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.DDD;

namespace NMolecules.Analyzers.ServiceAnalyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ServiceAnalyzer : Analyzer<ServiceAttribute>
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.LegacyServicesShouldUseSpecificRoleRule);

        protected override void Initialize(AnalysisContext<ServiceAttribute> context)
        {
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        private static void AnalyzeType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            if (type.IsDomainService() || type.IsApplicationService())
            {
                return;
            }

            context.ReportDiagnostic(type.Diagnostic(Rules.LegacyServicesShouldUseSpecificRoleRule));
        }
    }
}
