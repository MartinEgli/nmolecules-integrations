using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.OnionAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.OnionAnalyzers.OnionAnalyzer>;

namespace NMolecules.Analyzers.Test.OnionAnalyzerTests
{
    public class OnionDependencies
    {
        [Fact]
        public async Task Analyze_WithClassicDomainModelUsingApplicationServiceRing_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Onion.Classic;

    [ApplicationServiceRing]
    public class TransferMoney
    {
    }

    [DomainModelRing]
    public class Account
    {
        private readonly TransferMoney {|#0:useCase|};
    }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.DomainModelRingMustNotDependOnOuterRingsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithClassicDomainServiceUsingApplicationServiceRing_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Onion.Classic;

    [ApplicationServiceRing]
    public class TransferMoney
    {
    }

    [DomainServiceRing]
    public class AccountingDomainService
    {
        private readonly TransferMoney {|#0:useCase|};
    }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.DomainServiceRingMustNotDependOnOuterRingsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithClassicApplicationServiceUsingInfrastructureRing_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Onion.Classic;

    [InfrastructureRing]
    public class SqlOutbox
    {
    }

    [ApplicationServiceRing]
    public class TransferMoney
    {
        private readonly SqlOutbox {|#0:outbox|};
    }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.ApplicationServiceRingMustNotDependOnInfrastructureRingId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithSimplifiedDomainUsingApplicationRing_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Onion.Simplified;

    [ApplicationRing]
    public class TransferMoney
    {
    }

    [DomainRing]
    public class Account
    {
        public TransferMoney {|#0:UseCase|} { get; set; }
    }
}

namespace NMolecules.Architecture.Onion.Simplified
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.OnionDependenciesMustPointInwardId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithClassicAndSimplifiedMarkersInSameCompilation_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Onion.Classic;
    using NMolecules.Architecture.Onion.Simplified;

    [DomainModelRing]
    public class {|#0:ClassicDomain|}
    {
    }

    [DomainRing]
    public class {|#1:SimplifiedDomain|}
    {
    }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}

namespace NMolecules.Architecture.Onion.Simplified
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}";

            var expectedClassic = new DiagnosticResult(Rules.ClassicAndSimplifiedOnionStylesShouldNotMixId, DiagnosticSeverity.Error).WithLocation(0);
            var expectedSimplified = new DiagnosticResult(Rules.ClassicAndSimplifiedOnionStylesShouldNotMixId, DiagnosticSeverity.Error).WithLocation(1);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expectedClassic, expectedSimplified));
        }

        [Fact]
        public async Task Analyze_WithValidInwardDependency_DoesNotEmitViolations()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Onion.Classic;

    [DomainModelRing]
    public class Account
    {
    }

    [InfrastructureRing]
    public class SqlAccounts
    {
        private readonly Account account;
    }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApplicationServiceRingAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class InfrastructureRingAttribute : Attribute { }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
