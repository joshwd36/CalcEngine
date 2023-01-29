using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Tokenise;

namespace CalcEngine.Expressions;

public record InfixExpression(Expr Left, Operator Operator, Expr Right) : Expr
{
    private static string PrintOperator(Operator op)
    {
        return op switch
        {
            Operator.Addition => "+",
            Operator.Subtraction => "-",
            Operator.Multiplication => "*",
            Operator.Division => "/",
            Operator.Remainder => "%",
            Operator.OpenParen => "(",
            Operator.CloseParen => ")",
            Operator.Comma => ",",
            Operator.And => "&&",
            Operator.Or => "||",
            Operator.Equal => "==",
            Operator.LessThan => "<",
            Operator.LessThanEqual => "<=",
            Operator.GreaterThan => ">",
            Operator.GreaterThanEqual => ">=",
            _ => throw new ArgumentOutOfRangeException(nameof(op))
        };
    }

    public override string Format(IReadOnlyList<string> variables)
    {
        return $"({Left.Format(variables)} {PrintOperator(Operator)} {Right.Format(variables)})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        if (AsBoolOp() is var (boolOp, expressionType))
        {
            if (expectedType != ExprType.Any && expectedType != ExprType.Bool)
            {
                throw new InvalidTypeException(expectedType, ExprType.Bool);
            }
            TypedExpr left = Left.TypeCheck(expressionType, typedVariables, variables, constants, functionRegistry);
            TypedExpr right = Right.TypeCheck(expressionType, typedVariables, variables, constants, functionRegistry);

            return new TypedBoolInfixExpr(left, boolOp, right);
        }
        else if (AsEqualityOp() is EqualityOp equalityOp)
        {
            if (expectedType != ExprType.Any && expectedType != ExprType.Bool)
            {
                throw new InvalidTypeException(expectedType, ExprType.Bool);
            }
            ExprType equalityType = ExprType.Any;
            TypedExpr left = Left.TypeCheck(equalityType, typedVariables, variables, constants, functionRegistry);
            TypedExpr right;
            if (left.Type == ExprType.Any)
            {
                right = Right.TypeCheck(equalityType, typedVariables, variables, constants, functionRegistry);
                if (right.Type != ExprType.Any)
                {
                    equalityType = right.Type;
                    left = Left.TypeCheck(equalityType, typedVariables, variables, constants, functionRegistry);
                }
            }
            else
            {
                equalityType = left.Type;
                right = Right.TypeCheck(equalityType, typedVariables, variables, constants, functionRegistry);
            }
            return new TypedEqualityInfixExpr(left, equalityOp, right, equalityType);
        }
        else if (AsNumberOp() is NumberOp numberOp)
        {
            if (expectedType != ExprType.Any && expectedType != ExprType.Number)
            {
                throw new InvalidTypeException(expectedType, ExprType.Number);
            }
            TypedExpr left = Left.TypeCheck(ExprType.Number, typedVariables, variables, constants, functionRegistry);
            TypedExpr right = Right.TypeCheck(ExprType.Number, typedVariables, variables, constants, functionRegistry);

            return new TypedNumberInfixExpr(left, numberOp, right);
        }
        else
        {
            throw new Exception($"Invalid infix operator {Operator}");
        }
    }

    private (BoolOp, ExprType)? AsBoolOp()
    {
        return Operator switch
        {
            Operator.And => (BoolOp.And, ExprType.Bool),
            Operator.Or => (BoolOp.Or, ExprType.Bool),
            Operator.LessThan => (BoolOp.LessThan, ExprType.Number),
            Operator.LessThanEqual => (BoolOp.LessThanEqual, ExprType.Number),
            Operator.GreaterThan => (BoolOp.GreaterThan, ExprType.Number),
            Operator.GreaterThanEqual => (BoolOp.GreaterThanEqual, ExprType.Number),
            _ => null
        };
    }

    private EqualityOp? AsEqualityOp()
    {
        return Operator switch
        {
            Operator.Equal => EqualityOp.Equal,
            Operator.NotEqual => EqualityOp.NotEqual,
            _ => null
        };
    }

    private NumberOp? AsNumberOp()
    {
        return Operator switch
        {
            Operator.Addition => NumberOp.Addition,
            Operator.Subtraction => NumberOp.Subtraction,
            Operator.Multiplication => NumberOp.Multiplication,
            Operator.Division => NumberOp.Division,
            Operator.Remainder => NumberOp.Remainder,
            _ => null
        };
    }
}
