using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.BricksAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.BricksAnalyzers.BricksDependencyAnalyzer>;

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
        public async Task Analyze_WithLocalVariableDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-006"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        public void Execute()
        {
            {|#0:SqlGateway|} gateway = null;
        }
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
        public async Task Analyze_WithObjectCreationDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-007"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        public object Execute()
        {
            return new {|#0:SqlGateway|}();
        }
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
        public async Task Analyze_WithExcludedSourceName_SkipsExcludedSourceType()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-008"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]
[assembly: NMolecules.Bricks.ExcludedSourceNameContains(""BILL-ARCH-008"", ""Generated"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class GeneratedOrderPolicy
    {
        private readonly SqlGateway gateway;
    }

    [Role(""Infrastructure"")]
    public class SqlGateway
    {
    }
}
" + BricksShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithRequiredTargetName_RequiresMatchingTargetName()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-009"",
    ""Projection"",
    ""QueryModel"",
    NMolecules.Bricks.RuleMode.RequireDependency)]
[assembly: NMolecules.Bricks.RequiredTargetNameContains(""BILL-ARCH-009"", ""ReadModel"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Projection"")]
    public class {|#0:InvoiceProjection|}
    {
        private readonly InvoiceBalanceDto dto;
    }

    [Role(""QueryModel"")]
    public class InvoiceBalanceDto
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRequireDependencyRule_DoesNotEmitWhenMatchingDependencyExists()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-010"",
    ""Projection"",
    ""QueryModel"",
    NMolecules.Bricks.RuleMode.RequireDependency)]
[assembly: NMolecules.Bricks.RequiredTargetNameContains(""BILL-ARCH-010"", ""ReadModel"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Projection"")]
    public class InvoiceProjection
    {
        private readonly InvoiceBalanceReadModel model;
    }

    [Role(""QueryModel"")]
    public class InvoiceBalanceReadModel
    {
    }
}
" + BricksShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithExcludedTargetName_SkipsExcludedTargetType()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-011"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]
[assembly: NMolecules.Bricks.ExcludedTargetNameContains(""BILL-ARCH-011"", ""Facade"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        private readonly SqlFacade facade;
    }

    [Role(""Infrastructure"")]
    public class SqlFacade
    {
    }
}
" + BricksShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithGenericTypeArgumentDependency_EmitsError()
        {
            var testCode = @"using System;
using System.Collections.Generic;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-012"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        private readonly IReadOnlyList<SqlGateway> {|#0:gateways|};
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
        public async Task Analyze_WithArrayElementDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-013"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        private readonly SqlGateway[] {|#0:gateways|};
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
        public async Task Analyze_WithNestedSourceType_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-014"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]

namespace SampleData
{
    using NMolecules.Bricks;

    public class Container
    {
        [Role(""Domain"")]
        public class NestedPolicy
        {
            private readonly SqlGateway {|#0:gateway|};
        }
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
        public async Task Analyze_WithDeeplyNestedSourceType_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-029"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]

namespace SampleData
{
    using NMolecules.Bricks;

    public class Container
    {
        public class Inner
        {
            [Role(""Domain"")]
            public class NestedPolicy
            {
                private readonly SqlGateway {|#0:gateway|};
            }
        }
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
        public async Task Analyze_WithClassLevelRuleAndDefaultMode_EmitsError()
        {
            var testCode = @"using System;

namespace SampleData
{
    using NMolecules.Bricks;

    [Rule(""BILL-ARCH-015"", ""Domain"", ""Infrastructure"")]
    public class LocalPolicy
    {
    }

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

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRuleButNoTypes_DoesNotEmitIssues()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(
    ""BILL-ARCH-016"",
    ""Domain"",
    ""Infrastructure"",
    NMolecules.Bricks.RuleMode.ForbidDependency)]
" + BricksShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithEmptyCompilation_DoesNotEmitIssues()
        {
            await VerifyCS.VerifyAnalyzerAsync(string.Empty, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithThreeArgumentRuleAttribute_UsesDefaultForbidMode()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-017"", ""Domain"", ""Infrastructure"")]

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

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(string id, string sourceRole, string targetRole)
        {
        }
    }
}";

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithNamedRoleProperties_ResolvesRolesAndAliases()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-018"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [AttributeUsage(AttributeTargets.Class)]
    [RoleAlias(Role = ""Infrastructure"")]
    public class InfrastructureAliasAttribute : Attribute
    {
    }

    [Role(Name = ""Domain"")]
    public class OrderPolicy
    {
        private readonly SqlGateway {|#0:gateway|};
    }

    [InfrastructureAlias]
    public class SqlGateway
    {
    }
}

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute()
        {
        }

        public string Name { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RoleAliasAttribute : Attribute
    {
        public RoleAliasAttribute()
        {
        }

        public string Role { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(string id, string sourceRole, string targetRole)
        {
        }
    }
}";

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithBaseClassDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-019"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class {|#0:OrderPolicy|} : SqlGateway
    {
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
        public async Task Analyze_WithInterfaceDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-020"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class {|#0:OrderPolicy|} : ISqlGateway
    {
    }

    [Role(""Infrastructure"")]
    public interface ISqlGateway
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithPropertyDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-021"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        public SqlGateway {|#0:Gateway|} { get; }
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
        public async Task Analyze_WithMethodReturnDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-022"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        public SqlGateway {|#0:Build|}() => null;
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
        public async Task Analyze_WithMethodParameterDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-023"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public class OrderPolicy
    {
        public void Execute(SqlGateway {|#0:gateway|})
        {
        }
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
        public async Task Analyze_WithPointerDependency_EmitsError()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-024"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public unsafe class OrderPolicy
    {
        private SqlGateway* {|#0:gateway|};
    }

    [Role(""Infrastructure"")]
    public struct SqlGateway
    {
    }
}
" + BricksShims;

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithModuleLevelRule_EmitsError()
        {
            var testCode = @"using System;
[module: NMolecules.Bricks.Rule(""BILL-ARCH-025"", ""Domain"", ""Infrastructure"")]

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

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithInvalidRuleModeValue_UsesDefaultForbidMode()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-026"", ""Domain"", ""Infrastructure"", 999)]

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

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(string id, string sourceRole, string targetRole, int mode)
        {
        }
    }
}";

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithNonArrayFilterArgument_IgnoresMalformedFilter()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-027"", ""Domain"", ""Infrastructure"")]
[assembly: NMolecules.Bricks.ExcludedSourceNameContains(""BILL-ARCH-027"", ""Order"")]

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

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(string id, string sourceRole, string targetRole)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RuleFilterAttribute : Attribute
    {
        protected RuleFilterAttribute(string ruleId, string token)
        {
        }
    }

    public sealed class ExcludedSourceNameContainsAttribute : RuleFilterAttribute
    {
        public ExcludedSourceNameContainsAttribute(string ruleId, string token)
            : base(ruleId, token)
        {
        }
    }
}";

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithFilterWithoutTokenArgument_IgnoresMalformedFilter()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-030"", ""Domain"", ""Infrastructure"")]
[assembly: NMolecules.Bricks.ExcludedSourceNameContains(""BILL-ARCH-030"")]

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

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(string id, string sourceRole, string targetRole)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RuleFilterAttribute : Attribute
    {
        protected RuleFilterAttribute(string ruleId)
        {
        }
    }

    public sealed class ExcludedSourceNameContainsAttribute : RuleFilterAttribute
    {
        public ExcludedSourceNameContainsAttribute(string ruleId)
            : base(ruleId)
        {
        }
    }
}";

            var expected = new DiagnosticResult(Rules.BrickRuleViolationId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithRoleOnEnum_IgnoresNonTypeDeclarationSyntax()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-031"", ""Domain"", ""Infrastructure"")]

namespace SampleData
{
    using NMolecules.Bricks;

    [Role(""Domain"")]
    public enum OrderPolicy
    {
        Draft
    }

    [Role(""Infrastructure"")]
    public class SqlGateway
    {
    }
}

namespace NMolecules.Bricks
{
    using System;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string name)
        {
            Name = name ?? string.Empty;
        }

        public string Name { get; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public class RuleAttribute : Attribute
    {
        public RuleAttribute(string id, string sourceRole, string targetRole)
        {
        }
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithUnknownRuleFilter_IgnoresFilter()
        {
            var testCode = @"using System;
[assembly: NMolecules.Bricks.Rule(""BILL-ARCH-028"", ""Domain"", ""Infrastructure"")]
[assembly: SampleData.UnknownRuleFilter(""BILL-ARCH-028"", ""Order"")]

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

    public sealed class UnknownRuleFilterAttribute : RuleFilterAttribute
    {
        public UnknownRuleFilterAttribute(string ruleId, params string[] tokens)
            : base(ruleId, tokens)
        {
        }
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
    public sealed class ExcludedSourceNameContainsAttribute : RuleFilterAttribute
    {
        public ExcludedSourceNameContainsAttribute(string ruleId, params string[] tokens)
            : base(ruleId, tokens)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExcludedTargetNameContainsAttribute : RuleFilterAttribute
    {
        public ExcludedTargetNameContainsAttribute(string ruleId, params string[] tokens)
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

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequiredTargetNameContainsAttribute : RuleFilterAttribute
    {
        public RequiredTargetNameContainsAttribute(string ruleId, params string[] tokens)
            : base(ruleId, tokens)
        {
        }
    }
}";
    }
}
