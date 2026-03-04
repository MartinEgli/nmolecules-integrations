using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.EventAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DomainEventHandlerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.DomainEventHandlersMustConsumeDomainEventsRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;
            if (!method.IsDomainEventHandler())
            {
                return;
            }

            if (!method.Parameters.Any(parameter => parameter.Type.IsDomainEvent()))
            {
                context.ReportDiagnostic(method.Diagnostic(
                    Rules.DomainEventHandlersMustConsumeDomainEventsRule,
                    method.DiagnosticTargetName()));
            }
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            if (!type.IsDomainEventHandler() || type.TypeKind != TypeKind.Delegate)
            {
                return;
            }

            var invokeMethod = type.DelegateInvokeMethod;
            if (invokeMethod is null || !invokeMethod.Parameters.Any(parameter => parameter.Type.IsDomainEvent()))
            {
                context.ReportDiagnostic(type.Diagnostic(
                    Rules.DomainEventHandlersMustConsumeDomainEventsRule,
                    type.DiagnosticTargetName()));
            }
        }
    }
}
