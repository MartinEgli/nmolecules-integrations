using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.CrossStyleAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CrossStyleAnalyzer : DiagnosticAnalyzer
    {
        private const string BoundedContextAttributeName = "BoundedContextAttribute";
        private const string BoundedContextIdPropertyName = "Id";
        private const string BoundedContextNamePropertyName = "Name";
        private const string BoundedContextValuePropertyName = "Value";
        private const string GlobalContextKey = "__compilation__";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Rules.PrimaryStylesMustFollowCompatibilityMatrixRule,
            Rules.CqrsMayOverlayPrimaryStyleRule,
            Rules.ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var declarations = CollectStyleDeclarations(context.Compilation).ToArray();
            if (declarations.Length == 0)
            {
                return;
            }

            foreach (var group in declarations.GroupBy(it => it.ContextKey))
            {
                AnalyzePrimaryStyleCompatibility(group, context);
                AnalyzeCqrsOverlay(group, context);
                AnalyzeOnionStyleCoexistence(group, context);
            }
        }

        private static void AnalyzePrimaryStyleCompatibility(IGrouping<string, StyleDeclaration> group, CompilationAnalysisContext context)
        {
            var hasLayered = group.Any(it => it.Style == StyleKind.Layered);
            var hasOnion = group.Any(it => it.Style == StyleKind.Onion);

            if (!hasLayered || !hasOnion)
            {
                return;
            }

            var compatibilityViolation = "Layered + Onion";
            var targetDeclarations = group.Where(it => it.Style == StyleKind.Layered || it.Style == StyleKind.Onion);
            foreach (var declaration in targetDeclarations)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rules.PrimaryStylesMustFollowCompatibilityMatrixRule,
                    declaration.Location,
                    declaration.ContextLabel,
                    compatibilityViolation));
            }
        }

        private static void AnalyzeCqrsOverlay(IGrouping<string, StyleDeclaration> group, CompilationAnalysisContext context)
        {
            var hasCqrs = group.Any(it => it.Style == StyleKind.Cqrs);
            if (!hasCqrs)
            {
                return;
            }

            var hasPrimaryStyle = group.Any(it =>
                it.Style == StyleKind.Layered ||
                it.Style == StyleKind.Onion ||
                it.Style == StyleKind.Hexagonal);

            if (hasPrimaryStyle)
            {
                return;
            }

            foreach (var declaration in group.Where(it => it.Style == StyleKind.Cqrs))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rules.CqrsMayOverlayPrimaryStyleRule,
                    declaration.Location,
                    declaration.ContextLabel));
            }
        }

        private static void AnalyzeOnionStyleCoexistence(IGrouping<string, StyleDeclaration> group, CompilationAnalysisContext context)
        {
            var representative = group.FirstOrDefault();
            if (representative is null || !representative.HasExplicitBoundedContext)
            {
                return;
            }

            var hasClassic = group.Any(it => it.OnionStyle == OnionStyle.Classic);
            var hasSimplified = group.Any(it => it.OnionStyle == OnionStyle.Simplified);
            if (!hasClassic || !hasSimplified)
            {
                return;
            }

            foreach (var declaration in group.Where(it => it.OnionStyle == OnionStyle.Classic || it.OnionStyle == OnionStyle.Simplified))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Rules.ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextRule,
                    declaration.Location,
                    declaration.ContextLabel));
            }
        }

        private static IEnumerable<StyleDeclaration> CollectStyleDeclarations(Compilation compilation)
        {
            foreach (var declaration in CollectStyleDeclarations(compilation.Assembly, compilation.Assembly))
            {
                yield return declaration;
            }

            foreach (var declaration in CollectStyleDeclarations(compilation.SourceModule, compilation.Assembly))
            {
                yield return declaration;
            }

            foreach (var type in GetAllTypes(compilation.Assembly.GlobalNamespace))
            {
                foreach (var declaration in CollectStyleDeclarations(type, compilation.Assembly))
                {
                    yield return declaration;
                }
            }
        }

        private static IEnumerable<StyleDeclaration> CollectStyleDeclarations(ISymbol symbol, IAssemblySymbol assembly)
        {
            var scopeLabel = symbol switch
            {
                IAssemblySymbol assemblySymbol => $"assembly '{assemblySymbol.Name}'",
                IModuleSymbol moduleSymbol => $"module '{moduleSymbol.Name}'",
                _ => $"type '{symbol.Name}'"
            };

            var contextInfo = ResolveContextInfo(symbol, assembly);

            foreach (var attribute in symbol.GetAttributes())
            {
                if (!TryGetStyle(attribute, out var style, out var onionStyle))
                {
                    continue;
                }

                var location = symbol.Locations.FirstOrDefault() ??
                               attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation();
                if (location is null)
                {
                    continue;
                }

                yield return new StyleDeclaration(
                    style,
                    onionStyle,
                    location,
                    scopeLabel,
                    contextInfo.Key,
                    contextInfo.Label,
                    contextInfo.IsExplicit);
            }
        }

        private static ContextInfo ResolveContextInfo(ISymbol symbol, IAssemblySymbol assembly)
        {
            var boundedContextName = ResolveBoundedContextName(symbol) ?? ResolveBoundedContextName(assembly);
            if (string.IsNullOrWhiteSpace(boundedContextName))
            {
                return new ContextInfo(GlobalContextKey, "compilation", false);
            }

            return new ContextInfo(
                $"bounded-context:{boundedContextName}",
                $"bounded context '{boundedContextName}'",
                true);
        }

        private static string? ResolveBoundedContextName(ISymbol symbol)
        {
            var attribute = symbol.GetAttributes()
                .FirstOrDefault(it => string.Equals(it.AttributeClass?.Name, BoundedContextAttributeName, StringComparison.Ordinal));
            if (attribute is null)
            {
                return null;
            }

            var id = attribute.NamedArguments
                .FirstOrDefault(it => string.Equals(it.Key, BoundedContextIdPropertyName, StringComparison.Ordinal))
                .Value
                .Value as string;
            if (!string.IsNullOrWhiteSpace(id))
            {
                return id;
            }

            var name = attribute.NamedArguments
                .FirstOrDefault(it => string.Equals(it.Key, BoundedContextNamePropertyName, StringComparison.Ordinal))
                .Value
                .Value as string;
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            var value = attribute.NamedArguments
                .FirstOrDefault(it => string.Equals(it.Key, BoundedContextValuePropertyName, StringComparison.Ordinal))
                .Value
                .Value as string;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (attribute.ConstructorArguments.Length > 0 &&
                attribute.ConstructorArguments[0].Value is string constructorValue &&
                !string.IsNullOrWhiteSpace(constructorValue))
            {
                return constructorValue;
            }

            return null;
        }

        private static bool TryGetStyle(AttributeData attribute, out StyleKind style, out OnionStyle onionStyle)
        {
            var attributeName = attribute.AttributeClass?.Name;
            style = StyleKind.None;
            onionStyle = OnionStyle.None;

            switch (attributeName)
            {
                case "DomainLayerAttribute":
                case "ApplicationLayerAttribute":
                case "InfrastructureLayerAttribute":
                case "InterfaceLayerAttribute":
                case "UserInterfaceLayerAttribute":
                    style = StyleKind.Layered;
                    return true;

                case "DomainModelRingAttribute":
                case "DomainServiceRingAttribute":
                case "ApplicationServiceRingAttribute":
                case "InfrastructureRingAttribute":
                case "DomainRingAttribute":
                case "ApplicationRingAttribute":
                    style = StyleKind.Onion;
                    onionStyle = attributeName switch
                    {
                        "DomainModelRingAttribute" => OnionStyle.Classic,
                        "DomainServiceRingAttribute" => OnionStyle.Classic,
                        "ApplicationServiceRingAttribute" => OnionStyle.Classic,
                        "DomainRingAttribute" => OnionStyle.Simplified,
                        "ApplicationRingAttribute" => OnionStyle.Simplified,
                        _ => OnionStyle.None
                    };
                    return true;

                case "ApplicationAttribute":
                case "PrimaryPortAttribute":
                case "SecondaryPortAttribute":
                case "PrimaryAdapterAttribute":
                case "SecondaryAdapterAttribute":
                    style = StyleKind.Hexagonal;
                    return true;

                case "CommandAttribute":
                case "CommandDispatcherAttribute":
                case "CommandHandlerAttribute":
                case "QueryAttribute":
                case "QueryHandlerAttribute":
                case "QueryModelAttribute":
                case "ProjectionAttribute":
                    style = StyleKind.Cqrs;
                    return true;

                default:
                    return false;
            }
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

        private sealed class StyleDeclaration
        {
            public StyleDeclaration(
                StyleKind style,
                OnionStyle onionStyle,
                Location location,
                string scopeLabel,
                string contextKey,
                string contextLabel,
                bool hasExplicitBoundedContext)
            {
                Style = style;
                OnionStyle = onionStyle;
                Location = location;
                ScopeLabel = scopeLabel;
                ContextKey = contextKey;
                ContextLabel = contextLabel;
                HasExplicitBoundedContext = hasExplicitBoundedContext;
            }

            public StyleKind Style { get; }
            public OnionStyle OnionStyle { get; }
            public Location Location { get; }
            public string ScopeLabel { get; }
            public string ContextKey { get; }
            public string ContextLabel { get; }
            public bool HasExplicitBoundedContext { get; }
        }

        private sealed class ContextInfo
        {
            public ContextInfo(string key, string label, bool isExplicit)
            {
                Key = key;
                Label = label;
                IsExplicit = isExplicit;
            }

            public string Key { get; }
            public string Label { get; }
            public bool IsExplicit { get; }
        }

        private enum StyleKind
        {
            None = 0,
            Layered = 1,
            Onion = 2,
            Hexagonal = 3,
            Cqrs = 4
        }

        private enum OnionStyle
        {
            None = 0,
            Classic = 1,
            Simplified = 2
        }
    }
}
