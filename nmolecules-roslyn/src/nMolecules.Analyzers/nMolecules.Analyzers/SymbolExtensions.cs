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
        private const string AllowRepositoryCompositionAttributeName = "AllowRepositoryCompositionAttribute";
        private const string CommandHandlerAttributeName = "CommandHandlerAttribute";
        private const string CommandDispatcherAttributeName = "CommandDispatcherAttribute";
        private const string IdentityAttributeName = "IdentityAttribute";
        private const string DomainEventAttributeName = "DomainEventAttribute";
        private const string DomainEventPublisherAttributeName = "DomainEventPublisherAttribute";
        private const string DomainEventHandlerAttributeName = "DomainEventHandlerAttribute";
        private const string ApplicationLayerAttributeName = "ApplicationLayerAttribute";
        private const string DomainLayerAttributeName = "DomainLayerAttribute";
        private const string InterfaceLayerAttributeName = "InterfaceLayerAttribute";
        private const string InfrastructureLayerAttributeName = "InfrastructureLayerAttribute";
        private const string UserInterfaceLayerAttributeName = "UserInterfaceLayerAttribute";
        private const string QueryModelAttributeName = "QueryModelAttribute";
        private const string QueryAttributeName = "QueryAttribute";
        private const string QueryHandlerAttributeName = "QueryHandlerAttribute";
        private const string ProjectionAttributeName = "ProjectionAttribute";

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

        public static bool IsCommandHandler(this ISymbol symbol)
        {
            return symbol.HasAttributeNamed(CommandHandlerAttributeName);
        }

        public static bool IsCommandDispatcher(this ISymbol symbol)
        {
            return symbol.HasAttributeNamed(CommandDispatcherAttributeName);
        }

        public static bool IsQueryHandler(this ISymbol symbol)
        {
            return symbol.HasAttributeNamed(QueryHandlerAttributeName);
        }

        public static bool IsApplicationLayer(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(ApplicationLayerAttributeName);
        }

        public static bool IsDomainEvent(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(DomainEventAttributeName);
        }

        public static bool IsDomainEventPublisher(this ISymbol symbol)
        {
            return symbol.HasAttributeNamed(DomainEventPublisherAttributeName);
        }

        public static bool IsDomainEventHandler(this ISymbol symbol)
        {
            return symbol.HasAttributeNamed(DomainEventHandlerAttributeName);
        }

        public static bool IsQueryModel(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(QueryModelAttributeName);
        }

        public static bool IsQuery(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(QueryAttributeName);
        }

        public static bool IsProjection(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(ProjectionAttributeName);
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
            return type.HasAttributeNamed(UserInterfaceLayerAttributeName, InterfaceLayerAttributeName);
        }

        public static bool IsLayer(this ITypeSymbol type)
        {
            return type.IsApplicationLayer() || type.IsDomainLayer() || type.IsInfrastructureLayer() || type.IsUserInterfaceLayer();
        }

        public static bool IsRepository(this ITypeSymbol type)
        {
            return type.HasAttributeNamed(nameof(RepositoryAttribute));
        }

        public static bool AllowsRepositoryComposition(this ISymbol symbol)
        {
            for (var current = symbol; current is not null; current = current.ContainingSymbol)
            {
                if (current.HasAttributeNamed(AllowRepositoryCompositionAttributeName))
                {
                    return true;
                }
            }

            return false;
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
