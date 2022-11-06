namespace CalcEngine.Expressions;

public record NumberLiteralExpression(double Value) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"({Value})";
    }
}
