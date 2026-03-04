using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.DomainServiceAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.DomainServiceAnalyzers.DomainServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests
{
    public class DomainServicesShouldNotExposeInfrastructureSignatures
    {
        [Fact]
        public async Task Analyze_WithPublicInfrastructureParameter_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlClock
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        public void Calculate(SqlClock {|#0:clock|})
        {
        }
    }
}", ElementNames.DomainService, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.DomainServicesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublicInfrastructureReturnType_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlClock
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        public SqlClock {|#0:CreateClock|}()
        {
            return null;
        }
    }
}", ElementNames.DomainService, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.DomainServicesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPublicInfrastructureProperty_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlClock
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        public SqlClock {|#0:Clock|} { get; set; }
    }
}", ElementNames.DomainService, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.DomainServicesShouldNotExposeInfrastructureSignaturesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPrivateInfrastructureDependency_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.DDD;

    [InfrastructureLayer]
    public class SqlClock
    {
    }

    [DomainService]
    public class BillingPolicy
    {
        private readonly SqlClock clock;
    }
}", ElementNames.DomainService, ElementNames.InfrastructureLayer);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
