using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.OnionAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OnionAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                Rules.OnionDependenciesMustPointInwardRule,
                Rules.ClassicAndSimplifiedOnionStylesShouldNotMixRule);

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
            context.RegisterCompilationAction(AnalyzeStyleMixing);
        }

        private static void AnalyzeNamedType(SymbolAnalysisContext context)
        {
            var type = (INamedTypeSymbol)context.Symbol;
            if (!type.IsOnionRing())
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
        }

        private static void AnalyzeStyleMixing(CompilationAnalysisContext context)
        {
            var declarations = CollectOnionStyleDeclarations(context.Compilation).ToArray();
            var hasClassic = declarations.Any(it => it.Style == OnionStyle.Classic);
            var hasSimplified = declarations.Any(it => it.Style == OnionStyle.Simplified);
            if (!hasClassic || !hasSimplified)
            {
                return;
            }

            foreach (var declaration in declarations)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rules.ClassicAndSimplifiedOnionStylesShouldNotMixRule,
                    declaration.Location,
                    declaration.ScopeLabel));
            }
        }

        private static IEnumerable<OnionStyleDeclaration> CollectOnionStyleDeclarations(Compilation compilation)
        {
            foreach (var declaration in CollectOnionStyleDeclarations(compilation.Assembly))
            {
                yield return declaration;
            }

            foreach (var declaration in CollectOnionStyleDeclarations(compilation.SourceModule))
            {
                yield return declaration;
            }

            foreach (var type in GetAllTypes(compilation.Assembly.GlobalNamespace))
            {
                foreach (var declaration in CollectOnionStyleDeclarations(type))
                {
                    yield return declaration;
                }
            }
        }

        private static IEnumerable<OnionStyleDeclaration> CollectOnionStyleDeclarations(ISymbol symbol)
        {
            var scopeLabel = symbol switch
            {
                IAssemblySymbol assembly => $"assembly '{assembly.Name}'",
                IModuleSymbol module => $"module '{module.Name}'",
                _ => $"type '{symbol.Name}'"
            };

            foreach (var attribute in symbol.GetAttributes())
            {
                var style = GetOnionStyleFromAttribute(attribute);
                if (style == OnionStyle.None)
                {
                    continue;
                }

                var location = symbol.Locations.FirstOrDefault() ??
                               attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
                if (location is null)
                {
                    continue;
                }

                yield return new OnionStyleDeclaration(style, scopeLabel, location);
            }
        }

        private static OnionStyle GetOnionStyleFromAttribute(AttributeData attribute)
        {
            var name = attribute.AttributeClass?.Name;
            return name switch
            {
                "DomainModelRingAttribute" => OnionStyle.Classic,
                "DomainServiceRingAttribute" => OnionStyle.Classic,
                "ApplicationServiceRingAttribute" => OnionStyle.Classic,
                "DomainRingAttribute" => OnionStyle.Simplified,
                "ApplicationRingAttribute" => OnionStyle.Simplified,
                _ => OnionStyle.None
            };
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol current)
        {
            foreach (var type in current.GetTypeMembers())
            {
                yield return type;

                foreach (var nested in GetNestedTypes(type))
                {
                    yield return nested;
                }
            }

            foreach (var nestedNamespace in current.GetNamespaceMembers())
            {
                foreach (var nestedType in GetAllTypes(nestedNamespace))
                {
                    yield return nestedType;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol current)
        {
            foreach (var nested in current.GetTypeMembers())
            {
                yield return nested;

                foreach (var nestedChild in GetNestedTypes(nested))
                {
                    yield return nestedChild;
                }
            }
        }

        private sealed class OnionStyleDeclaration
        {
            public OnionStyleDeclaration(OnionStyle style, string scopeLabel, Location location)
            {
                Style = style;
                ScopeLabel = scopeLabel;
                Location = location;
            }

            public OnionStyle Style { get; }
            public string ScopeLabel { get; }
            public Location Location { get; }
        }

        private enum OnionStyle
        {
            None = 0,
            Classic = 1,
            Simplified = 2
        }
    }
}
