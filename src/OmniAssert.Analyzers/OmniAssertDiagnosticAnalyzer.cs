using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OmniAssert.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OmniAssertDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public const string LegacyAssertId = "OA001";
    public const string LegacyVerifyId = "OA002";
    public const string LegacyFluentGrammarId = "OA003";
    public const string LegacyVerifyExpressionId = "OA004";
    public const string ComparisonInBoolAssertId = "OA005";
    public const string HaveCountZeroId = "OA006";
    public const string RedundantNotNullAfterNewId = "OA007";

    private static readonly LocalizableString Title = "OmniAssert legacy API usage";
    private static readonly LocalizableString MessageFormat = "{0}";
    private static readonly LocalizableString Description = "Migrate to Ensure, Must(), Be*, and Expression syntax.";

    private static readonly DiagnosticDescriptor LegacyAssertRule = new(
        LegacyAssertId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor LegacyVerifyRule = new(
        LegacyVerifyId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor LegacyFluentGrammarRule = new(
        LegacyFluentGrammarId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor LegacyVerifyExpressionRule = new(
        LegacyVerifyExpressionId, Title, MessageFormat, "Usage", DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    private static readonly DiagnosticDescriptor ComparisonInBoolAssertRule = new(
        ComparisonInBoolAssertId,
        "Prefer subject-first comparison assertions",
        "{0}",
        "Style",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Rewrite (a < b).Must().BeTrue() as a.Must().BeLessThan(b) for clearer failures.");

    private static readonly DiagnosticDescriptor HaveCountZeroRule = new(
        HaveCountZeroId,
        "Prefer BeEmpty() over HaveCount(0)",
        "{0}",
        "Style",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Use BeEmpty() instead of HaveCount(0) for collections and spans.");

    private static readonly DiagnosticDescriptor RedundantNotNullAfterNewRule = new(
        RedundantNotNullAfterNewId,
        "Redundant NotBeNull after new",
        "{0}",
        "Style",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "A 'new' expression can never be null, so NotBeNull() adds no value.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        LegacyAssertRule,
        LegacyVerifyRule,
        LegacyFluentGrammarRule,
        LegacyVerifyExpressionRule,
        ComparisonInBoolAssertRule,
        HaveCountZeroRule,
        RedundantNotNullAfterNewRule
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeIdentifier, SyntaxKind.IdentifierName);
        context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MemberAccessExpressionSyntax memberAccess)
            return;

        if (memberAccess.Expression is IdentifierNameSyntax { Identifier.ValueText: "Assert" })
        {
            var assertSymbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
            if (assertSymbol?.ContainingType.Name == "Assert" && IsOmniAssertNamespace(assertSymbol.ContainingNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LegacyAssertRule,
                    memberAccess.Name.GetLocation(),
                    $"Use Ensure.{memberAccess.Name.Identifier.ValueText} instead of legacy Assert.{memberAccess.Name.Identifier.ValueText}."));
                if (memberAccess.Name.Identifier.ValueText != "VerifyExpression")
                    return;
            }
        }

        var name = memberAccess.Name.Identifier.ValueText;

        if (name == "VerifyExpression")
        {
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
            if (IsOmniAssertVerifyExpressionMethod(symbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LegacyVerifyExpressionRule,
                    memberAccess.Name.GetLocation(),
                    "Use Ensure.Expression(...) instead of the legacy VerifyExpression()."));
            }

            return;
        }

        if (name == "Verify")
        {
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol;
            if (symbol is IMethodSymbol { Name: "Verify", ContainingType.Name: "Assert" or "Ensure" } method &&
                IsOmniAssertNamespace(method.ContainingNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    LegacyVerifyRule,
                    memberAccess.Name.GetLocation(),
                    "Use Must() instead of the legacy Verify() fluent entry point."));
            }

            return;
        }

        if (!name.StartsWith("To", StringComparison.Ordinal) &&
            !name.StartsWith("NotTo", StringComparison.Ordinal))
        {
            return;
        }

        if (!IsLegacyFluentAssertionName(name))
            return;

        if (!IsChainedAfterOmniAssertFluentRoot(memberAccess, context.SemanticModel, context.CancellationToken))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            LegacyFluentGrammarRule,
            memberAccess.Name.GetLocation(),
            $"Use {SuggestNewFluentName(name)}() instead of legacy {name}()."));
    }

    private static bool IsLegacyFluentAssertionName(string name) =>
        name is "ToBeTrue" or "ToBeFalse" or "ToBe" or "ToBeNull" or "ToNotBeNull" or "NotToBeNull" or "NotToBe" or "ToBeEmpty" or "NotToBeEmpty"
        || name.StartsWith("ToBe", StringComparison.Ordinal)
        || name.StartsWith("NotToBe", StringComparison.Ordinal)
        || name.StartsWith("ToHave", StringComparison.Ordinal)
        || name.StartsWith("ToContain", StringComparison.Ordinal)
        || name.StartsWith("ToMatch", StringComparison.Ordinal)
        || name.StartsWith("ToStart", StringComparison.Ordinal)
        || name.StartsWith("ToEnd", StringComparison.Ordinal)
        || name.StartsWith("ToEqual", StringComparison.Ordinal)
        || name.StartsWith("NotToEqual", StringComparison.Ordinal)
        || name.StartsWith("NotToEnd", StringComparison.Ordinal);

    internal static string SuggestNewFluentName(string legacyName)
    {
        if (legacyName.StartsWith("NotTo", StringComparison.Ordinal))
            return "Not" + legacyName.Substring("NotTo".Length);

        if (legacyName.StartsWith("To", StringComparison.Ordinal))
            return legacyName.Substring("To".Length);

        return legacyName;
    }

    private static bool IsChainedAfterOmniAssertFluentRoot(
        MemberAccessExpressionSyntax memberAccess,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        for (ExpressionSyntax? current = memberAccess.Expression; current is not null; current = current switch
        {
            MemberAccessExpressionSyntax ma => ma.Expression,
            InvocationExpressionSyntax inv when inv.Expression is ExpressionSyntax expr => expr,
            ParenthesizedExpressionSyntax p => p.Expression,
            _ => null
        })
        {
            if (current is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax invokedMember)
            {
                var methodName = invokedMember.Name.Identifier.ValueText;
                if (methodName is "Verify" or "Must" or "VerifyNullable")
                {
                    var symbol = semanticModel.GetSymbolInfo(invokedMember, cancellationToken).Symbol;
                    if (symbol is IMethodSymbol method && IsOmniAssertNamespace(method.ContainingNamespace))
                        return true;
                }
            }

            if (current is MemberAccessExpressionSyntax member &&
                member.Name.Identifier.ValueText is "FileExists" or "DirectoryExists")
            {
                var symbol = semanticModel.GetSymbolInfo(member, cancellationToken).Symbol;
                if (symbol is IMethodSymbol method && IsOmniAssertNamespace(method.ContainingNamespace))
                    return true;
            }
        }

        return false;
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
            return;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
            return;

        var methodName = memberAccess.Name.Identifier.ValueText;

        if (methodName is "BeTrue" or "BeFalse")
        {
            AnalyzeComparisonInBoolAssert(context, invocation, memberAccess, methodName);
            return;
        }

        if (methodName == "HaveCount")
        {
            AnalyzeHaveCountZero(context, invocation, memberAccess);
            return;
        }

        if (methodName is "NotBeNull" or "NotToBeNull")
        {
            AnalyzeRedundantNotNullAfterNew(context, invocation, memberAccess, methodName);
        }
    }

    private static void AnalyzeComparisonInBoolAssert(
        SyntaxNodeAnalysisContext context,
        InvocationExpressionSyntax invocation,
        MemberAccessExpressionSyntax memberAccess,
        string methodName)
    {
        var mustInvocation = GetFluentRootInvocation(invocation);
        if (mustInvocation is null)
            return;

        var mustMember = mustInvocation.Expression as MemberAccessExpressionSyntax;
        if (mustMember?.Name.Identifier.ValueText is not ("Must" or "Verify"))
            return;

        if (!IsOmniAssertMustSymbol(context.SemanticModel.GetSymbolInfo(mustMember, context.CancellationToken).Symbol))
            return;

        var receiver = UnwrapParenthesized(mustMember.Expression);
        if (receiver is not BinaryExpressionSyntax binary)
            return;

        if (!TryGetComparisonMapping(binary.Kind(), methodName == "BeTrue", out var targetMethod))
            return;

        if (!IsValidFluentSubject(binary.Left) || IsLiteral(binary.Left))
            return;

        var leftType = context.SemanticModel.GetTypeInfo(binary.Left, context.CancellationToken).Type;
        var rightType = context.SemanticModel.GetTypeInfo(binary.Right, context.CancellationToken).Type;
        var subjectReturnType = leftType is null ? null : ResolveMustReturnType(context.Compilation, leftType);
        var fixable = false;
        if (subjectReturnType is not null && rightType is not null &&
            subjectReturnType.Name is "NumericAssertions" or "TimeSpanAssertions")
        {
            fixable = HasCompatibleInstanceMethod(subjectReturnType, targetMethod, rightType, context.Compilation);
        }

        var awkwardText = invocation.ToString();
        string message;
        ImmutableDictionary<string, string?> properties;
        if (fixable)
        {
            var leftText = binary.Left.ToString();
            var rightText = binary.Right.ToString();
            var suggested = $"{leftText}.Must().{targetMethod}({rightText})";
            message = $"Prefer {suggested} over {awkwardText}.";
            properties = ToProperties(("assertionMethod", targetMethod), ("ruleId", ComparisonInBoolAssertId));
        }
        else
        {
            message = $"Prefer Ensure.Expression({awkwardText}) for multi-operand boolean assertions; subject-first comparison assertions are only available for numeric/TimeSpan subjects.";
            properties = ToProperties(("ruleId", ComparisonInBoolAssertId));
        }

        context.ReportDiagnostic(Diagnostic.Create(
            ComparisonInBoolAssertRule,
            invocation.GetLocation(),
            additionalLocations: null,
            properties,
            message));
    }

    private static void AnalyzeHaveCountZero(
        SyntaxNodeAnalysisContext context,
        InvocationExpressionSyntax invocation,
        MemberAccessExpressionSyntax memberAccess)
    {
        if (invocation.ArgumentList?.Arguments.Count != 1)
            return;

        var arg = invocation.ArgumentList.Arguments[0].Expression;
        if (arg is not LiteralExpressionSyntax literal)
            return;

        if (!literal.Token.IsKind(SyntaxKind.NumericLiteralToken))
            return;
        if (!int.TryParse(literal.Token.ValueText, out var value) || value != 0)
            return;

        var mustInvocation = GetFluentRootInvocation(invocation);
        if (mustInvocation is null)
            return;

        if (mustInvocation.Expression is not MemberAccessExpressionSyntax mustMember)
            return;

        if (mustMember.Name.Identifier.ValueText != "Must")
            return;

        if (!IsOmniAssertMustSymbol(context.SemanticModel.GetSymbolInfo(mustMember, context.CancellationToken).Symbol))
            return;

        var mustReturnType = (context.SemanticModel.GetSymbolInfo(mustMember, context.CancellationToken).Symbol as IMethodSymbol)?.ReturnType;
        if (mustReturnType is null || !HasInstanceMethod(mustReturnType, "BeEmpty"))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            HaveCountZeroRule,
            invocation.GetLocation(),
            additionalLocations: null,
            ToProperties(("ruleId", HaveCountZeroId)),
            $"Prefer {memberAccess.Expression}.BeEmpty() over {invocation.ToString()}."));
    }

    private static void AnalyzeRedundantNotNullAfterNew(
        SyntaxNodeAnalysisContext context,
        InvocationExpressionSyntax invocation,
        MemberAccessExpressionSyntax memberAccess,
        string methodName)
    {
        var mustInvocation = GetFluentRootInvocation(invocation);
        if (mustInvocation is null)
            return;

        if (mustInvocation.Expression is not MemberAccessExpressionSyntax mustMember)
            return;

        if (mustMember.Name.Identifier.ValueText is not ("Must" or "Verify"))
            return;

        if (!IsOmniAssertMustSymbol(context.SemanticModel.GetSymbolInfo(mustMember, context.CancellationToken).Symbol))
            return;

        var receiver = UnwrapParenthesized(mustMember.Expression);
        if (receiver is not ObjectCreationExpressionSyntax)
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            RedundantNotNullAfterNewRule,
            invocation.GetLocation(),
            additionalLocations: null,
            ToProperties(("ruleId", RedundantNotNullAfterNewId)),
            $"{invocation.ToString()} is redundant: a 'new' expression can never be null."));
    }

    private static InvocationExpressionSyntax? GetFluentRootInvocation(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is not MemberAccessExpressionSyntax outerMember)
            return null;
        if (outerMember.Expression is not InvocationExpressionSyntax mustInvocation)
            return null;
        return mustInvocation;
    }

    private static ExpressionSyntax UnwrapParenthesized(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
            expression = parenthesized.Expression;
        return expression;
    }

    private static bool IsOmniAssertMustSymbol(ISymbol? symbol)
    {
        if (symbol is not IMethodSymbol method)
            return false;
        if (method.Name is not ("Must" or "Verify"))
            return false;
        return IsOmniAssertNamespace(method.ContainingNamespace);
    }

    private static bool IsValidFluentSubject(ExpressionSyntax expression)
    {
        return expression switch
        {
            IdentifierNameSyntax => true,
            MemberAccessExpressionSyntax ma => IsValidFluentSubject(ma.Expression),
            ElementAccessExpressionSyntax ea => IsValidFluentSubject(ea.Expression),
            ParenthesizedExpressionSyntax p => IsValidFluentSubject(p.Expression),
            _ => false
        };
    }

    private static bool IsLiteral(ExpressionSyntax expression) =>
        expression is LiteralExpressionSyntax;

    private static ITypeSymbol? ResolveMustReturnType(Compilation compilation, ITypeSymbol subjectType)
    {
        var ensureType = compilation.GetTypeByMetadataName("OmniAssert.Ensure");
        if (ensureType is null)
            return null;

        foreach (var member in ensureType.GetMembers("Must"))
        {
            if (member is not IMethodSymbol method || !method.IsExtensionMethod)
                continue;
            if (method.Parameters.Length < 1)
                continue;
            var receiverType = method.Parameters[0].Type;
            if (receiverType is null)
                continue;
            var conversion = compilation.ClassifyConversion(subjectType, receiverType);
            if (conversion.IsImplicit)
                return method.ReturnType;
        }

        return null;
    }

    internal static bool TryGetComparisonMapping(SyntaxKind binaryKind, bool isBeTrue, out string methodName)
    {
        methodName = (binaryKind, isBeTrue) switch
        {
            (SyntaxKind.LessThanExpression, true) => "BeLessThan",
            (SyntaxKind.LessThanExpression, false) => "BeGreaterThanOrEqualTo",
            (SyntaxKind.LessThanOrEqualExpression, true) => "BeLessThanOrEqualTo",
            (SyntaxKind.LessThanOrEqualExpression, false) => "BeGreaterThan",
            (SyntaxKind.GreaterThanExpression, true) => "BeGreaterThan",
            (SyntaxKind.GreaterThanExpression, false) => "BeLessThanOrEqualTo",
            (SyntaxKind.GreaterThanOrEqualExpression, true) => "BeGreaterThanOrEqualTo",
            (SyntaxKind.GreaterThanOrEqualExpression, false) => "BeLessThan",
            (SyntaxKind.EqualsExpression, true) => "Be",
            (SyntaxKind.EqualsExpression, false) => "NotBe",
            (SyntaxKind.NotEqualsExpression, true) => "NotBe",
            (SyntaxKind.NotEqualsExpression, false) => "Be",
            _ => string.Empty
        };

        return methodName.Length > 0;
    }

    private static bool HasInstanceMethod(ITypeSymbol type, string name)
    {
        foreach (var member in type.GetMembers(name))
        {
            if (member is IMethodSymbol { IsStatic: false })
                return true;
        }
        return false;
    }

    private static bool HasCompatibleInstanceMethod(ITypeSymbol type, string name, ITypeSymbol argType, Compilation compilation)
    {
        foreach (var member in type.GetMembers(name))
        {
            if (member is not IMethodSymbol { IsStatic: false } m)
                continue;
            if (m.Parameters.Length == 0)
                continue;
            var paramType = m.Parameters[0].Type;
            var conversion = compilation.ClassifyConversion(argType, paramType);
            if (conversion.IsImplicit)
                return true;
        }
        return false;
    }

    private static ImmutableDictionary<string, string?> ToProperties(params (string Key, string Value)[] pairs)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string?>();
        foreach (var (key, value) in pairs)
            builder.Add(key, value);
        return builder.ToImmutable();
    }

    private static void AnalyzeIdentifier(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not IdentifierNameSyntax identifier)
            return;

        if (identifier.Identifier.ValueText != "Assert")
            return;

        var symbol = context.SemanticModel.GetSymbolInfo(identifier, context.CancellationToken).Symbol;
        if (symbol is not INamedTypeSymbol typeSymbol)
            return;

        if (!IsOmniAssertAssert(typeSymbol))
            return;

        context.ReportDiagnostic(Diagnostic.Create(
            LegacyAssertRule,
            identifier.GetLocation(),
            "Use Ensure instead of the legacy Assert entry point."));
    }

    private static bool IsOmniAssertAssert(INamedTypeSymbol typeSymbol) =>
        typeSymbol.Name == "Assert" && IsOmniAssertNamespace(typeSymbol.ContainingNamespace);

    private static bool IsOmniAssertNamespace(INamespaceSymbol ns) =>
        ns.ToDisplayString() == "OmniAssert";

    private static bool IsOmniAssertVerifyExpressionMethod(ISymbol? symbol)
    {
        if (symbol is not IMethodSymbol method || method.Name != "VerifyExpression")
            return false;

        if (method.ContainingType.Name is not ("Assert" or "Ensure") || !IsOmniAssertNamespace(method.ContainingNamespace))
            return false;

        var original = method.ReducedFrom ?? method;
        return original.Parameters.Length >= 1 &&
               original.Parameters[0].Type.SpecialType == SpecialType.System_Boolean;
    }
}
