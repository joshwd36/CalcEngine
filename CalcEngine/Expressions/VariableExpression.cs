namespace CalcEngine.Expressions;

public record VariableExpression(string Identifier, int Index) : Expr
{
    public override string ToString() => $"({Identifier})";
}
