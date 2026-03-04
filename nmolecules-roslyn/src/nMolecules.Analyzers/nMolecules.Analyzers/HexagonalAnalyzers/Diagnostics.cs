using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.HexagonalAnalyzers
{
    internal static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            var owner = symbol as ITypeSymbol ?? symbol.ContainingType;
            if (owner is null || !owner.IsHexagonalPort())
            {
                yield break;
            }

            if (owner.IsPrimaryPort() && type.IsHexagonalAdapter())
            {
                yield return symbol.Diagnostic(Rules.PrimaryPortsShouldNotDependOnAdaptersRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }

            if (owner.IsSecondaryPort() && type.IsHexagonalAdapter())
            {
                yield return symbol.Diagnostic(Rules.SecondaryPortsShouldNotDependOnAdaptersRule, symbol.DiagnosticTargetName(), type.DisplayName());
            }
        }

        internal static bool IsHexagonalPort(this ITypeSymbol type) =>
            type.IsPrimaryPort() || type.IsSecondaryPort();

        internal static bool IsPrimaryPort(this ITypeSymbol type) =>
            type.HasAttributeNamed("PrimaryPortAttribute");

        internal static bool IsSecondaryPort(this ITypeSymbol type) =>
            type.HasAttributeNamed("SecondaryPortAttribute");

        internal static bool IsHexagonalAdapter(this ITypeSymbol type) =>
            type.HasAttributeNamed("PrimaryAdapterAttribute", "SecondaryAdapterAttribute");
    }
}
