namespace Olympus.CodeAnalysis.Analyzers;

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
using Microsoft.CodeAnalysis.Editing;

/// <summary>
/// Provides a code fix for the NamespaceAtTopAnalyzer diagnostic.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamespaceAtTopCodeFixProvider)), Shared]
public class NamespaceAtTopCodeFixProvider : CodeFixProvider
{
  public sealed override ImmutableArray<string> FixableDiagnosticIds =>
      ImmutableArray.Create(NamespaceAtTopAnalyzer.DiagnosticId);

  public sealed override FixAllProvider GetFixAllProvider() =>
      WellKnownFixAllProviders.BatchFixer;

  public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
  {
    var root = await context.Document
        .GetSyntaxRootAsync(context.CancellationToken)
        .ConfigureAwait(false);

    var diagnostic = context.Diagnostics.First();
    var diagnosticSpan = diagnostic.Location.SourceSpan;        // Find the using directive identified by the diagnostic
    var token = root.FindToken(diagnosticSpan.Start);
    if (token.Parent == null)
      return;

    var usingDirective = token.Parent
        .AncestorsAndSelf()
        .OfType<UsingDirectiveSyntax>()
        .FirstOrDefault();

    if (usingDirective == null)
      return;

    // Register a code action that will move the using directive before the namespace
    context.RegisterCodeFix(
        CodeAction.Create(
            title: "Move using directive before namespace",
            createChangedDocument: c => MoveUsingDirectiveAsync(context.Document, usingDirective, c),
            equivalenceKey: nameof(CodeFixProvider)),
        diagnostic);
  }

  private async Task<Document> MoveUsingDirectiveAsync(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
  {
    var documentEditor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
    var root = documentEditor.OriginalRoot;
    var compilationUnit = (CompilationUnitSyntax)root;

    // Find the namespaces
    var namespaces = compilationUnit.DescendantNodes()
        .OfType<BaseNamespaceDeclarationSyntax>()
        .ToList();

    if (!namespaces.Any())
      return document;

    var firstNamespace = namespaces.First();

    // Get all usings that are currently before the namespace
    var existingUsings = compilationUnit.Usings.Where(u => u.SpanStart < firstNamespace.SpanStart).ToList();

    // Remove the problematic using directive
    documentEditor.RemoveNode(usingDirective);

    // Create a new list of usings with our directive inserted at the end of existing usings
    var newUsings = existingUsings.ToList();
    newUsings.Add(usingDirective);

    // Create a new compilation unit with the updated usings
    var newCompilationUnit = compilationUnit.WithUsings(SyntaxFactory.List(newUsings));

    // Replace the root
    documentEditor.ReplaceNode(compilationUnit, newCompilationUnit);

    return documentEditor.GetChangedDocument();
  }
}
