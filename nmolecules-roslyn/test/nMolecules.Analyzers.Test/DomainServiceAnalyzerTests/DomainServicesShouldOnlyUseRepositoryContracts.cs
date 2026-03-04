using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.DomainServiceAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.DomainServiceAnalyzers.DomainServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests
{
    public class DomainServicesShouldOnlyUseRepositoryContracts
    {
        [Fact]
        public async Task Analyze_WithConcreteRepositoryField_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public class AccountRepository
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        private readonly AccountRepository {|#0:repository|};
    }
}", ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.DomainServicesShouldOnlyUseRepositoryContractsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithConcreteRepositoryParameter_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public class AccountRepository
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        public void Calculate(AccountRepository {|#0:repository|})
        {
        }
    }
}", ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.DomainServicesShouldOnlyUseRepositoryContractsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryInterface_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public interface IAccountRepository
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        private readonly IAccountRepository repository;
    }
}", ElementNames.DomainService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
