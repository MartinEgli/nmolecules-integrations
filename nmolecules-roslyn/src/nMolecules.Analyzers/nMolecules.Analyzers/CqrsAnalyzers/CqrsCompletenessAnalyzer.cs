using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CqrsCompletenessAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.CqrsSupportRequiresQueryAndQueryHandlerRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(InitializeCompilation);
        }

        private static void InitializeCompilation(CompilationStartAnalysisContext context)
        {
            var queries = new ConcurrentBag<INamedTypeSymbol>();
            var queryHandlers = new ConcurrentBag<IMethodSymbol>();

            context.RegisterSymbolAction(symbolContext =>
            {
                var type = (INamedTypeSymbol)symbolContext.Symbol;
                if (type.IsQuery())
                {
                    queries.Add(type);
                }
            }, SymbolKind.NamedType);

            context.RegisterSymbolAction(symbolContext =>
            {
                var method = (IMethodSymbol) symbolContext.Symbol;
                if (method.IsQueryHandler())
                {
                    queryHandlers.Add(method);
                }
            }, SymbolKind.Method);

            context.RegisterCompilationEndAction(endContext =>
            {
                var hasQueries = !queries.IsEmpty;
                var hasQueryHandlers = !queryHandlers.IsEmpty;

                if (hasQueries && !hasQueryHandlers)
                {
                    foreach (var query in queries)
                    {
                        endContext.ReportDiagnostic(query.ViolatesCqrsCompleteness("Query", "[QueryHandler]"));
                    }
                }

                if (!hasQueries && hasQueryHandlers)
                {
                    foreach (var handler in queryHandlers)
                    {
                        endContext.ReportDiagnostic(handler.ViolatesCqrsCompleteness("Query handler", "[Query]"));
                    }
                }
            });
        }
    }
}
