using System.Reflection.Emit;

namespace CalcEngine.Check;

public record TypedBoolInfixExpr(TypedExpr Left, BoolOp Operator, TypedExpr Right) : TypedExpr(ExprType.Bool)
{
    public override void GenerateIl(ILGenerator il, double comparisonFactor)
    {
        Left.GenerateIl(il, comparisonFactor);
        Right.GenerateIl(il, comparisonFactor);

        switch (Operator)
        {
            case BoolOp.And:
                il.Emit(OpCodes.And);
                break;
            case BoolOp.Or:
                il.Emit(OpCodes.Or);
                break;
            case BoolOp.LessThan:
                il.Emit(OpCodes.Clt);
                break;
            case BoolOp.LessThanEqual:
                il.Emit(OpCodes.Cgt_Un);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                break;
            case BoolOp.GreaterThan:
                il.Emit(OpCodes.Cgt);
                break;
            case BoolOp.GreaterThanEqual:
                il.Emit(OpCodes.Clt_Un);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Ceq);
                break;
        }
    }
}

public enum BoolOp
{
    And,
    Or,
    LessThan,
    LessThanEqual,
    GreaterThan,
    GreaterThanEqual,
}
