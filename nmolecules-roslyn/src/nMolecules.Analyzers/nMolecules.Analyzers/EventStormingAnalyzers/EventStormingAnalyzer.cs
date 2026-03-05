using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.EventStormingAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EventStormingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Rules.ActorsShouldNotDependOnAggregatesRule,
            Rules.CommandsShouldDependOnAggregatesRule,
            Rules.PoliciesShouldDependOnDomainEventsRule,
            Rules.ReadModelsShouldNotDependOnAggregatesRule,
            Rules.ExternalSystemsShouldNotDependOnAggregatesRule);

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
            if (!type.IsEventStormingType())
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

            if (type.IsEventStormingCommand() && !DependsOn(type, candidate => candidate.IsEventStormingAggregate()))
            {
                context.ReportDiagnostic(type.Diagnostic(
                    Rules.CommandsShouldDependOnAggregatesRule,
                    type.DisplayName()));
            }

            if (type.IsEventStormingPolicy() && !DependsOn(type, candidate => candidate.IsEventStormingDomainEvent()))
            {
                context.ReportDiagnostic(type.Diagnostic(
                    Rules.PoliciesShouldDependOnDomainEventsRule,
                    type.DisplayName()));
            }
        }

        private static bool DependsOn(INamedTypeSymbol type, Func<ITypeSymbol, bool> predicate)
        {
            if (type.BaseType is { SpecialType: not SpecialType.System_Object } baseType && predicate(baseType))
            {
                return true;
            }

            foreach (var implementedInterface in type.Interfaces)
            {
                if (predicate(implementedInterface))
                {
                    return true;
                }
            }

            foreach (var member in type.GetMembers())
            {
                switch (member)
                {
                    case IFieldSymbol field when predicate(field.Type):
                        return true;
                    case IPropertySymbol property when predicate(property.Type):
                        return true;
                    case IMethodSymbol method when method.MethodKind is not MethodKind.PropertyGet and not MethodKind.PropertySet:
                    {
                        if (!method.ReturnsVoid && predicate(method.ReturnType))
                        {
                            return true;
                        }

                        foreach (var parameter in method.Parameters)
                        {
                            if (predicate(parameter.Type))
                            {
                                return true;
                            }
                        }

                        break;
                    }
                }
            }

            return false;
        }
    }
}
