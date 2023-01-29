using System.Reflection;
using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedVariableExpr(int Index, ExprType Type) : TypedExpr(Type)
{
    private static readonly MethodInfo _index = typeof(IReadOnlyList<object>).GetMethod("get_Item")!;
    private static readonly MethodInfo _convertDouble = typeof(Convert).GetMethod(nameof(Convert.ToDouble), new[] { typeof(object) })!;
    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldc_I4, Index);
        il.EmitCall(OpCodes.Callvirt, _index, null);
        switch (Type)
        {
            case ExprType.Bool:
                il.Emit(OpCodes.Unbox_Any, typeof(bool));
                break;
            case ExprType.Number:
                il.EmitCall(OpCodes.Call, _convertDouble, null);
                break;
            case ExprType.String:
                il.Emit(OpCodes.Castclass, typeof(string));
                break;
            case ExprType.Any:
                throw new InvalidOperationException("Cannot use variable of type any");
        }
    }
}
