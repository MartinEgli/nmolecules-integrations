using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    public static class Rules
    {
        public const string CqrsSupportRequiresQueryAndQueryHandlerId = "XMoleculesCQRS0001";
        public const string CommandHandlersMustNotDependOnQueryModelsId = "XMoleculesCQRS0002";
        public const string QueryModelsMustBeReadOnlyId = "XMoleculesCQRS0004";
        public const string CommandDispatchersMustNotContainDomainRulesId = "XMoleculesCQRS0006";

        public static readonly DiagnosticDescriptor CqrsSupportRequiresQueryAndQueryHandlerRule = new(
            CqrsSupportRequiresQueryAndQueryHandlerId,
            "CQRS support requires both query and query handler markers",
            "{0} '{1}' requires at least one {2} in the same compilation",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "A CQRS read side is only structurally complete when both query request markers and query handler markers are present.");

        public static readonly DiagnosticDescriptor CommandHandlersMustNotDependOnQueryModelsRule = new(
            CommandHandlersMustNotDependOnQueryModelsId,
            "Command handlers must not depend on query models directly",
            "Command handler symbol '{0}' must not depend on query model '{1}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Command handlers belong to the write side and should not couple directly to read-model types.");

        public static readonly DiagnosticDescriptor QueryModelsMustBeReadOnlyRule = new(
            QueryModelsMustBeReadOnlyId,
            "Query models must be read-only",
            "Query model '{0}' must be read-only; member '{1}' is writable",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Query models belong to the read side and should not expose writable state for ad-hoc mutation.");

        public static readonly DiagnosticDescriptor CommandDispatchersMustNotContainDomainRulesRule = new(
            CommandDispatchersMustNotContainDomainRulesId,
            "Command dispatchers must route and must not contain domain rules",
            "Command dispatcher symbol '{0}' must not depend on {1} '{2}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            "Command dispatchers should only route commands and must not couple directly to domain building blocks or orchestration roles.");
    }
}
