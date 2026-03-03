using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.IdentityAnalyzers
{
    public static class Rules
    {
        public const string IdentityMustBelongToEntityOrAggregateRootId = "XMoleculesIdentity0001";

        public static readonly DiagnosticDescriptor IdentityMustBelongToEntityOrAggregateRootRule = new(
            IdentityMustBelongToEntityOrAggregateRootId,
            "Identity members must belong to entities or aggregate roots",
            "[Identity] may only be declared inside an [Entity] or [AggregateRoot]",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "DDD identities belong to entities and aggregate roots, not arbitrary model elements.");
    }
}
