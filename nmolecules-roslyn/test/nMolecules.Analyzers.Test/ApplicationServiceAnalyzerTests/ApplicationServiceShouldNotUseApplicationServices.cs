using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.ApplicationServiceAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.ApplicationServiceAnalyzers.ApplicationServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests
{
    public class ApplicationServiceShouldNotUseApplicationServices
    {
        [Fact]
        public async Task Analyze_WithApplicationServiceUsingApplicationService_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    public class TransferMoney
    {
    }

    [ApplicationService]
    public class OpenAccount
    {
        private readonly TransferMoney {|#0:dependency|};

        public OpenAccount(TransferMoney {|#1:value|})
        {
            Value = value;
        }

        public TransferMoney {|#2:Value|} { get; set; }

        public TransferMoney {|#3:Execute|}(TransferMoney {|#4:input|})
        {
            var {|#5:local|} = new TransferMoney();
            return local;
        }
    }
}", ElementNames.ApplicationService);

            var expected = new DiagnosticResult(Rules.ApplicationServicesShouldNotUseApplicationServicesId, DiagnosticSeverity.Warning);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                expected.WithLocation(0),
                expected.WithLocation(1),
                expected.WithLocation(2),
                expected.WithLocation(3),
                expected.WithLocation(4),
                expected.WithLocation(5)));
        }

        [Fact]
        public async Task Analyze_WithApplicationServiceUsingDomainService_DoesNotEmitAnyViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ApplicationServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [DomainService]
    public class PolicyService
    {
    }

    [ApplicationService]
    public class TransferMoney
    {
        private readonly PolicyService dependency;
    }
}", ElementNames.ApplicationService, ElementNames.DomainService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
