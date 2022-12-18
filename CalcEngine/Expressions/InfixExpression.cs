using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;
using CalcEngine.Tokenise;

namespace CalcEngine.Expressions;

public record InfixExpression(int Left, Operator Operator, int Right) : Expr
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

    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"({expressions[Left].Format(expressions, variables)} {PrintOperator(Operator)} {expressions[Right].Format(expressions, variables)})";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        if (AsBoolOp() is var (boolOp, expressionType))
        {
            if (expectedType != ExprType.Any && expectedType != ExprType.Bool)
            {
                throw new InvalidTypeException(expectedType, ExprType.Bool);
            }
            typedExpressions[Left] = parseResult.Expressions[Left].TypeCheck(expressionType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            typedExpressions[Right] = parseResult.Expressions[Right].TypeCheck(expressionType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);

            return new TypedBoolInfixExpr(Left, boolOp, Right);
        }
        else if (AsEqualityOp() is EqualityOp equalityOp)
        {
            if (expectedType != ExprType.Any && expectedType != ExprType.Bool)
            {
                throw new InvalidTypeException(expectedType, ExprType.Bool);
            }
            ExprType equalityType = ExprType.Any;
            TypedExpr left = parseResult.Expressions[Left].TypeCheck(equalityType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            TypedExpr right;
            if (left.Type == ExprType.Any)
            {
                right = parseResult.Expressions[Right].TypeCheck(equalityType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
                if (right.Type != ExprType.Any)
                {
                    equalityType = right.Type;
                    left = parseResult.Expressions[Left].TypeCheck(equalityType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
                }
            }
            else
            {
                equalityType = left.Type;
                right = parseResult.Expressions[Right].TypeCheck(equalityType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            }
            typedExpressions[Left] = left;
            typedExpressions[Right] = right;
            return new TypedEqualityInfixExpr(Left, equalityOp, Right, equalityType);
        }
        else if (AsNumberOp() is NumberOp numberOp)
        {
            if (expectedType != ExprType.Any && expectedType != ExprType.Number)
            {
                throw new InvalidTypeException(expectedType, ExprType.Number);
            }
            typedExpressions[Left] = parseResult.Expressions[Left].TypeCheck(ExprType.Number, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            typedExpressions[Right] = parseResult.Expressions[Right].TypeCheck(ExprType.Number, typedExpressions, typedVariables, constants, parseResult, functionRegistry);

            return new TypedNumberInfixExpr(Left, numberOp, Right);
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
