using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.MicroservicesAnalyzers
{
    internal static class Diagnostics
    {
        private const string MicroservicesNamespace = "NMolecules.Architecture.Microservices";

        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            var owner = symbol as ITypeSymbol ?? symbol.ContainingType;
            if (owner is null || !owner.IsMicroservicesType())
            {
                yield break;
            }

            if (owner.IsServiceContract() && type.IsMicroservice())
            {
                yield return symbol.Diagnostic(
                    Rules.ServiceContractsShouldNotDependOnMicroserviceImplementationsRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }

            if (owner.IsIntegrationEvent() && type.IsMicroservice())
            {
                yield return symbol.Diagnostic(
                    Rules.IntegrationEventsShouldNotDependOnMicroserviceImplementationsRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }

            if (owner.IsSagaParticipant() && (type.IsApiGateway() || type.IsBackendForFrontend()))
            {
                yield return symbol.Diagnostic(
                    Rules.SagaParticipantsShouldNotDependOnGatewayOrBffRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }
        }

        internal static bool IsMicroservicesType(this ITypeSymbol type) =>
            type.IsMicroservice() ||
            type.IsApiGateway() ||
            type.IsBackendForFrontend() ||
            type.IsServiceContract() ||
            type.IsIntegrationEvent() ||
            type.IsSagaOrchestrator() ||
            type.IsSagaParticipant();

        internal static bool IsMicroservice(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "MicroserviceAttribute");

        internal static bool IsApiGateway(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "ApiGatewayAttribute");

        internal static bool IsBackendForFrontend(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "BackendForFrontendAttribute");

        internal static bool IsServiceContract(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "ServiceContractAttribute");

        internal static bool IsIntegrationEvent(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "IntegrationEventAttribute");

        internal static bool IsSagaOrchestrator(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "SagaOrchestratorAttribute");

        internal static bool IsSagaParticipant(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(MicroservicesNamespace, "SagaParticipantAttribute");
    }
}
