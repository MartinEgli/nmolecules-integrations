using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.BricksAnalyzers
{
    internal static class BricksAnalyzerInternals
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
        private static readonly string[] RequireExactlyOneMemberAttributeNames = { "RequireExactlyOneMemberAttribute" };
        private static readonly string[] RequireAllMembersAttributeNames = { "RequireAllMembersAttribute" };
        private static readonly string[] RequireMemberCountAttributeNames = { "RequireMemberCountAttribute" };
        private static readonly string[] RequireExclusiveChoiceAttributeNames = { "RequireExclusiveChoiceAttribute" };
        private static readonly string[] RequireMemberRangeAttributeNames = { "RequireMemberRangeAttribute" };
        private static readonly string[] ForbidMemberAttributeNames = { "ForbidMemberAttribute" };
        private static readonly string[] RequireUniqueNamedMemberAttributeNames = { "RequireUniqueNamedMemberAttribute" };
        private static readonly string[] RequireNamedMembersAttributeNames = { "RequireNamedMembersAttribute" };

        internal static IReadOnlyList<INamedTypeSymbol> GetAllTypesInCompilation(Compilation compilation)
        {
            return GetAllTypes(compilation.Assembly.GlobalNamespace).ToArray();
        }

        internal static void AnalyzeDependencyRules(
            CompilationAnalysisContext context,
            IReadOnlyList<INamedTypeSymbol> allTypes)
        {
            if (allTypes.Count == 0)
            {
                return;
            }

            var roleMap = BuildRoleMap(allTypes);
            var rules = CollectRuleDeclarations(context.Compilation).ToArray();

            foreach (var rule in rules)
            {
                if (!rule.IsValid)
                {
                    context.ReportDiagnostic(BricksDiagnosticProperties.Create(
                        Rules.BrickRuleConfigurationRule,
                        rule.Location,
                        $"Brick rule declaration is invalid. Id='{rule.Id}', SourceRole='{rule.SourceRole}', TargetRole='{rule.TargetRole}'",
                        configurationKind: "Rule",
                        ruleId: rule.Id,
                        sourceRole: rule.SourceRole,
                        targetRole: rule.TargetRole));
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
                foreach (var observation in CollectDependencyObservations(sourceType, context.Compilation))
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

                        context.ReportDiagnostic(BricksDiagnosticProperties.Create(
                            Rules.BrickRuleViolationRule,
                            observation.Location ?? observation.MemberSymbol.Locations.FirstOrDefault() ?? sourceType.Locations.FirstOrDefault() ?? Location.None,
                            FormatViolationMessage(rule, sourceType, candidateType.DisplayName(), observation.MemberSymbol.Name),
                            violationKind: "ForbiddenDependency",
                            ruleId: rule.Id,
                            sourceRole: rule.SourceRole,
                            targetRole: rule.TargetRole,
                            source: sourceType.DisplayName(),
                            target: candidateType.DisplayName()));
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
                var hasRequiredDependency = CollectDependencyObservations(sourceType, context.Compilation)
                    .Where(observation => !ContainsAnyToken(observation.MemberSymbol.Name, rule.GetFilterValue<ExcludedMemberNameContainsRuleFilter>()))
                    .SelectMany(observation => ExpandType(observation.DependencyType).OfType<INamedTypeSymbol>())
                    .Any(candidateType => HasRole(candidateType, rule.TargetRole, roleMap) && IsTargetCandidate(rule, candidateType));

                if (hasRequiredDependency)
                {
                    continue;
                }

                context.ReportDiagnostic(BricksDiagnosticProperties.Create(
                    Rules.BrickRuleViolationRule,
                    sourceType.Locations.FirstOrDefault() ?? Location.None,
                    FormatViolationMessage(rule, sourceType, rule.TargetRole, "<none>"),
                    violationKind: "RequiredDependencyMissing",
                    ruleId: rule.Id,
                    sourceRole: rule.SourceRole,
                    targetRole: rule.TargetRole,
                    source: sourceType.DisplayName(),
                    target: rule.TargetRole));
            }
        }

        internal static void AnalyzeMemberContracts(
            IReadOnlyList<INamedTypeSymbol> allTypes,
            CompilationAnalysisContext context)
        {
            foreach (var type in allTypes)
            {
                foreach (var contract in CollectMemberContracts(type))
                {
                    switch (contract.Kind)
                    {
                        case BrickMemberContractKind.ExactlyOne:
                            AnalyzeExactlyOneMemberContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.AllRequired:
                            AnalyzeAllRequiredMembersContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.ExactCount:
                            AnalyzeExactMemberCountContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.ExclusiveChoice:
                            AnalyzeExclusiveChoiceContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.MemberRange:
                            AnalyzeMemberRangeContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.ForbiddenMember:
                            AnalyzeForbiddenMemberContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.UniqueNamedMember:
                            AnalyzeUniqueNamedMemberContract(type, contract, context);
                            break;
                        case BrickMemberContractKind.RequiredNamedMembers:
                            AnalyzeRequiredNamedMembersContract(type, contract, context);
                            break;
                    }
                }
            }
        }

        private static void AnalyzeExactlyOneMemberContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var count = CountMarkedMembers(type, contract.PrimaryMarkerType!);
            if (count == 1)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickExactlyOneMemberContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires exactly one member marked with '{contract.PrimaryMarkerType!.DisplayName()}', but '{type.DisplayName()}' declares {count}.",
                "ExactlyOneMember");
        }

        private static void AnalyzeAllRequiredMembersContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var missing = contract.MarkerTypes
                .Where(markerType => CountMarkedMembers(type, markerType) == 0)
                .Select(markerType => markerType.DisplayName())
                .ToArray();

            if (missing.Length == 0)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickRequireAllMembersContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires members marked with all configured marker attributes, but '{type.DisplayName()}' is missing: {string.Join(", ", missing)}.",
                "RequireAllMembers");
        }

        private static void AnalyzeExactMemberCountContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var count = CountMarkedMembers(type, contract.PrimaryMarkerType!);
            if (count == contract.Count)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickMemberCountContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires exactly {contract.Count} members marked with '{contract.PrimaryMarkerType!.DisplayName()}', but '{type.DisplayName()}' declares {count}.",
                "RequireMemberCount");
        }

        private static void AnalyzeExclusiveChoiceContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var leftCount = CountMarkedMembers(type, contract.PrimaryMarkerType!);
            var rightCount = CountMarkedMembers(type, contract.SecondaryMarkerType!);
            var satisfied = (leftCount > 0 && rightCount == 0) || (leftCount == 0 && rightCount > 0);

            if (satisfied)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickExclusiveChoiceContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires exactly one of '{contract.PrimaryMarkerType!.DisplayName()}' or '{contract.SecondaryMarkerType!.DisplayName()}', but '{type.DisplayName()}' declares {leftCount} and {rightCount}.",
                "ExclusiveChoice");
        }

        private static void AnalyzeMemberRangeContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var count = CountMarkedMembers(type, contract.PrimaryMarkerType!);
            if (count >= contract.MinimumCount && count <= contract.MaximumCount)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickMemberRangeContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires between {contract.MinimumCount} and {contract.MaximumCount} members marked with '{contract.PrimaryMarkerType!.DisplayName()}', but '{type.DisplayName()}' declares {count}.",
                "MemberRange");
        }

        private static void AnalyzeForbiddenMemberContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var count = CountMarkedMembers(type, contract.PrimaryMarkerType!);
            if (count == 0)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickForbiddenMemberContractRule,
                $"Brick contract '{contract.ContractAttributeName}' forbids members marked with '{contract.PrimaryMarkerType!.DisplayName()}', but '{type.DisplayName()}' declares {count}.",
                "ForbiddenMember");
        }

        private static void AnalyzeUniqueNamedMemberContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var duplicateNames = CollectMarkerNameObservations(type, contract.PrimaryMarkerType!, contract.NameArgument)
                .GroupBy(observation => observation.Name, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => FormatMarkerName(group.Key))
                .ToArray();

            if (duplicateNames.Length == 0)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickUniqueNamedMemberContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires unique '{contract.NameArgument}' marker names for '{contract.PrimaryMarkerType!.DisplayName()}', but '{type.DisplayName()}' duplicates: {string.Join(", ", duplicateNames)}.",
                "UniqueNamedMember");
        }

        private static void AnalyzeRequiredNamedMembersContract(
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            CompilationAnalysisContext context)
        {
            var observedNames = new HashSet<string>(
                CollectMarkerNameObservations(type, contract.PrimaryMarkerType!, contract.NameArgument)
                    .Select(observation => observation.Name),
                StringComparer.OrdinalIgnoreCase);

            var missingNames = contract.RequiredNames
                .Where(requiredName => !observedNames.Contains(requiredName))
                .Select(FormatMarkerName)
                .ToArray();

            if (missingNames.Length == 0)
            {
                return;
            }

            ReportMemberContractViolation(
                context,
                type,
                contract,
                Rules.BrickRequiredNamedMembersContractRule,
                $"Brick contract '{contract.ContractAttributeName}' requires marker names on '{contract.PrimaryMarkerType!.DisplayName()}' via '{contract.NameArgument}', but '{type.DisplayName()}' is missing: {string.Join(", ", missingNames)}.",
                "RequiredNamedMembers");
        }

        private static void ReportMemberContractViolation(
            CompilationAnalysisContext context,
            INamedTypeSymbol type,
            BrickMemberContractDeclaration contract,
            DiagnosticDescriptor descriptor,
            string message,
            string contractKind)
        {
            context.ReportDiagnostic(BricksDiagnosticProperties.Create(
                descriptor,
                type.Locations.FirstOrDefault() ?? Location.None,
                message,
                violationKind: "MemberContractViolation",
                source: type.DisplayName(),
                target: contract.ContractAttributeName,
                contractKind: contractKind));
        }

        private static IEnumerable<BrickMemberContractDeclaration> CollectMemberContracts(INamedTypeSymbol type)
        {
            foreach (var attributeClass in type.GetAttributes()
                         .Select(attribute => attribute.AttributeClass)
                         .OfType<INamedTypeSymbol>())
            {
                foreach (var contract in CollectMemberContractsFromAttributeClass(attributeClass))
                {
                    yield return contract;
                }
            }
        }

        private static IEnumerable<BrickMemberContractDeclaration> CollectMemberContractsFromAttributeClass(INamedTypeSymbol attributeClass)
        {
            for (var current = attributeClass; current is not null; current = current.BaseType)
            {
                foreach (var attribute in current.GetAttributes())
                {
                    if (IsRequireExactlyOneMemberAttribute(attribute.AttributeClass))
                    {
                        var markerType = GetConstructorType(attribute, 0);
                        if (markerType is not null)
                        {
                            yield return BrickMemberContractDeclaration.ExactlyOne(attributeClass, markerType);
                        }
                    }
                    else if (IsRequireAllMembersAttribute(attribute.AttributeClass))
                    {
                        var markerTypes = GetConstructorTypes(attribute, 0);
                        if (markerTypes.Length > 0)
                        {
                            yield return BrickMemberContractDeclaration.AllRequired(attributeClass, markerTypes);
                        }
                    }
                    else if (IsRequireMemberCountAttribute(attribute.AttributeClass))
                    {
                        var markerType = GetConstructorType(attribute, 0);
                        if (markerType is not null)
                        {
                            yield return BrickMemberContractDeclaration.ExactCount(attributeClass, markerType, GetConstructorInt(attribute, 1));
                        }
                    }
                    else if (IsRequireExclusiveChoiceAttribute(attribute.AttributeClass))
                    {
                        var leftType = GetConstructorType(attribute, 0);
                        var rightType = GetConstructorType(attribute, 1);
                        if (leftType is not null && rightType is not null)
                        {
                            yield return BrickMemberContractDeclaration.ExclusiveChoice(attributeClass, leftType, rightType);
                        }
                    }
                    else if (IsRequireMemberRangeAttribute(attribute.AttributeClass))
                    {
                        var markerType = GetConstructorType(attribute, 0);
                        var minimumCount = GetConstructorInt(attribute, 1);
                        var maximumCount = GetConstructorInt(attribute, 2);
                        if (markerType is not null && minimumCount >= 0 && maximumCount >= minimumCount)
                        {
                            yield return BrickMemberContractDeclaration.MemberRange(attributeClass, markerType, minimumCount, maximumCount);
                        }
                    }
                    else if (IsForbidMemberAttribute(attribute.AttributeClass))
                    {
                        var markerType = GetConstructorType(attribute, 0);
                        if (markerType is not null)
                        {
                            yield return BrickMemberContractDeclaration.ForbiddenMember(attributeClass, markerType);
                        }
                    }
                    else if (IsRequireUniqueNamedMemberAttribute(attribute.AttributeClass))
                    {
                        var markerType = GetConstructorType(attribute, 0);
                        if (markerType is not null)
                        {
                            yield return BrickMemberContractDeclaration.UniqueNamedMember(
                                attributeClass,
                                markerType,
                                GetNameArgument(attribute, 1));
                        }
                    }
                    else if (IsRequireNamedMembersAttribute(attribute.AttributeClass))
                    {
                        var markerType = GetConstructorType(attribute, 0);
                        var requiredNames = GetConstructorNames(attribute, 1);
                        if (markerType is not null && requiredNames.Length > 0)
                        {
                            yield return BrickMemberContractDeclaration.RequiredNamedMembers(
                                attributeClass,
                                markerType,
                                requiredNames,
                                GetNameArgument(attribute, -1));
                        }
                    }
                }
            }
        }

        private static int CountMarkedMembers(INamedTypeSymbol type, ITypeSymbol markerAttributeType)
        {
            return GetMembersInHierarchy(type)
                .Where(IsEligibleContractMember)
                .Count(member => member.GetAttributes().Any(attribute => MatchesMarkerAttribute(attribute.AttributeClass, markerAttributeType)));
        }

        private static IReadOnlyList<MarkerNameObservation> CollectMarkerNameObservations(
            INamedTypeSymbol type,
            ITypeSymbol markerAttributeType,
            string nameArgument)
        {
            return GetMembersInHierarchy(type)
                .Where(IsEligibleContractMember)
                .SelectMany(member => member.GetAttributes()
                    .Where(attribute => MatchesMarkerAttribute(attribute.AttributeClass, markerAttributeType))
                    .Select(attribute => new MarkerNameObservation(member, GetMarkerName(attribute, nameArgument))))
                .ToArray();
        }

        private static string GetMarkerName(AttributeData attribute, string nameArgument)
        {
            var normalizedNameArgument = NormalizeNameArgument(nameArgument);

            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, normalizedNameArgument, StringComparison.Ordinal)
                    && namedArgument.Value.Value is string namedValue)
                {
                    return NormalizeMarkerName(namedValue);
                }
            }

            var parameters = attribute.AttributeConstructor?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty;
            for (var index = 0; index < parameters.Length && index < attribute.ConstructorArguments.Length; index++)
            {
                if (string.Equals(parameters[index].Name, normalizedNameArgument, StringComparison.OrdinalIgnoreCase)
                    && attribute.ConstructorArguments[index].Value is string parameterValue)
                {
                    return NormalizeMarkerName(parameterValue);
                }
            }

            if (string.Equals(normalizedNameArgument, "Name", StringComparison.Ordinal))
            {
                foreach (var constructorArgument in attribute.ConstructorArguments)
                {
                    if (constructorArgument.Value is string constructorValue)
                    {
                        return NormalizeMarkerName(constructorValue);
                    }
                }
            }

            return string.Empty;
        }

        private static string FormatMarkerName(string name) =>
            string.IsNullOrWhiteSpace(name) ? "<unnamed>" : $"'{name}'";

        private static string NormalizeMarkerName(string? value) =>
            value?.Trim() ?? string.Empty;

        private static string NormalizeNameArgument(string? nameArgument)
        {
            var trimmed = nameArgument?.Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? "Name" : trimmed!;
        }

        private static IEnumerable<ISymbol> GetMembersInHierarchy(INamedTypeSymbol type)
        {
            for (var current = type; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers())
                {
                    yield return member;
                }
            }
        }

        private static bool IsEligibleContractMember(ISymbol member)
        {
            if (member is IMethodSymbol method)
            {
                return method.MethodKind is not MethodKind.Constructor
                    and not MethodKind.StaticConstructor
                    and not MethodKind.PropertyGet
                    and not MethodKind.PropertySet
                    and not MethodKind.EventAdd
                    and not MethodKind.EventRemove;
            }

            return member is IFieldSymbol or IPropertySymbol or IEventSymbol;
        }

        private static bool MatchesMarkerAttribute(INamedTypeSymbol? candidateAttributeClass, ITypeSymbol markerAttributeType)
        {
            for (var current = candidateAttributeClass; current is not null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, markerAttributeType))
                {
                    return true;
                }
            }

            return false;
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

                    foreach (var alias in (attribute.AttributeClass?.GetAttributes() ?? Enumerable.Empty<AttributeData>())
                                 .Where(alias => IsBrickRoleAliasAttribute(alias.AttributeClass)))
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
                var location = attribute.ApplicationSyntaxReference?.GetSyntax().GetLocation()
                    ?? symbol.Locations.FirstOrDefault()
                    ?? Location.None;

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

        private static string[] GetConstructorNames(AttributeData attribute, int index)
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

            return argument.Values
                .Where(value => value.Value is string)
                .Select(value => NormalizeMarkerName(value.Value as string))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string GetNameArgument(AttributeData attribute, int constructorIndex)
        {
            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (string.Equals(namedArgument.Key, "NameArgument", StringComparison.Ordinal)
                    && namedArgument.Value.Value is string namedValue)
                {
                    return NormalizeNameArgument(namedValue);
                }
            }

            if (constructorIndex >= 0)
            {
                var constructorValue = GetConstructorString(attribute, constructorIndex);
                if (!string.IsNullOrWhiteSpace(constructorValue))
                {
                    return NormalizeNameArgument(constructorValue);
                }
            }

            return "Name";
        }

        private static ITypeSymbol? GetConstructorType(AttributeData attribute, int index)
        {
            if (attribute.ConstructorArguments.Length <= index)
            {
                return null;
            }

            return attribute.ConstructorArguments[index].Value as ITypeSymbol;
        }

        private static ITypeSymbol[] GetConstructorTypes(AttributeData attribute, int index)
        {
            if (attribute.ConstructorArguments.Length <= index)
            {
                return Array.Empty<ITypeSymbol>();
            }

            var argument = attribute.ConstructorArguments[index];
            if (argument.Kind != TypedConstantKind.Array)
            {
                return Array.Empty<ITypeSymbol>();
            }

            return argument.Values
                .Select(value => value.Value as ITypeSymbol)
                .Where(value => value is not null)
                .Cast<ITypeSymbol>()
                .ToArray();
        }

        private static int GetConstructorInt(AttributeData attribute, int index)
        {
            if (attribute.ConstructorArguments.Length <= index)
            {
                return 0;
            }

            return attribute.ConstructorArguments[index].Value is int value ? value : 0;
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

        private static bool IsRequireExactlyOneMemberAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireExactlyOneMemberAttributeNames);

        private static bool IsRequireAllMembersAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireAllMembersAttributeNames);

        private static bool IsRequireMemberCountAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireMemberCountAttributeNames);

        private static bool IsRequireExclusiveChoiceAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireExclusiveChoiceAttributeNames);

        private static bool IsRequireMemberRangeAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireMemberRangeAttributeNames);

        private static bool IsForbidMemberAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, ForbidMemberAttributeNames);

        private static bool IsRequireUniqueNamedMemberAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireUniqueNamedMemberAttributeNames);

        private static bool IsRequireNamedMembersAttribute(INamedTypeSymbol? attributeClass) =>
            InheritsFromAnyAttribute(attributeClass, RequireNamedMembersAttributeNames);

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

        private static IEnumerable<DependencyObservation> CollectDependencyObservations(
            INamedTypeSymbol sourceType,
            Compilation compilation)
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
                    case IFieldSymbol { IsImplicitlyDeclared: false } field:
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

            foreach (var observation in CollectSyntaxDependencyObservations(sourceType, compilation))
            {
                yield return observation;
            }
        }

        private static IEnumerable<DependencyObservation> CollectSyntaxDependencyObservations(
            INamedTypeSymbol sourceType,
            Compilation compilation)
        {
            foreach (var syntaxReference in sourceType.DeclaringSyntaxReferences)
            {
                if (syntaxReference.GetSyntax() is not TypeDeclarationSyntax typeDeclaration)
                {
                    continue;
                }

                var semanticModel = compilation.GetSemanticModel(typeDeclaration.SyntaxTree);
                var descendants = typeDeclaration.DescendantNodes(node =>
                    ReferenceEquals(node, typeDeclaration) || node is not TypeDeclarationSyntax);

                foreach (var localDeclaration in descendants.OfType<LocalDeclarationStatementSyntax>())
                {
                    var type = semanticModel.GetTypeInfo(localDeclaration.Declaration.Type).Type;
                    if (type is not null)
                    {
                        yield return new DependencyObservation(
                            sourceType,
                            type,
                            localDeclaration.Declaration.Type.GetLocation());
                    }
                }

                foreach (var objectCreation in descendants.OfType<ObjectCreationExpressionSyntax>())
                {
                    var type = semanticModel.GetTypeInfo(objectCreation).Type
                        ?? semanticModel.GetTypeInfo(objectCreation.Type).Type;
                    if (type is not null)
                    {
                        yield return new DependencyObservation(
                            sourceType,
                            type,
                            objectCreation.Type.GetLocation());
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

        private sealed class BrickMemberContractDeclaration
        {
            private BrickMemberContractDeclaration(
                BrickMemberContractKind kind,
                INamedTypeSymbol contractAttributeType,
                ITypeSymbol? primaryMarkerType,
                ITypeSymbol? secondaryMarkerType,
                IReadOnlyList<ITypeSymbol> markerTypes,
                int count,
                int minimumCount,
                int maximumCount,
                IReadOnlyList<string> requiredNames,
                string nameArgument)
            {
                Kind = kind;
                ContractAttributeName = contractAttributeType.DisplayName();
                PrimaryMarkerType = primaryMarkerType;
                SecondaryMarkerType = secondaryMarkerType;
                MarkerTypes = markerTypes;
                Count = count;
                MinimumCount = minimumCount;
                MaximumCount = maximumCount;
                RequiredNames = requiredNames;
                NameArgument = NormalizeNameArgument(nameArgument);
            }

            public BrickMemberContractKind Kind { get; }

            public string ContractAttributeName { get; }

            public ITypeSymbol? PrimaryMarkerType { get; }

            public ITypeSymbol? SecondaryMarkerType { get; }

            public IReadOnlyList<ITypeSymbol> MarkerTypes { get; }

            public int Count { get; }

            public int MinimumCount { get; }

            public int MaximumCount { get; }

            public IReadOnlyList<string> RequiredNames { get; }

            public string NameArgument { get; }

            public static BrickMemberContractDeclaration ExactlyOne(INamedTypeSymbol contractAttributeType, ITypeSymbol markerType) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.ExactlyOne,
                    contractAttributeType,
                    markerType,
                    null,
                    Array.Empty<ITypeSymbol>(),
                    1,
                    0,
                    0,
                    Array.Empty<string>(),
                    "Name");

            public static BrickMemberContractDeclaration AllRequired(INamedTypeSymbol contractAttributeType, IReadOnlyList<ITypeSymbol> markerTypes) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.AllRequired,
                    contractAttributeType,
                    null,
                    null,
                    markerTypes,
                    0,
                    0,
                    0,
                    Array.Empty<string>(),
                    "Name");

            public static BrickMemberContractDeclaration ExactCount(INamedTypeSymbol contractAttributeType, ITypeSymbol markerType, int count) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.ExactCount,
                    contractAttributeType,
                    markerType,
                    null,
                    Array.Empty<ITypeSymbol>(),
                    count,
                    0,
                    0,
                    Array.Empty<string>(),
                    "Name");

            public static BrickMemberContractDeclaration ExclusiveChoice(INamedTypeSymbol contractAttributeType, ITypeSymbol leftType, ITypeSymbol rightType) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.ExclusiveChoice,
                    contractAttributeType,
                    leftType,
                    rightType,
                    Array.Empty<ITypeSymbol>(),
                    0,
                    0,
                    0,
                    Array.Empty<string>(),
                    "Name");

            public static BrickMemberContractDeclaration MemberRange(INamedTypeSymbol contractAttributeType, ITypeSymbol markerType, int minimumCount, int maximumCount) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.MemberRange,
                    contractAttributeType,
                    markerType,
                    null,
                    Array.Empty<ITypeSymbol>(),
                    0,
                    minimumCount,
                    maximumCount,
                    Array.Empty<string>(),
                    "Name");

            public static BrickMemberContractDeclaration ForbiddenMember(INamedTypeSymbol contractAttributeType, ITypeSymbol markerType) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.ForbiddenMember,
                    contractAttributeType,
                    markerType,
                    null,
                    Array.Empty<ITypeSymbol>(),
                    0,
                    0,
                    0,
                    Array.Empty<string>(),
                    "Name");

            public static BrickMemberContractDeclaration UniqueNamedMember(INamedTypeSymbol contractAttributeType, ITypeSymbol markerType, string nameArgument) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.UniqueNamedMember,
                    contractAttributeType,
                    markerType,
                    null,
                    Array.Empty<ITypeSymbol>(),
                    0,
                    0,
                    0,
                    Array.Empty<string>(),
                    nameArgument);

            public static BrickMemberContractDeclaration RequiredNamedMembers(INamedTypeSymbol contractAttributeType, ITypeSymbol markerType, IReadOnlyList<string> requiredNames, string nameArgument) =>
                new BrickMemberContractDeclaration(
                    BrickMemberContractKind.RequiredNamedMembers,
                    contractAttributeType,
                    markerType,
                    null,
                    Array.Empty<ITypeSymbol>(),
                    0,
                    0,
                    0,
                    requiredNames,
                    nameArgument);
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
            }

            public string[] Tokens { get; }
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
            public DependencyObservation(ISymbol memberSymbol, ITypeSymbol dependencyType, Location? location = null)
            {
                MemberSymbol = memberSymbol;
                DependencyType = dependencyType;
                Location = location;
            }

            public ISymbol MemberSymbol { get; }
            public ITypeSymbol DependencyType { get; }
            public Location? Location { get; }
        }

        private sealed class MarkerNameObservation
        {
            public MarkerNameObservation(ISymbol memberSymbol, string name)
            {
                MemberSymbol = memberSymbol;
                Name = name;
            }

            public ISymbol MemberSymbol { get; }

            public string Name { get; }
        }

        private enum BrickRuleMode
        {
            ForbidDependency = 0,
            RequireDependency = 1
        }

        private enum BrickMemberContractKind
        {
            ExactlyOne = 0,
            AllRequired = 1,
            ExactCount = 2,
            ExclusiveChoice = 3,
            MemberRange = 4,
            ForbiddenMember = 5,
            UniqueNamedMember = 6,
            RequiredNamedMembers = 7
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
