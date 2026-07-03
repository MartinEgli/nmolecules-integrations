using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NMolecules.Analyzers.BricksAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class BricksMemberContractAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            Rules.BrickExactlyOneMemberContractRule,
            Rules.BrickRequireAllMembersContractRule,
            Rules.BrickMemberCountContractRule,
            Rules.BrickExclusiveChoiceContractRule,
            Rules.BrickMemberRangeContractRule,
            Rules.BrickForbiddenMemberContractRule,
            Rules.BrickUniqueNamedMemberContractRule,
            Rules.BrickRequiredNamedMembersContractRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationAction(AnalyzeCompilation);
        }

        private static void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var allTypes = BricksAnalyzerInternals.GetAllTypesInCompilation(context.Compilation);
            BricksAnalyzerInternals.AnalyzeMemberContracts(allTypes, context);
        }
    }
}
