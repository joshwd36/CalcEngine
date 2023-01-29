using CalcEngine.Expressions;

namespace CalcEngine.Parse;

public record ParseResult(Expr Root, IReadOnlyList<string> Variables, int ConstantCount)
{
    public override string ToString()
    {
        return Root.Format(Variables);
    }
}
