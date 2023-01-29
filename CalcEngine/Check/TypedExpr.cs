using System.Reflection.Emit;

namespace CalcEngine.Check;

public abstract record TypedExpr(ExprType Type)
{
    public abstract void GenerateIl(ILGenerator il, double comparisonFactor);
}
