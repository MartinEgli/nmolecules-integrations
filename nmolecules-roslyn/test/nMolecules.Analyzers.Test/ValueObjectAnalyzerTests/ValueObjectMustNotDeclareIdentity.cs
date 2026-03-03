using System.Threading.Tasks;
using NMolecules.Analyzers.ValueObjectAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.ValueObjectAnalyzers.ValueObjectAnalyzer>;

namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests
{
    public class ValueObjectMustNotDeclareIdentity
    {
        [Fact]
        public async Task Analyze_WithIdentityProperty_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [ValueObject]
    public sealed class Money : IEquatable<Money>
    {
        [Identity]
        public string {|#0:Currency|} { get; }

        public bool Equals(Money other) => other?.Currency == Currency;
    }
}";

            var compileError = CompilerError(Rules.ValueObjectsMustNotDeclareIdentityId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(compileError));
        }

        [Fact]
        public async Task Analyze_WithIdentityField_EmitsError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [ValueObject]
    public sealed class Currency : IEquatable<Currency>
    {
        [Identity]
        private readonly string {|#0:code|};

        public bool Equals(Currency other) => other?.code == code;
    }
}";

            var compileError = CompilerError(Rules.ValueObjectsMustNotDeclareIdentityId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(compileError));
        }

        [Fact]
        public async Task Analyze_WithoutIdentity_DoesNotEmitAnyViolations()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [ValueObject]
    public sealed class Money : IEquatable<Money>
    {
        public string Currency { get; }

        public bool Equals(Money other) => other?.Currency == Currency;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
