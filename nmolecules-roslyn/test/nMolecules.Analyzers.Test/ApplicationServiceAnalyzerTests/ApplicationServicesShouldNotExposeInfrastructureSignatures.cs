using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.ApplicationServiceAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.ApplicationServiceAnalyzers.ApplicationServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests
{
    public class ApplicationServicesShouldNotExposeInfrastructureSignatures
    {
        [Fact]
        public async Task Analyze_WithPublicInfrastructureParameter_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlGateway
    {
    }

    [ApplicationService]
    public class BillingApplicationService
    {
        public void Run(SqlGateway {|#0:gateway|})
        {
        }
    }
}", ElementNames.ApplicationService, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.ApplicationServicesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublicInfrastructureReturnType_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlGateway
    {
    }

    [ApplicationService]
    public class BillingApplicationService
    {
        public SqlGateway {|#0:CreateGateway|}()
        {
            return null;
        }
    }
}", ElementNames.ApplicationService, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.ApplicationServicesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublicInfrastructureProperty_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlGateway
    {
    }

    [ApplicationService]
    public class BillingApplicationService
    {
        public SqlGateway {|#0:Gateway|} { get; set; }
    }
}", ElementNames.ApplicationService, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.ApplicationServicesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPrivateInfrastructureDependency_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlGateway
    {
    }

    [ApplicationService]
    public class BillingApplicationService
    {
        private readonly SqlGateway gateway;
    }
}", ElementNames.ApplicationService, ElementNames.InfrastructureLayer);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
