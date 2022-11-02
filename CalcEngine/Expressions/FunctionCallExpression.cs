namespace CalcEngine.Expressions;

public record FunctionCallExpression(string FunctionName, IReadOnlyList<Expr> Arguments) : Expr
{
    public override string ToString() => $"({FunctionName}({string.Join(", ", Arguments)}))";
}
