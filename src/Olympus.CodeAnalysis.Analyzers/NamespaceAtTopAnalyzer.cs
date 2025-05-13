namespace Olympus.CodeAnalysis.Analyzers;

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

/// <summary>
/// Analyzer that enforces the Olympus code style rule that namespace declarations should come at the top of the file.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamespaceAtTopAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "OLY0001";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NamespaceAtTopAnalyzerTitle),
        Resources.ResourceManager,
        typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.NamespaceAtTopAnalyzerMessageFormat),
        Resources.ResourceManager,
        typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.NamespaceAtTopAnalyzerDescription),
        Resources.ResourceManager,
        typeof(Resources));

    private const string Category = "Convention";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);

        var namespaceDeclarations = root.DescendantNodes()
            .OfType<BaseNamespaceDeclarationSyntax>() // Handles both regular and file-scoped namespaces
            .ToList();

        if (!namespaceDeclarations.Any())
            return;

        var usingDirectives = root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Where(u => u.Parent is CompilationUnitSyntax)
            .ToList();

        if (!usingDirectives.Any())
            return;

        var firstNamespace = namespaceDeclarations.First();

        foreach (var usingDirective in usingDirectives)
        {
            if (usingDirective.SpanStart > firstNamespace.SpanStart)
            {
                var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
