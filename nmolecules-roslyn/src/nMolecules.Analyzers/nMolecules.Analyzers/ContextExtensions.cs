using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers
{
    public static class ContextExtensions
    {
        public static void ReportDiagnostics(this SymbolAnalysisContext context, IEnumerable<Diagnostic> violations)
        {
            var seen = new HashSet<string>();

            foreach (var violation in violations)
            {
                if (violation is null)
                {
                    continue;
                }

                if (seen.Add(CreateDeduplicationKey(violation)))
                {
                    context.ReportDiagnostic(violation);
                }
            }
        }

        public static void ReportDiagnostics(this SyntaxNodeAnalysisContext context, IEnumerable<Diagnostic> violations)
        {
            var seen = new HashSet<string>();

            foreach (var violation in violations)
            {
                if (violation is null)
                {
                    continue;
                }

                if (seen.Add(CreateDeduplicationKey(violation)))
                {
                    context.ReportDiagnostic(violation);
                }
            }
        }

        private static string CreateDeduplicationKey(Diagnostic diagnostic)
        {
            var location = diagnostic.Location;
            var hasSourceSpan = location != Location.None && location.IsInSource;
            var start = hasSourceSpan ? location.SourceSpan.Start : -1;
            var length = hasSourceSpan ? location.SourceSpan.Length : -1;
            var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);

            return $"{diagnostic.Id}|{start}|{length}|{message}";
        }
    }
}
