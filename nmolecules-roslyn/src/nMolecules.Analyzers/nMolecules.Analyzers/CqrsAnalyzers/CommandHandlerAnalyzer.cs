using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommandHandlerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rules.CommandHandlersMustNotDependOnQueryModelsRule);

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
            if (!method.IsCommandHandler() || method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
            {
                return;
            }

            if (!method.ReturnsVoid && method.ReturnType.IsQueryModel())
            {
                context.ReportDiagnostic(method.ViolatesCommandHandlerQueryModelDependency(method.ReturnType));
            }

            foreach (var parameter in method.Parameters)
            {
                if (parameter.Type.IsQueryModel())
                {
                    context.ReportDiagnostic(parameter.ViolatesCommandHandlerQueryModelDependency(parameter.Type));
                }
            }
        }

        private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is not IMethodSymbol method || !method.IsCommandHandler())
            {
                return;
            }

            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;
            if (localDeclaration.Declaration.Variables.Count != 1)
            {
                return;
            }

            var variable = localDeclaration.Declaration.Variables[0];
            if (context.SemanticModel.GetDeclaredSymbol(variable) is ILocalSymbol local && local.Type.IsQueryModel())
            {
                context.ReportDiagnostic(local.ViolatesCommandHandlerQueryModelDependency(local.Type));
            }
        }
    }
}
