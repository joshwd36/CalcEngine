using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public record NotExpression(Expr Expression) : Expr
{
    public override string Format(IReadOnlyList<string> variables)
    {
        return $"(!{Expression.Format(variables)})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        if (expectedType == ExprType.Any || expectedType == ExprType.Bool)
        {
            TypedExpr expr = Expression.TypeCheck(ExprType.Bool, typedVariables, variables, constants, functionRegistry);
            return new TypedNotExpr(expr);
        }
        else
        {
            throw new InvalidTypeException(expectedType, ExprType.Bool);
        }
    }
}
