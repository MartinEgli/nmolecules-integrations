using System.Threading.Tasks;
using NMolecules.Analyzers.ApplicationServiceAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ExpectedResults;
using static NMolecules.Analyzers.Test.ElementNames;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.ApplicationServiceAnalyzers.ApplicationServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests
{
    public class ApplicationServiceShouldNotAlsoBeDomainBuildingBlocks
    {
        [Fact]
        public async Task Analyze_WithApplicationServiceAlsoMarkedAsEntity_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    [Entity]
    public class {|#0:InvalidApplicationService|}
    {
        [Identity]
        public string Id { get; }
    }
}", ApplicationService);

            var expected = CompilerError(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidApplicationService", Entity)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithApplicationServiceAlsoMarkedAsAggregateRoot_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    [AggregateRoot]
    public class {|#0:InvalidApplicationService|}
    {
        [Identity]
        public string Id { get; }
    }
}", ApplicationService);

            var expected = CompilerError(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidApplicationService", AggregateRoot)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithApplicationServiceAlsoMarkedAsValueObject_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    [ValueObject]
    public sealed class {|#0:InvalidApplicationService|}
    {
    }
}", ApplicationService);

            var expected = CompilerError(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidApplicationService", ValueObject)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithApplicationServiceAlsoMarkedAsDomainService_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    [DomainService]
    public class {|#0:InvalidApplicationService|}
    {
    }
}", ApplicationService, DomainService);

            var expected = CompilerError(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksId)
                .WithArguments("InvalidApplicationService", DomainService)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPlainApplicationService_DoesNotEmitAnyViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    public class TransferMoney
    {
    }
}", ApplicationService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
