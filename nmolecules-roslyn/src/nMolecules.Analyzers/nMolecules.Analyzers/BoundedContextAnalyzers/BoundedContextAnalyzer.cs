using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static NMolecules.Analyzers.BoundedContextAnalyzers.Rules;

namespace NMolecules.Analyzers.BoundedContextAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BoundedContextAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                BoundedContextShouldDefineIdRule,
                BoundedContextShouldDefineNameRule,
                BoundedContextShouldUseSingleIdPerCompilationRule,
                BoundedContextShouldUseSingleNamePerIdRule,
                BoundedContextModuleOwnershipShouldMatchScopeIdRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var declarations = new List<BoundedContextDeclaration>();
            declarations.AddRange(AnalyzeScope(context, context.Compilation.Assembly));
            declarations.AddRange(AnalyzeScope(context, context.Compilation.SourceModule));
            AnalyzeIdConsistency(context, declarations);
            AnalyzeNameConsistencyPerId(context, declarations);
            AnalyzeModuleOwnershipConsistency(context, declarations);
        }

        private static IEnumerable<BoundedContextDeclaration> AnalyzeScope(CompilationAnalysisContext context, ISymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes().Where(IsBoundedContextAttribute))
            {
                var id = attribute.GetNamedString("Id");

                if (attribute.SupportsMember("Id") &&
                    IsBlank(id))
                {
                    context.Report(attribute, symbol, BoundedContextShouldDefineIdRule, symbol.MetadataScopeLabel());
                }

                var supportsName = attribute.SupportsMember("Name");
                var supportsValue = attribute.SupportsMember("Value");
                var name = attribute.GetNameOrAliasValue();
                if ((supportsName || supportsValue) &&
                    IsBlank(name))
                {
                    context.Report(attribute, symbol, BoundedContextShouldDefineNameRule, symbol.MetadataScopeLabel());
                }

                yield return new BoundedContextDeclaration(symbol, attribute, id, name);
            }
        }

        private static void AnalyzeIdConsistency(
            CompilationAnalysisContext context,
            IEnumerable<BoundedContextDeclaration> declarations)
        {
            var declaredIds = declarations
                .Select(it => it.Id)
                .Where(it => !IsBlank(it))
                .Distinct(System.StringComparer.OrdinalIgnoreCase)
                .OrderBy(it => it)
                .ToArray();

            if (declaredIds.Length <= 1)
            {
                return;
            }

            var declaredList = string.Join(", ", declaredIds);
            foreach (var declaration in declarations.Where(it => !IsBlank(it.Id)))
            {
                context.Report(
                    declaration.Attribute,
                    declaration.Symbol,
                    BoundedContextShouldUseSingleIdPerCompilationRule,
                    declaration.Symbol.MetadataScopeLabel(),
                    declaration.Id!,
                    declaredList);
            }
        }

        private static void AnalyzeNameConsistencyPerId(
            CompilationAnalysisContext context,
            IEnumerable<BoundedContextDeclaration> declarations)
        {
            var groups = declarations
                .Where(it => !IsBlank(it.Id))
                .Where(it => !IsBlank(it.Name))
                .GroupBy(it => it.Id!, System.StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                var names = group
                    .Select(it => it.Name!)
                    .Distinct(System.StringComparer.OrdinalIgnoreCase)
                    .OrderBy(it => it)
                    .ToArray();

                if (names.Length <= 1)
                {
                    continue;
                }

                var declaredNames = string.Join(", ", names);
                foreach (var declaration in group)
                {
                    context.Report(
                        declaration.Attribute,
                        declaration.Symbol,
                        BoundedContextShouldUseSingleNamePerIdRule,
                        declaration.Symbol.MetadataScopeLabel(),
                        declaration.Name!,
                        group.Key,
                        declaredNames);
                }
            }
        }

        private static void AnalyzeModuleOwnershipConsistency(
            CompilationAnalysisContext context,
            IEnumerable<BoundedContextDeclaration> declarations)
        {
            var idsByScope = declarations
                .Where(it => !IsBlank(it.Id))
                .GroupBy(it => it.Symbol, SymbolEqualityComparer.Default)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(it => it.Id!)
                        .Distinct(System.StringComparer.OrdinalIgnoreCase)
                        .ToArray(),
                    SymbolEqualityComparer.Default);

            AnalyzeScopeModuleOwnership(context, context.Compilation.Assembly, idsByScope);
            AnalyzeScopeModuleOwnership(context, context.Compilation.SourceModule, idsByScope);
        }

        private static void AnalyzeScopeModuleOwnership(
            CompilationAnalysisContext context,
            ISymbol symbol,
            IReadOnlyDictionary<ISymbol, string[]> idsByScope)
        {
            if (!idsByScope.TryGetValue(symbol, out var scopeIds) ||
                scopeIds.Length != 1)
            {
                return;
            }

            var scopeId = scopeIds[0];
            foreach (var moduleAttribute in symbol.GetAttributes().Where(IsModuleAttribute))
            {
                if (!moduleAttribute.SupportsMember("BoundedContextId"))
                {
                    continue;
                }

                var moduleBoundedContextId = moduleAttribute.GetNamedString("BoundedContextId");
                if (IsBlank(moduleBoundedContextId))
                {
                    continue;
                }

                if (!moduleBoundedContextId!.Equals(scopeId, System.StringComparison.OrdinalIgnoreCase))
                {
                    context.Report(
                        moduleAttribute,
                        symbol,
                        BoundedContextModuleOwnershipShouldMatchScopeIdRule,
                        symbol.MetadataScopeLabel(),
                        moduleBoundedContextId,
                        scopeId);
                }
            }
        }

        private static bool IsBoundedContextAttribute(AttributeData attribute) =>
            attribute.AttributeClass?.Name == "BoundedContextAttribute";

        private static bool IsModuleAttribute(AttributeData attribute) =>
            attribute.AttributeClass?.Name == "ModuleAttribute";

        private static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);

        private sealed class BoundedContextDeclaration
        {
            public BoundedContextDeclaration(ISymbol symbol, AttributeData attribute, string? id, string? name)
            {
                Symbol = symbol;
                Attribute = attribute;
                Id = id;
                Name = name;
            }

            public ISymbol Symbol { get; }
            public AttributeData Attribute { get; }
            public string? Id { get; }
            public string? Name { get; }
        }
    }

    internal static class BoundedContextAnalyzerExtensions
    {
        public static bool SupportsMember(this AttributeData attribute, string memberName) =>
            attribute.AttributeClass?.GetMembers(memberName).Length > 0;

        public static string? GetNamedString(this AttributeData attribute, string memberName) =>
            attribute.NamedArguments.FirstOrDefault(it => it.Key == memberName).Value.Value as string;

        public static string? GetNameOrAliasValue(this AttributeData attribute)
        {
            var name = attribute.GetNamedString("Name");
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            var alias = attribute.GetNamedString("Value");
            if (!string.IsNullOrWhiteSpace(alias))
            {
                return alias;
            }

            if (attribute.ConstructorArguments.Length > 0 &&
                attribute.ConstructorArguments[0].Value is string constructorName &&
                !string.IsNullOrWhiteSpace(constructorName))
            {
                return constructorName;
            }

            return null;
        }

        public static string MetadataScopeLabel(this ISymbol symbol) => symbol switch
        {
            IAssemblySymbol assembly => $"assembly '{assembly.Name}'",
            IModuleSymbol module => $"module '{module.Name}'",
            _ => $"symbol '{symbol.Name}'"
        };

        public static void Report(
            this CompilationAnalysisContext context,
            AttributeData attribute,
            ISymbol symbol,
            DiagnosticDescriptor descriptor,
            params object[] arguments)
        {
            var location = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ??
                           symbol.Locations.FirstOrDefault();
            if (location is null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(descriptor, location, arguments));
        }
    }
}
