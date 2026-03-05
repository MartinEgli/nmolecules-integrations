using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.ModuleAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.ModuleAnalyzers.ModuleAnalyzer>;

namespace NMolecules.Analyzers.Test.ModuleAnalyzerTests
{
    public class ModuleMetadata
    {
        [Fact]
        public async Task Analyze_WithMissingId_EmitsWarning()
        {
            var testCode = @"using System;
[module: {|#0:DomainModel.Module(Id = """", Name = ""Accounts"", BoundedContextId = ""Billing"")|}]

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

            var expected = new DiagnosticResult(Rules.ModuleShouldDefineIdId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithMissingNameAndValue_EmitsWarning()
        {
            var testCode = @"using System;
[module: {|#0:DomainModel.Module(Id = ""Accounts"", BoundedContextId = ""Billing"")|}]

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

            var expected = new DiagnosticResult(Rules.ModuleShouldDefineNameId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithMissingBoundedContextId_EmitsWarning()
        {
            var testCode = @"using System;
[module: {|#0:DomainModel.Module(Id = ""Accounts"", Name = ""Accounts"")|}]

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

            var expected = new DiagnosticResult(Rules.ModuleShouldDefineBoundedContextIdId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithConstructorNameAndMetadata_DoesNotEmitViolations()
        {
            var testCode = @"using System;
[module: DomainModel.Module(""Accounts"", Id = ""Accounts"", BoundedContextId = ""Billing"")]
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]

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
        public ModuleAttribute()
        {
        }

        public ModuleAttribute(string name)
        {
            Name = name;
            Value = name;
        }

        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string BoundedContextId { get; set; } = string.Empty;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithLegacyAttributeShape_DoesNotEmitViolations()
        {
            var testCode = @"using System;
[module: LegacyModel.Module]

namespace LegacyModel
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
    public sealed class ModuleAttribute : Attribute
    {
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithUnknownBoundedContextReference_EmitsWarning()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[module: {|#0:DomainModel.Module(Id = ""Accounts"", Name = ""Accounts"", BoundedContextId = ""Sales"")|}]

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

            var expected = new DiagnosticResult(Rules.ModuleShouldReferenceDeclaredBoundedContextId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithKnownBoundedContextReference_DoesNotEmitViolations()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[module: DomainModel.Module(Id = ""Accounts"", Name = ""Accounts"", BoundedContextId = ""Billing"")]

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
        public async Task Analyze_WithSameModuleIdButDifferentNames_EmitsWarnings()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[assembly: {|#0:DomainModel.Module(Id = ""Accounts"", Name = ""Accounts Core"", BoundedContextId = ""Billing"")|}]
[module: {|#1:DomainModel.Module(Id = ""Accounts"", Name = ""Accounts Api"", BoundedContextId = ""Billing"")|}]

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

            var assemblyExpected = new DiagnosticResult(Rules.ModuleShouldUseSingleNamePerIdId, DiagnosticSeverity.Warning)
                .WithLocation(0);
            var moduleExpected = new DiagnosticResult(Rules.ModuleShouldUseSingleNamePerIdId, DiagnosticSeverity.Warning)
                .WithLocation(1);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(assemblyExpected, moduleExpected));
        }

        [Fact]
        public async Task Analyze_WithSameModuleIdAndSameName_DoesNotEmitNameConsistencyViolations()
        {
            var testCode = @"using System;
[assembly: DomainModel.BoundedContext(Id = ""Billing"", Name = ""Billing"")]
[assembly: DomainModel.Module(Id = ""Accounts"", Name = ""Accounts Core"", BoundedContextId = ""Billing"")]
[module: DomainModel.Module(Id = ""Accounts"", Value = ""Accounts Core"", BoundedContextId = ""Billing"")]

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
    }
}
