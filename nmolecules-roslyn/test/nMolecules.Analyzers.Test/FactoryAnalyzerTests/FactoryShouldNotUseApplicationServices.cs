using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.FactoryAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.FactoryAnalyzers.FactoryAnalyzer>;

namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests
{
    public class FactoryShouldNotUseApplicationServices
    {
        [Fact]
        public async Task Analyze_WithFactoryUsingApplicationService_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ApplicationService]
    public class TransferMoney
    {
    }

    [Factory]
    public class OrderFactory
    {
        private readonly TransferMoney {|#0:dependency|};

        public OrderFactory(TransferMoney {|#1:value|})
        {
            Value = value;
        }

        public TransferMoney {|#2:Value|} { get; set; }

        public TransferMoney {|#3:Create|}(TransferMoney {|#4:input|})
        {
            var {|#5:local|} = new TransferMoney();
            return local;
        }
    }
}", ElementNames.ApplicationService);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                CompilerError(Rules.FactoriesShouldNotUseApplicationServicesId).WithLocation(0),
                CompilerError(Rules.FactoriesShouldNotUseApplicationServicesId).WithLocation(1),
                CompilerError(Rules.FactoriesShouldNotUseApplicationServicesId).WithLocation(2),
                CompilerError(Rules.FactoriesShouldNotUseApplicationServicesId).WithLocation(3),
                CompilerError(Rules.FactoriesShouldNotUseApplicationServicesId).WithLocation(4),
                CompilerError(Rules.FactoriesShouldNotUseApplicationServicesId).WithLocation(5)));
        }

        [Fact]
        public async Task Analyze_WithFactoryUsingEntity_DoesNotEmitAnyViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Entity]
    public class Customer
    {
        [Identity]
        public string Id { get; }
    }

    [Factory]
    public class CustomerFactory
    {
        public Customer Create() => new Customer();
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
