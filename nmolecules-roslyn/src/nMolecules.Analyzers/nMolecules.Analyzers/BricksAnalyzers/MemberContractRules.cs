using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.BricksAnalyzers
{
    public static partial class Rules
    {
        public static readonly DiagnosticDescriptor BrickExactlyOneMemberContractRule = new(
            BrickExactlyOneMemberContractId,
            "Brick member contract must declare exactly one marker",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract does not declare exactly one member carrying the required marker attribute.",
                "Exactly-one marker contracts model identity-like or primary-slot invariants in a custom brick attribute ecosystem.",
                "Add one correctly marked member, or remove extra marked members until the contract resolves to exactly one match."));

        public static readonly DiagnosticDescriptor BrickRequireAllMembersContractRule = new(
            BrickRequireAllMembersContractId,
            "Brick member contract must include all required markers",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract is missing one or more required marker attributes on its members.",
                "All-of marker contracts model compound invariants where each required member role must be present.",
                "Add members for each missing marker, or adjust the contract when the type should not require the full marker set."));

        public static readonly DiagnosticDescriptor BrickMemberCountContractRule = new(
            BrickMemberCountContractId,
            "Brick member contract must use the configured marker count",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract does not declare the exact number of members required for a repeated marker.",
                "Fixed-count marker contracts model slot cardinality, such as dual approval or paired endpoints, in a custom brick attribute ecosystem.",
                "Add or remove marked members until the configured count matches the intended type design."));

        public static readonly DiagnosticDescriptor BrickExclusiveChoiceContractRule = new(
            BrickExclusiveChoiceContractId,
            "Brick member contract must satisfy an exclusive choice",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract does not satisfy the required exclusive choice between two marker attributes.",
                "Exclusive-choice marker contracts model XOR semantics where exactly one of two marker families may appear.",
                "Keep one allowed marker family and remove the other, or add exactly one marker when neither side is present."));
    }
}
