using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniAssert.Generator.Rewrite;

/// <summary>
/// Lowers a boolean tree into temporaries, fills a dictionary of operand values, and emits a final
/// <see cref="Assert.VerifyExpression(bool, AssertionCapture)"/> call. Intended for tests and external tooling that compile the lowered block in
/// the original lexical scope.
/// </summary>
/// <remarks>
/// Emitted C# interceptors do not use this lowering for arbitrary expressions that reference caller locals, because
/// arguments are evaluated before interception.
/// </remarks>
internal sealed class VerifyExpansionEngine(SemanticModel model)
{
    private int _tempId;
    private readonly List<string> _captureKeys = [];

    /// <summary>Builds a statement block that evaluates the first argument as <c>bool</c> and calls <see cref="Assert.VerifyExpression(bool, AssertionCapture)"/>.</summary>
    /// <param name="invocation">Call whose first argument is a boolean expression (e.g. <c>VerifyExpression(x &gt; y)</c>).</param>
    /// <param name="cancellationToken">Cancellation token for semantic analysis.</param>
    /// <returns>A block of statements, or <c>null</c> if the expression cannot be lowered.</returns>
    public BlockSyntax? TryExpandVerifyInvocation(InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        if (invocation.ArgumentList.Arguments.Count < 1)
            return null;

        var firstArg = invocation.ArgumentList.Arguments[0].Expression;
        var typeInfo = model.GetTypeInfo(firstArg, cancellationToken);
        if (typeInfo.Type?.SpecialType != SpecialType.System_Boolean)
            return null;

        var exprText = firstArg.ToString();
        _captureKeys.Clear();
        _tempId = 0;

        var dictVar = Identifier($"__oa_cap_{_tempId++}");
        if (!TryExpandBooleanExpression(firstArg, cancellationToken, dictVar, out var stmts, out var resultExpr))
            return null;

        var dictDecl = LocalDeclarationStatement(
            VariableDeclaration(ParseTypeName("global::System.Collections.Generic.Dictionary<string, object?>"))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(dictVar)
                        .WithInitializer(EqualsValueClause(
                            ObjectCreationExpression(ParseTypeName("global::System.Collections.Generic.Dictionary<string, object?>"))
                                .WithArgumentList(ArgumentList()))))));

        var captureExpr = ObjectCreationExpression(ParseTypeName("global::OmniAssert.AssertionCapture"))
            .WithArgumentList(
                ArgumentList(SeparatedList([
                    Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(exprText))),
                    Argument(IdentifierName(dictVar))
                ])));

        var verifyCall = InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ParseTypeName("global::OmniAssert.Assert"),
                    IdentifierName("VerifyExpression")))
            .WithArgumentList(
                ArgumentList(SeparatedList([
                    Argument(resultExpr),
                    Argument(captureExpr)
                ])));

        var allStmts = new List<StatementSyntax>
        {
            ParseStatement("#line hidden"),
            dictDecl
        };
        allStmts.AddRange(stmts);
        allStmts.Add(ExpressionStatement(verifyCall));
        allStmts.Add(ParseStatement("#line default"));

        return Block(allStmts);
    }

    /// <summary>Same lowering as <see cref="TryExpandVerifyInvocation"/>; reserved for scenarios that need a method body block.</summary>
    public BlockSyntax? TryBuildInterceptorBody(InvocationExpressionSyntax invocation, CancellationToken cancellationToken) =>
        TryExpandVerifyInvocation(invocation, cancellationToken);

    /// <summary><c>true</c> when <paramref name="expr"/> is a single identifier (after stripping parentheses).</summary>
    internal bool IsSimpleBooleanIdentifier(ExpressionSyntax expr)
    {
        expr = expr.SkipParentheses();
        return expr is IdentifierNameSyntax;
    }

    private bool TryExpandBooleanExpression(
        ExpressionSyntax expr,
        CancellationToken cancellationToken,
        SyntaxToken dictVar,
        out List<StatementSyntax> statements,
        out ExpressionSyntax resultExpr)
    {
        statements = [];
        return TryExpandCore(expr, cancellationToken, dictVar, statements, out resultExpr);
    }

    private bool TryExpandCore(
        ExpressionSyntax expr,
        CancellationToken cancellationToken,
        SyntaxToken dictVar,
        List<StatementSyntax> statements,
        out ExpressionSyntax resultExpr)
    {
        expr = expr.SkipParentheses();
        switch (expr)
        {
            case PrefixUnaryExpressionSyntax u when u.IsKind(SyntaxKind.LogicalNotExpression):
                if (!TryExpandCore(u.Operand, cancellationToken, dictVar, statements, out var inner))
                    return Fail(out resultExpr);
                resultExpr = PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, Token(SyntaxKind.ExclamationToken), inner);
                return true;
            case BinaryExpressionSyntax b:
                return TryExpandBinary(b, cancellationToken, dictVar, statements, out resultExpr);
            default:
                return Materialise(expr, KeyFor(expr), dictVar, statements, out resultExpr);
        }
    }

    private bool TryExpandBinary(
        BinaryExpressionSyntax b,
        CancellationToken cancellationToken,
        SyntaxToken dictVar,
        List<StatementSyntax> statements,
        out ExpressionSyntax resultExpr)
    {
        if (b.IsKind(SyntaxKind.LogicalAndExpression))
            return ExpandShortCircuitAnd(b, cancellationToken, dictVar, statements, out resultExpr);
        if (b.IsKind(SyntaxKind.LogicalOrExpression))
            return ExpandShortCircuitOr(b, cancellationToken, dictVar, statements, out resultExpr);

        if (!TryExpandCore(b.Left, cancellationToken, dictVar, statements, out var left))
            return Fail(out resultExpr);
        if (!TryExpandCore(b.Right, cancellationToken, dictVar, statements, out var right))
            return Fail(out resultExpr);

        resultExpr = BinaryExpression(b.Kind(), left, b.OperatorToken, right);
        return true;
    }

    private bool ExpandShortCircuitAnd(
        BinaryExpressionSyntax b,
        CancellationToken cancellationToken,
        SyntaxToken dictVar,
        List<StatementSyntax> statements,
        out ExpressionSyntax resultExpr)
    {
        if (!TryExpandCore(b.Left, cancellationToken, dictVar, statements, out var left))
            return Fail(out resultExpr);

        var rightStmts = new List<StatementSyntax>();
        if (!TryExpandCore(b.Right, cancellationToken, dictVar, rightStmts, out var right))
            return Fail(out resultExpr);

        var resultId = NewTempName();
        statements.Add(LocalDeclarationStatement(
            VariableDeclaration(ParseTypeName("bool"))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(resultId)
                        .WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.FalseLiteralExpression)))))));

        var assignRight = ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(resultId),
                right));

        var inner = Block(rightStmts.Concat([assignRight]).ToArray());
        statements.Add(IfStatement(left, inner));

        resultExpr = IdentifierName(resultId);
        return true;
    }

    private bool ExpandShortCircuitOr(
        BinaryExpressionSyntax b,
        CancellationToken cancellationToken,
        SyntaxToken dictVar,
        List<StatementSyntax> statements,
        out ExpressionSyntax resultExpr)
    {
        if (!TryExpandCore(b.Left, cancellationToken, dictVar, statements, out var left))
            return Fail(out resultExpr);

        var rightStmts = new List<StatementSyntax>();
        if (!TryExpandCore(b.Right, cancellationToken, dictVar, rightStmts, out var right))
            return Fail(out resultExpr);

        var resultId = NewTempName();
        statements.Add(LocalDeclarationStatement(
            VariableDeclaration(ParseTypeName("bool"))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(resultId)
                        .WithInitializer(EqualsValueClause(LiteralExpression(SyntaxKind.TrueLiteralExpression)))))));

        var assignRight = ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                IdentifierName(resultId),
                right));

        var negLeft = PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, Token(SyntaxKind.ExclamationToken), left);
        var inner = Block(rightStmts.Concat([assignRight]).ToArray());
        statements.Add(IfStatement(negLeft, inner));

        resultExpr = IdentifierName(resultId);
        return true;
    }

    private static bool Fail(out ExpressionSyntax resultExpr)
    {
        resultExpr = LiteralExpression(SyntaxKind.FalseLiteralExpression);
        return false;
    }

    private bool Materialise(
        ExpressionSyntax expr,
        string key,
        SyntaxToken dictVar,
        List<StatementSyntax> statements,
        out ExpressionSyntax resultExpr)
    {
        var id = NewTempName();
        statements.Add(LocalDeclarationStatement(
            VariableDeclaration(ParseTypeName("var"))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(id).WithInitializer(EqualsValueClause(expr))))));

        var disambiguated = DisambiguateKey(key);
        statements.Add(MakeDictAssign(dictVar, disambiguated, IdentifierName(id)));

        resultExpr = IdentifierName(id);
        return true;
    }

    private string DisambiguateKey(string key)
    {
        var baseKey = key;
        var n = 2;
        var k = key;
        while (_captureKeys.Contains(k))
            k = baseKey + " (" + n++ + ")";
        _captureKeys.Add(k);
        return k;
    }

    private static ExpressionStatementSyntax MakeDictAssign(SyntaxToken dictVar, string key, ExpressionSyntax valueExpr) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                ElementAccessExpression(IdentifierName(dictVar))
                    .WithArgumentList(
                        BracketedArgumentList(
                            SingletonSeparatedList(
                                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(key)))))),
                CastExpression(ParseTypeName("object?"), valueExpr)));

    private SyntaxToken NewTempName() => Identifier($"__oa_t_{_tempId++}");

    private static string KeyFor(ExpressionSyntax expr) => expr.ToString().Trim();
}

/// <summary>Syntax helpers for boolean lowering.</summary>
internal static class ExpressionExtensions
{
    /// <summary>Strips redundant parentheses from <paramref name="expr"/>.</summary>
    public static ExpressionSyntax SkipParentheses(this ExpressionSyntax expr)
    {
        while (expr is ParenthesizedExpressionSyntax p)
            expr = p.Expression;
        return expr;
    }
}
