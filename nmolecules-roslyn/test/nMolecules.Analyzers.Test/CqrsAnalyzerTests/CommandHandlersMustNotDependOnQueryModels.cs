using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CqrsAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CqrsAnalyzers.CommandHandlerAnalyzer>;

namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests
{
    public class CommandHandlersMustNotDependOnQueryModels
    {
        [Fact]
        public async Task Analyze_WithQueryModelParameter_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
    }

    public class Handlers
    {
        [CommandHandler]
        public void Handle(AccountBalanceReadModel {|#0:model|})
        {
        }
    }
}", ElementNames.QueryModel, ElementNames.CommandHandler);

            var expected = new DiagnosticResult(Rules.CommandHandlersMustNotDependOnQueryModelsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryModelReturnType_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
    }

    public class Handlers
    {
        [CommandHandler]
        public AccountBalanceReadModel {|#0:Handle|}()
        {
            return new AccountBalanceReadModel();
        }
    }
}", ElementNames.QueryModel, ElementNames.CommandHandler);

            var expected = new DiagnosticResult(Rules.CommandHandlersMustNotDependOnQueryModelsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryModelLocal_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class AccountBalanceReadModel
    {
    }

    public class Handlers
    {
        [CommandHandler]
        public void Handle()
        {
            var {|#0:model|} = new AccountBalanceReadModel();
        }
    }
}", ElementNames.QueryModel, ElementNames.CommandHandler);

            var expected = new DiagnosticResult(Rules.CommandHandlersMustNotDependOnQueryModelsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithoutQueryModelDependency_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    public class TransferMoney
    {
    }

    public class Handlers
    {
        [CommandHandler]
        public void Handle(TransferMoney command)
        {
            var amount = 42;
        }
    }
}", ElementNames.CommandHandler);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
