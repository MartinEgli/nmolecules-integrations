using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CrossStyleAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CrossStyleAnalyzers.CrossStyleAnalyzer>;

namespace NMolecules.Analyzers.Test.CrossStyleAnalyzerTests
{
    public class CrossStyleCompatibility
    {
        [Fact]
        public async Task Analyze_WithLayeredAndOnionPrimaryStylesInSameScope_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Layered;
    using NMolecules.Architecture.Onion.Classic;

    [DomainLayer]
    public class {|#0:LayeredDomain|}
    {
    }

    [DomainModelRing]
    public class {|#1:OnionDomain|}
    {
    }
}

namespace NMolecules.Architecture.Layered
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainLayerAttribute : Attribute { }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
}";

            var expectedLayered = new DiagnosticResult(Rules.PrimaryStylesMustFollowCompatibilityMatrixId, DiagnosticSeverity.Error).WithLocation(0);
            var expectedOnion = new DiagnosticResult(Rules.PrimaryStylesMustFollowCompatibilityMatrixId, DiagnosticSeverity.Error).WithLocation(1);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expectedLayered, expectedOnion));
        }

        [Fact]
        public async Task Analyze_WithCqrsWithoutPrimaryStyle_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Cqrs;

    [QueryModel]
    public class {|#0:AccountProjection|}
    {
    }
}

namespace NMolecules.Architecture.Cqrs
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class QueryModelAttribute : Attribute { }
}";

            var expected = new DiagnosticResult(Rules.CqrsMayOverlayPrimaryStyleId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithCqrsOverlayOnLayered_DoesNotEmitViolations()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Cqrs;
    using NMolecules.Architecture.Layered;

    [DomainLayer]
    public class Account
    {
    }

    [QueryModel]
    public class AccountProjection
    {
    }
}

namespace NMolecules.Architecture.Cqrs
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class QueryModelAttribute : Attribute { }
}

namespace NMolecules.Architecture.Layered
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainLayerAttribute : Attribute { }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithClassicAndSimplifiedOnionInSameBoundedContext_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.DDD.BoundedContext(Id = ""Sales"")]

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

namespace NMolecules.DDD
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
    }
}

namespace NMolecules.Architecture.Onion.Classic
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainModelRingAttribute : Attribute { }
}

namespace NMolecules.Architecture.Onion.Simplified
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class DomainRingAttribute : Attribute { }
}";

            var expectedClassic = new DiagnosticResult(Rules.ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextId, DiagnosticSeverity.Error).WithLocation(0);
            var expectedSimplified = new DiagnosticResult(Rules.ClassicAndSimplifiedOnionMustNotCoexistInBoundedContextId, DiagnosticSeverity.Error).WithLocation(1);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expectedClassic, expectedSimplified));
        }
    }
}
