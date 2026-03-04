using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.HexagonalAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HexagonalAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Rules.ApplicationCoreShouldNotDependOnPortsOrAdaptersRule,
                Rules.PrimaryPortsShouldNotDependOnAdaptersRule,
                Rules.SecondaryPortsShouldNotDependOnAdaptersRule,
                Rules.PrimaryAdaptersShouldDependOnPrimaryPortsRule,
                Rules.SecondaryAdaptersShouldDependOnSecondaryPortsRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            var fieldAnalyzer = new FieldAnalyzer(it => Diagnostics.AnalyzeTypeInSymbol(it, it.Type));
            var methodAnalyzer = new MethodAnalyzer(Diagnostics.AnalyzeTypeInSymbol);
            var propertyAnalyzer = new PropertyAnalyzer(it => Diagnostics.AnalyzeTypeInSymbol(it, it.Type));

            context.RegisterSymbolAction(fieldAnalyzer.AnalyzeField, SymbolKind.Field);
            context.RegisterSymbolAction(methodAnalyzer.AnalyzeMethod, SymbolKind.Method);
            context.RegisterSymbolAction(propertyAnalyzer.AnalyzeProperty, SymbolKind.Property);
            context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(methodAnalyzer.AnalyzeDeclarations, SyntaxKind.LocalDeclarationStatement);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            if (!type.IsHexagonalType())
            {
                return;
            }

            if (type.BaseType is { SpecialType: not SpecialType.System_Object } baseType)
            {
                context.ReportDiagnostics(Diagnostics.AnalyzeTypeInSymbol(type, baseType));
            }

            foreach (var implementedInterface in type.Interfaces)
            {
                context.ReportDiagnostics(Diagnostics.AnalyzeTypeInSymbol(type, implementedInterface));
            }

            if (type.IsPrimaryAdapter() && !DependsOnPort(type, isPrimary: true))
            {
                context.ReportDiagnostic(type.Diagnostic(
                    Rules.PrimaryAdaptersShouldDependOnPrimaryPortsRule,
                    type.DisplayName()));
            }

            if (type.IsSecondaryAdapter() && !DependsOnPort(type, isPrimary: false))
            {
                context.ReportDiagnostic(type.Diagnostic(
                    Rules.SecondaryAdaptersShouldDependOnSecondaryPortsRule,
                    type.DisplayName()));
            }
        }

        private static bool DependsOnPort(INamedTypeSymbol type, bool isPrimary)
        {
            if (MatchesRequiredPort(type.BaseType, isPrimary))
            {
                return true;
            }

            if (type.Interfaces.Any(it => MatchesRequiredPort(it, isPrimary)))
            {
                return true;
            }

            foreach (var member in type.GetMembers())
            {
                switch (member)
                {
                    case IFieldSymbol field when MatchesRequiredPort(field.Type, isPrimary):
                        return true;
                    case IPropertySymbol property when MatchesRequiredPort(property.Type, isPrimary):
                        return true;
                    case IMethodSymbol method when method.MethodKind is not MethodKind.PropertyGet and not MethodKind.PropertySet:
                    {
                        if (MatchesRequiredPort(method.ReturnType, isPrimary))
                        {
                            return true;
                        }

                        if (method.Parameters.Any(parameter => MatchesRequiredPort(parameter.Type, isPrimary)))
                        {
                            return true;
                        }

                        break;
                    }
                }
            }

            return false;
        }

        private static bool MatchesRequiredPort(ITypeSymbol? type, bool isPrimary) =>
            type is not null &&
            ((isPrimary && type.IsPrimaryPort()) || (!isPrimary && type.IsSecondaryPort()));
    }
}
