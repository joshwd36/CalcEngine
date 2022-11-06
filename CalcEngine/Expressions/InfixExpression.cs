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
            Operator.OpenParen => "(",
            Operator.CloseParen => ")",
            Operator.Comma => ",",
            _ => throw new ArgumentOutOfRangeException(nameof(op))
        };
    }

    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        return $"({expressions[Left].Format(expressions, variables)} {PrintOperator(Operator)} {expressions[Right].Format(expressions, variables)})";
    }
}
