using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.BoundedContextAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.BoundedContextAnalyzers.BoundedContextAnalyzer>;

namespace NMolecules.Analyzers.Test.BoundedContextAnalyzerTests
{
    public class BoundedContextMetadata
    {
        [Fact]
        public async Task Analyze_WithMissingId_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = """", Name = ""Billing"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            var expected = new DiagnosticResult(Rules.BoundedContextShouldDefineIdId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithMissingNameAndValue_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            var expected = new DiagnosticResult(Rules.BoundedContextShouldDefineNameId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithConstructorNameAndId_DoesNotEmitViolations()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(""Billing"", Id = ""Billing"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public BoundedContextAttribute()
        {
        }

        public BoundedContextAttribute(string name)
        {
            Name = name;
            Value = name;
        }

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithConflictingAssemblyAndModuleIds_EmitsWarnings()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")|}]
[module: {|#1:DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            var assemblyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(assemblyExpected, moduleExpected));
        }

        [Fact]
        public async Task Analyze_WithConsistentAssemblyAndModuleIds_DoesNotEmitViolations()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[module: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithSameIdButDifferentNames_EmitsWarnings()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing Core"")|}]
[module: {|#1:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing Payments"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            var assemblyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleNamePerIdId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleNamePerIdId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(assemblyExpected, moduleExpected));
        }

        [Fact]
        public async Task Analyze_WithSameIdAndSameName_DoesNotEmitNameConsistencyViolations()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing Core"")]
[module: DomainModel.BoundedContext(Id = ""Billing"", Value = ""Billing Core"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithModuleOwnershipOnSameScopeUsingDifferentContextId_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[assembly: {|#0:DomainModel.Module(Id = ""Accounts"", Name = ""Accounts"", BoundedContextId = ""Sales"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class ModuleAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BoundedContextId { get; set; } = string.Empty;
    }
}";

            var expected = new DiagnosticResult(Rules.BoundedContextModuleOwnershipShouldMatchScopeIdId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithModuleOwnershipMatchingScopeContextId_DoesNotEmitOwnershipViolation()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[assembly: DomainModel.Module(Id = ""Accounts"", Name = ""Accounts"", BoundedContextId = ""Billing"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class ModuleAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BoundedContextId { get; set; } = string.Empty;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithDependencyToUndeclaredBoundedContext_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""SharedKernel"" })|}]
[module: DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var dependencyExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldReferenceDeclaredContextsId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var assemblyIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithSpan(3, 10, 3, 66);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                dependencyExpected,
                assemblyIdConsistencyExpected,
                moduleIdConsistencyExpected));
        }

        [Fact]
        public async Task Analyze_WithDependencyToDeclaredBoundedContext_DoesNotEmitDependencyViolation()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""Sales"" })]
[module: DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var assemblyIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithSpan(2, 12, 2, 113);
            var moduleIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithSpan(3, 10, 3, 66);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(assemblyIdConsistencyExpected, moduleIdConsistencyExpected));
        }

        [Fact]
        public async Task Analyze_WithDependencyTargetDifferentCasing_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""sales"" })|}]
[module: {|#1:DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var dependencyCasingExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldUseCanonicalTargetCasingId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var assemblyIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(1);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                dependencyCasingExpected,
                assemblyIdConsistencyExpected,
                moduleIdConsistencyExpected));
        }

        [Fact]
        public async Task Analyze_WithBidirectionalDependencies_EmitsWarnings()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""Sales"" })|}]
[module: {|#1:DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"", DependsOnContextIds = new[] { ""Billing"" })|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var assemblyDirectionExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldNotBeBidirectionalId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleDirectionExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldNotBeBidirectionalId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            var assemblyIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(1);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                assemblyDirectionExpected,
                moduleDirectionExpected,
                assemblyIdConsistencyExpected,
                moduleIdConsistencyExpected));
        }

        [Fact]
        public async Task Analyze_WithSelfDependency_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""Billing"" })|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var expected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldNotReferenceSelfId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDuplicateDependencyTargets_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""SharedKernel"", ""sharedkernel"" })|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var duplicateExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldNotContainDuplicateTargetsId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var targetExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldReferenceDeclaredContextsId, DiagnosticSeverity.Warning)
                .WithLocation(0);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(duplicateExpected, targetExpected));
        }

        [Fact]
        public async Task Analyze_WithUnidirectionalDependencies_DoesNotEmitDirectionWarnings()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""Sales"" })]
[module: DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"")]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var assemblyIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithSpan(2, 12, 2, 113);
            var moduleIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithSpan(3, 10, 3, 66);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(assemblyIdConsistencyExpected, moduleIdConsistencyExpected));
        }

        [Fact]
        public async Task Analyze_WithTransitiveDependencyCycle_EmitsWarnings()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""Sales"" })|}]
[assembly: {|#1:DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"", DependsOnContextIds = new[] { ""Shipping"" })|}]
[assembly: {|#2:DomainModel.BoundedContext(Id = ""Shipping"", Name = ""Shipping"", DependsOnContextIds = new[] { ""Billing"" })|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var billingIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var salesIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            var shippingIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(2);
            var billingCycleExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldBeAcyclicId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var salesCycleExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldBeAcyclicId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            var shippingCycleExpected = new DiagnosticResult(Rules.BoundedContextDependenciesShouldBeAcyclicId, DiagnosticSeverity.Warning)
                .WithLocation(2);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                billingIdConsistencyExpected,
                salesIdConsistencyExpected,
                shippingIdConsistencyExpected,
                billingCycleExpected,
                salesCycleExpected,
                shippingCycleExpected));
        }

        [Fact]
        public async Task Analyze_WithAcyclicTransitiveDependencies_DoesNotEmitCycleWarnings()
        {
            var testCode = @"using System;
[assembly: {|#0:DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"", DependsOnContextIds = new[] { ""Sales"" })|}]
[assembly: {|#1:DomainModel.BoundedContext(Id = ""Sales"", Name = ""Sales"", DependsOnContextIds = new[] { ""SharedKernel"" })|}]
[assembly: {|#2:DomainModel.BoundedContext(Id = ""SharedKernel"", Name = ""Shared Kernel"")|}]

namespace DomainModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple = true)]
    public sealed class BoundedContextAttribute : Attribute
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string[] DependsOnContextIds { get; set; } = Array.Empty<string>();
    }
}";

            var billingIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var salesIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            var sharedKernelIdConsistencyExpected = new DiagnosticResult(Rules.BoundedContextShouldUseSingleIdPerCompilationId, DiagnosticSeverity.Warning)
                .WithLocation(2);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(
                billingIdConsistencyExpected,
                salesIdConsistencyExpected,
                sharedKernelIdConsistencyExpected));
        }

        [Fact]
        public async Task Analyze_WithLegacyAttributeShape_DoesNotEmitViolations()
        {
            var testCode = @"using System;
[assembly: LegacyModel.BoundedContext]

namespace LegacyModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class BoundedContextAttribute : Attribute
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
