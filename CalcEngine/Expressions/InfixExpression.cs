using CalcEngine.Tokenise;

namespace CalcEngine.Expressions;

public record InfixExpression(Expr Left, Operator Operator, Expr Right) : Expr
{
    public override string ToString() => $"({Left} {PrintOperator(Operator)} {Right})";

    private static string PrintOperator(Operator op)
    {
        return op switch
        {
            Operator.Addition => "+",
            Operator.Subtraction => "-",
            Operator.Multiplication => "*",
            Operator.Division => "/",
            Operator.OpenParen => "(",
            Operator.CloseParen => ")",
            Operator.Comma => ",",
            _ => throw new ArgumentOutOfRangeException(nameof(op))
        };
    }
}
