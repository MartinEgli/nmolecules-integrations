using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.ServiceAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.ServiceAnalyzers.ServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.ServiceAnalyzerTests
{
    public class ServiceShouldUseSpecificRole
    {
        [Fact]
        public async Task Analyze_WithLegacyServiceOnly_EmitsWarning()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Service]
    public class {|#0:LegacyPolicyService|}
    {
    }
}";

            var expected = new DiagnosticResult(Rules.LegacyServicesShouldUseSpecificRoleId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithSpecificServiceRole_DoesNotEmitAnyViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Service]
    [DomainService]
    public class PaymentPolicy
    {
    }
}", ElementNames.DomainService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
