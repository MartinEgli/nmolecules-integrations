using Microsoft.CodeAnalysis;

namespace NMolecules.Analyzers.CqrsAnalyzers
{
    public static class Rules
    {
        public const string CommandHandlersMustNotDependOnQueryModelsId = "XMoleculesCQRS0002";
        public const string QueryModelsMustBeReadOnlyId = "XMoleculesCQRS0004";

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
    }
}
