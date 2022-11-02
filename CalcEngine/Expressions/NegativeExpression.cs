namespace CalcEngine.Expressions;

public record NegativeExpression(Expr Expression) : Expr
{
    public override string ToString() => $"(-{Expression})";
}
