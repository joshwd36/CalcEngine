using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public record VariableExpression(int Index) : Expr
{
    public override string Format(IReadOnlyList<string> variables)
    {
        return $"({variables[Index]})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        if (typedVariables[Index] is TypedVariable existing)
        {
            if (existing.Type == expectedType)
            {
                return new TypedVariableExpr(Index, expectedType);
            }
            else if (expectedType != ExprType.Any && existing.Type == ExprType.Any)
            {
                typedVariables[Index] = existing with { Type = expectedType };
                return new TypedVariableExpr(Index, expectedType);
            }
            else // if (expectedType != existing.Type)
            {
                throw new InvalidTypeException(expectedType, existing.Type);
            }
        }
        else
        {
            typedVariables[Index] = new TypedVariable(variables[Index], expectedType);
            return new TypedVariableExpr(Index, expectedType);
        }
    }
}
