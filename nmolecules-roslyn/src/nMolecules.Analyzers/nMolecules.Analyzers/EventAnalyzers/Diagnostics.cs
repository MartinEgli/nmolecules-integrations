using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.EventAnalyzers
{
    internal static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (symbol.ContainingType is not { } domainEvent || !domainEvent.IsDomainEvent())
            {
                yield break;
            }

            if (type.IsEntity())
            {
                yield return symbol.Diagnostic(
                    Rules.DomainEventsMustNotReferenceEntitiesRule,
                    domainEvent.DisplayName(),
                    type.DisplayName(),
                    symbol.Name);
            }

            if (type.IsAggregateRoot())
            {
                yield return symbol.Diagnostic(
                    Rules.DomainEventsMustNotReferenceAggregateRootsRule,
                    domainEvent.DisplayName(),
                    type.DisplayName(),
                    symbol.Name);
            }

            if (type.IsRepository())
            {
                yield return symbol.Diagnostic(
                    Rules.DomainEventsMustNotReferenceRepositoriesRule,
                    domainEvent.DisplayName(),
                    type.DisplayName(),
                    symbol.Name);
            }

            if (type.IsService())
            {
                var role = type.IsDomainService()
                    ? "domain service"
                    : type.IsApplicationService()
                        ? "application service"
                        : "service";

                yield return symbol.Diagnostic(
                    Rules.DomainEventsMustNotReferenceServicesRule,
                    domainEvent.DisplayName(),
                    role,
                    type.DisplayName(),
                    symbol.Name);
            }
        }
    }
}
