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
                ModuleShouldUseSingleNamePerIdRule);

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

                yield return new ModuleDeclaration(symbol, attribute, id, name);
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
            attribute.AttributeClass?.Name == "ModuleAttribute";

        private static bool IsBoundedContextAttribute(AttributeData attribute) =>
            attribute.AttributeClass?.Name == "BoundedContextAttribute";

        private static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);

        private sealed class ModuleDeclaration
        {
            public ModuleDeclaration(ISymbol symbol, AttributeData attribute, string? id, string? name)
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
