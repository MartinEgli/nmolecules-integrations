using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using static NMolecules.DDD.Analyzers.ValueObjectAnalyzers.Diagnostics;

namespace NMolecules.DDD.Analyzers.ValueObjectAnalyzers;

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
        if (!propertySymbol.IsReadOnly)
        {
            yield return propertySymbol.ViolatesImmutability();
        }
    }
}