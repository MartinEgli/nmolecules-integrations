namespace NMolecules.Analyzers
{
    internal static class DiagnosticDescriptions
    {
        public static string Create(string whyItHappens, string violatedRule, string targetSolution)
            => $"Why it happens: {whyItHappens} Violated rule: {violatedRule} Target solution: {targetSolution}";
    }
}
