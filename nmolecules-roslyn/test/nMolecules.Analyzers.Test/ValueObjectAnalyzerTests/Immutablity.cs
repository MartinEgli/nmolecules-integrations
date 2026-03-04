using System.Threading.Tasks;
using NMolecules.Analyzers.ValueObjectAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<
    NMolecules.Analyzers.ValueObjectAnalyzers.ValueObjectAnalyzer>;

namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests
{
    public class Immutablity
    {
        [Fact]
        public async Task AnalyzeImmutability_WithValueObjectIsImmutable_IsValid()
        {
            var testCode = SampleDataLoader.LoadFromNamespaceOf<Immutablity>("ValidValueObject.cs");

            await VerifyCS.VerifyAnalyzerAsync(testCode, EmptyDiagnosticResults);
        }

        [Fact]
        public async Task AnalyzeImmutability_WithPropertyHasPublicSetter_EmitsCompilerError()
        {
            var testCode = SampleDataLoader.LoadFromNamespaceOf<Immutablity>("ValueObjectWithPublicPropertyGetter.cs");
            const int lineNumber = 14;

            var expectedCompilerError = CompilerError(Rules.ValueObjectsShouldBeImmutableId)
                .WithSpan(lineNumber, 23, lineNumber, 28);
            await VerifyCS.VerifyAnalyzerAsync(testCode, expectedCompilerError);
        }

        [Fact]
        public async Task AnalyzeImmutability_WithFieldIsNotReadonly_EmitsCompilerError()
        {
            var testCode = SampleDataLoader.LoadFromNamespaceOf<Immutablity>("ValueObjectWithFieldNotReadonly.cs");
            const int lineNumber = 9;

            var expectedCompilerError = CompilerError(Rules.ValueObjectsShouldBeImmutableId)
                .WithSpan(lineNumber, 24, lineNumber, 29);
            await VerifyCS.VerifyAnalyzerAsync(testCode, expectedCompilerError);
        }

        [Fact]
        public async Task AnalyzeImmutability_WithPrivateSetter_EmitsCompilerError()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [ValueObject]
    public sealed class Money : IEquatable<Money>
    {
        public string {|#0:Currency|} { get; private set; }

        public bool Equals(Money other) => true;
    }
}";

            var expectedCompilerError = CompilerError(Rules.ValueObjectsShouldBeImmutableId).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, expectedCompilerError);
        }

        [Fact]
        public async Task AnalyzeImmutability_WithInitOnlyProperty_IsValid()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using System;
    using NMolecules.DDD;

    [ValueObject]
    public sealed class Money : IEquatable<Money>
    {
        public string Currency { get; init; }

        public bool Equals(Money other) => true;
    }
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, EmptyDiagnosticResults);
        }

        [Fact]
        public async Task AnalyzeImmutability_WithSealedRecordValueObject_IsValid()
        {
            var testCode = @"namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests.SampleData
{
    using NMolecules.DDD;

    [ValueObject]
    public sealed record Money(string Currency);
}";

            await VerifyCS.VerifyAnalyzerAsync(testCode, EmptyDiagnosticResults);
        }
    }
}
