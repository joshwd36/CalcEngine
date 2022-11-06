using CalcEngine.Expressions;

namespace CalcEngine.Parse;

public record ParseResult(int Root, IReadOnlyList<Expr> Expressions, IReadOnlyList<string> Variables)
{
    public override string ToString()
    {
        return Expressions[Root].Format(Expressions, Variables);
    }
}
