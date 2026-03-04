using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.RepositoryAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.RepositoryAnalyzers.RepositoryAnalyzer>;

namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests
{
    public class RepositoriesShouldNotExposeInfrastructureSignatures
    {
        [Fact]
        public async Task Analyze_WithPublicInfrastructureParameter_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlSession
    {
    }

    [Repository]
    public interface Accounts
    {
        void Store(SqlSession {|#0:session|});
    }
}", ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.RepositoriesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublicInfrastructureReturnType_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlSession
    {
    }

    [Repository]
    public interface Accounts
    {
        SqlSession {|#0:Open|}();
    }
}", ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.RepositoriesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublicInfrastructureProperty_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlSession
    {
    }

    [Repository]
    public interface Accounts
    {
        SqlSession {|#0:Session|} { get; }
    }
}", ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.RepositoriesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainTypesOnly_DoesNotEmitViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [AggregateRoot]
    public class BankAccount
    {
        [Identity]
        public string Id { get; }
    }

    [Repository]
    public interface Accounts
    {
        BankAccount Find(string id);
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
