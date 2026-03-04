using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers
{
    public static class IdAnalyzer
    {
        public static void AnalyzeEntityForId(
            SymbolAnalysisContext it,
            Func<INamedTypeSymbol, Diagnostic> onMissingIdentity,
            Func<INamedTypeSymbol, Diagnostic> onMultipleIdentities)
        {
            var classSymbol = (INamedTypeSymbol)it.Symbol;
            var identityMembers = GetIdentityMembers(classSymbol).ToArray();
            var identityCount = identityMembers.Length;

            if (identityCount == 0)
            {
                it.ReportDiagnostic(onMissingIdentity(classSymbol));
            }
            else if (identityCount > 1)
            {
                it.ReportDiagnostic(onMultipleIdentities(classSymbol));
            }
        }

        public static IReadOnlyList<ISymbol> GetIdentityMembers(INamedTypeSymbol classSymbol)
        {
            var baseMembers = Array.Empty<ISymbol>();
            var classSymbolBaseType = classSymbol.BaseType;
            if (classSymbolBaseType is not null && classSymbolBaseType.SpecialType != SpecialType.System_Object)
            {
                baseMembers = GetIdentityMembers(classSymbolBaseType).ToArray();
            }

            return baseMembers.Concat(GetOwnIdentityMembers(classSymbol)).ToArray();
        }

        private static IEnumerable<ISymbol> GetOwnIdentityMembers(INamedTypeSymbol classSymbol)
        {
            foreach (var member in classSymbol.GetMembers())
            {
                if (member.IsImplicitlyDeclared)
                {
                    continue;
                }

                if (member is not IFieldSymbol and not IPropertySymbol)
                {
                    continue;
                }

                if (member.IsIdentity())
                {
                    yield return member;
                }
            }
        }
    }
}
