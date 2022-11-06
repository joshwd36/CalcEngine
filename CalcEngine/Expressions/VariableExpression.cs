namespace CalcEngine.Expressions;

public record VariableExpression(int Index) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"({variables[Index]})";
    }
}
