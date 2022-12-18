using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record VariableExpression(int Index) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"({variables[Index]})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
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
            typedVariables[Index] = new TypedVariable(parseResult.Variables[Index], expectedType);
            return new TypedVariableExpr(Index, expectedType);
        }
    }
}
