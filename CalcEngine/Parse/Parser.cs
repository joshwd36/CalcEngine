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
        int constantCount = 0;
        Expr root = ParseExpression(tokens, 0, variables, ref constantCount);
        return new ParseResult(root, variables, constantCount);
    }

    private Expr ParseExpression(Peekable<Token> tokens, byte minBp, List<string> variables, ref int constantCount)
    {
        Expr lhs = tokens.Next() switch
        {
            NumberLiteralToken numberLiteral => new NumberLiteralExpression(numberLiteral.Value),
            BoolLiteralToken boolLiteral => new BoolLiteralExpression(boolLiteral.Value),
            StringLiteralToken stringLiteral => new StringLiteralExpression(stringLiteral.Value),
            IdentifierToken identifier => ParseIdentifier(identifier.Name, tokens, variables, ref constantCount),
            OperatorToken { Operator: Operator.OpenParen } => ParseParen(tokens, variables, ref constantCount),
            OperatorToken infixToken => ParsePrefix(infixToken, tokens, variables, ref constantCount),
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
                Expr rhs = ParseExpression(tokens, rbp, variables, ref constantCount);
                lhs = new InfixExpression(lhs, op, rhs);
            }
            else
            {
                break;
            }
        }

        return lhs;
    }

    private Expr ParseIdentifier(string name, Peekable<Token> tokens, List<string> variables, ref int constantCount)
    {
        if (tokens.Peek() is OperatorToken { Operator: Operator.OpenParen })
        {
            tokens.Next();
            var arguments = new List<Expr>();
            while (true)
            {
                if (tokens.Peek() is OperatorToken { Operator: Operator.CloseParen })
                {
                    tokens.Next();
                    break;
                }
                var argument = ParseExpression(tokens, 0, variables, ref constantCount);
                arguments.Add(argument);
                if (tokens.Peek() is OperatorToken { Operator: Operator.Comma })
                {
                    tokens.Next();
                }
            }
            return new FunctionCallExpression(name, arguments, constantCount++);
        }
        else
        {
            int existingIndex = variables.FindIndex(v => v == name);
            if (existingIndex >= 0)
            {
                return new VariableExpression(existingIndex);
            }
            else
            {
                int index = variables.Count;
                variables.Add(name);
                return new VariableExpression(index);
            }
        }
    }

    private Expr ParsePrefix(OperatorToken infixToken, Peekable<Token> tokens, List<string> variables, ref int constantCount)
    {
        byte rbp = PrefixBindingPower(infixToken.Operator) ?? throw new InvalidTokenException(infixToken);
        Expr rhs = ParseExpression(tokens, rbp, variables, ref constantCount);
        if (infixToken.Operator == Operator.Subtraction)
        {
            return new NegativeExpression(rhs);
        }
        else if (infixToken.Operator == Operator.Not)
        {
            return new NotExpression(rhs);
        }
        else
        {
            return rhs;
        }
    }

    private Expr ParseParen(Peekable<Token> tokens, List<string> variables, ref int constantCount)
    {
        Expr body = ParseExpression(tokens, 0, variables, ref constantCount);
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
