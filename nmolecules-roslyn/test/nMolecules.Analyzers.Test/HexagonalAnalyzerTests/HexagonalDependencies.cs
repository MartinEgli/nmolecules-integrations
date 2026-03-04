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
        public async Task Analyze_WithApplicationDependingOnPrimaryPort_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [PrimaryPort]
    public interface TransferPort
    {
    }

    [Application]
    public class TransferDomain
    {
        private readonly TransferPort {|#0:port|};
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.ApplicationCoreShouldNotDependOnPortsOrAdaptersId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPrimaryPortUsingAdapter_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [PrimaryPort]
    public interface InboundPort
    {
    }

    [PrimaryAdapter]
    public class RestApiAdapter
    {
        private readonly InboundPort port;
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
    public class ApplicationAttribute : Attribute { }
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

    [SecondaryPort]
    public interface PersistenceContract
    {
    }

    [SecondaryAdapter]
    public class SqlAdapter
    {
        private readonly PersistenceContract contract;
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
    public class ApplicationAttribute : Attribute { }
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
    public class ApplicationAttribute : Attribute { }
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
    public class ApplicationAttribute : Attribute { }
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
        public async Task Analyze_WithPrimaryAdapterWithoutPrimaryPortDependency_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [PrimaryAdapter]
    public class {|#0:HttpInboundAdapter|}
    {
        public string Endpoint { get; set; }
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.PrimaryAdaptersShouldDependOnPrimaryPortsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithSecondaryAdapterWithoutSecondaryPortDependency_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [SecondaryAdapter]
    public class {|#0:SqlOutboundAdapter|}
    {
        public string ConnectionString { get; set; }
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryPortAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class PrimaryAdapterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SecondaryAdapterAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.SecondaryAdaptersShouldDependOnSecondaryPortsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPrimaryAdapterDependingOnPrimaryPort_DoesNotEmitWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Hexagonal;

    [PrimaryPort]
    public interface InboundPort
    {
    }

    [PrimaryAdapter]
    public class HttpInboundAdapter
    {
        private readonly InboundPort port;
    }
}

namespace NMolecules.Architecture.Hexagonal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationAttribute : Attribute { }
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
