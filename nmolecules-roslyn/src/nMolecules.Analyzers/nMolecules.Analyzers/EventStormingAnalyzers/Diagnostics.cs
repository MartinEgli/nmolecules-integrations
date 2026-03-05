using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.EventStormingAnalyzers
{
    internal static class Diagnostics
    {
        private const string EventStormingNamespace = "NMolecules.Architecture.EventStorming";

        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            var owner = symbol as ITypeSymbol ?? symbol.ContainingType;
            if (owner is null || !owner.IsEventStormingType())
            {
                yield break;
            }

            if (owner.IsEventStormingActor() && type.IsEventStormingAggregate())
            {
                yield return symbol.Diagnostic(
                    Rules.ActorsShouldNotDependOnAggregatesRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }

            if (owner.IsEventStormingReadModel() && type.IsEventStormingAggregate())
            {
                yield return symbol.Diagnostic(
                    Rules.ReadModelsShouldNotDependOnAggregatesRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }

            if (owner.IsEventStormingExternalSystem() && type.IsEventStormingAggregate())
            {
                yield return symbol.Diagnostic(
                    Rules.ExternalSystemsShouldNotDependOnAggregatesRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }
        }

        internal static bool IsEventStormingType(this ITypeSymbol type) =>
            type.IsEventStormingActor() ||
            type.IsEventStormingCommand() ||
            type.IsEventStormingDomainEvent() ||
            type.IsEventStormingPolicy() ||
            type.IsEventStormingReadModel() ||
            type.IsEventStormingExternalSystem() ||
            type.IsEventStormingAggregate();

        internal static bool IsEventStormingActor(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "ActorAttribute");

        internal static bool IsEventStormingCommand(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "CommandAttribute");

        internal static bool IsEventStormingDomainEvent(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "DomainEventAttribute");

        internal static bool IsEventStormingPolicy(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "PolicyAttribute");

        internal static bool IsEventStormingReadModel(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "ReadModelAttribute");

        internal static bool IsEventStormingExternalSystem(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "ExternalSystemAttribute");

        internal static bool IsEventStormingAggregate(this ITypeSymbol type) =>
            type.HasAttributeNamedInNamespace(EventStormingNamespace, "AggregateAttribute");
    }
}
