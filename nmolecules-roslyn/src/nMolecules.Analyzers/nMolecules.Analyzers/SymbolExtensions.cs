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
        private const string IdentityAttributeName = "IdentityAttribute";
        private const string ApplicationLayerAttributeName = "ApplicationLayerAttribute";
        private const string DomainLayerAttributeName = "DomainLayerAttribute";
        private const string InfrastructureLayerAttributeName = "InfrastructureLayerAttribute";
        private const string UserInterfaceLayerAttributeName = "UserInterfaceLayerAttribute";

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

        public static bool IsLegacyService(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(ServiceAttribute));
        }

        public static bool IsDomainService(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(DomainServiceAttributeName);
        }

        public static bool IsApplicationService(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(ApplicationServiceAttributeName);
        }

        public static bool IsApplicationLayer(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(ApplicationLayerAttributeName);
        }

        public static bool IsDomainLayer(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(DomainLayerAttributeName);
        }

        public static bool IsInfrastructureLayer(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(InfrastructureLayerAttributeName);
        }

        public static bool IsUserInterfaceLayer(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(UserInterfaceLayerAttributeName);
        }

        public static bool IsLayer(this ITypeSymbol type)
        {
            return type.IsApplicationLayer() || type.IsDomainLayer() || type.IsInfrastructureLayer() || type.IsUserInterfaceLayer();
        }

        public static bool IsRepository(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(RepositoryAttribute));
        }

        public static bool IsFactory(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(FactoryAttribute));
        }

        public static bool IsAggregateRoot(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(AggregateRootAttribute));
        }

        public static bool IsIdentity(this ISymbol symbol)
        {
            return symbol.HasAttributeNamed(IdentityAttributeName);
        }

        public static bool IsEnum(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Enum;

        public static bool HasAttributeNamed(this ISymbol symbol, params string[] attributeNames)
        {
            var attributes = symbol.GetAttributes().ToArray();
            return attributes.Any(it => it.AttributeClass is { Name: var name } && attributeNames.Contains(name));
        }

        public static Diagnostic Diagnostic(
            this ISymbol symbol,
            DiagnosticDescriptor descriptor,
            params object[] parameters) =>
            Microsoft.CodeAnalysis.Diagnostic.Create(descriptor, symbol.Locations[0], parameters);

        public static string DisplayName(this ISymbol symbol) =>
            symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        public static string DiagnosticTargetName(this ISymbol symbol) =>
            symbol is ITypeSymbol ? symbol.DisplayName() : symbol.Name;
    }
}
