using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.EventAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.EventAnalyzers.DomainEventPublisherAnalyzer>;

namespace NMolecules.Analyzers.Test.EventAnalyzerTests
{
    public class RepositoriesAndFactoriesMustNotPublishDomainEvents
    {
        [Fact]
        public async Task Analyze_WithRepositoryPublisherType_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [Repository]
    [DomainEventPublisher]
    public interface {|#0:Accounts|}
    {
    }
}", ElementNames.DomainEventPublisher);

            var expected = new DiagnosticResult(Rules.RepositoriesAndFactoriesMustNotPublishDomainEventsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithFactoryPublisherMethod_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [Factory]
    public class AccountFactory
    {
        [DomainEventPublisher]
        public void {|#0:Publish|}()
        {
        }
    }
}", ElementNames.DomainEventPublisher);

            var expected = new DiagnosticResult(Rules.RepositoriesAndFactoriesMustNotPublishDomainEventsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithApplicationServicePublisher_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [ApplicationService]
    [DomainEventPublisher]
    public class AccountPublisher
    {
    }
}", ElementNames.DomainEventPublisher, ElementNames.ApplicationService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
