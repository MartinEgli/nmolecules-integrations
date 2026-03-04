using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.FactoryAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.FactoryAnalyzers.FactoryAnalyzer>;

namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests
{
    public class FactoryShouldNotUseRepositories
    {
        [Fact]
        public async Task Analyze_WithFactoryUsingRepository_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.FactoryAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [Repository]
    public interface Customers
    {
    }

    [Factory]
    public class OrderFactory
    {
        private readonly Customers {|#0:dependency|};

        public OrderFactory(Customers {|#1:value|})
        {
            Value = value;
        }

        public Customers {|#2:Value|} { get; set; }

        public Customers {|#3:Create|}(Customers {|#4:input|})
        {
            Customers {|#5:local|} = default;
            return local;
        }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                CompilerError(Rules.FactoriesShouldNotUseRepositoriesId).WithLocation(0),
                CompilerError(Rules.FactoriesShouldNotUseRepositoriesId).WithLocation(1),
                CompilerError(Rules.FactoriesShouldNotUseRepositoriesId).WithLocation(2),
                CompilerError(Rules.FactoriesShouldNotUseRepositoriesId).WithLocation(3),
                CompilerError(Rules.FactoriesShouldNotUseRepositoriesId).WithLocation(4),
                CompilerError(Rules.FactoriesShouldNotUseRepositoriesId).WithLocation(5)));
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
