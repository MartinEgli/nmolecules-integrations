using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.BricksAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BrickRuleAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string[] RuleAttributeNames = { "RuleAttribute" };
        private static readonly string[] RuleFilterAttributeNames = { "RuleFilterAttribute" };
        private static readonly string[] RoleAttributeNames = { "RoleAttribute" };
        private static readonly string[] RoleAliasAttributeNames = { "RoleAliasAttribute" };
        private static readonly string[] ExcludedSourceNameContainsAttributeNames = { "ExcludedSourceNameContainsAttribute" };
        private static readonly string[] ExcludedTargetNameContainsAttributeNames = { "ExcludedTargetNameContainsAttribute" };
        private static readonly string[] ExcludedMemberNameContainsAttributeNames = { "ExcludedMemberNameContainsAttribute" };
        private static readonly string[] RequiredSourceNameContainsAttributeNames = { "RequiredSourceNameContainsAttribute" };
        private static readonly string[] RequiredTargetNameContainsAttributeNames = { "RequiredTargetNameContainsAttribute" };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Rules.BrickRuleViolationRule,
            Rules.BrickRuleConfigurationRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var allTypes = GetAllTypes(context.Compilation.Assembly.GlobalNamespace).ToArray();
            if (allTypes.Length == 0)
            {
                return;
            }

            var roleMap = BuildRoleMap(allTypes);
            var rules = CollectRuleDeclarations(context.Compilation).ToArray();

            foreach (var rule in rules)
            {
                if (!rule.IsValid)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        Rules.BrickRuleConfigurationRule,
                        rule.Location,
                        $"Brick rule declaration is invalid. Id='{rule.Id}', SourceRole='{rule.SourceRole}', TargetRole='{rule.TargetRole}'"));
                    continue;
                }

                AnalyzeRule(rule, allTypes, roleMap, context);
            }
        }

        private static void AnalyzeRule(
            BrickRuleDeclaration rule,
            IReadOnlyList<INamedTypeSymbol> allTypes,
            IReadOnlyDictionary<INamedTypeSymbol, ISet<string>> roleMap,
            CompilationAnalysisContext context)
        {
            var sourceTypes = allTypes
                .Where(type => HasRole(type, rule.SourceRole, roleMap))
                .Where(type => IsSourceCandidate(rule, type))
                .ToArray();

            if (rule.Mode == BrickRuleMode.RequireDependency)
            {
                AnalyzeRequiredDependencyRule(rule, sourceTypes, roleMap, context);
                return;
            }

            foreach (var sourceType in sourceTypes)
            {
                foreach (var observation in CollectDependencyObservations(sourceType))
                {
                    if (ContainsAnyToken(observation.MemberSymbol.Name, rule.GetFilterValue<ExcludedMemberNameContainsRuleFilter>()))
                    {
                        continue;
                    }

                    foreach (var candidateType in ExpandType(observation.DependencyType).OfType<INamedTypeSymbol>())
                    {
                        if (!HasRole(candidateType, rule.TargetRole, roleMap))
                        {
                            continue;
                        }

                        if (!IsTargetCandidate(rule, candidateType))
                        {
                            continue;
                        }

                        context.ReportDiagnostic(Diagnostic.Create(
                            Rules.BrickRuleViolationRule,
                            observation.MemberSymbol.Locations.FirstOrDefault() ?? sourceType.Locations.FirstOrDefault() ?? Location.None,
                            FormatViolationMessage(rule, sourceType, candidateType.DisplayName(), observation.MemberSymbol.Name)));
                    }
                }
            }
        }

        private static void AnalyzeRequiredDependencyRule(
            BrickRuleDeclaration rule,
            IReadOnlyList<INamedTypeSymbol> sourceTypes,
            IReadOnlyDictionary<INamedTypeSymbol, ISet<string>> roleMap,
            CompilationAnalysisContext context)
        {
            foreach (var sourceType in sourceTypes)
            {
                var hasRequiredDependency = CollectDependencyObservations(sourceType)
                    .Where(observation => !ContainsAnyToken(observation.MemberSymbol.Name, rule.GetFilterValue<ExcludedMemberNameContainsRuleFilter>()))
                    .SelectMany(observation => ExpandType(observation.DependencyType).OfType<INamedTypeSymbol>())
                    .Any(candidateType => HasRole(candidateType, rule.TargetRole, roleMap) && IsTargetCandidate(rule, candidateType));

                if (hasRequiredDependency)
                {
                    continue;
                }

                context.ReportDiagnostic(Diagnostic.Create(
                    Rules.BrickRuleViolationRule,
                    sourceType.Locations.FirstOrDefault() ?? Location.None,
                    FormatViolationMessage(rule, sourceType, rule.TargetRole, "<none>")));
            }
        }

        private static string FormatViolationMessage(BrickRuleDeclaration rule, ISymbol source, string target, string member)
        {
            if (!string.IsNullOrWhiteSpace(rule.Message))
            {
                return rule.Message
                    .Replace("{rule}", rule.Id)
                    .Replace("{source}", source.DisplayName())
                    .Replace("{target}", target)
                    .Replace("{member}", member);
            }

            return rule.Mode == BrickRuleMode.RequireDependency
                ? $"Brick rule '{rule.Id}' requires dependency: '{source.DisplayName()}' must depend on role '{rule.TargetRole}'."
                : $"Brick rule '{rule.Id}' forbids dependency: '{source.DisplayName()}' must not depend on '{target}' via member '{member}'.";
        }

        private static bool IsSourceCandidate(BrickRuleDeclaration rule, ITypeSymbol sourceType)
        {
            var sourceName = sourceType.DisplayName();
            if (ContainsAnyToken(sourceName, rule.GetFilterValue<ExcludedSourceNameContainsRuleFilter>()))
            {
                return false;
            }

            return MatchesRequiredTokens(sourceName, rule.GetFilterValue<RequiredSourceNameContainsRuleFilter>());
        }

        private static bool IsTargetCandidate(BrickRuleDeclaration rule, ITypeSymbol targetType)
        {
            var targetName = targetType.DisplayName();
            if (ContainsAnyToken(targetName, rule.GetFilterValue<ExcludedTargetNameContainsRuleFilter>()))
            {
                return false;
            }

            return MatchesRequiredTokens(targetName, rule.GetFilterValue<RequiredTargetNameContainsRuleFilter>());
        }

        private static bool MatchesRequiredTokens(string value, string tokenList)
        {
            var tokens = SplitTokens(tokenList);
            if (tokens.Length == 0)
            {
                return true;
            }

            return tokens.Any(token => value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool ContainsAnyToken(string value, string tokenList)
        {
            var tokens = SplitTokens(tokenList);
            return tokens.Any(token => value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static string[] SplitTokens(string values) =>
            values.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(it => it.Trim())
                .Where(it => !string.IsNullOrWhiteSpace(it))
                .ToArray();

        private static IReadOnlyDictionary<INamedTypeSymbol, ISet<string>> BuildRoleMap(IEnumerable<INamedTypeSymbol> types)
        {
            var map = new Dictionary<INamedTypeSymbol, ISet<string>>(SymbolEqualityComparer.Default);

            foreach (var type in types)
            {
                var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var attribute in type.GetAttributes())
                {
                    if (IsBrickRoleAttribute(attribute.AttributeClass))
                    {
                        var role = GetRoleNameFromBrickRole(attribute);
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            roles.Add(role);
                        }
                    }

                    if (attribute.AttributeClass is null)
                    {
                        continue;
                    }

                    foreach (var alias in attribute.AttributeClass.GetAttributes().Where(alias => IsBrickRoleAliasAttribute(alias.AttributeClass)))
                    {
                        var role = GetRoleNameFromBrickRoleAlias(alias);
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            roles.Add(role);
                        }
                    }
                }

                if (roles.Count > 0)
                {
                    map[type] = roles;
                }
            }

            return map;
        }

        private static IEnumerable<BrickRuleDeclaration> CollectRuleDeclarations(Compilation compilation)
        {
            foreach (var declaration in CollectRuleDeclarations(compilation.Assembly))
            {
                yield return declaration;
            }

            foreach (var declaration in CollectRuleDeclarations(compilation.SourceModule))
            {
                yield return declaration;
            }

            foreach (var type in GetAllTypes(compilation.Assembly.GlobalNamespace))
            {
                foreach (var declaration in CollectRuleDeclarations(type))
                {
                    yield return declaration;
                }
            }
        }

        private static IEnumerable<BrickRuleDeclaration> CollectRuleDeclarations(ISymbol symbol)
        {
            var attributes = symbol.GetAttributes().ToArray();

            foreach (var attribute in attributes.Where(candidate => IsBrickRuleAttribute(candidate.AttributeClass)))
            {
                var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation() ?? symbol.Locations.FirstOrDefault();
                if (location is null)
                {
                    continue;
                }

                var id = GetConstructorString(attribute, 0);
                var sourceRole = GetConstructorString(attribute, 1);
                var targetRole = GetConstructorString(attribute, 2);
                var mode = GetRuleMode(attribute);
                var message = GetConstructorString(attribute, 4);
                var filters = CollectRuleFilters(attributes, id);

                yield return new BrickRuleDeclaration(
                    id,
                    sourceRole,
                    targetRole,
                    mode,
                    message,
                    filters,
                    location);
            }
        }

        private static BrickRuleMode GetRuleMode(AttributeData attribute)
        {
            if (attribute.ConstructorArguments.Length <= 3)
            {
                return BrickRuleMode.ForbidDependency;
            }

            var argument = attribute.ConstructorArguments[3];
            if (argument.Value is int integerValue && Enum.IsDefined(typeof(BrickRuleMode), integerValue))
            {
                return (BrickRuleMode)integerValue;
            }

            return BrickRuleMode.ForbidDependency;
        }

        private static string GetConstructorString(AttributeData attribute, int index)
        {
            if (attribute.ConstructorArguments.Length <= index)
            {
                return string.Empty;
            }

            return attribute.ConstructorArguments[index].Value as string ?? string.Empty;
        }

        private static string[] GetConstructorStrings(AttributeData attribute, int index)
        {
            if (attribute.ConstructorArguments.Length <= index)
            {
                return Array.Empty<string>();
            }

            var argument = attribute.ConstructorArguments[index];
            if (argument.Kind != TypedConstantKind.Array)
            {
                return Array.Empty<string>();
            }

            return NormalizeTokens(argument.Values
                .Select(value => value.Value as string)
                .Where(value => value is not null)
                .Select(value => value!));
        }

        private static IReadOnlyList<BrickRuleFilter> CollectRuleFilters(IEnumerable<AttributeData> attributes, string ruleId)
        {
            return attributes
                .Where(attribute => IsBrickRuleFilterAttribute(attribute.AttributeClass))
                .Where(attribute => string.Equals(GetConstructorString(attribute, 0), ruleId, StringComparison.Ordinal))
                .Select(CreateFilter)
                .Where(filter => filter is not null)
                .Cast<BrickRuleFilter>()
                .ToArray();
        }

        private static BrickRuleFilter? CreateFilter(AttributeData attribute)
        {
            var tokens = GetConstructorStrings(attribute, 1);

            if (IsExcludedSourceNameContainsAttribute(attribute.AttributeClass))
            {
                return new ExcludedSourceNameContainsRuleFilter(tokens);
            }

            if (IsExcludedTargetNameContainsAttribute(attribute.AttributeClass))
            {
                return new ExcludedTargetNameContainsRuleFilter(tokens);
            }

            if (IsExcludedMemberNameContainsAttribute(attribute.AttributeClass))
            {
                return new ExcludedMemberNameContainsRuleFilter(tokens);
            }

            if (IsRequiredSourceNameContainsAttribute(attribute.AttributeClass))
            {
                return new RequiredSourceNameContainsRuleFilter(tokens);
            }

            if (IsRequiredTargetNameContainsAttribute(attribute.AttributeClass))
            {
                return new RequiredTargetNameContainsRuleFilter(tokens);
            }

            return null;
        }

        private static string GetRoleNameFromBrickRole(AttributeData attribute)
        {
            var ctorRole = GetConstructorString(attribute, 0);
            if (!string.IsNullOrWhiteSpace(ctorRole))
            {
                return ctorRole;
            }

            var namedRole = attribute.NamedArguments
                .FirstOrDefault(argument => string.Equals(argument.Key, "Name", StringComparison.Ordinal))
                .Value
                .Value as string;

            return namedRole ?? string.Empty;
        }

        private static string GetRoleNameFromBrickRoleAlias(AttributeData attribute)
        {
            var ctorRole = GetConstructorString(attribute, 0);
            if (!string.IsNullOrWhiteSpace(ctorRole))
            {
                return ctorRole;
            }

            var namedRole = attribute.NamedArguments
                .FirstOrDefault(argument => string.Equals(argument.Key, "Role", StringComparison.Ordinal))
                .Value
                .Value as string;

            return namedRole ?? string.Empty;
        }

        private static bool HasRole(
            INamedTypeSymbol type,
            string role,
            IReadOnlyDictionary<INamedTypeSymbol, ISet<string>> roleMap) =>
            roleMap.TryGetValue(type, out var roles) && roles.Contains(role);

        private static bool IsBrickRuleAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RuleAttributeNames);

        private static bool IsBrickRuleFilterAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RuleFilterAttributeNames);

        private static bool IsBrickRoleAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RoleAttributeNames);

        private static bool IsBrickRoleAliasAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RoleAliasAttributeNames);

        private static bool IsExcludedSourceNameContainsAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, ExcludedSourceNameContainsAttributeNames);

        private static bool IsExcludedTargetNameContainsAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, ExcludedTargetNameContainsAttributeNames);

        private static bool IsExcludedMemberNameContainsAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, ExcludedMemberNameContainsAttributeNames);

        private static bool IsRequiredSourceNameContainsAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequiredSourceNameContainsAttributeNames);

        private static bool IsRequiredTargetNameContainsAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequiredTargetNameContainsAttributeNames);

        private static bool InheritsFromAnyAttribute(INamedTypeSymbol? type, IReadOnlyCollection<string> attributeNames)
        {
            for (var current = type; current is not null; current = current.BaseType)
            {
                if (attributeNames.Contains(current.Name))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<DependencyObservation> CollectDependencyObservations(INamedTypeSymbol sourceType)
        {
            if (sourceType.BaseType is { SpecialType: not SpecialType.System_Object } baseType)
            {
                yield return new DependencyObservation(sourceType, baseType);
            }

            foreach (var implementedInterface in sourceType.Interfaces)
            {
                yield return new DependencyObservation(sourceType, implementedInterface);
            }

            foreach (var member in sourceType.GetMembers())
            {
                switch (member)
                {
                    case IFieldSymbol field:
                        yield return new DependencyObservation(field, field.Type);
                        break;
                    case IPropertySymbol property:
                        yield return new DependencyObservation(property, property.Type);
                        break;
                    case IMethodSymbol method when method.MethodKind is not MethodKind.PropertyGet and not MethodKind.PropertySet:
                    {
                        if (!method.ReturnsVoid)
                        {
                            yield return new DependencyObservation(method, method.ReturnType);
                        }

                        foreach (var parameter in method.Parameters)
                        {
                            yield return new DependencyObservation(parameter, parameter.Type);
                        }

                        break;
                    }
                }
            }
        }

        private static IEnumerable<ITypeSymbol> ExpandType(ITypeSymbol type)
        {
            yield return type;

            switch (type)
            {
                case IArrayTypeSymbol array:
                {
                    foreach (var nested in ExpandType(array.ElementType))
                    {
                        yield return nested;
                    }

                    break;
                }
                case INamedTypeSymbol named:
                {
                    foreach (var typeArgument in named.TypeArguments)
                    {
                        foreach (var nested in ExpandType(typeArgument))
                        {
                            yield return nested;
                        }
                    }

                    break;
                }
                case IPointerTypeSymbol pointer:
                {
                    foreach (var nested in ExpandType(pointer.PointedAtType))
                    {
                        yield return nested;
                    }

                    break;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
            {
                yield return type;

                foreach (var nested in GetNestedTypes(type))
                {
                    yield return nested;
                }
            }

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            {
                foreach (var type in GetAllTypes(nestedNamespace))
                {
                    yield return type;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
        {
            foreach (var nested in type.GetTypeMembers())
            {
                yield return nested;

                foreach (var nestedType in GetNestedTypes(nested))
                {
                    yield return nestedType;
                }
            }
        }

        private sealed class BrickRuleDeclaration
        {
            public BrickRuleDeclaration(
                string id,
                string sourceRole,
                string targetRole,
                BrickRuleMode mode,
                string message,
                IReadOnlyList<BrickRuleFilter> filters,
                Location location)
            {
                Id = id;
                SourceRole = sourceRole;
                TargetRole = targetRole;
                Mode = mode;
                Message = message;
                Filters = filters;
                Location = location;
            }

            public string Id { get; }
            public string SourceRole { get; }
            public string TargetRole { get; }
            public BrickRuleMode Mode { get; }
            public string Message { get; }
            public IReadOnlyList<BrickRuleFilter> Filters { get; }
            public Location Location { get; }

            public bool IsValid =>
                !string.IsNullOrWhiteSpace(Id) &&
                !string.IsNullOrWhiteSpace(SourceRole) &&
                !string.IsNullOrWhiteSpace(TargetRole);

            public string GetFilterValue<TFilter>()
                where TFilter : BrickRuleFilter
            {
                return string.Join("|", Filters.OfType<TFilter>().SelectMany(filter => filter.Tokens));
            }
        }

        private abstract class BrickRuleFilter
        {
            protected BrickRuleFilter(IEnumerable<string> tokens)
            {
                Tokens = NormalizeTokens(tokens);
                Value = string.Join("|", Tokens);
            }

            public string[] Tokens { get; }

            public string Value { get; }
        }

        private sealed class ExcludedSourceNameContainsRuleFilter : BrickRuleFilter
        {
            public ExcludedSourceNameContainsRuleFilter(IEnumerable<string> tokens) : base(tokens)
            {
            }
        }

        private sealed class ExcludedTargetNameContainsRuleFilter : BrickRuleFilter
        {
            public ExcludedTargetNameContainsRuleFilter(IEnumerable<string> tokens) : base(tokens)
            {
            }
        }

        private sealed class ExcludedMemberNameContainsRuleFilter : BrickRuleFilter
        {
            public ExcludedMemberNameContainsRuleFilter(IEnumerable<string> tokens) : base(tokens)
            {
            }
        }

        private sealed class RequiredSourceNameContainsRuleFilter : BrickRuleFilter
        {
            public RequiredSourceNameContainsRuleFilter(IEnumerable<string> tokens) : base(tokens)
            {
            }
        }

        private sealed class RequiredTargetNameContainsRuleFilter : BrickRuleFilter
        {
            public RequiredTargetNameContainsRuleFilter(IEnumerable<string> tokens) : base(tokens)
            {
            }
        }

        private sealed class DependencyObservation
        {
            public DependencyObservation(ISymbol memberSymbol, ITypeSymbol dependencyType)
            {
                MemberSymbol = memberSymbol;
                DependencyType = dependencyType;
            }

            public ISymbol MemberSymbol { get; }
            public ITypeSymbol DependencyType { get; }
        }

        private enum BrickRuleMode
        {
            ForbidDependency = 0,
            RequireDependency = 1
        }

        private static string[] NormalizeTokens(IEnumerable<string> tokens)
        {
            return tokens
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .Select(token => token.Trim())
                .Where(token => token.Length > 0)
                .ToArray();
        }
    }
}
