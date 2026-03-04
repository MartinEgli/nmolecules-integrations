using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NMolecules.Analyzers.ServiceAnalyzers;

namespace NMolecules.Analyzers.ServiceCodeFixProvider
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LegacyServiceRoleCodeFixProvider))]
    [Shared]
    public class LegacyServiceRoleCodeFixProvider : CodeFixProvider
    {
        private const string DomainServiceTitle = "Replace [Service] with [DomainService]";
        private const string ApplicationServiceTitle = "Replace [Service] with [ApplicationService]";

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(Rules.LegacyServicesShouldUseSpecificRoleId);

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First(it => it.Id.Equals(Rules.LegacyServicesShouldUseSpecificRoleId));
            var typeDeclaration = root.FindToken(diagnostic.Location.SourceSpan.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<TypeDeclarationSyntax>()
                .FirstOrDefault();

            if (typeDeclaration is null)
            {
                return;
            }

            var serviceAttribute = typeDeclaration.AttributeLists
                .SelectMany(it => it.Attributes)
                .FirstOrDefault(IsLegacyServiceAttribute);

            if (serviceAttribute is null)
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    DomainServiceTitle,
                    it => ReplaceLegacyServiceAttribute(context.Document, root, serviceAttribute, "DomainService", it),
                    DomainServiceTitle),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    ApplicationServiceTitle,
                    it => ReplaceLegacyServiceAttribute(context.Document, root, serviceAttribute, "ApplicationService", it),
                    ApplicationServiceTitle),
                diagnostic);
        }

        private static Task<Document> ReplaceLegacyServiceAttribute(
            Document document,
            SyntaxNode root,
            AttributeSyntax attribute,
            string replacementName,
            CancellationToken cancellationToken)
        {
            var replacement = attribute.WithName(ReplaceAttributeName(attribute.Name, replacementName));
            var newRoot = root.ReplaceNode(attribute, replacement);
            return Task.FromResult(document.WithSyntaxRoot(newRoot));
        }

        private static bool IsLegacyServiceAttribute(AttributeSyntax attribute)
        {
            return attribute.Name switch
            {
                IdentifierNameSyntax identifier => identifier.Identifier.ValueText is "Service" or "ServiceAttribute",
                QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText is "Service" or "ServiceAttribute",
                AliasQualifiedNameSyntax aliasQualified => aliasQualified.Name.Identifier.ValueText is "Service" or "ServiceAttribute",
                _ => false
            };
        }

        private static NameSyntax ReplaceAttributeName(NameSyntax originalName, string replacementName)
        {
            return originalName switch
            {
                IdentifierNameSyntax _ => SyntaxFactory.IdentifierName(replacementName),
                QualifiedNameSyntax qualified => qualified.WithRight(SyntaxFactory.IdentifierName(replacementName)),
                AliasQualifiedNameSyntax aliasQualified => aliasQualified.WithName(SyntaxFactory.IdentifierName(replacementName)),
                _ => SyntaxFactory.IdentifierName(replacementName)
            };
        }
    }
}
