using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.RepositoryAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.RepositoryAnalyzers.RepositoryAnalyzer>;

namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests
{
    public class RepositoriesShouldNotDependOnRepositories
    {
        [Fact]
        public async Task Analyze_WithRepositoryParameter_EmitsWarning()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public interface Accounts
    {
    }

    [Repository]
    public interface Payments
    {
        void Store(Accounts {|#0:accounts|});
    }
}";

            var expected = new DiagnosticResult(Rules.RepositoriesShouldNotDependOnRepositoriesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryReturnType_EmitsWarning()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public interface Accounts
    {
    }

    [Repository]
    public interface Payments
    {
        Accounts {|#0:LoadAccounts|}();
    }
}";

            var expected = new DiagnosticResult(Rules.RepositoriesShouldNotDependOnRepositoriesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRepositoryField_EmitsWarning()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public interface Accounts
    {
    }

    [Repository]
    public class Payments
    {
        private readonly Accounts {|#0:accounts|};
    }
}";

            var expected = new DiagnosticResult(Rules.RepositoriesShouldNotDependOnRepositoriesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithoutRepositoryDependency_DoesNotEmitViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [AggregateRoot]
    public class Payment
    {
        [Identity]
        public string Id { get; }
    }

    [Repository]
    public interface Payments
    {
        Payment Load(string id);
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
