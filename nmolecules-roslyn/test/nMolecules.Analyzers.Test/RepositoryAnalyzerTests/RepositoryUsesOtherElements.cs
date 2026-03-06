using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.RepositoryAnalyzers;
using NMolecules.Analyzers.Test.RepositoryAnalyzerTests.SampleData;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ElementNames;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.RepositoryAnalyzers.RepositoryAnalyzer>;

namespace NMolecules.Analyzers.Test.RepositoryAnalyzerTests
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
            var serviceAsField = CompilerError(Rules.RepositoriesShouldNotUseLegacyServicesId)
                .WithSpan(FieldLineNumber, 38, FieldLineNumber, 45);
            var serviceAsParameterInCtor = CompilerError(Rules.RepositoriesShouldNotUseLegacyServicesId)
                .WithSpan(CtorLineNumber, 46, CtorLineNumber, 51);
            var serviceAsProperty = CompilerError(Rules.RepositoriesShouldNotUseLegacyServicesId)
                .WithSpan(PropertyLineNumber, 28, PropertyLineNumber, 33);
            var serviceAsReturnValue = CompilerError(Rules.RepositoriesShouldNotUseLegacyServicesId)
                .WithSpan(MethodLineNumber, 28, MethodLineNumber, 38);
            var serviceAsParameterInMethod = CompilerError(Rules.RepositoriesShouldNotUseLegacyServicesId)
                .WithSpan(MethodLineNumber, 51, MethodLineNumber, 58);
            var serviceUsedInMethodBody = CompilerError(Rules.RepositoriesShouldNotUseLegacyServicesId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 28);
            await VerifyCS.VerifyAnalyzerAsync(testCode,
                serviceAsField,
                serviceAsProperty,
                serviceAsParameterInCtor,
                serviceAsParameterInMethod,
                serviceAsReturnValue,
                serviceUsedInMethodBody);
        }

        [Fact]
        public async Task Analyze_WithRepositoryUsesDomainService_EmitsCompilerError()
        {
            var testCode = GenerateClass(DomainService);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ExpectedServiceRoleViolations(DomainService, Rules.RepositoriesShouldNotUseDomainServicesId));
        }

        [Fact]
        public async Task Analyze_WithRepositoryUsesApplicationService_EmitsCompilerError()
        {
            var testCode = GenerateClass(ApplicationService);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ExpectedServiceRoleViolations(ApplicationService, Rules.RepositoriesShouldNotUseApplicationServicesId));
        }

        [Fact]
        public async Task Analyze_WithRepositoryUsesFactory_EmitsWarning()
        {
            var testCode = GenerateClass(Factory);
            var factoryAsField = CompilerWarning(Rules.RepositoriesShouldNotDependOnFactoriesId)
                .WithSpan(FieldLineNumber, 38, FieldLineNumber, 45);
            var factoryAsParameterInCtor = CompilerWarning(Rules.RepositoriesShouldNotDependOnFactoriesId)
                .WithSpan(CtorLineNumber, 46, CtorLineNumber, 51);
            var factoryAsProperty = CompilerWarning(Rules.RepositoriesShouldNotDependOnFactoriesId)
                .WithSpan(PropertyLineNumber, 28, PropertyLineNumber, 33);
            var factoryAsReturnValue = CompilerWarning(Rules.RepositoriesShouldNotDependOnFactoriesId)
                .WithSpan(MethodLineNumber, 28, MethodLineNumber, 38);
            var factoryAsParameterInMethod = CompilerWarning(Rules.RepositoriesShouldNotDependOnFactoriesId)
                .WithSpan(MethodLineNumber, 51, MethodLineNumber, 58);
            var factoryUsedInMethodBody = CompilerWarning(Rules.RepositoriesShouldNotDependOnFactoriesId)
                .WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 28);
            await VerifyCS.VerifyAnalyzerAsync(testCode,
                factoryAsField,
                factoryAsProperty,
                factoryAsParameterInCtor,
                factoryAsParameterInMethod,
                factoryAsReturnValue,
                factoryUsedInMethodBody);
        }

        private static string GenerateClass(string type)
        {
            var invalidUsageTemplate = new InvalidUsageTemplate
            {
                Session = new Dictionary<string, object> { { "type", type }, { "name", type.ToLowerInvariant() } }
            };
            return ServiceRoleShims.AppendIfNeeded(invalidUsageTemplate.TransformText(), type);
        }

        private static DiagnosticResult[] ExpectedServiceRoleViolations(string dependencyType, string ruleId)
        {
            var roleLength = dependencyType.Length;
            var fieldStart = 31 + roleLength;
            var ctorStart = 39 + roleLength;
            var propertyStart = 21 + roleLength;
            var methodStart = 21 + roleLength;
            var methodParameterStart = 37 + 2 * roleLength;

            return new[]
            {
                CompilerError(ruleId).WithSpan(FieldLineNumber, fieldStart, FieldLineNumber, fieldStart + roleLength),
                CompilerError(ruleId).WithSpan(PropertyLineNumber, propertyStart, PropertyLineNumber, propertyStart + 5),
                CompilerError(ruleId).WithSpan(CtorLineNumber, ctorStart, CtorLineNumber, ctorStart + 5),
                CompilerError(ruleId).WithSpan(MethodLineNumber, methodParameterStart, MethodLineNumber, methodParameterStart + roleLength),
                CompilerError(ruleId).WithSpan(MethodLineNumber, methodStart, MethodLineNumber, methodStart + 10),
                CompilerError(ruleId).WithSpan(TypeViolationInMethodBodyLineNumber, 17, TypeViolationInMethodBodyLineNumber, 21 + roleLength)
            };
        }
    }
}
