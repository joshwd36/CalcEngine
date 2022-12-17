using System.Reflection.Emit;

namespace CalcEngine.Check;

public abstract record TypedExpr(ExprType Type)
{
    public abstract void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor);
}
