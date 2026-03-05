using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.EventStormingAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.EventStormingAnalyzers.EventStormingAnalyzer>;

namespace NMolecules.Analyzers.Test.EventStormingAnalyzerTests
{
    public class EventStormingDependencies
    {
        [Fact]
        public async Task Analyze_WithActorDependingOnAggregate_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.EventStorming;

    [Aggregate]
    public class BookingAggregate { }

    [Actor]
    public class CustomerActor
    {
        private readonly BookingAggregate {|#0:aggregate|};
    }
}
" + EventStormingShims;

            var expected = new DiagnosticResult(Rules.ActorsShouldNotDependOnAggregatesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithCommandWithoutAggregateDependency_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.EventStorming;

    [Command]
    public class {|#0:PlaceBooking|}
    {
        public string BookingId { get; set; }
    }
}
" + EventStormingShims;

            var expected = new DiagnosticResult(Rules.CommandsShouldDependOnAggregatesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPolicyWithoutDomainEventDependency_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.EventStorming;

    [Policy]
    public class {|#0:InvoicePolicy|}
    {
        public void Apply()
        {
        }
    }
}
" + EventStormingShims;

            var expected = new DiagnosticResult(Rules.PoliciesShouldDependOnDomainEventsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithReadModelDependingOnAggregate_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.EventStorming;

    [Aggregate]
    public class BookingAggregate { }

    [ReadModel]
    public class BookingReadModel
    {
        public BookingAggregate {|#0:Aggregate|} { get; set; }
    }
}
" + EventStormingShims;

            var expected = new DiagnosticResult(Rules.ReadModelsShouldNotDependOnAggregatesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithExternalSystemDependingOnAggregate_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.EventStorming;

    [Aggregate]
    public class PaymentAggregate { }

    [ExternalSystem]
    public class PaymentProvider
    {
        public void Sync(PaymentAggregate {|#0:aggregate|})
        {
        }
    }
}
" + EventStormingShims;

            var expected = new DiagnosticResult(Rules.ExternalSystemsShouldNotDependOnAggregatesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithCommandAndPolicyHavingExpectedDependencies_DoesNotEmitViolations()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.EventStorming;

    [Aggregate]
    public class BookingAggregate { }

    [DomainEvent]
    public class BookingPlaced { }

    [Command]
    public class PlaceBooking
    {
        public BookingAggregate Target { get; set; }
    }

    [Policy]
    public class BookingPolicy
    {
        public void Handle(BookingPlaced domainEvent)
        {
        }
    }
}
" + EventStormingShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        private const string EventStormingShims = @"
namespace NMolecules.Architecture.EventStorming
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ActorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class AggregateAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class CommandAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainEventAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ExternalSystemAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PolicyAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ReadModelAttribute : Attribute { }
}";
    }
}
