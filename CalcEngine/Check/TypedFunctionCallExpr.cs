using CalcEngine.Functions;
using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedFunctionCallExpr(FunctionEntry FunctionEntry, IReadOnlyList<int> Arguments, ExprType Type, int ConstantIndex) : TypedExpr(Type)
{
    public override void GenerateIl(IReadOnlyList<TypedExpr> expressions, ILGenerator il, double comparisonFactor)
    {
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4, ConstantIndex);
        il.Emit(OpCodes.Ldelem_Ref);
        foreach (int argumentIndex in Arguments)
        {
            expressions[argumentIndex].GenerateIl(expressions, il, comparisonFactor);
        }
        il.EmitCall(OpCodes.Callvirt, FunctionEntry.Delegate.GetType().GetMethod("Invoke", FunctionEntry.DelegateArgs)!, null);
    }
}
