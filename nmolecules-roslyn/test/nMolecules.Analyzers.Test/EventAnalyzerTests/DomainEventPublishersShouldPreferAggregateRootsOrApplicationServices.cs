using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.EventAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.EventAnalyzers.DomainEventPublisherAnalyzer>;

namespace NMolecules.Analyzers.Test.EventAnalyzerTests
{
    public class DomainEventPublishersShouldPreferAggregateRootsOrApplicationServices
    {
        [Fact]
        public async Task Analyze_WithEntityPublisher_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [Entity]
    [DomainEventPublisher]
    public class {|#0:Account|}
    {
    }
}", ElementNames.Entity, ElementNames.DomainEventPublisher);

            var expected = new DiagnosticResult(Rules.DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPlainPublisherMethod_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.Events;

    public class AccountPublisher
    {
        [DomainEventPublisher]
        public void {|#0:Publish|}()
        {
        }
    }
}", ElementNames.DomainEventPublisher);

            var expected = new DiagnosticResult(Rules.DomainEventPublishersShouldPreferAggregateRootsOrApplicationServicesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithAggregateRootPublisher_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [AggregateRoot]
    [DomainEventPublisher]
    public class AccountAggregate
    {
    }
}", ElementNames.AggregateRoot, ElementNames.DomainEventPublisher);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
