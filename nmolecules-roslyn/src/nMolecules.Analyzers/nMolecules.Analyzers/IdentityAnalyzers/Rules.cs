using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.IdentityAnalyzers
{
    public static class Rules
    {
        public const string IdentityMustBelongToEntityOrAggregateRootId = "XMoleculesIdentity0001";

        public static readonly DiagnosticDescriptor IdentityMustBelongToEntityOrAggregateRootRule = new(
            IdentityMustBelongToEntityOrAggregateRootId,
            "Identity members must belong to entities or aggregate roots",
            "[Identity] member '{0}' is declared in '{1}', but only [Entity] or [AggregateRoot] may own identities",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "An [Identity] member is declared on a type that is not an entity or aggregate root.",
                "Model identity expresses continuity over time and therefore belongs only to DDD elements with identity semantics.",
                "Move the [Identity] member onto an [Entity] or [AggregateRoot], or remodel the current type as a value object without identity."));
    }
}
