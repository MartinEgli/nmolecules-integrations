using Microsoft.CodeAnalysis;

namespace NMolecules.Shared.Analyzers;

public static class SymbolExtensions
{
    public static bool Is<TAttribute>(this ITypeSymbol type) where TAttribute : Attribute
    {
        var attributes = type.GetAttributes();
        return attributes.Any(it => it.AttributeClass!.Name.Equals(typeof(TAttribute).Name));
    }

    public static bool IsEnum(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Enum;

    public static Diagnostic Diagnostic(
        this ISymbol symbol,
        DiagnosticDescriptor descriptor,
        params object[] parameters) =>
        Microsoft.CodeAnalysis.Diagnostic.Create(descriptor, symbol.Locations[0], parameters);
}