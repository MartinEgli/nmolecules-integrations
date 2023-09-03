﻿using System.Collections.Generic;
using System.Threading.Tasks;
using NMolecules.DDD.Analyzers.RepositoryAnalyzers;
using NMolecules.DDD.Analyzers.Test.RepositoryAnalyzerTests.SampleData;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.DDD.Analyzers.Test.ElementNames;
using VerifyCS = NMolecules.DDD.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.DDD.Analyzers.RepositoryAnalyzers.RepositoryAnalyzer>;

namespace NMolecules.DDD.Analyzers.Test.RepositoryAnalyzerTests
{
    public class RepositoryUsesOtherElements
    {
        private const int FieldLineNumber = 14;
        private const int CtorLineNumber = 15;
        private const int PropertyLineNumber = 20;
        private const int MethodLineNumber = 22;
        private const int TypeViolationInMethodBodyLineNumber = 24;

        [Fact]
        public async Task Analyze_ValidRepository_DoesNotEmitCompilerErrors()
        {
            var validRepository = SampleDataLoader.LoadFromNamespaceOf<RepositoryUsesOtherElements>("ValidMaximumRepository.cs");
            await VerifyCS.VerifyAnalyzerAsync(validRepository);
        }

        [Fact]
        public async Task Analyze_WithRepositoryUsesService_EmitsCompilerError()
        {
            var testCode = GenerateClass(Service);
            var serviceAsField = CompilerError(Rules.RepositoriesShouldNotUseServicesId)
                .WithSpan(FieldLineNumber, 38, FieldLineNumber, 45);
            var serviceAsParameterInCtor = CompilerError(Rules.RepositoriesShouldNotUseServicesId)
                .WithSpan(CtorLineNumber, 46, CtorLineNumber, 51);
            var serviceAsProperty = CompilerError(Rules.RepositoriesShouldNotUseServicesId)
                .WithSpan(PropertyLineNumber, 28, PropertyLineNumber, 33);
            var serviceAsReturnValue = CompilerError(Rules.RepositoriesShouldNotUseServicesId)
                .WithSpan(MethodLineNumber, 28, MethodLineNumber, 38);
            var serviceAsParameterInMethod = CompilerError(Rules.RepositoriesShouldNotUseServicesId)
                .WithSpan(MethodLineNumber, 51, MethodLineNumber, 58);
            var serviceUsedInMethodBody = CompilerError(Rules.RepositoriesShouldNotUseServicesId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 28);
            await VerifyCS.VerifyAnalyzerAsync(testCode,
                serviceAsField,
                serviceAsProperty,
                serviceAsParameterInCtor,
                serviceAsParameterInMethod,
                serviceAsReturnValue,
                serviceUsedInMethodBody);
        }

        private static string GenerateClass(string type)
        {
            var invalidUsageTemplate = new InvalidUsageTemplate
            {
                Session = new Dictionary<string, object> { { "type", type }, { "name", type.ToLowerInvariant() } }
            };
            return invalidUsageTemplate.TransformText();
        }
    }
}