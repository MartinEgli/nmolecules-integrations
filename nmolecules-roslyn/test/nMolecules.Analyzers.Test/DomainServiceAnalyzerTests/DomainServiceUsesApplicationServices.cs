using System.Threading.Tasks;
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

            await VerifyCS.VerifyAnalyzerAsync(testCode,
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId));
        }

        [Fact]
        public async Task Analyze_WithDomainServiceUsesDomainService_DoesNotEmitCompilerError()
        {
            var testCode = GenerateClass(DomainService);
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
    }
}
