using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.EventAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.EventAnalyzers.DomainEventPublisherAnalyzer>;

namespace NMolecules.Analyzers.Test.EventAnalyzerTests
{
    public class DomainEventPublishersShouldExposeDomainEventPayloads
    {
        [Fact]
        public async Task Analyze_WithPublisherMethodWithoutDomainEventPayload_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [ApplicationService]
    public class AccountPublisher
    {
        [DomainEventPublisher]
        public void {|#0:Publish|}(string id)
        {
        }
    }
}", ElementNames.ApplicationService, ElementNames.DomainEventPublisher);

            var expected = new DiagnosticResult(Rules.DomainEventPublishersShouldExposeDomainEventPayloadsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublisherMethodUsingDomainEventParameter_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [DomainEvent]
    public class AccountImported
    {
    }

    [ApplicationService]
    public class AccountPublisher
    {
        [DomainEventPublisher]
        public void Publish(AccountImported imported)
        {
        }
    }
}", ElementNames.ApplicationService, ElementNames.DomainEventPublisher, ElementNames.DomainEvent);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithPublisherMethodReturningDomainEvent_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [DomainEvent]
    public class AccountImported
    {
    }

    [ApplicationService]
    public class AccountPublisher
    {
        [DomainEventPublisher]
        public AccountImported Publish()
        {
            return null;
        }
    }
}", ElementNames.ApplicationService, ElementNames.DomainEventPublisher, ElementNames.DomainEvent);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
