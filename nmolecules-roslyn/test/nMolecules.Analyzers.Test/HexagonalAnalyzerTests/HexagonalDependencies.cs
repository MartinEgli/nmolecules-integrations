using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.HexagonalAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.HexagonalAnalyzers.HexagonalAnalyzer>;

namespace NMolecules.Analyzers.Test.HexagonalAnalyzerTests
{
    public class HexagonalDependencies
    {
        [Fact]
        public async Task Analyze_WithPrimaryPortUsingAdapter_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [PrimaryAdapter]
    public class RestApiAdapter
    {
    }

    [PrimaryPort]
    public class UseCasePort
    {
        private readonly RestApiAdapter {|#0:adapter|};
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.PrimaryPortsShouldNotDependOnAdaptersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithSecondaryPortUsingAdapter_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [SecondaryAdapter]
    public class SqlAdapter
    {
    }

    [SecondaryPort]
    public class PersistencePort
    {
        public void Save(SqlAdapter {|#0:adapter|})
        {
        }
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.SecondaryPortsShouldNotDependOnAdaptersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPrimaryPortUsingSecondaryPort_DoesNotEmitViolations()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [SecondaryPort]
    public interface LedgerPort
    {
    }

    [PrimaryPort]
    public class UseCasePort
    {
        public LedgerPort Port { get; set; }
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithSecondaryPortUsingPrimaryPort_DoesNotEmitViolations()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [PrimaryPort]
    public interface CommandPort
    {
    }

    [SecondaryPort]
    public class PersistencePort
    {
        private readonly CommandPort port;
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
