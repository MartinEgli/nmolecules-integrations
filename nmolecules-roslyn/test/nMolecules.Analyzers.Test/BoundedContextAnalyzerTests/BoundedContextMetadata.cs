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
