using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    public static class Rules
    {
        public const string CqrsSupportRequiresQueryAndQueryHandlerId = "XMoleculesCQRS0001";
        public const string CommandHandlersMustNotDependOnQueryModelsId = "XMoleculesCQRS0002";
        public const string QueryHandlersMustStayOnReadSideId = "XMoleculesCQRS0003";
        public const string QueryModelsMustBeReadOnlyId = "XMoleculesCQRS0004";
        public const string ProjectionsMustNotDependOnWriteSideRolesId = "XMoleculesCQRS0005";
        public const string CommandDispatchersMustNotContainDomainRulesId = "XMoleculesCQRS0006";

        public static readonly DiagnosticDescriptor CqrsSupportRequiresQueryAndQueryHandlerRule = new(
            CqrsSupportRequiresQueryAndQueryHandlerId,
            "CQRS queries and query handlers must coexist in the same compilation",
            "{0} '{1}' requires at least one {2} in the same compilation",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The compilation declares only one side of the CQRS query pair: queries without handlers or handlers without queries.",
                "A CQRS read side is structurally complete only when query requests and query handlers coexist.",
                "Add the missing query or query-handler markers in the same compilation, or remove the incomplete CQRS read-side markers."));

        public static readonly DiagnosticDescriptor CommandHandlersMustNotDependOnQueryModelsRule = new(
            CommandHandlersMustNotDependOnQueryModelsId,
            "Command handlers must not depend on query models directly",
            "Command handler '{0}' must not depend on query model '{1}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A command handler directly consumes or stores a query model from the read side.",
                "CQRS separates write-side command handling from read-side projection models.",
                "Use domain entities, aggregates, commands, or dedicated contracts on the write side and let projections update query models separately."));

        public static readonly DiagnosticDescriptor QueryHandlersMustStayOnReadSideRule = new(
            QueryHandlersMustStayOnReadSideId,
            "Query handlers must stay on the read side",
            "Query handler '{0}' must not depend on {1} '{2}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A query handler directly depends on a write-side domain role such as an entity, aggregate root, repository, or command-side component.",
                "Read-side query handling should stay isolated from write-side domain behavior in CQRS.",
                "Query a read model, projection store, or read-oriented port instead of traversing write-side building blocks directly."));

        public static readonly DiagnosticDescriptor QueryModelsMustBeReadOnlyRule = new(
            QueryModelsMustBeReadOnlyId,
            "Query models must be read-only",
            "Query model '{0}' must be read-only; member '{1}' is writable",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The query model exposes writable members that allow ad-hoc mutation after publication.",
                "CQRS query models are read-side representations and should be treated as read-only projections.",
                "Make the query model immutable or expose only initialization paths controlled by projection updates."));

        public static readonly DiagnosticDescriptor ProjectionsMustNotDependOnWriteSideRolesRule = new(
            ProjectionsMustNotDependOnWriteSideRolesId,
            "Projections may update query models but must not depend on write-side roles directly",
            "Projection '{0}' must not depend on {1} '{2}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "A projection directly depends on write-side roles instead of staying on the read-model update path.",
                "CQRS projections should translate events into read models without coupling back to write-side domain components.",
                "Feed the projection with events or explicit projection inputs and keep write-side model access out of the projection."));

        public static readonly DiagnosticDescriptor CommandDispatchersMustNotContainDomainRulesRule = new(
            CommandDispatchersMustNotContainDomainRulesId,
            "Command dispatchers must route and must not contain domain rules",
            "Command dispatcher '{0}' must not depend on {1} '{2}' directly",
            Category.Architecture,
            DiagnosticSeverity.Error,
            true,
            DiagnosticDescriptions.Create(
                "The command dispatcher performs direct domain collaboration instead of only routing commands.",
                "Command dispatchers are transport and routing components, not holders of domain rules or orchestration logic.",
                "Move domain behavior into handlers or domain services and keep the dispatcher limited to command routing concerns."));
    }
}
