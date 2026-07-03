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

        public static readonly DiagnosticDescriptor BrickMemberRangeContractRule = new(
            BrickMemberRangeContractId,
            "Brick member contract must use the configured marker range",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract declares too few or too many members carrying the configured marker attribute.",
                "Range marker contracts model flexible cardinality where the analyzer enforces a lower and upper member bound.",
                "Add or remove marked members until the configured minimum and maximum count are satisfied."));

        public static readonly DiagnosticDescriptor BrickForbiddenMemberContractRule = new(
            BrickForbiddenMemberContractId,
            "Brick member contract must not declare forbidden markers",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract declares a member carrying a marker attribute that the contract forbids.",
                "Forbidden marker contracts model negative capability or role-exclusion semantics in custom brick ecosystems.",
                "Remove the forbidden marker, move the member to a compatible type, or adjust the contract when the marker should be allowed."));

        public static readonly DiagnosticDescriptor BrickUniqueNamedMemberContractRule = new(
            BrickUniqueNamedMemberContractId,
            "Brick member contract must use unique marker names",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract declares duplicate marker names for members carrying the configured marker attribute.",
                "Unique named marker contracts model named slots where each configured name may appear at most once; unnamed markers also form one slot.",
                "Give duplicate markers distinct names, remove duplicates, or configure the contract to read the intended marker name argument."));

        public static readonly DiagnosticDescriptor BrickRequiredNamedMembersContractRule = new(
            BrickRequiredNamedMembersContractId,
            "Brick member contract must include required marker names",
            "{0}",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A type marked with a brick contract is missing one or more required marker names on members carrying the configured marker attribute.",
                "Required named marker contracts model explicit named slots such as X/Y channels, primary/secondary mappings, or input/output pairs.",
                "Add members with the missing marker names, or configure the contract to read the intended marker name argument."));
    }
}
