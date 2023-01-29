using CalcEngine.Functions;
using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedFunctionCallExpr(FunctionEntry FunctionEntry, IReadOnlyList<TypedExpr> Arguments, ExprType Type, int ConstantIndex) : TypedExpr(Type)
{
    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4, ConstantIndex);
        il.Emit(OpCodes.Ldelem_Ref);
        foreach (TypedExpr argumentExpr in Arguments)
        {
            argumentExpr.GenerateIl(il, comparisonFactor);
        }
        il.EmitCall(OpCodes.Callvirt, FunctionEntry.Delegate.GetType().GetMethod("Invoke", FunctionEntry.DelegateArgs)!, null);
    }
}
