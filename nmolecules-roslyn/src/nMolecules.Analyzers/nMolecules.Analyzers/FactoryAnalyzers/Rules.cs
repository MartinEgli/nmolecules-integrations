using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.FactoryAnalyzers
{
    public static class Rules
    {
        public const string FactoriesShouldNotUseApplicationServicesId = "XMoleculesFactory0001";

        public static readonly DiagnosticDescriptor FactoriesShouldNotUseApplicationServicesRule = new(
            FactoriesShouldNotUseApplicationServicesId,
            "Factories should not use application services",
            "Factory '{0}' must not depend on application service '{1}'",
            Category.DDD,
            DiagnosticSeverity.Error,
            true,
            "Factories create valid model objects and should not depend on application-level orchestration.");
    }
}
