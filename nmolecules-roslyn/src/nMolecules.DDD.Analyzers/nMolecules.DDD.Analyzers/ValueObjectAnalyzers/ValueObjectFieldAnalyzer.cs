﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using static NMolecules.DDD.Analyzers.ValueObjectAnalyzers.Diagnostics;

namespace NMolecules.DDD.Analyzers.ValueObjectAnalyzers
{
    public class ValueObjectFieldAnalyzer : FieldAnalyzer
    {
        public ValueObjectFieldAnalyzer()
            : base(AnalyzeField)
        {
        }

        private static IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol) => AnalyzeTypeUsageInSymbol(symbol, symbol.Type).Concat(EnsureFieldIsReadonly(symbol));

        private static IEnumerable<Diagnostic> EnsureFieldIsReadonly(IFieldSymbol fieldSymbol)
        {
            if (!(fieldSymbol.IsReadOnly || fieldSymbol.IsConst))
            {
                yield return fieldSymbol.ViolatesImmutability();
            }
        }
    }
}