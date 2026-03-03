using System.Linq;
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
            await VerifyCS.VerifyAnalyzerAsync(testCode,
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(13, 49, 13, 59),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(15, 60, 15, 65),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(20, 39, 20, 44),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(22, 36, 22, 46),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(22, 65, 22, 70),
                CompilerError(Rules.DomainServicesShouldNotUseApplicationServicesId).WithSpan(24, 17, 24, 22));
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
