using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.FactoryAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.FactoryAnalyzers.FactoryAnalyzer>;

namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests
{
    public class FactoryShouldNotUseFactories
    {
        [Fact]
        public async Task Analyze_WithFactoryUsingFactory_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Factory]
    public interface CustomerFactory
    {
    }

    [Factory]
    public class OrderFactory
    {
        private readonly CustomerFactory {|#0:dependency|};

        public OrderFactory(CustomerFactory {|#1:value|})
        {
            Value = value;
        }

        public CustomerFactory {|#2:Value|} { get; set; }

        public CustomerFactory {|#3:Create|}(CustomerFactory {|#4:input|})
        {
            CustomerFactory {|#5:local|} = default;
            return local;
        }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                CompilerError(Rules.FactoriesShouldNotUseFactoriesId).WithLocation(0),
                CompilerError(Rules.FactoriesShouldNotUseFactoriesId).WithLocation(1),
                CompilerError(Rules.FactoriesShouldNotUseFactoriesId).WithLocation(2),
                CompilerError(Rules.FactoriesShouldNotUseFactoriesId).WithLocation(3),
                CompilerError(Rules.FactoriesShouldNotUseFactoriesId).WithLocation(4),
                CompilerError(Rules.FactoriesShouldNotUseFactoriesId).WithLocation(5)));
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
