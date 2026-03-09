using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.BricksAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.BricksAnalyzers.BricksMemberContractAnalyzer>;

namespace NMolecules.Analyzers.Test.BricksAnalyzerTests
{
    public class BrickMemberContractsAreAnalyzed
    {
        [Fact]
        public async Task Analyze_WithExactlyOneMemberContract_EmitsErrorWhenMissing()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IdMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExactlyOneMember(typeof(IdMarkerAttribute))]
    public sealed class ExactlyOneIdContractAttribute : Attribute
    {
    }

    [ExactlyOneIdContract]
    public sealed class {|#0:OrderDocument|}
    {
        public string Description { get; } = ""missing id"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickExactlyOneMemberContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'ExactlyOneIdContractAttribute' requires exactly one member marked with 'IdMarkerAttribute', but 'OrderDocument' declares 0.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRequireAllMembersContract_EmitsErrorWhenMarkerIsMissing()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MarkerXAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MarkerYAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireAllMembers(typeof(MarkerXAttribute), typeof(MarkerYAttribute))]
    public sealed class RequireXAndYContractAttribute : Attribute
    {
    }

    [RequireXAndYContract]
    public sealed class {|#0:TransferRoute|}
    {
        [MarkerX]
        public string X { get; } = ""x"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickRequireAllMembersContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'RequireXAndYContractAttribute' requires members marked with all configured marker attributes, but 'TransferRoute' is missing: MarkerYAttribute.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithExactMemberCountContract_EmitsErrorWhenCountDiffers()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ApprovalMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireMemberCount(typeof(ApprovalMarkerAttribute), 2)]
    public sealed class DualApprovalContractAttribute : Attribute
    {
    }

    [DualApprovalContract]
    public sealed class {|#0:ApprovalPolicy|}
    {
        [ApprovalMarker]
        public string FirstApproval { get; } = ""first"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickMemberCountContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'DualApprovalContractAttribute' requires exactly 2 members marked with 'ApprovalMarkerAttribute', but 'ApprovalPolicy' declares 1.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithExclusiveChoiceContract_EmitsErrorWhenBothAppear()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionAAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionBAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExclusiveChoice(typeof(OptionAAttribute), typeof(OptionBAttribute))]
    public sealed class ExclusiveChoiceContractAttribute : Attribute
    {
    }

    [ExclusiveChoiceContract]
    public sealed class {|#0:DeliveryOptions|}
    {
        [OptionA]
        public string A { get; } = ""a"";

        [OptionB]
        public string B { get; } = ""b"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickExclusiveChoiceContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'ExclusiveChoiceContractAttribute' requires exactly one of 'OptionAAttribute' or 'OptionBAttribute', but 'DeliveryOptions' declares 1 and 1.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithValidContracts_EmitsNoIssues()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IdMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MarkerXAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MarkerYAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ApprovalMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionAAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionBAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExactlyOneMember(typeof(IdMarkerAttribute))]
    public sealed class ExactlyOneIdContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireAllMembers(typeof(MarkerXAttribute), typeof(MarkerYAttribute))]
    public sealed class RequireXAndYContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireMemberCount(typeof(ApprovalMarkerAttribute), 2)]
    public sealed class DualApprovalContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExclusiveChoice(typeof(OptionAAttribute), typeof(OptionBAttribute))]
    public sealed class ExclusiveChoiceContractAttribute : Attribute
    {
    }

    [ExactlyOneIdContract]
    public sealed class OrderDocument
    {
        [IdMarker]
        public string Id { get; } = ""id"";
    }

    [RequireXAndYContract]
    public sealed class TransferRoute
    {
        [MarkerX]
        public string X { get; } = ""x"";

        [MarkerY]
        public string Y { get; } = ""y"";
    }

    [DualApprovalContract]
    public sealed class ApprovalPolicy
    {
        [ApprovalMarker]
        public string FirstApproval { get; } = ""first"";

        [ApprovalMarker]
        public string SecondApproval { get; } = ""second"";
    }

    [ExclusiveChoiceContract]
    public sealed class DeliveryOptions
    {
        [OptionA]
        public string A { get; } = ""a"";
    }
}
" + ContractShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        private const string ContractShims = @"
namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireExactlyOneMemberAttribute : Attribute
    {
        public RequireExactlyOneMemberAttribute(Type memberAttributeType)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireAllMembersAttribute : Attribute
    {
        public RequireAllMembersAttribute(params Type[] memberAttributeTypes)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireMemberCountAttribute : Attribute
    {
        public RequireMemberCountAttribute(Type memberAttributeType, int count)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireExclusiveChoiceAttribute : Attribute
    {
        public RequireExclusiveChoiceAttribute(Type leftMemberAttributeType, Type rightMemberAttributeType)
        {
        }
    }
}";
    }
}
