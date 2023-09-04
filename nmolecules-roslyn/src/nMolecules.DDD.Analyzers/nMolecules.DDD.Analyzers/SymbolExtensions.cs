using Microsoft.CodeAnalysis;

namespace NMolecules.DDD.Analyzers;

public static class SymbolExtensions
{
    public static bool IsAggregateRoot(this ITypeSymbol type)
    {
        var attributes = type.GetAttributes().ToArray();
        return attributes.Any(it => it.AttributeClass!.Name.Equals(nameof(AggregateRootAttribute)));
    }

    public static bool IsEntity(this ITypeSymbol type)
    {
        var attributes = type.GetAttributes().ToArray();
        return attributes.Any(it => it.AttributeClass!.Name.Equals(nameof(EntityAttribute)));
    }

    public static bool IsEnum(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Enum;

    public static bool IsRepository(this ITypeSymbol type)
    {
        var attributes = type.GetAttributes().ToArray();
        return attributes.Any(it => it.AttributeClass!.Name.Equals(nameof(RepositoryAttribute)));
    }

    public static bool IsFactory(this ITypeSymbol type)
    {
        var attributes = type.GetAttributes().ToArray();
        return attributes.Any(it => it.AttributeClass!.Name.Equals(nameof(FactoryAttribute)));
    }
    
    public static bool IsService(this ITypeSymbol type)
    {
        var attributes = type.GetAttributes().ToArray();
        return attributes.Any(it => it.AttributeClass!.Name.Equals(nameof(ServiceAttribute)));
    }
}