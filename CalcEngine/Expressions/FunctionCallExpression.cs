namespace CalcEngine.Expressions;

public record FunctionCallExpression(string FunctionName, IReadOnlyList<int> Arguments) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        var joined = string.Join(", ", Arguments.Select(a => expressions[a].Format(expressions, variables)));
       return $"({FunctionName}({joined}))";
    }
}
