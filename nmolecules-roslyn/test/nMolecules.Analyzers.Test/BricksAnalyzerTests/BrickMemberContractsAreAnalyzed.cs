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

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EndpointMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LegacyMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SlotMarkerAttribute : Attribute
    {
        public SlotMarkerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChannelMarkerAttribute : Attribute
    {
        public ChannelMarkerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExclusiveChoice(typeof(OptionAAttribute), typeof(OptionBAttribute))]
    public sealed class ExclusiveChoiceContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireMemberRange(typeof(EndpointMarkerAttribute), 2, 3)]
    public sealed class EndpointRangeContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.ForbidMember(typeof(LegacyMarkerAttribute))]
    public sealed class NoLegacyMemberContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireUniqueNamedMember(typeof(SlotMarkerAttribute))]
    public sealed class UniqueSlotContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireNamedMembers(typeof(ChannelMarkerAttribute), ""X"", ""Y"")]
    public sealed class RequiredChannelsContractAttribute : Attribute
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
        public async Task Analyze_WithMemberRangeContract_EmitsErrorWhenTooFewMarkersExist()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EndpointMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireMemberRange(typeof(EndpointMarkerAttribute), 2, 3)]
    public sealed class EndpointRangeContractAttribute : Attribute
    {
    }

    [EndpointRangeContract]
    public sealed class {|#0:EndpointSet|}
    {
        [EndpointMarker]
        public string Primary { get; } = ""primary"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickMemberRangeContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'EndpointRangeContractAttribute' requires between 2 and 3 members marked with 'EndpointMarkerAttribute', but 'EndpointSet' declares 1.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithForbiddenMemberContract_EmitsErrorWhenMarkerExists()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LegacyMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.ForbidMember(typeof(LegacyMarkerAttribute))]
    public sealed class NoLegacyMemberContractAttribute : Attribute
    {
    }

    [NoLegacyMemberContract]
    public sealed class {|#0:ModernRoute|}
    {
        [LegacyMarker]
        public string LegacyBridge { get; } = ""legacy"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickForbiddenMemberContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'NoLegacyMemberContractAttribute' forbids members marked with 'LegacyMarkerAttribute', but 'ModernRoute' declares 1.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithUniqueNamedMemberContract_EmitsErrorWhenNameIsDuplicated()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SlotMarkerAttribute : Attribute
    {
        public SlotMarkerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireUniqueNamedMember(typeof(SlotMarkerAttribute))]
    public sealed class UniqueSlotContractAttribute : Attribute
    {
    }

    [UniqueSlotContract]
    public sealed class {|#0:DuplicatedSlots|}
    {
        [SlotMarker(""X"")]
        public string First { get; } = ""first"";

        [SlotMarker(""X"")]
        public string Second { get; } = ""second"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickUniqueNamedMemberContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'UniqueSlotContractAttribute' requires unique 'Name' marker names for 'SlotMarkerAttribute', but 'DuplicatedSlots' duplicates: 'X'.");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRequiredNamedMembersContract_EmitsErrorWhenNameIsMissing()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChannelMarkerAttribute : Attribute
    {
        public ChannelMarkerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireNamedMembers(typeof(ChannelMarkerAttribute), ""X"", ""Y"")]
    public sealed class RequiredChannelsContractAttribute : Attribute
    {
    }

    [RequiredChannelsContract]
    public sealed class {|#0:PartialChannels|}
    {
        [ChannelMarker(""X"")]
        public string Left { get; } = ""left"";
    }
}
" + ContractShims;

            var expected = new DiagnosticResult(Rules.BrickRequiredNamedMembersContractId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Brick contract 'RequiredChannelsContractAttribute' requires marker names on 'ChannelMarkerAttribute' via 'Name', but 'PartialChannels' is missing: 'Y'.");

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

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EndpointMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LegacyMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SlotMarkerAttribute : Attribute
    {
        public SlotMarkerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChannelMarkerAttribute : Attribute
    {
        public ChannelMarkerAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
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

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireMemberRange(typeof(EndpointMarkerAttribute), 2, 3)]
    public sealed class EndpointRangeContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.ForbidMember(typeof(LegacyMarkerAttribute))]
    public sealed class NoLegacyMemberContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireUniqueNamedMember(typeof(SlotMarkerAttribute))]
    public sealed class UniqueSlotContractAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireNamedMembers(typeof(ChannelMarkerAttribute), ""X"", ""Y"")]
    public sealed class RequiredChannelsContractAttribute : Attribute
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

    [EndpointRangeContract]
    public sealed class EndpointSet
    {
        [EndpointMarker]
        public string Primary { get; } = ""primary"";

        [EndpointMarker]
        public string Secondary { get; } = ""secondary"";
    }

    [NoLegacyMemberContract]
    public sealed class ModernRoute
    {
        public string CurrentBridge { get; } = ""current"";
    }

    [UniqueSlotContract]
    public sealed class DistinctSlots
    {
        [SlotMarker(""X"")]
        public string Left { get; } = ""left"";

        [SlotMarker(""Y"")]
        public string Right { get; } = ""right"";
    }

    [RequiredChannelsContract]
    public sealed class CompleteChannels
    {
        [ChannelMarker(""X"")]
        public string Left { get; } = ""left"";

        [ChannelMarker(""Y"")]
        public string Right { get; } = ""right"";
    }
}
" + ContractShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithInheritedMemberMarker_SatisfiesContract()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdMarkerAttribute : Attribute
    {
    }

    public sealed class PublicIdMarkerAttribute : IdMarkerAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExactlyOneMember(typeof(IdMarkerAttribute))]
    public sealed class ExactlyOneIdContractAttribute : Attribute
    {
    }

    [ExactlyOneIdContract]
    public sealed class OrderDocument
    {
        [PublicIdMarker]
        public string Id { get; } = ""id"";
    }
}
" + ContractShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithInheritedBaseMember_SatisfiesContract()
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

    public abstract class DocumentBase
    {
        [IdMarker]
        public string Id { get; } = ""id"";
    }

    [ExactlyOneIdContract]
    public sealed class OrderDocument : DocumentBase
    {
    }
}
" + ContractShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithMethodAndEventMembers_SatisfiesAllMembersContract()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PublishMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Event)]
    public sealed class EventMarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireAllMembers(typeof(PublishMarkerAttribute), typeof(EventMarkerAttribute))]
    public sealed class EventContractAttribute : Attribute
    {
    }

    [EventContract]
    public sealed class DomainEvents
    {
        [EventMarker]
        public event EventHandler Published;

        [PublishMarker]
        public void Publish()
        {
        }
    }
}
" + ContractShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithMalformedContractAttributes_IgnoresInvalidContracts()
        {
            var testCode = @"using System;

namespace SampleData
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MarkerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    [NMolecules.Bricks.RequireExactlyOneMember]
    [NMolecules.Bricks.RequireAllMembers]
    [NMolecules.Bricks.RequireAllMembers(typeof(MarkerAttribute))]
    [NMolecules.Bricks.RequireMemberCount(typeof(MarkerAttribute))]
    [NMolecules.Bricks.RequireExclusiveChoice(typeof(MarkerAttribute))]
    [NMolecules.Bricks.RequireMemberRange(typeof(MarkerAttribute), 1)]
    [NMolecules.Bricks.ForbidMember]
    [NMolecules.Bricks.RequireUniqueNamedMember]
    [NMolecules.Bricks.RequireNamedMembers(typeof(MarkerAttribute))]
    public sealed class MalformedContractAttribute : Attribute
    {
    }

    [MalformedContract]
    public sealed class Document
    {
    }
}

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireExactlyOneMemberAttribute : Attribute
    {
        public RequireExactlyOneMemberAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireAllMembersAttribute : Attribute
    {
        public RequireAllMembersAttribute()
        {
        }

        public RequireAllMembersAttribute(Type memberAttributeType)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireMemberCountAttribute : Attribute
    {
        public RequireMemberCountAttribute(Type memberAttributeType)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireExclusiveChoiceAttribute : Attribute
    {
        public RequireExclusiveChoiceAttribute(Type leftMemberAttributeType)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireMemberRangeAttribute : Attribute
    {
        public RequireMemberRangeAttribute(Type memberAttributeType, int minimumCount)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ForbidMemberAttribute : Attribute
    {
        public ForbidMemberAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireUniqueNamedMemberAttribute : Attribute
    {
        public RequireUniqueNamedMemberAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireNamedMembersAttribute : Attribute
    {
        public RequireNamedMembersAttribute(Type memberAttributeType)
        {
        }
    }
}";

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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireMemberRangeAttribute : Attribute
    {
        public RequireMemberRangeAttribute(Type memberAttributeType, int minimumCount, int maximumCount)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ForbidMemberAttribute : Attribute
    {
        public ForbidMemberAttribute(Type memberAttributeType)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireUniqueNamedMemberAttribute : Attribute
    {
        public RequireUniqueNamedMemberAttribute(Type memberAttributeType)
        {
            NameArgument = ""Name"";
        }

        public RequireUniqueNamedMemberAttribute(Type memberAttributeType, string nameArgument)
        {
            NameArgument = nameArgument;
        }

        public string NameArgument { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequireNamedMembersAttribute : Attribute
    {
        public RequireNamedMembersAttribute(Type memberAttributeType, params string[] requiredNames)
        {
            NameArgument = ""Name"";
        }

        public string NameArgument { get; set; }
    }
}";
    }
}
