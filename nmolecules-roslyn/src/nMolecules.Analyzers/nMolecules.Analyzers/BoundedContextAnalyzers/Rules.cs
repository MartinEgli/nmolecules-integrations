using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.BoundedContextAnalyzers
{
    public static class Rules
    {
        public const string BoundedContextShouldDefineIdId = "XMoleculesBoundedContext0001";
        public const string BoundedContextShouldDefineNameId = "XMoleculesBoundedContext0002";
        public const string BoundedContextShouldUseSingleIdPerCompilationId = "XMoleculesBoundedContext0003";
        public const string BoundedContextShouldUseSingleNamePerIdId = "XMoleculesBoundedContext0004";
        public const string BoundedContextModuleOwnershipShouldMatchScopeIdId = "XMoleculesBoundedContext0005";
        public const string BoundedContextDependenciesShouldReferenceDeclaredContextsId = "XMoleculesBoundedContext0006";
        public const string BoundedContextDependenciesShouldNotBeBidirectionalId = "XMoleculesBoundedContext0007";
        public const string BoundedContextDependenciesShouldNotReferenceSelfId = "XMoleculesBoundedContext0008";
        public const string BoundedContextDependenciesShouldNotContainDuplicateTargetsId = "XMoleculesBoundedContext0009";

        public static readonly DiagnosticDescriptor BoundedContextShouldDefineIdRule = new(
            BoundedContextShouldDefineIdId,
            "BoundedContext should define stable Id metadata",
            "BoundedContext on {0} should define a non-empty Id",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The bounded-context declaration omits a stable Id.",
                "Bounded-context metadata needs a durable identifier so tooling can correlate ownership, dependencies, and reports across scopes.",
                "Add a non-empty Id that remains stable even if the readable display name changes."));

        public static readonly DiagnosticDescriptor BoundedContextShouldDefineNameRule = new(
            BoundedContextShouldDefineNameId,
            "BoundedContext should define readable Name metadata",
            "BoundedContext on {0} should define Name or Value",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The bounded-context declaration only provides technical metadata and no readable name.",
                "Bounded-context metadata should stay understandable in diagnostics, docs, and catalogs.",
                "Set Name or Value to a readable bounded-context label that matches the model language."));

        public static readonly DiagnosticDescriptor BoundedContextShouldUseSingleIdPerCompilationRule = new(
            BoundedContextShouldUseSingleIdPerCompilationId,
            "BoundedContext declarations should use a single Id per compilation",
            "BoundedContext on {0} declares Id '{1}', but compilation contains multiple BoundedContext Ids: {2}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "Multiple bounded-context Ids are declared in the same compilation scope.",
                "A compilation that models one bounded-context scope should converge on one bounded-context identity.",
                "Align the declarations on one canonical Id or split unrelated bounded contexts into separate compilations."));

        public static readonly DiagnosticDescriptor BoundedContextShouldUseSingleNamePerIdRule = new(
            BoundedContextShouldUseSingleNamePerIdId,
            "BoundedContext declarations with same Id should use a single Name",
            "BoundedContext on {0} declares Name/Value '{1}' for Id '{2}', but this Id has multiple names in compilation: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The same bounded-context Id is associated with different names or values in one compilation.",
                "One bounded-context identity should map to one readable name so diagnostics and documentation stay stable.",
                "Choose one canonical Name or Value for the shared Id and update the remaining declarations to match it."));

        public static readonly DiagnosticDescriptor BoundedContextModuleOwnershipShouldMatchScopeIdRule = new(
            BoundedContextModuleOwnershipShouldMatchScopeIdId,
            "Module ownership should match the bounded context declared on the same metadata scope",
            "Module on {0} declares BoundedContextId '{1}', but the same scope declares BoundedContext Id '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A module declaration on the same scope points to a different bounded-context Id than the bounded-context metadata itself.",
                "Module ownership metadata on one scope must align with the bounded context declared for that same scope.",
                "Change the module's BoundedContextId or the bounded-context declaration so both describe the same ownership boundary."));

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldReferenceDeclaredContextsRule = new(
            BoundedContextDependenciesShouldReferenceDeclaredContextsId,
            "BoundedContext dependencies should reference declared contexts",
            "BoundedContext on {0} declares dependency '{1} -> {2}', but target context is not declared in compilation metadata. Declared bounded contexts: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "A bounded-context dependency points to a target Id that is not declared anywhere in the compilation metadata.",
                "Context dependency metadata must only reference known bounded contexts so the dependency graph is analyzable and trustworthy.",
                "Declare the missing target bounded context or fix the dependency Id to reference an existing context."));

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldNotBeBidirectionalRule = new(
            BoundedContextDependenciesShouldNotBeBidirectionalId,
            "BoundedContext dependency direction should be unidirectional per context pair",
            "BoundedContext on {0} declares dependency '{1} -> {2}', but reverse dependency '{2} -> {1}' is also declared",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "Both directions of the same bounded-context dependency pair are declared.",
                "Bounded-context dependency metadata should make upstream and downstream direction explicit instead of modeling a cycle as two directed edges.",
                "Keep the intended direction only, or remodel the relationship if the two contexts actually belong to one larger boundary."));

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldNotReferenceSelfRule = new(
            BoundedContextDependenciesShouldNotReferenceSelfId,
            "BoundedContext should not depend on itself",
            "BoundedContext on {0} declares self dependency '{1} -> {1}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The bounded-context declaration lists itself as a dependency target.",
                "A bounded context cannot meaningfully depend on itself; self-dependencies only add noise and usually indicate configuration mistakes.",
                "Remove the self-reference and keep DependsOnContextIds for external context dependencies only."));

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldNotContainDuplicateTargetsRule = new(
            BoundedContextDependenciesShouldNotContainDuplicateTargetsId,
            "BoundedContext dependency targets should be unique",
            "BoundedContext on {0} declares duplicate dependency target '{1}' in DependsOnContextIds",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            DiagnosticDescriptions.Create(
                "The same dependency target is repeated inside one bounded-context declaration.",
                "Each outbound bounded-context dependency should appear once so the dependency graph remains clear and deterministic.",
                "Deduplicate the target list and keep each dependent bounded-context Id only once."));
    }
}
