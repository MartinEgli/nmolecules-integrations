using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NMolecules.Analyzers.MicroservicesAnalyzers;
using Xunit;
using static NMolecules.Analyzers.Test.ExpectedResults;
using VerifyCS = NMolecules.Analyzers.Test.Verifiers.CSharpAnalyzerVerifier<NMolecules.Analyzers.MicroservicesAnalyzers.MicroservicesAnalyzer>;

namespace NMolecules.Analyzers.Test.MicroservicesAnalyzerTests
{
    public class MicroservicesDependencies
    {
        [Fact]
        public async Task Analyze_WithApiGatewayWithoutServiceContract_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [ApiGateway]
    public class {|#0:EdgeGateway|}
    {
    }
}
" + MicroservicesShims;

            var expected = new DiagnosticResult(Rules.ApiGatewaysShouldDependOnServiceContractsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithBffWithoutServiceContract_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [BackendForFrontend]
    public class {|#0:PortalBff|}
    {
    }
}
" + MicroservicesShims;

            var expected = new DiagnosticResult(Rules.BackendForFrontendsShouldDependOnServiceContractsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithServiceContractDependingOnMicroservice_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [Microservice]
    public class OrdersService { }

    [ServiceContract]
    public class OrdersContract
    {
        private readonly OrdersService {|#0:service|};
    }
}
" + MicroservicesShims;

            var expected = new DiagnosticResult(Rules.ServiceContractsShouldNotDependOnMicroserviceImplementationsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithIntegrationEventDependingOnMicroservice_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [Microservice]
    public class BillingService { }

    [IntegrationEvent]
    public class InvoicePaid
    {
        public BillingService {|#0:Service|} { get; set; }
    }
}
" + MicroservicesShims;

            var expected = new DiagnosticResult(Rules.IntegrationEventsShouldNotDependOnMicroserviceImplementationsId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithSagaOrchestratorWithoutContractOrEventDependency_EmitsWarning()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [SagaOrchestrator]
    public class {|#0:OrderSaga|}
    {
    }
}
" + MicroservicesShims;

            var expected = new DiagnosticResult(Rules.SagaOrchestratorsShouldDependOnContractsOrIntegrationEventsId, DiagnosticSeverity.Warning).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithSagaParticipantDependingOnGateway_EmitsError()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [ServiceContract]
    public interface IEdgeContract { }

    [ApiGateway]
    public class GatewayEdge
    {
        private readonly IEdgeContract contract;
    }

    [SagaParticipant]
    public class InventoryParticipant
    {
        private readonly GatewayEdge {|#0:gateway|};
    }
}
" + MicroservicesShims;

            var expected = new DiagnosticResult(Rules.SagaParticipantsShouldNotDependOnGatewayOrBffId, DiagnosticSeverity.Error).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldEmitIssues(expected));
        }

        [Fact]
        public async Task Analyze_WithGatewayBffAndSagaUsingContractsAndEvents_DoesNotEmitViolations()
        {
            var testCode = @"using System;
namespace SampleData
{
    using NMolecules.Architecture.Microservices;

    [ServiceContract]
    public interface IOrdersContract { }

    [IntegrationEvent]
    public class OrderPlaced { }

    [ApiGateway]
    public class EdgeGateway
    {
        private readonly IOrdersContract contract;
    }

    [BackendForFrontend]
    public class PortalBff
    {
        public IOrdersContract Contract { get; set; }
    }

    [SagaOrchestrator]
    public class FulfillmentSaga
    {
        public void Handle(OrderPlaced domainEvent)
        {
        }
    }
}
" + MicroservicesShims;

            await VerifyCS.VerifyAnalyzerAsync(testCode, ShouldNotEmitAnyIssues());
        }

        private const string MicroservicesShims = @"
namespace NMolecules.Architecture.Microservices
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class MicroserviceAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ApiGatewayAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class BackendForFrontendAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class ServiceContractAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class IntegrationEventAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SagaOrchestratorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public class SagaParticipantAttribute : Attribute { }
}";
    }
}
