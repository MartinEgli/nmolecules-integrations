using System.Collections.Immutable;
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
                BoundedContextShouldDefineNameRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            AnalyzeScope(context, context.Compilation.Assembly);
            AnalyzeScope(context, context.Compilation.SourceModule);
        }

        private static void AnalyzeScope(CompilationAnalysisContext context, ISymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes().Where(IsBoundedContextAttribute))
            {
                if (attribute.SupportsMember("Id") &&
                    IsBlank(attribute.GetNamedString("Id")))
                {
                    context.Report(attribute, symbol, BoundedContextShouldDefineIdRule, symbol.MetadataScopeLabel());
                }

                var supportsName = attribute.SupportsMember("Name");
                var supportsValue = attribute.SupportsMember("Value");
                if ((supportsName || supportsValue) &&
                    IsBlank(attribute.GetNameOrAliasValue()))
                {
                    context.Report(attribute, symbol, BoundedContextShouldDefineNameRule, symbol.MetadataScopeLabel());
                }
            }
        }

        private static bool IsBoundedContextAttribute(AttributeData attribute) =>
            attribute.AttributeClass?.Name == "BoundedContextAttribute";

        private static bool IsBlank(string? value) => string.IsNullOrWhiteSpace(value);
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
