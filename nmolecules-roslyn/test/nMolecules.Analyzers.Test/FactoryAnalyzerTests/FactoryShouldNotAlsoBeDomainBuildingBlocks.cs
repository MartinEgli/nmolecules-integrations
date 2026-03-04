using System.Threading.Tasks;
using NMolecules.Analyzers.FactoryAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ElementNames;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.FactoryAnalyzers.FactoryAnalyzer>;

namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests
{
    public class FactoryShouldNotAlsoBeDomainBuildingBlocks
    {
        [Fact]
        public async Task Analyze_WithFactoryAlsoMarkedAsEntity_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    [Entity]
    public class {|#0:InvalidFactory|}
    {
        [Identity]
        public string Id { get; }
    }
}";

            var expected = CompilerError(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidFactory", Entity)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithFactoryAlsoMarkedAsAggregateRoot_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    [AggregateRoot]
    public class {|#0:InvalidFactory|}
    {
        [Identity]
        public string Id { get; }
    }
}";

            var expected = CompilerError(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidFactory", AggregateRoot)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithFactoryAlsoMarkedAsValueObject_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    [ValueObject]
    public sealed class {|#0:InvalidFactory|}
    {
    }
}";

            var expected = CompilerError(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidFactory", ValueObject)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithFactoryAlsoMarkedAsRepository_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    [Repository]
    public interface {|#0:InvalidFactory|}
    {
    }
}";

            var expected = CompilerError(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidFactory", Repository)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithFactoryAlsoMarkedAsDomainService_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    [DomainService]
    public class {|#0:InvalidFactory|}
    {
    }
}", DomainService);

            var expected = CompilerError(Rules.FactoriesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidFactory", DomainService)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPlainFactory_DoesNotEmitAnyViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    public class CustomerFactory
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
