using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NMolecules.DDD;

namespace NMolecules.Analyzers.ApplicationServiceAnalyzers
{
    public static class Diagnostics
    {
        private const string DomainServiceRoleName = "DomainService";

        public static IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol type)
        {
            if (type.IsEntity())
            {
                yield return type.Diagnostic(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(EntityAttribute).Replace("Attribute", ""));
            }

            if (type.IsAggregateRoot())
            {
                yield return type.Diagnostic(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(AggregateRootAttribute).Replace("Attribute", ""));
            }

            if (type.Is<ValueObjectAttribute>())
            {
                yield return type.Diagnostic(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), nameof(ValueObjectAttribute).Replace("Attribute", ""));
            }

            if (type.IsDomainService())
            {
                yield return type.Diagnostic(Rules.ApplicationServicesShouldNotAlsoBeDomainBuildingBlocksRule, type.DisplayName(), DomainServiceRoleName);
            }
        }
    }
}
