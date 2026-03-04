using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class QueryHandlerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.QueryHandlersMustStayOnReadSideRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;
            if (!method.IsQueryHandler() || method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
            {
                return;
            }

            if (!method.ReturnsVoid)
            {
                context.ReportDiagnostics(AnalyzeTypeInSymbol(method, method.ReturnType));
            }

            foreach (var parameter in method.Parameters)
            {
                context.ReportDiagnostics(AnalyzeTypeInSymbol(parameter, parameter.Type));
            }
        }

        private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is not IMethodSymbol method || !method.IsQueryHandler())
            {
                return;
            }

            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
            if (localDeclaration.Declaration.Variables.Count != 1)
            {
                return;
            }

            var variable = localDeclaration.Declaration.Variables[0];
            if (context.SemanticModel.GetDeclaredSymbol(variable) is ILocalSymbol local)
            {
                context.ReportDiagnostics(AnalyzeTypeInSymbol(local, local.Type));
            }
        }

        private static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsEntity())
            {
                yield return symbol.ViolatesQueryHandlerReadSideDependency("entity", type);
            }

            if (type.IsAggregateRoot())
            {
                yield return symbol.ViolatesQueryHandlerReadSideDependency("aggregate root", type);
            }

            if (type.IsRepository())
            {
                yield return symbol.ViolatesQueryHandlerReadSideDependency("repository", type);
            }

            if (type.IsFactory())
            {
                yield return symbol.ViolatesQueryHandlerReadSideDependency("factory", type);
            }

            if (type.IsDomainService())
            {
                yield return symbol.ViolatesQueryHandlerReadSideDependency("domain service", type);
            }

            if (type.IsApplicationService())
            {
                yield return symbol.ViolatesQueryHandlerReadSideDependency("application service", type);
            }
        }
    }
}
