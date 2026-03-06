using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.BricksAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.BricksAnalyzers.BrickRuleAnalyzer>;

namespace NMolecules.Analyzers.Test.BricksAnalyzerTests
{
    public class BrickRulesAreRuntimeConfigurable
    {
        [Fact]
        public async Task Analyze_WithForbidRuleAndCustomMessage_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-001"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency,
    ""Rule {rule}: {source} must not depend on {target} via {member}"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        private readonly SqlGateway {|#0:gateway|};
    }

    [Role(""Infrastructure"")]
    public class SqlGateway
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error)
                .WithLocation(0)
                .WithMessage("Rule BILL-ARCH-001: OrderPolicy must not depend on SqlGateway via gateway");

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

[Fact]
        public async Task Analyze_WithExcludedMemberName_SkipsExcludedDependency()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-002"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]
[assembly: NMolecules.Bricks.ExcludedMemberNameContains(""BILL-ARCH-002"", ""Allowed"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        private readonly SqlGateway AllowedSqlGateway;
        private readonly SqlGateway {|#0:BlockedSqlGateway|};
    }

    [Role(""Infrastructure"")]
    public class SqlGateway
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

[Fact]
        public async Task Analyze_WithRequireDependencyRule_EmitsErrorWhenMissing()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-003"",
    ""Projection"",
    ""QueryModel"",
    NMolecules.Bricks.RuleMode.RequireDependency)]
[assembly: NMolecules.Bricks.RequiredSourceNameContains(""BILL-ARCH-003"", ""Invoice"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Projection"")]
    public class {|#0:InvoiceProjection|}
    {
    }

    [Role(""QueryModel"")]
    public class InvoiceBalanceReadModel
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRoleAliases_ResolvesCustomAttributes()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-004"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    [RoleAlias(""Domain"")]
    public class DomainAliasAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    [RoleAlias(""Infrastructure"")]
    public class InfrastructureAliasAttribute : Attribute
    {
    }

    [DomainAlias]
    public class OrderPolicy
    {
        private readonly SqlGateway {|#0:gateway|};
    }

    [InfrastructureAlias]
    public class SqlGateway
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithInvalidRuleConfiguration_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:NMolecules.Bricks.Rule(""BILL-ARCH-005"", """", ""Infrastructure"")|}]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Infrastructure"")]
    public class SqlGateway
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleConfigurationId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        private const string BricksShims = @"

namespace NMolecules.Bricks
{
    using System;

    public enum RuleMode
    {
        ForbidDependency = 0,
        RequireDependency = 1
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RoleAliasAttribute : Attribute
    {
        public RoleAliasAttribute(string role)
        {
            Role = role ?? string.Empty;
        }

        public string Role { get; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(
            string id,
            string sourceRole,
            string targetRole,
            RuleMode mode = RuleMode.ForbidDependency,
            string message = """")
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RuleFilterAttribute : Attribute
    {
        protected RuleFilterAttribute(string ruleId, params string[] tokens)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExcludedMemberNameContainsAttribute : RuleFilterAttribute
    {
        public ExcludedMemberNameContainsAttribute(string ruleId, params string[] tokens)
            : base(ruleId, tokens)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiredSourceNameContainsAttribute : RuleFilterAttribute
    {
        public RequiredSourceNameContainsAttribute(string ruleId, params string[] tokens)
            : base(ruleId, tokens)
        {
        }
    }
}";
    }
}
