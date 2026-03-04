using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.BoundedContextAnalyzers
{
    public static class Rules
    {
        public const string BoundedContextShouldDefineIdId = "XMoleculesBoundedContext0001";
        public const string BoundedContextShouldDefineNameId = "XMoleculesBoundedContext0002";
        public const string BoundedContextShouldUseSingleIdPerCompilationId = "XMoleculesBoundedContext0003";

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
    }
}
