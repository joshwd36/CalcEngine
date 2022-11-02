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
        var variables = new List<string>();
        var parsed = ParseExpression(tokens, 0, variables);
        return new ParseResult(parsed, variables);
    }

    private Expr ParseExpression(Peekable<Token> tokens, byte minBp, List<string> variables)
    {
        Expr lhs = tokens.Next() switch
        {
            NumberLiteralToken numberLiteral => new NumberLiteralExpression(numberLiteral.Value),
            IdentifierToken identifier => ParseIdentifier(identifier.Name, tokens, variables),
            OperatorToken op when op.Operator == Operator.OpenParen => ParseParen(tokens, variables),
            OperatorToken infixToken => ParsePrefix(infixToken, tokens, variables),
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
                Expr rhs = ParseExpression(tokens, rbp, variables);
                lhs = new InfixExpression(lhs, op, rhs);
            }
            else
            {
                break;
            }
        }

        return lhs;
    }

    private Expr ParseIdentifier(string name, Peekable<Token> tokens, List<string> variables)
    {
        if (tokens.Peek() is OperatorToken { Operator: Operator.OpenParen })
        {
            var arguments = new List<Expr>();
            while (true)
            {
                if (tokens.Peek() is OperatorToken { Operator: Operator.CloseParen })
                {
                    tokens.Next();
                    break;
                }
                var argument = ParseExpression(tokens, 0, variables);
                arguments.Add(argument);
                if (tokens.Peek() is OperatorToken { Operator: Operator.Comma })
                {
                    tokens.Next();
                }
            }
            return new FunctionCallExpression(name, arguments);
        }
        else
        {
            int existingIndex = variables.FindIndex(v => v == name);
            if (existingIndex >= 0)
            {
                return new VariableExpression(name, existingIndex);
            }
            else
            {
                int index = variables.Count;
                variables.Add(name);
                return new VariableExpression(name, index);
            }
        }
    }

    private Expr ParsePrefix(OperatorToken infixToken, Peekable<Token> tokens, List<string> variables)
    {
        byte rbp = PrefixBindingPower(infixToken.Operator) ?? throw new InvalidTokenException(infixToken);
        Expr rhs = ParseExpression(tokens, rbp, variables);
        if (infixToken.Operator == Operator.Subtraction)
        {
            return new NegativeExpression(rhs);
        }
        else
        {
            return rhs;
        }
    }

    private Expr ParseParen(Peekable<Token> tokens, List<string> variables)
    {
        Expr body = ParseExpression(tokens, 0, variables);
        var next = tokens.Next();
        if (next is null)
        {
            throw new UnexpectedEofException();
        }
        else if (next is not OperatorToken op || op.Operator != Operator.CloseParen)
        {
            throw new InvalidTokenException(next);
        }
        return body;
    }

    private (byte, byte)? InfixBindingPower(Operator infixOperator)
    {
        return infixOperator switch
        {
            Operator.Addition or Operator.Subtraction => (1, 2),
            Operator.Multiplication or Operator.Division => (3, 4),
            _ => null
        };
    }

    private byte? PrefixBindingPower(Operator infixOperator)
    {
        return infixOperator switch
        {
            Operator.Addition or Operator.Subtraction => 5,
            _ => null
        };
    }

    private byte? PostfixBindingPower(Operator postfixOperator)
    {
        return postfixOperator switch
        {
            Operator.OpenParen => 7,
            _ => null
        };
    }
}
