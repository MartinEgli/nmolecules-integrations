using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static NMolecules.Analyzers.ModuleAnalyzers.Rules;

namespace NMolecules.Analyzers.ModuleAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ModuleAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(
                ModuleShouldDefineIdRule,
                ModuleShouldDefineNameRule,
                ModuleShouldDefineBoundedContextIdRule,
                ModuleShouldReferenceDeclaredBoundedContextRule,
                ModuleShouldUseSingleNamePerIdRule,
                ModuleShouldUseSingleBoundedContextIdPerIdRule,
                ModuleNameShouldMapToSingleIdPerBoundedContextRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var declaredBoundedContextIds = GetDeclaredBoundedContextIds(context.Compilation);
            var declarations = new List<ModuleDeclaration>();
            declarations.AddRange(AnalyzeScope(context, context.Compilation.Assembly, declaredBoundedContextIds));
            declarations.AddRange(AnalyzeScope(context, context.Compilation.SourceModule, declaredBoundedContextIds));
            AnalyzeNameConsistencyPerId(context, declarations);
            AnalyzeBoundedContextConsistencyPerId(context, declarations);
            AnalyzeIdConsistencyPerBoundedContextAndName(context, declarations);
        }

        private static IEnumerable<ModuleDeclaration> AnalyzeScope(
            CompilationAnalysisContext context,
            ISymbol symbol,
            ISet<string> declaredBoundedContextIds)
        {
            foreach (var attribute in symbol.GetAttributes().Where(IsModuleAttribute))
            {
                var id = attribute.GetNamedString("Id");
                if (attribute.SupportsMember("Id") &&
                    IsBlank(id))
                {
                    context.Report(attribute, symbol, ModuleShouldDefineIdRule, symbol.MetadataScopeLabel());
                }

                var supportsName = attribute.SupportsMember("Name");
                var supportsValue = attribute.SupportsMember("Value");
                var name = attribute.GetNameOrAliasValue();
                if ((supportsName || supportsValue) &&
                    IsBlank(name))
                {
                    context.Report(attribute, symbol, ModuleShouldDefineNameRule, symbol.MetadataScopeLabel());
                }

                if (attribute.SupportsMember("BoundedContextId") &&
                    IsBlank(attribute.GetNamedString("BoundedContextId")))
                {
                    context.Report(attribute, symbol, ModuleShouldDefineBoundedContextIdRule, symbol.MetadataScopeLabel());
                }

                var boundedContextId = attribute.GetNamedString("BoundedContextId");
                if (!IsBlank(boundedContextId) &&
                    declaredBoundedContextIds.Count > 0 &&
                    !declaredBoundedContextIds.Contains(boundedContextId!))
                {
                    var declared = string.Join(", ", declaredBoundedContextIds.OrderBy(it => it));
                    context.Report(
                        attribute,
                        symbol,
                        ModuleShouldReferenceDeclaredBoundedContextRule,
                        symbol.MetadataScopeLabel(),
                        boundedContextId!,
                        declared);
                }

                yield return new ModuleDeclaration(symbol, attribute, id, name, boundedContextId);
            }
        }

        private static void AnalyzeNameConsistencyPerId(
            CompilationAnalysisContext context,
            IEnumerable<ModuleDeclaration> declarations)
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
                        ModuleShouldUseSingleNamePerIdRule,
                        declaration.Symbol.MetadataScopeLabel(),
                        declaration.Name!,
                        group.Key,
                        declaredNames);
                }
            }
        }

        private static void AnalyzeBoundedContextConsistencyPerId(
            CompilationAnalysisContext context,
            IEnumerable<ModuleDeclaration> declarations)
        {
            var groups = declarations
                .Where(it => !IsBlank(it.Id))
                .Where(it => !IsBlank(it.BoundedContextId))
                .GroupBy(it => it.Id!, System.StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                var boundedContextIds = group
                    .Select(it => it.BoundedContextId!)
                    .Distinct(System.StringComparer.OrdinalIgnoreCase)
                    .OrderBy(it => it)
                    .ToArray();

                if (boundedContextIds.Length <= 1)
                {
                    continue;
                }

                var declaredBoundedContextIds = string.Join(", ", boundedContextIds);
                foreach (var declaration in group)
                {
                    context.Report(
                        declaration.Attribute,
                        declaration.Symbol,
                        ModuleShouldUseSingleBoundedContextIdPerIdRule,
                        declaration.Symbol.MetadataScopeLabel(),
                        declaration.BoundedContextId!,
                        group.Key,
                        declaredBoundedContextIds);
                }
            }
        }

        private static void AnalyzeIdConsistencyPerBoundedContextAndName(
            CompilationAnalysisContext context,
            IEnumerable<ModuleDeclaration> declarations)
        {
            var groups = declarations
                .Where(it => !IsBlank(it.Id))
                .Where(it => !IsBlank(it.Name))
                .Where(it => !IsBlank(it.BoundedContextId))
                .GroupBy(it => $"{it.BoundedContextId!.Trim().ToUpperInvariant()}::{it.Name!.Trim().ToUpperInvariant()}");

            foreach (var group in groups)
            {
                var ids = group
                    .Select(it => it.Id!)
                    .Distinct(System.StringComparer.OrdinalIgnoreCase)
                    .OrderBy(it => it)
                    .ToArray();

                if (ids.Length <= 1)
                {
                    continue;
                }

                var declaredIds = string.Join(", ", ids);
                var representative = group.First();
                foreach (var declaration in group)
                {
                    context.Report(
                        declaration.Attribute,
                        declaration.Symbol,
                        ModuleNameShouldMapToSingleIdPerBoundedContextRule,
                        declaration.Symbol.MetadataScopeLabel(),
                        declaration.Name!,
                        representative.BoundedContextId!,
                        declaration.Id!,
                        declaredIds);
                }
            }
        }

        private static HashSet<string> GetDeclaredBoundedContextIds(Compilation compilation)
        {
            var result = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            CollectBoundedContextIds(compilation.Assembly, result);
            CollectBoundedContextIds(compilation.SourceModule, result);
            return result;
        }

        private static void CollectBoundedContextIds(ISymbol symbol, ISet<string> ids)
        {
            foreach (var attribute in symbol.GetAttributes().Where(IsBoundedContextAttribute))
            {
                if (!attribute.SupportsMember("Id"))
                {
                    continue;
                }

                var id = attribute.GetNamedString("Id");
                if (!IsBlank(id))
                {
                    ids.Add(id!);
                }
            }
        }

        private static bool IsModuleAttribute(AttributeData attribute) =>
            InheritsFromAttribute(attribute.AttributeClass, "ModuleAttribute");

        private static bool IsBoundedContextAttribute(AttributeData attribute) =>
            InheritsFromAttribute(attribute.AttributeClass, "BoundedContextAttribute");

        private static bool InheritsFromAttribute(INamedTypeSymbol? attributeClass, string attributeName)
        {
            for (var current = attributeClass; current is not null; current = current.BaseType)
            {
                if (string.Equals(current.Name, attributeName, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);

        private sealed class ModuleDeclaration
        {
            public ModuleDeclaration(ISymbol symbol, AttributeData attribute, string? id, string? name, string? boundedContextId)
            {
                Symbol = symbol;
                Attribute = attribute;
                Id = id;
                Name = name;
                BoundedContextId = boundedContextId;
            }

            public ISymbol Symbol { get; }
            public AttributeData Attribute { get; }
            public string? Id { get; }
            public string? Name { get; }
            public string? BoundedContextId { get; }
        }
    }

    internal static class ModuleAnalyzerExtensions
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
