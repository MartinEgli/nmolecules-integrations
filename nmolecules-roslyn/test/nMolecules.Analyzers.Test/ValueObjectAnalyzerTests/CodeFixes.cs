using System.Threading.Tasks;
using NMolecules.Analyzers.ValueObjectAnalyzers;
using NMolecules.Analyzers.ValueObjectCodeFixProvider;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using SealedFixVerifier = NMolecules.Analyzers.Test.Verifiers.CSharpCodeFixVerifier<
    NMolecules.Analyzers.ValueObjectAnalyzers.ValueObjectAnalyzer,
    NMolecules.Analyzers.ValueObjectCodeFixProvider.ValueObjectShouldBeSealedCodeFixProvider>;
using IEquatableFixVerifier = NMolecules.Analyzers.Test.Verifiers.CSharpCodeFixVerifier<
    NMolecules.Analyzers.ValueObjectAnalyzers.ValueObjectAnalyzer,
    NMolecules.Analyzers.ValueObjectCodeFixProvider.ValueObjectImplementsIEquatableCodeFixProvider>;

namespace NMolecules.Analyzers.Test.ValueObjectAnalyzerTests
{
    public class ValueObjectCodeFixTests
    {
        [Fact]
        public async Task Fix_WithValueObjectShouldBeSealed_MakesClassSealed()
        {
            var source = SampleDataLoader.LoadFromNamespaceOf<ValueObjectCodeFixTests>("ValueObjectNotSealed.cs");
            var fixedSource = source.Replace(
                "public class ValueObjectNotSealed : IEquatable<ValueObjectNotSealed>",
                "public sealed class ValueObjectNotSealed : IEquatable<ValueObjectNotSealed>");

            var expectedCompilerError = CompilerError(Rules.ValueObjectsShouldBeSealedId)
                .WithSpan(7, 18, 7, 38);

            await SealedFixVerifier.VerifyCodeFixAsync(source, expectedCompilerError, fixedSource);
        }

        [Fact]
        public async Task Fix_WithValueObjectMustImplementIEquatable_AddsInterface()
        {
            var source = SampleDataLoader.LoadFromNamespaceOf<ValueObjectCodeFixTests>("ValueObjectWithoutIEquatable.cs");
            var fixedSource = source.Replace(
                "public sealed class ValueObjectNotSealed",
                "public sealed class ValueObjectNotSealed : IEquatable<ValueObjectNotSealed>");

            var expectedCompilerError = CompilerError(Rules.ValueObjectsMustImplementIEquatableId)
                .WithSpan(7, 25, 7, 45);

            await IEquatableFixVerifier.VerifyCodeFixAsync(source, expectedCompilerError, fixedSource);
        }
    }
}
