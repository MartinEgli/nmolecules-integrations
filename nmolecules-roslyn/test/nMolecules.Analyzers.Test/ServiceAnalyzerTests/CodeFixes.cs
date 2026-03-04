using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.ServiceAnalyzers;
using NMolecules.Analyzers.ServiceCodeFixProvider;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpCodeFixVerifier<
    NMolecules.Analyzers.ServiceAnalyzers.ServiceAnalyzer,
    NMolecules.Analyzers.ServiceCodeFixProvider.LegacyServiceRoleCodeFixProvider>;

namespace NMolecules.Analyzers.Test.ServiceAnalyzerTests
{
    public class ServiceCodeFixTests
    {
        [Fact]
        public async Task Fix_WithLegacyService_OffersDomainServiceRole()
        {
            var source = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Service]
    public class LegacyPolicyService
    {
    }
}", ElementNames.DomainService, ElementNames.ApplicationService);

            var fixedSource = source.Replace("[Service]", "[DomainService]");
            var expected = CompilerWarning(Rules.LegacyServicesShouldUseSpecificRoleId).WithSpan(6, 18, 6, 37);

            await VerifyCS.VerifyCodeFixAsync(source, expected, fixedSource);
        }

        [Fact]
        public async Task Fix_WithLegacyService_OffersApplicationServiceRole()
        {
            var source = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.ServiceAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Service]
    public class LegacyPolicyService
    {
    }
}", ElementNames.DomainService, ElementNames.ApplicationService);

            var fixedSource = source.Replace("[Service]", "[ApplicationService]");
            var expected = CompilerWarning(Rules.LegacyServicesShouldUseSpecificRoleId).WithSpan(6, 18, 6, 37);

            var test = new VerifyCS.CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                CodeActionIndex = 1,
                TestState =
                {
                    AdditionalReferences = { "nMolecules.DDD.dll" }
                }
            };

            test.ExpectedDiagnostics.Add(expected);
            await test.RunAsync(CancellationToken.None);
        }
    }
}
