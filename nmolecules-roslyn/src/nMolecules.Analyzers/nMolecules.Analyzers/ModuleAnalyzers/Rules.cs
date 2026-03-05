using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.ModuleAnalyzers
{
    public static class Rules
    {
        public const string ModuleShouldDefineIdId = "XMoleculesModule0001";
        public const string ModuleShouldDefineNameId = "XMoleculesModule0002";
        public const string ModuleShouldDefineBoundedContextIdId = "XMoleculesModule0003";
        public const string ModuleShouldReferenceDeclaredBoundedContextId = "XMoleculesModule0004";
        public const string ModuleShouldUseSingleNamePerIdId = "XMoleculesModule0005";

        public static readonly DiagnosticDescriptor ModuleShouldDefineIdRule = new(
            ModuleShouldDefineIdId,
            "Module should define stable Id metadata",
            "Module on {0} should define a non-empty Id",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Modules should expose a stable identifier for tooling, reporting, and traceability.");

        public static readonly DiagnosticDescriptor ModuleShouldDefineNameRule = new(
            ModuleShouldDefineNameId,
            "Module should define readable Name metadata",
            "Module on {0} should define Name or Value",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Modules should expose readable naming metadata for diagnostics and documentation.");

        public static readonly DiagnosticDescriptor ModuleShouldDefineBoundedContextIdRule = new(
            ModuleShouldDefineBoundedContextIdId,
            "Module should define BoundedContextId metadata",
            "Module on {0} should define a non-empty BoundedContextId",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Modules should declare the bounded context they belong to so tooling can derive context-module mappings.");

        public static readonly DiagnosticDescriptor ModuleShouldReferenceDeclaredBoundedContextRule = new(
            ModuleShouldReferenceDeclaredBoundedContextId,
            "Module should reference a declared BoundedContext",
            "Module on {0} references unknown BoundedContextId '{1}'. Declared bounded contexts: {2}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Module BoundedContextId values should reference bounded contexts declared in the same compilation metadata.");

        public static readonly DiagnosticDescriptor ModuleShouldUseSingleNamePerIdRule = new(
            ModuleShouldUseSingleNamePerIdId,
            "Module declarations with same Id should use a single Name",
            "Module on {0} declares Name/Value '{1}' for Id '{2}', but this Id has multiple names in compilation: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Module declarations that share one Id should converge on a single Name/Value for stable diagnostics and reporting.");
    }
}
