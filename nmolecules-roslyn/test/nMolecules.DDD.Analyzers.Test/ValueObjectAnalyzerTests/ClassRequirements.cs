﻿using System.Threading.Tasks;
using NMolecules.DDD.Analyzers.ValueObjectAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using VerifyCS = NMolecules.DDD.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<
    NMolecules.DDD.Analyzers.ValueObjectAnalyzers.ValueObjectAnalyzer>;

namespace NMolecules.DDD.Analyzers.Test.ValueObjectAnalyzerTests;

public class ClassRequirements
{
    [Fact]
    public async Task AnalyzeClass_WithValueObjectIsNotSealed_EmitsCompilerError()
    {
        var testCode = SampleDataLoader.LoadFromNamespaceOf<ClassRequirements>("ValueObjectNotSealed.cs");
        const int lineNumber = 7;

        var expectedCompilerError = CompilerError(Rules.ValueObjectsShouldBeSealedId)
            .WithSpan(lineNumber, 18, lineNumber, 38);
        await VerifyCS.VerifyAnalyzerAsync(testCode, expectedCompilerError);
    }

    [Fact]
    public async Task AnalyzeClass_WithValueObjectDoesNotImplementIEquatable_EmitsCompilerError()
    {
        var testCode = SampleDataLoader.LoadFromNamespaceOf<ClassRequirements>("ValueObjectWithoutIEquatable.cs");
        const int lineNumber = 7;

        var expectedCompilerError = CompilerError(Rules.ValueObjectsMustImplementIEquatableId)
            .WithSpan(lineNumber, 25, lineNumber, 45);
        await VerifyCS.VerifyAnalyzerAsync(testCode, expectedCompilerError);
    }

    [Fact]
    public async Task AnalyzeEnum_DoesNotEnforceIEquatableOrSealed()
    {
        var testCode = SampleDataLoader.LoadFromNamespaceOf<ClassRequirements>("ValidEnumAsValueObject.cs");

        await VerifyCS.VerifyAnalyzerAsync(testCode, EmptyDiagnosticResults);
    }
}