using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.DomainServiceAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.DomainServiceAnalyzers.DomainServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests
{
    public class DomainServicesShouldNotUseLegacyServices
    {
        [Fact]
        public async Task Analyze_WithLegacyServiceDependency_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Service]
    public class LegacyBillingService
    {
    }

    [DomainService]
    public class BillingDomainService
    {
        private readonly LegacyBillingService {|#0:legacy|};
    }
}", ElementNames.DomainService);

            var expected = new DiagnosticResult(Rules.DomainServicesShouldNotUseLegacyServicesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithoutLegacyServiceDependency_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [DomainService]
    public class BillingDomainService
    {
    }
}", ElementNames.DomainService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
