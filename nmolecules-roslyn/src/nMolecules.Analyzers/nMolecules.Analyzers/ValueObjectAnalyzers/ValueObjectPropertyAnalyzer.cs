using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using static NMolecules.Analyzers.ValueObjectAnalyzers.Diagnostics;

namespace NMolecules.Analyzers.ValueObjectAnalyzers
{
    public class ValueObjectPropertyAnalyzer : PropertyAnalyzer
    {
        public ValueObjectPropertyAnalyzer()
            : base(Analyze)
        {
        }

        private static IEnumerable<Diagnostic> Analyze(IPropertySymbol propertySymbol) =>
            AnalyzeTypeUsageInSymbol(propertySymbol, propertySymbol.Type).Concat(EnsureThatPropertyIsReadonly(propertySymbol));

        private static IEnumerable<Diagnostic> EnsureThatPropertyIsReadonly(IPropertySymbol propertySymbol)
        {
            var isInitOnly = propertySymbol.SetMethod?.IsInitOnly == true;
            if (!propertySymbol.IsReadOnly && !isInitOnly)
            {
                yield return propertySymbol.ViolatesImmutability();
            }
        }
    }
}
