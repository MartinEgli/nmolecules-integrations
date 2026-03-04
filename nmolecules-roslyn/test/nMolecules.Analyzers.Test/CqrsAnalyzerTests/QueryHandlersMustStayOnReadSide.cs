using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CqrsAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CqrsAnalyzers.QueryHandlerAnalyzer>;

namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests
{
    public class QueryHandlersMustStayOnReadSide
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

    public class Handlers
    {
        [QueryHandler]
        public int Handle(BankAccount {|#0:account|})
        {
            return 1;
        }
    }
}", ElementNames.QueryHandler);

            var expected = new DiagnosticResult(Rules.QueryHandlersMustStayOnReadSideId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryReturnType_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.DDD;

    [Repository]
    public interface Accounts
    {
    }

    public class Handlers
    {
        [QueryHandler]
        public Accounts {|#0:Handle|}()
        {
            return default;
        }
    }
}", ElementNames.QueryHandler);

            var expected = new DiagnosticResult(Rules.QueryHandlersMustStayOnReadSideId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainServiceLocal_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.DDD;

    [DomainService]
    public class BalancePolicy
    {
    }

    public class Handlers
    {
        [QueryHandler]
        public int Handle()
        {
            BalancePolicy {|#0:policy|} = default;
            return 1;
        }
    }
}", ElementNames.QueryHandler, ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.QueryHandlersMustStayOnReadSideId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryModelFlow_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
        public int Balance { get; }
    }

    public class Handlers
    {
        [QueryHandler]
        public AccountBalanceReadModel Handle()
        {
            AccountBalanceReadModel model = null;
            return model;
        }
    }
}", ElementNames.QueryHandler, ElementNames.QueryModel);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
