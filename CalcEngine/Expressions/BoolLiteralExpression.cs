using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public record BoolLiteralExpression(bool Value) : Expr
{
    public override string Format(IReadOnlyList<string> variables)
    {
        return $"{Value}";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Bool || expectedType == ExprType.Any)
        {
            return new TypedBoolLiteralExpr(Value);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.Bool);
        }
    }
}
