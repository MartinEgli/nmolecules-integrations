using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.ApplicationServiceAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ApplicationServiceAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule,
                Rules.ApplicationServicesShouldNotUseLegacyServicesRule,
                Rules.ApplicationServicesShouldNotUseApplicationServicesRule,
                Rules.ApplicationServicesShouldNotExposeInfrastructureSignaturesRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
            context.RegisterSymbolAction(AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            if (!type.IsApplicationService())
            {
                return;
            }

            context.ReportDiagnostics(Diagnostics.AnalyzeType(type));
        }

        private static void AnalyzeField(SymbolAnalysisContext context)
        {
            var field = (IFieldSymbol)context.Symbol;
            if (!field.ContainingType.IsApplicationService())
            {
                return;
            }

            context.ReportDiagnostics(TypeDependencyAnalyzer.AnalyzeTypeInSymbol(field, field.Type));

            if (field.DeclaredAccessibility == Accessibility.Public && field.Type.IsInfrastructureLayer())
            {
                context.ReportDiagnostic(field.ViolatesInfrastructureSignature(field.Type));
            }
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var method = (IMethodSymbol)context.Symbol;
            if (!method.ContainingType.IsApplicationService() || method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
            {
                return;
            }

            if (!method.ReturnsVoid)
            {
                context.ReportDiagnostics(TypeDependencyAnalyzer.AnalyzeTypeInSymbol(method, method.ReturnType));
            }

            foreach (var parameter in method.Parameters)
            {
                context.ReportDiagnostics(TypeDependencyAnalyzer.AnalyzeTypeInSymbol(parameter, parameter.Type));

                if (method.DeclaredAccessibility == Accessibility.Public && parameter.Type.IsInfrastructureLayer())
                {
                    context.ReportDiagnostic(parameter.ViolatesInfrastructureSignature(parameter.Type));
                }
            }

            if (method.DeclaredAccessibility == Accessibility.Public && !method.ReturnsVoid && method.ReturnType.IsInfrastructureLayer())
            {
                context.ReportDiagnostic(method.ViolatesInfrastructureSignature(method.ReturnType));
            }
        }

        private static void AnalyzeProperty(SymbolAnalysisContext context)
        {
            var property = (IPropertySymbol)context.Symbol;
            if (!property.ContainingType.IsApplicationService())
            {
                return;
            }

            context.ReportDiagnostics(TypeDependencyAnalyzer.AnalyzeTypeInSymbol(property, property.Type));

            if (property.DeclaredAccessibility == Accessibility.Public && property.Type.IsInfrastructureLayer())
            {
                context.ReportDiagnostic(property.ViolatesInfrastructureSignature(property.Type));
            }
        }

        private static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol is not { ContainingType: { } containingType } || !containingType.IsApplicationService())
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
                context.ReportDiagnostics(TypeDependencyAnalyzer.AnalyzeTypeInSymbol(local, local.Type));
            }
        }
    }
}
