using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.CqrsAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.CqrsAnalyzers.CqrsCompletenessAnalyzer>;

namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests
{
    public class CqrsSupportRequiresQueryAndQueryHandler
    {
        [Fact]
        public async Task Analyze_WithQueryWithoutQueryHandler_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [Query]
    public class {|#0:FindAccountBalance|}
    {
    }
}", ElementNames.Query);

            var expected = new DiagnosticResult(Rules.CqrsSupportRequiresQueryAndQueryHandlerId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryHandlerWithoutQuery_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    public class Handlers
    {
        [QueryHandler]
        public void {|#0:Handle|}()
        {
        }
    }
}", ElementNames.QueryHandler);

            var expected = new DiagnosticResult(Rules.CqrsSupportRequiresQueryAndQueryHandlerId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithQueryAndQueryHandler_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.CqrsAnalyzerTests.SampleData
{
    using NMolecules.Architecture.Cqrs;

    [Query]
    public class FindAccountBalance
    {
    }

    public class Handlers
    {
        [QueryHandler]
        public void Handle(FindAccountBalance query)
        {
        }
    }
}", ElementNames.Query, ElementNames.QueryHandler);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
