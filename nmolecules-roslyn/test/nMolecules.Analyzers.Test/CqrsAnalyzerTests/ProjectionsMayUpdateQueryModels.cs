using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CqrsAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CqrsAnalyzers.ProjectionAnalyzer>;

namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests
{
    public class ProjectionsMayUpdateQueryModels
    {
        [Fact]
        public async Task Analyze_WithQueryModelParameter_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
    }

    [Projection]
    public class AccountBalanceProjection
    {
        public void Apply(AccountBalanceReadModel model)
        {
        }
    }
}", ElementNames.QueryModel, ElementNames.Projection);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

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

    [Projection]
    public class AccountBalanceProjection
    {
        public void Apply(BankAccount {|#0:account|})
        {
        }
    }
}", ElementNames.Projection);

            var expected = new DiagnosticResult(Rules.ProjectionsMustNotDependOnWriteSideRolesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryField_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.DDD;

    [Repository]
    public interface Accounts
    {
    }

    [Projection]
    public class AccountBalanceProjection
    {
        private readonly Accounts {|#0:accounts|};
    }
}", ElementNames.Projection);

            var expected = new DiagnosticResult(Rules.ProjectionsMustNotDependOnWriteSideRolesId, DiagnosticSeverity.Error).WithLocation(0);
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

    [Projection]
    public class AccountBalanceProjection
    {
        public void Apply()
        {
            BalancePolicy {|#0:policy|} = default;
        }
    }
}", ElementNames.Projection, ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.ProjectionsMustNotDependOnWriteSideRolesId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }
    }
}
