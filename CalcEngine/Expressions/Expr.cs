namespace CalcEngine.Expressions;

public abstract record Expr
{
    public abstract string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables);
}
