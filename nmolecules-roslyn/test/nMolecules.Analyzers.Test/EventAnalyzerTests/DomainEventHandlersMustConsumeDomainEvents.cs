using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.EventAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.EventAnalyzers.DomainEventHandlerAnalyzer>;

namespace NMolecules.Analyzers.Test.EventAnalyzerTests
{
    public class DomainEventHandlersMustConsumeDomainEvents
    {
        [Fact]
        public async Task Analyze_WithHandlerMethodWithoutDomainEventParameter_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.Events;

    public class AccountImported
    {
    }

    public class Handlers
    {
        [DomainEventHandler]
        public void {|#0:Handle|}(AccountImported imported)
        {
        }
    }
}", ElementNames.DomainEventHandler);

            var expected = new DiagnosticResult(Rules.DomainEventHandlersMustConsumeDomainEventsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithHandlerMethodUsingDomainEventParameter_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.Events;

    [DomainEvent]
    public class AccountImported
    {
    }

    public class Handlers
    {
        [DomainEventHandler]
        public void Handle(AccountImported imported)
        {
        }
    }
}", ElementNames.DomainEvent, ElementNames.DomainEventHandler);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        [Fact]
        public async Task Analyze_WithDelegateWithoutDomainEventParameter_EmitsError()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.Events;

    [DomainEventHandler]
    public delegate void {|#0:AccountImportedHandler|}(string id);
}", ElementNames.DomainEventHandler);

            var expected = new DiagnosticResult(Rules.DomainEventHandlersMustConsumeDomainEventsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithDelegateUsingDomainEventParameter_DoesNotEmitViolations()
        {
            var testCode = ServiceRoleShims.AppendIfNeeded(@"namespace NMolecules.Analyzers.Test.EventAnalyzerTests.SampleData
{
    using NMolecules.Events;

    [DomainEvent]
    public class AccountImported
    {
    }

    [DomainEventHandler]
    public delegate void AccountImportedHandler(AccountImported imported);
}", ElementNames.DomainEvent, ElementNames.DomainEventHandler);

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }
    }
}
