using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.LayerAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.LayerAnalyzers.LayerAnalyzer>;

namespace NMolecules.Analyzers.Test.LayerAnalyzerTests
{
    public class LayerDependencies
    {
        [Fact]
        public async Task Analyze_WithDomainLayerUsingApplicationLayer_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [ApplicationLayer]
    public class TransferMoney
    {
    }

    [DomainLayer]
    public class BankAccount
    {
        private readonly TransferMoney {|#0:useCase|};
    }
}", ElementNames.ApplicationLayer, ElementNames.DomainLayer);

            var expected = new DiagnosticResult(Rules.DomainLayersShouldNotUseApplicationLayersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainLayerUsingUserInterfaceLayer_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [UserInterfaceLayer]
    public class AccountsController
    {
    }

    [DomainLayer]
    public class BankAccount
    {
        public AccountsController {|#0:Controller|} { get; set; }
    }
}", ElementNames.UserInterfaceLayer, ElementNames.DomainLayer);

            var expected = new DiagnosticResult(Rules.DomainLayersShouldNotUseUserInterfaceLayersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainLayerUsingInterfaceLayer_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [InterfaceLayer]
    public class AccountsEndpoint
    {
    }

    [DomainLayer]
    public class BankAccount
    {
        public AccountsEndpoint {|#0:endpoint|} { get; set; }
    }
}", ElementNames.InterfaceLayer, ElementNames.DomainLayer);

            var expected = new DiagnosticResult(Rules.DomainLayersShouldNotUseUserInterfaceLayersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDomainLayerUsingInfrastructureLayer_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [InfrastructureLayer]
    public class SqlAccounts
    {
    }

    [DomainLayer]
    public class BankAccount
    {
        public void Attach(SqlAccounts {|#0:accounts|})
        {
        }
    }
}", ElementNames.InfrastructureLayer, ElementNames.DomainLayer);

            var expected = new DiagnosticResult(Rules.DomainLayersShouldNotUseInfrastructureLayersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithInterfaceLayerUsingDomainLayer_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [DomainLayer]
    public class BankAccount
    {
    }

    [UserInterfaceLayer]
    public class AccountsController
    {
        private readonly BankAccount {|#0:account|};
    }
}", ElementNames.UserInterfaceLayer, ElementNames.DomainLayer);

            var expected = new DiagnosticResult(Rules.InterfaceLayersShouldNotUseDomainLayersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithInterfaceLayerUsingDomainLayerAlias_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [InterfaceLayer]
    public class AccountsEndpoint
    {
    }

    [DomainLayer]
    public class BankAccount
    {
        public AccountsEndpoint {|#0:Endpoint|} { get; set; }
    }
}", ElementNames.InterfaceLayer, ElementNames.DomainLayer);

            var expected = new DiagnosticResult(Rules.DomainLayersShouldNotUseUserInterfaceLayersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithApplicationLayerUsingDomainLayer_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [DomainLayer]
    public class BankAccount
    {
    }

    [ApplicationLayer]
    public class TransferMoney
    {
        private readonly BankAccount account;
    }
}", ElementNames.ApplicationLayer, ElementNames.DomainLayer);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithApplicationLayerUsingInfrastructureLayer_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [InfrastructureLayer]
    public class SqlAccounts
    {
    }

    [ApplicationLayer]
    public class TransferMoney
    {
        private readonly SqlAccounts {|#0:sqlAccounts|};
    }
}", ElementNames.ApplicationLayer, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.ApplicationLayersShouldLimitInfrastructureDependenciesId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithInfrastructureLayerUsingApplicationLayer_EmitsWarning()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.LayerAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Layered;

    [ApplicationLayer]
    public class TransferMoney
    {
    }

    [InfrastructureLayer]
    public class SqlHost
    {
        public TransferMoney {|#0:UseCase|} { get; set; }
    }
}", ElementNames.ApplicationLayer, ElementNames.InfrastructureLayer);

            var expected = new DiagnosticResult(Rules.InfrastructureLayersShouldUseApplicationLayersForWiringOnlyId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }
    }
}
