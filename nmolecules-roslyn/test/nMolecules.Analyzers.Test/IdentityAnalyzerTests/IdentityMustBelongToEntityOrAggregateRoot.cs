using System.Threading.Tasks;
using NMolecules.Analyzers.IdentityAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.IdentityAnalyzers.IdentityAnalyzer>;

namespace NMolecules.Analyzers.Test.IdentityAnalyzerTests
{
    public class IdentityMustBelongToEntityOrAggregateRoot
    {
        [Fact]
        public async Task Analyze_WithIdentityInsidePlainClass_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.IdentityAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    public class CustomerRecord
    {
        [Identity]
        public string {|#0:Id|} { get; }
    }
}";

            var compileError = CompilerError(Rules.IdentityMustBelongToEntityOrAggregateRootId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(compileError));
        }

        [Fact]
        public async Task Analyze_WithIdentityInsideValueObject_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.IdentityAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ValueObject]
    public sealed class Money
    {
        [Identity]
        public string {|#0:Currency|} { get; }
    }
}";

            var compileError = CompilerError(Rules.IdentityMustBelongToEntityOrAggregateRootId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(compileError));
        }

        [Fact]
        public async Task Analyze_WithIdentityInsideEntity_DoesNotEmitAnyViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.IdentityAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Entity]
    public class Customer
    {
        [Identity]
        public string Id { get; }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithIdentityInsideAggregateRoot_DoesNotEmitAnyViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.IdentityAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [AggregateRoot]
    public class CustomerAggregate
    {
        [Identity]
        public string Id { get; }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
