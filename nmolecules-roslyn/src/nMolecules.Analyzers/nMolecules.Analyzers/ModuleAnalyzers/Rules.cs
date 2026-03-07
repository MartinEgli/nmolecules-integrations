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
        public const string ModuleShouldUseSingleBoundedContextIdPerIdId = "XMoleculesModule0006";
        public const string ModuleNameShouldMapToSingleIdPerBoundedContextId = "XMoleculesModule0007";

        public static readonly DiagnosticDescriptor ModuleShouldDefineIdRule = new(
            ModuleShouldDefineIdId,
            "Module should define stable Id metadata",
            "Module on {0} should define a non-empty Id",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The module declaration omits a stable Id.",
                "Module metadata needs a durable identifier so tooling can correlate ownership, naming, and references over time.",
                "Add a non-empty module Id that remains stable even if the display name changes."));

        public static readonly DiagnosticDescriptor ModuleShouldDefineNameRule = new(
            ModuleShouldDefineNameId,
            "Module should define readable Name metadata",
            "Module on {0} should define Name or Value",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The module declaration lacks a readable Name or Value.",
                "Module metadata should remain understandable in diagnostics, catalogs, and ownership documentation.",
                "Set Name or Value to a readable module label from the domain language."));

        public static readonly DiagnosticDescriptor ModuleShouldDefineBoundedContextIdRule = new(
            ModuleShouldDefineBoundedContextIdId,
            "Module should define BoundedContextId metadata",
            "Module on {0} should define a non-empty BoundedContextId",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The module metadata does not declare which bounded context owns the module.",
                "Module ownership must be explicit so tooling can build a reliable bounded-context to module map.",
                "Provide a non-empty BoundedContextId that points to the owning bounded context."));

        public static readonly DiagnosticDescriptor ModuleShouldReferenceDeclaredBoundedContextRule = new(
            ModuleShouldReferenceDeclaredBoundedContextId,
            "Module should reference a declared BoundedContext",
            "Module on {0} references unknown BoundedContextId '{1}'. Declared bounded contexts: {2}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The module references a BoundedContextId that is unknown in the current compilation metadata.",
                "Module ownership metadata must point to declared bounded contexts so architectural ownership remains verifiable.",
                "Declare the missing bounded context or correct the module's BoundedContextId to an existing one."));

        public static readonly DiagnosticDescriptor ModuleShouldUseSingleNamePerIdRule = new(
            ModuleShouldUseSingleNamePerIdId,
            "Module declarations with same Id should use a single Name",
            "Module on {0} declares Name/Value '{1}' for Id '{2}', but this Id has multiple names in compilation: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The same module Id is associated with different names or values across declarations.",
                "One module identity should map to one readable name so catalogs and diagnostics stay stable.",
                "Choose one canonical Name or Value for the shared module Id and align the remaining declarations."));

        public static readonly DiagnosticDescriptor ModuleShouldUseSingleBoundedContextIdPerIdRule = new(
            ModuleShouldUseSingleBoundedContextIdPerIdId,
            "Module declarations with same Id should use a single BoundedContextId",
            "Module on {0} declares BoundedContextId '{1}' for Id '{2}', but this Id has multiple bounded-context assignments in compilation: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The same module Id is assigned to different bounded-context Ids across declarations.",
                "One module identity should belong to one bounded context so ownership semantics remain unambiguous.",
                "Keep one bounded-context assignment for the module Id or split the declarations into separate modules with distinct Ids."));

        public static readonly DiagnosticDescriptor ModuleNameShouldMapToSingleIdPerBoundedContextRule = new(
            ModuleNameShouldMapToSingleIdPerBoundedContextId,
            "Module names should map to a single Id inside a bounded context",
            "Module on {0} declares Name/Value '{1}' in BoundedContextId '{2}' with Id '{3}', but this name maps to multiple Ids in that context: {4}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "Inside one bounded context, the same module name maps to multiple module Ids.",
                "Within a bounded context, a readable module name should point to one stable module identity.",
                "Pick one canonical module Id for the name or rename the conflicting modules so name-to-id mapping stays unique."));
    }
}
