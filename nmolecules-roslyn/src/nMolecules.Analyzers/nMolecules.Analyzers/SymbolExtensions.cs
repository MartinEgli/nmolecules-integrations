using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using NMolecules.DDD;

namespace NMolecules.Analyzers
{
    public static class SymbolExtensions
    {
        private const string DomainServiceAttributeName = "DomainServiceAttribute";
        private const string ApplicationServiceAttributeName = "ApplicationServiceAttribute";

        public static bool Is<TAttribute>(this ITypeSymbol type) where TAttribute : Attribute
        {
            return type.HasAttributeNamed(typeof(TAttribute).Name);
        }

        public static bool IsEntity(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(EntityAttribute));
        }

        public static bool IsService(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(
                nameof(ServiceAttribute),
                DomainServiceAttributeName,
                ApplicationServiceAttributeName);
        }

        public static bool IsDomainService(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(DomainServiceAttributeName);
        }

        public static bool IsApplicationService(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(ApplicationServiceAttributeName);
        }

        public static bool IsRepository(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(RepositoryAttribute));
        }

        public static bool IsAggregateRoot(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(AggregateRootAttribute));
        }

        public static bool IsEnum(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Enum;

        private static bool HasAttributeNamed(this ITypeSymbol type, params string[] attributeNames)
        {
            var attributes = type.GetAttributes().ToArray();
            return attributes.Any(it => it.AttributeClass is { Name: var name } && attributeNames.Contains(name));
        }

        public static Diagnostic Diagnostic(
            this ISymbol symbol,
            DiagnosticDescriptor descriptor,
            params object[] parameters) =>
            Microsoft.CodeAnalysis.Diagnostic.Create(descriptor, symbol.Locations[0], parameters);
    }
}
