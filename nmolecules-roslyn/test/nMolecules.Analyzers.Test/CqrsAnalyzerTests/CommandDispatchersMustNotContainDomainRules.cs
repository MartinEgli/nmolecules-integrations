using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CqrsAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CqrsAnalyzers.CommandDispatcherAnalyzer>;

namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests
{
    public class CommandDispatchersMustNotContainDomainRules
    {
        [Fact]
        public async Task Analyze_WithEntityParameter_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.DDD;

    [Entity]
    public class BankAccount
    {
        [Identity]
        public string Id { get; }
    }

    public class Dispatcher
    {
        [CommandDispatcher]
        public void Dispatch(BankAccount {|#0:account|})
        {
        }
    }
}", ElementNames.CommandDispatcher);

            var expected = new DiagnosticResult(Rules.CommandDispatchersMustNotContainDomainRulesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainServiceReturnType_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.DDD;

    [DomainService]
    public class RoutingPolicy
    {
    }

    public class Dispatcher
    {
        [CommandDispatcher]
        public RoutingPolicy {|#0:Dispatch|}()
        {
            return new RoutingPolicy();
        }
    }
}", ElementNames.CommandDispatcher, ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.CommandDispatchersMustNotContainDomainRulesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryLocal_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.DDD;

    [Repository]
    public interface Accounts
    {
    }

    public class Dispatcher
    {
        [CommandDispatcher]
        public void Dispatch()
        {
            Accounts {|#0:accounts|} = default;
        }
    }
}", ElementNames.CommandDispatcher);

            var expected = new DiagnosticResult(Rules.CommandDispatchersMustNotContainDomainRulesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithoutDomainDependency_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    public class TransferMoney
    {
    }

    public class Dispatcher
    {
        [CommandDispatcher]
        public void Dispatch(TransferMoney command)
        {
            var route = ""payments"";
        }
    }
}", ElementNames.CommandDispatcher);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
