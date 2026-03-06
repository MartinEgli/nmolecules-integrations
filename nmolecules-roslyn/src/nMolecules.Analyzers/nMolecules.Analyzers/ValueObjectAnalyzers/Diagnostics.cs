using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NMolecules.DDD;
using static NMolecules.Analyzers.ValueObjectAnalyzers.Rules;

namespace NMolecules.Analyzers.ValueObjectAnalyzers
{
    public static class Diagnostics
    {
        public static Diagnostic ViolatesImmutability(this ISymbol symbol) => symbol.Diagnostic(ValueObjectShouldBeImmutableRule, symbol.ContainingType.DisplayName(), symbol.Name);

        public static Diagnostic DoesNotImplementIEquatable(this ISymbol symbol) => symbol.Diagnostic(ValueObjectMustImplementIEquatableRule, symbol.DisplayName());

        public static Diagnostic IsNotSealed(this ISymbol symbol) => symbol.Diagnostic(ValueObjectShouldBeSealedRule, symbol.DisplayName());

        public static Diagnostic DeclaresIdentity(this ISymbol symbol) => symbol.Diagnostic(ValueObjectMustNotDeclareIdentityRule, symbol.ContainingType.DisplayName(), symbol.Name);

        public static IEnumerable<Diagnostic> AnalyzeTypeUsageInSymbol(ISymbol symbol, ITypeSymbol type)
        {
            if (type.IsEntity())
            {
                yield return symbol.ViolatesEntityUsage();
            }

            if (type.IsDomainService())
            {
                yield return symbol.ViolatesDomainServiceUsage(type);
            }

            if (type.IsApplicationService())
            {
                yield return symbol.ViolatesApplicationServiceUsage(type);
            }

            if (type.IsLegacyService())
            {
                yield return symbol.ViolatesLegacyServiceUsage(type);
            }

            if (type.IsRepository())
            {
                yield return symbol.ViolatesRepositoryUsage();
            }

            if (type.IsAggregateRoot())
            {
                yield return symbol.ViolatesAggregateRootUsage();
            }

            if (type.IsFactory())
            {
                yield return symbol.ViolatesFactoryUsage(type);
            }
        }

        public static Diagnostic? AnalyzeIdentityDeclaration(ISymbol symbol)
        {
            return symbol.IsIdentity() && symbol.ContainingType.Is<ValueObjectAttribute>()
                ? symbol.DeclaresIdentity()
                : null;
        }

        private static Diagnostic ViolatesEntityUsage(this ISymbol symbol) => symbol.Diagnostic(ValueObjectMustNotUseEntityRule);

        private static Diagnostic ViolatesDomainServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(
                ValueObjectMustNotUseDomainServiceRule,
                symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(),
                type.DisplayName(),
                symbol.Name);

        private static Diagnostic ViolatesApplicationServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(
                ValueObjectMustNotUseApplicationServiceRule,
                symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(),
                type.DisplayName(),
                symbol.Name);

        private static Diagnostic ViolatesLegacyServiceUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(
                ValueObjectMustNotUseLegacyServiceRule,
                symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(),
                type.DisplayName(),
                symbol.Name);

        private static Diagnostic ViolatesRepositoryUsage(this ISymbol symbol) => symbol.Diagnostic(ValueObjectMustNotUseRepositoryRule);

        private static Diagnostic ViolatesAggregateRootUsage(this ISymbol symbol) => symbol.Diagnostic(ValueObjectMustNotUseAggregateRootRule);

        private static Diagnostic ViolatesFactoryUsage(this ISymbol symbol, ITypeSymbol type) =>
            symbol.Diagnostic(
                ValueObjectMustNotUseFactoryRule,
                symbol.ContainingType?.DisplayName() ?? symbol.DisplayName(),
                type.DisplayName(),
                symbol.Name);
    }
}
