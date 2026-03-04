using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CqrsAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CqrsAnalyzers.QueryModelAnalyzer>;

namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests
{
    public class QueryModelsMustBeReadOnly
    {
        [Fact]
        public async Task Analyze_WithQueryModelHavingWritableProperty_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
        public decimal {|#0:Balance|} { get; set; }
    }
}", ElementNames.QueryModel);

            var expected = new DiagnosticResult(Rules.QueryModelsMustBeReadOnlyId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryModelHavingWritableField_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
        public decimal {|#0:balance|};
    }
}", ElementNames.QueryModel);

            var expected = new DiagnosticResult(Rules.QueryModelsMustBeReadOnlyId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryModelHavingPrivateSetter_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
        public decimal Balance { get; private set; }
    }
}", ElementNames.QueryModel);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithImmutableQueryModel_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
        private readonly decimal balance;

        public AccountBalanceReadModel(decimal balance)
        {
            this.balance = balance;
        }

        public decimal Balance => balance;
    }
}", ElementNames.QueryModel);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
