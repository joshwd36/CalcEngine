namespace CalcEngine.Expressions;

public record NegativeExpression(int Expression) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"(-{expressions[Expression].Format(expressions, variables)})";
    }
}
