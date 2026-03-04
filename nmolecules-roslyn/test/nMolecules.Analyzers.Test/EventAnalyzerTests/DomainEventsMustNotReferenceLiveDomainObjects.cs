using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.EventAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.EventAnalyzers.DomainEventAnalyzer>;

namespace NMolecules.Analyzers.Test.EventAnalyzerTests
{
    public class DomainEventsMustNotReferenceLiveDomainObjects
    {
        [Fact]
        public async Task Analyze_WithEntityProperty_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [Entity]
    public class BankAccount
    {
        [Identity]
        public string Id { get; }
    }

    [DomainEvent]
    public class AccountImported
    {
        public BankAccount {|#0:Account|} { get; }
    }
}", ElementNames.DomainEvent);

            var expected = new DiagnosticResult(Rules.DomainEventsMustNotReferenceEntitiesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithAggregateRootField_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [AggregateRoot]
    public class Payment
    {
        [Identity]
        public string Id { get; }
    }

    [DomainEvent]
    public class PaymentImported
    {
        private readonly Payment {|#0:payment|};
    }
}", ElementNames.DomainEvent);

            var expected = new DiagnosticResult(Rules.DomainEventsMustNotReferenceAggregateRootsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryProperty_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [Repository]
    public interface Accounts
    {
    }

    [DomainEvent]
    public class AccountImported
    {
        public Accounts {|#0:Accounts|} { get; }
    }
}", ElementNames.DomainEvent);

            var expected = new DiagnosticResult(Rules.DomainEventsMustNotReferenceRepositoriesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainServiceProperty_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.DDD;
    using NMolecules.Events;

    [DomainService]
    public class BalancePolicy
    {
    }

    [DomainEvent]
    public class AccountImported
    {
        public BalancePolicy {|#0:Policy|} { get; }
    }
}", ElementNames.DomainEvent, ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.DomainEventsMustNotReferenceServicesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithTransportFriendlyPayload_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using System;
    using NMolecules.Events;

    [DomainEvent]
    public class AccountImported
    {
        public Guid AccountId { get; }
        public string Name { get; }
    }
}", ElementNames.DomainEvent);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
