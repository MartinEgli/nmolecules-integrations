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
            "Bounded contexts should expose a stable identifier for tooling, reporting, and cross-repository references.");

        public static readonly DiagnosticDescriptor BoundedContextShouldDefineNameRule = new(
            BoundedContextShouldDefineNameId,
            "BoundedContext should define readable Name metadata",
            "BoundedContext on {0} should define Name or Value",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Bounded contexts should expose a readable Name (or Value alias) for diagnostics and documentation.");

        public static readonly DiagnosticDescriptor BoundedContextShouldUseSingleIdPerCompilationRule = new(
            BoundedContextShouldUseSingleIdPerCompilationId,
            "BoundedContext declarations should use a single Id per compilation",
            "BoundedContext on {0} declares Id '{1}', but compilation contains multiple BoundedContext Ids: {2}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Bounded context declarations in one compilation should converge on a single context Id for unambiguous tooling semantics.");

        public static readonly DiagnosticDescriptor BoundedContextShouldUseSingleNamePerIdRule = new(
            BoundedContextShouldUseSingleNamePerIdId,
            "BoundedContext declarations with same Id should use a single Name",
            "BoundedContext on {0} declares Name/Value '{1}' for Id '{2}', but this Id has multiple names in compilation: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Bounded context declarations that share one Id should also converge on a single Name/Value for stable diagnostics and reporting.");

        public static readonly DiagnosticDescriptor BoundedContextModuleOwnershipShouldMatchScopeIdRule = new(
            BoundedContextModuleOwnershipShouldMatchScopeIdId,
            "Module ownership should match the bounded context declared on the same metadata scope",
            "Module on {0} declares BoundedContextId '{1}', but the same scope declares BoundedContext Id '{2}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "When a metadata scope declares a bounded context, module ownership declared on that same scope should use the same context Id.");

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldReferenceDeclaredContextsRule = new(
            BoundedContextDependenciesShouldReferenceDeclaredContextsId,
            "BoundedContext dependencies should reference declared contexts",
            "BoundedContext on {0} declares dependency '{1} -> {2}', but target context is not declared in compilation metadata. Declared bounded contexts: {3}",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Declared bounded-context dependency pairs should reference context identifiers declared in the same compilation metadata.");

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldNotBeBidirectionalRule = new(
            BoundedContextDependenciesShouldNotBeBidirectionalId,
            "BoundedContext dependency direction should be unidirectional per context pair",
            "BoundedContext on {0} declares dependency '{1} -> {2}', but reverse dependency '{2} -> {1}' is also declared",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "For one bounded-context pair, dependency direction should remain unidirectional to avoid cyclic context coupling.");

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldNotReferenceSelfRule = new(
            BoundedContextDependenciesShouldNotReferenceSelfId,
            "BoundedContext should not depend on itself",
            "BoundedContext on {0} declares self dependency '{1} -> {1}'",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Bounded-context dependency metadata should not contain self-dependencies because they are semantically redundant and usually indicate misconfiguration.");

        public static readonly DiagnosticDescriptor BoundedContextDependenciesShouldNotContainDuplicateTargetsRule = new(
            BoundedContextDependenciesShouldNotContainDuplicateTargetsId,
            "BoundedContext dependency targets should be unique",
            "BoundedContext on {0} declares duplicate dependency target '{1}' in DependsOnContextIds",
            Category.DDD,
            DiagnosticSeverity.Warning,
            true,
            "Bounded-context dependency metadata should not repeat the same target context identifier (case-insensitive) inside one declaration.");
    }
}
