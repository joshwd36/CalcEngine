using CalcEngine.Expressions;
using CalcEngine.Tokenise;
using CalcEngine.Utils;

namespace CalcEngine.Parse;

public class Parser
{
    private readonly Tokeniser _tokeniser;

    public Parser()
    {
        _tokeniser = new Tokeniser();
    }

    public ParseResult Parse(string expression)
    {
        Peekable<Token> tokens = _tokeniser.Tokenise(expression).Peekable();
        var expressions = new List<Expr>();
        var variables = new List<string>();
        var root = ParseExpression(tokens, 0, expressions, variables);
        return new ParseResult(root, expressions, variables);
    }

    private static int AddExpression(List<Expr> expressions, Expr newExpression)
    {
        int index = expressions.Count;
        expressions.Add(newExpression);
        return index;
    }

    private int ParseExpression(Peekable<Token> tokens, byte minBp, List<Expr> expressions, List<string> variables)
    {
        int lhs = tokens.Next() switch
        {
            NumberLiteralToken numberLiteral => AddExpression(expressions, new NumberLiteralExpression(numberLiteral.Value)),
            BoolLiteralToken boolLiteral => AddExpression(expressions, new BoolLiteralExpression(boolLiteral.Value)),
            StringLiteralToken stringLiteral => AddExpression(expressions, new StringLiteralExpression(stringLiteral.Value)),
            IdentifierToken identifier => ParseIdentifier(identifier.Name, tokens, expressions, variables),
            OperatorToken { Operator: Operator.OpenParen } => ParseParen(tokens, expressions, variables),
            OperatorToken infixToken => ParsePrefix(infixToken, tokens, expressions, variables),
            Token token => throw new InvalidTokenException(token),
            _ => throw new UnexpectedEofException()
        };

        while (true)
        {
            Token? infixOperatorToken = tokens.Peek();
            if (infixOperatorToken is null)
            {
                break;
            }

            Operator op = (infixOperatorToken as OperatorToken ?? throw new InvalidTokenException(infixOperatorToken)).Operator;

            if (InfixBindingPower(op) is var (lbp, rbp))
            {
                if (lbp < minBp)
                {
                    break;
                }

                tokens.Next();
                int rhs = ParseExpression(tokens, rbp, expressions, variables);
                lhs = AddExpression(expressions, new InfixExpression(lhs, op, rhs));
            }
            else
            {
                break;
            }
        }

        return lhs;
    }

    private int ParseIdentifier(string name, Peekable<Token> tokens, List<Expr> expressions, List<string> variables)
    {
        if (tokens.Peek() is OperatorToken { Operator: Operator.OpenParen })
        {
            tokens.Next();
            var arguments = new List<int>();
            while (true)
            {
                if (tokens.Peek() is OperatorToken { Operator: Operator.CloseParen })
                {
                    tokens.Next();
                    break;
                }
                var argument = ParseExpression(tokens, 0, expressions, variables);
                arguments.Add(argument);
                if (tokens.Peek() is OperatorToken { Operator: Operator.Comma })
                {
                    tokens.Next();
                }
            }
            return AddExpression(expressions, new FunctionCallExpression(name, arguments));
        }
        else
        {
            int existingIndex = variables.FindIndex(v => v == name);
            if (existingIndex >= 0)
            {
                return AddExpression(expressions, new VariableExpression(existingIndex));
            }
            else
            {
                int index = variables.Count;
                variables.Add(name);
                return AddExpression(expressions, new VariableExpression(index));
            }
        }
    }

    private int ParsePrefix(OperatorToken infixToken, Peekable<Token> tokens, List<Expr> expressions, List<string> variables)
    {
        byte rbp = PrefixBindingPower(infixToken.Operator) ?? throw new InvalidTokenException(infixToken);
        int rhs = ParseExpression(tokens, rbp, expressions, variables);
        if (infixToken.Operator == Operator.Subtraction)
        {
            return AddExpression(expressions, new NegativeExpression(rhs));
        }
        else if (infixToken.Operator == Operator.Not)
        {
            return AddExpression(expressions, new NotExpression(rhs));
        }
        else
        {
            return rhs;
        }
    }

    private int ParseParen(Peekable<Token> tokens, List<Expr> expressions, List<string> variables)
    {
        int body = ParseExpression(tokens, 0, expressions, variables);
        var next = tokens.Next();
        if (next is null)
        {
            throw new UnexpectedEofException();
        }
        else if (next is not OperatorToken { Operator: Operator.CloseParen })
        {
            throw new InvalidTokenException(next);
        }
        return body;
    }

    private static (byte, byte)? InfixBindingPower(Operator infixOperator)
    {
        return infixOperator switch
        {
            Operator.Or => (1, 2),
            Operator.And => (3, 4),
            Operator.Equal => (5, 6),
            Operator.LessThan or Operator.LessThanEqual or Operator.GreaterThan or Operator.GreaterThanEqual => (7, 8),
            Operator.Addition or Operator.Subtraction => (9, 10),
            Operator.Multiplication or Operator.Division or Operator.Remainder => (11, 12),
            _ => null
        };
    }

    private static byte? PrefixBindingPower(Operator infixOperator)
    {
        return infixOperator switch
        {
            Operator.Addition or Operator.Subtraction => 13,
            Operator.Not => 15,
            _ => null
        };
    }
}
