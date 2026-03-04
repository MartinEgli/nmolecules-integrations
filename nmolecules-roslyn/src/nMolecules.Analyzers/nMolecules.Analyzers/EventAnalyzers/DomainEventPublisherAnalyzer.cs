using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.EventAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DomainEventPublisherAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Rules.DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesRule,
                Rules.RepositoriesAndFactoriesMustNotPublishDomainEventsRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            if (!type.IsDomainEventPublisher())
            {
                return;
            }

            ReportIfForbiddenHost(context, type, type);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;
            if (!method.IsDomainEventPublisher() || method.ContainingType is null)
            {
                return;
            }

            ReportIfForbiddenHost(context, method, method.ContainingType);
        }

        private static void ReportIfForbiddenHost(SymbolAnalysisContext context, ISymbol publisher, ITypeSymbol host)
        {
            if (!host.IsAggregateRoot() && !host.IsApplicationService() && !host.IsRepository() && !host.IsFactory())
            {
                context.ReportDiagnostic(publisher.Diagnostic(
                    Rules.DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesRule,
                    publisher.Kind.ToString(),
                    publisher.DiagnosticTargetName(),
                    host.DisplayName()));
            }

            if (host.IsRepository())
            {
                context.ReportDiagnostic(publisher.Diagnostic(
                    Rules.RepositoriesAndFactoriesMustNotPublishDomainEventsRule,
                    "Repository",
                    publisher.DiagnosticTargetName()));
            }

            if (host.IsFactory())
            {
                context.ReportDiagnostic(publisher.Diagnostic(
                    Rules.RepositoriesAndFactoriesMustNotPublishDomainEventsRule,
                    "Factory",
                    publisher.DiagnosticTargetName()));
            }
        }
    }
}
