using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedNotExpr(int Index) : TypedExpr(ExprType.Bool)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        expressions[Index].GenerateIl(expressions, il, comparisonFactor);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ceq);
    }
}
