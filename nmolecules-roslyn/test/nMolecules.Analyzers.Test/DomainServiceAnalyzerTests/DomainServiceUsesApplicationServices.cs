using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.DomainServiceAnalyzers;
using Xunit;
using static Microsoft.CodeAnalysis.Testing.DiagnosticResult;
using static NMolecules.Analyzers.Test.ElementNames;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.DomainServiceAnalyzers.DomainServiceAnalyzer>;

namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests
{
    public class DomainServiceUsesApplicationServices
    {
        [Fact]
        public async Task Analyze_WithDomainServiceUsesApplicationService_EmitsCompilerError()
        {
            var testCode = GenerateClass(ApplicationService);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ExpectedViolations(ApplicationService));
        }

        [Fact]
        public async Task Analyze_WithDomainServiceUsesDomainService_DoesNotEmitCompilerError()
        {
            var testCode = GenerateClass(DomainService);
            await VerifyCS.VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task Analyze_WithApplicationServiceUsesApplicationService_DoesNotEmitCompilerError()
        {
            var testCode = GenerateIgnoredHostClass(ApplicationService, ApplicationService);
            await VerifyCS.VerifyAnalyzerAsync(testCode);
        }

        [Fact]
        public async Task Analyze_WithDomainServiceUsesApplicationServiceInMultiVariableDeclaration_DoesNotEmitCompilerError()
        {
            var testCode = GenerateMultiVariableLocalDeclarationClass(ApplicationService);
            await VerifyCS.VerifyAnalyzerAsync(testCode);
        }

        private static string GenerateClass(string dependencyType)
        {
            var code = $@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{{
    using NMolecules.DDD;

    [{dependencyType}]
    public class Some{dependencyType}
    {{
    }}

    [DomainService]
    public sealed class InvalidDomainService
    {{
        private readonly Some{dependencyType} dependency;

        public InvalidDomainService(Some{dependencyType} value)
        {{
            Value = value;
        }}

        public Some{dependencyType} Value {{ get; set; }}

        public Some{dependencyType} SomeMethod(Some{dependencyType} input)
        {{
            var local = new Some{dependencyType}();
            return local;
        }}
    }}
}}";

            return ServiceRoleShims.AppendIfNeeded(code, DomainService, dependencyType);
        }

        private static string GenerateIgnoredHostClass(string hostType, string dependencyType)
        {
            var code = $@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{{
    using NMolecules.DDD;

    [{dependencyType}]
    public class Some{dependencyType}
    {{
    }}

    [{hostType}]
    public sealed class ValidHost
    {{
        private readonly Some{dependencyType} dependency;

        public ValidHost(Some{dependencyType} value)
        {{
            Value = value;
        }}

        public Some{dependencyType} Value {{ get; set; }}

        public Some{dependencyType} SomeMethod(Some{dependencyType} input)
        {{
            var local = new Some{dependencyType}();
            return local;
        }}
    }}
}}";

            return ServiceRoleShims.AppendIfNeeded(code, hostType, dependencyType);
        }

        private static string GenerateMultiVariableLocalDeclarationClass(string dependencyType)
        {
            var code = $@"namespace NMolecules.Analyzers.Test.DomainServiceAnalyzerTests.SampleData
{{
    using NMolecules.DDD;

    [{dependencyType}]
    public class Some{dependencyType}
    {{
    }}

    [DomainService]
    public sealed class ValidDomainService
    {{
        public void SomeMethod()
        {{
            Some{dependencyType} first = null, second = null;
        }}
    }}
}}";

            return ServiceRoleShims.AppendIfNeeded(code, DomainService, dependencyType);
        }

        private static DiagnosticResult[] ExpectedViolations(string dependencyType)
        {
            var roleLength = dependencyType.Length;
            var fieldStart = 31 + roleLength;
            var ctorStart = 42 + roleLength;
            var propertyStart = 21 + roleLength;
            var methodStart = 21 + roleLength;
            var methodParameterStart = 37 + 2 * roleLength;

            return new[]
            {
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(13, fieldStart, 13, fieldStart + 10),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(15, ctorStart, 15, ctorStart + 5),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(20, propertyStart, 20, propertyStart + 5),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(22, methodStart, 22, methodStart + 10),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(22, methodParameterStart, 22, methodParameterStart + 5),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(24, 17, 24, 22)
            };
        }

    }
}
