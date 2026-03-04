using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.HexagonalAnalyzers
{
    internal static class Diagnostics
    {
        public static IEnumerable<Diagnostic> AnalyzeTypeInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            var owner = symbol as ITypeSymbol ?? symbol.ContainingType;
            if (owner is null || !owner.IsHexagonalType())
            {
                yield break;
            }

            if (owner.IsHexagonalApplication() && type.IsHexagonalPortOrAdapter())
            {
                yield return symbol.Diagnostic(
                    Rules.ApplicationCoreShouldNotDependOnPortsOrAdaptersRule,
                    symbol.DiagnosticTargetName(),
                    type.DisplayName());
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

        internal static bool IsHexagonalType(this ITypeSymbol type) =>
            type.IsHexagonalApplication() || type.IsHexagonalPort() || type.IsHexagonalAdapter();

        internal static bool IsHexagonalApplication(this ITypeSymbol type) =>
            type.HasAttributeNamed("ApplicationAttribute");

        internal static bool IsHexagonalPort(this ITypeSymbol type) =>
            type.IsPrimaryPort() || type.IsSecondaryPort();

        internal static bool IsPrimaryPort(this ITypeSymbol type) =>
            type.HasAttributeNamed("PrimaryPortAttribute");

        internal static bool IsSecondaryPort(this ITypeSymbol type) =>
            type.HasAttributeNamed("SecondaryPortAttribute");

        internal static bool IsPrimaryAdapter(this ITypeSymbol type) =>
            type.HasAttributeNamed("PrimaryAdapterAttribute");

        internal static bool IsSecondaryAdapter(this ITypeSymbol type) =>
            type.HasAttributeNamed("SecondaryAdapterAttribute");

        internal static bool IsHexagonalAdapter(this ITypeSymbol type) =>
            type.IsPrimaryAdapter() || type.IsSecondaryAdapter();

        internal static bool IsHexagonalPortOrAdapter(this ITypeSymbol type) =>
            type.IsHexagonalPort() || type.IsHexagonalAdapter();
    }
}
