using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.DomainServiceAnalyzers
{
    public static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsApplicationService())
            {
                yield return symbol.Diagnostic(Rules.DomainServicesShouldNotUseApplicationServicesRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }

            if (type.IsRepository() && type is INamedTypeSymbol { TypeKind: not TypeKind.Interface })
            {
                yield return symbol.Diagnostic(
                    Rules.DomainServicesShouldOnlyUseRepositoryContractsRule,
                    symbol.ContainingType?.DisplayName() ?? symbol.DiagnosticTargetName(),
                    type.DisplayName());
            }
        }

        public static Diagnostic ViolatesInfrastructureSignature(this ISymbol symbol, ITypeSymbol type)
        {
            var domainService = symbol.ContainingType?.DisplayName() ?? symbol.DisplayName();
            return symbol.Diagnostic(
                Rules.DomainServicesShouldNotExposeInfrastructureSignaturesRule,
                domainService,
                type.DisplayName(),
                symbol.Name);
        }
    }
}
