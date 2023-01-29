using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedNotExpr(TypedExpr Expr) : TypedExpr(ExprType.Bool)
{
    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        Expr.GenerateIl(il, comparisonFactor);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ceq);
    }
}
