using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public record StringLiteralExpression(string Value) : Expr
{
    public override string Format(IReadOnlyList<string> variables)
    {
        return $"\"{Value}\"";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Any || expectedType == ExprType.String)
        {
            return new TypedStringLiteralExpr(Value);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.String);
        }
    }
}
