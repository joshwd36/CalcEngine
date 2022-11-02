namespace CalcEngine.Expressions;

public record NumberLiteralExpression(double Value) : Expr
{
    public override string ToString() => $"({Value})";
}
