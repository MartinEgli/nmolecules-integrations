using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NMolecules.DDD;

namespace NMolecules.Analyzers.RepositoryAnalyzers
{
    [Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RepositoryAnalyzer : Analyzer<RepositoryAttribute>
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Rules.RepositoriesShouldNotUseServicesRule,
                Rules.RepositoriesShouldNotExposeInfrastructureSignaturesRule);

        protected override void Initialize(AnalysisContext<RepositoryAttribute> context)
        {
            var fieldAnalyzer = new FieldAnalyzer(AnalyzeField);
            var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
            var propertyAnalyzer = new PropertyAnalyzer(AnalyzeProperty);
            context.RegisterSymbolAction(fieldAnalyzer.AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(propertyAnalyzer.AnalyzeProperty, SymbolKind.Property);
            context.RegisterSyntaxNodeAction(methodAnalyzer.AnalyzeDeclarations, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
            methodAnalyzer.AnalyzeMethod(context);

            var method = (IMethodSymbol)context.Symbol;
            if (method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet || method.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            if (!method.ReturnsVoid && method.ReturnType.IsInfrastructureLayer())
            {
                context.ReportDiagnostic(method.ViolatesInfrastructureSignature(method.ReturnType));
            }

            foreach (var parameter in method.Parameters)
            {
                if (parameter.Type.IsInfrastructureLayer())
                {
                    context.ReportDiagnostic(parameter.ViolatesInfrastructureSignature(parameter.Type));
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol field)
        {
            foreach (var diagnostic in Diagnostics.AnalyzeTypeInSymbol(field, field.Type))
            {
                yield return diagnostic;
            }

            if (field.DeclaredAccessibility == Accessibility.Public && field.Type.IsInfrastructureLayer())
            {
                yield return field.ViolatesInfrastructureSignature(field.Type);
            }
        }

        private static System.Collections.Generic.IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol property)
        {
            foreach (var diagnostic in Diagnostics.AnalyzeTypeInSymbol(property, property.Type))
            {
                yield return diagnostic;
            }

            if (property.DeclaredAccessibility == Accessibility.Public && property.Type.IsInfrastructureLayer())
            {
                yield return property.ViolatesInfrastructureSignature(property.Type);
            }
        }
    }
}
