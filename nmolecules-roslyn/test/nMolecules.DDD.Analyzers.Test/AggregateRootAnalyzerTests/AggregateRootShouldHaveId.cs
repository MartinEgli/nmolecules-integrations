using System.Threading.Tasks;
using NMolecules.DDD.Analyzers.AggregateRootAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.DDD.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.DDD.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.DDD.Analyzers.AggregateRootAnalyzers.AggregateRootAnalyzer>;

namespace NMolecules.DDD.Analyzers.Test.AggregateRootAnalyzerTests
{
    public class AggregateRootShouldHaveId
    {
        [Fact]
        public async Task AggregateRootShouldHaveId_WithIdAsProperty_DoesNotEmitAnyViolations()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithIdAsProperty.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldNotEmitAnyIssues());
        }
        
        [Fact]
        public async Task AggregateRootShouldHaveId_WithIdAsField_DoesNotEmitAnyViolations()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithIdAsField.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldNotEmitAnyIssues());
        }
        
        [Fact]
        public async Task AggregateRootShouldHaveId_WithIdAsFieldInBaseClass_DoesNotEmitAnyViolations()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithIdAsFieldInBaseClass.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldNotEmitAnyIssues());
        }
        
        [Fact]
        public async Task AggregateRootShouldHaveId_WithIdAsFieldInPartialClass_DoesNotEmitAnyViolations()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithIdAsFieldInPartialClass.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldNotEmitAnyIssues());
        }
        
        [Fact]
        public async Task AggregateRootShouldHaveId_WithIdAsPropertyInBaseClass_DoesNotEmitAnyViolations()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithIdAsPropertyInBaseClass.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldNotEmitAnyIssues());
        }
        
        [Fact]
        public async Task AggregateRootShouldHaveId_WithIdAsPropertyInPartialClass_DoesNotEmitAnyViolations()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithIdAsPropertyInPartialClass.cs");
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldNotEmitAnyIssues());
        }
        
        [Fact]
        public async Task AggregateRootShouldHaveId_WithNoIdProperty_EmitsError()
        {
            var validAggregateRoot = SampleDataLoader.LoadFromNamespaceOf<AggregateRootUsesOtherElements>("AggregateRootWithoutId.cs");
            var compileError = CompilerError(Rules.AggregateRootsShouldHaveIdRuleId).WithSpan(6, 18, 6, 40);
            await VerifyCS.VerifyAnalyzerAsync(validAggregateRoot, ShouldEmitIssues(compileError));
        }
    }
}