namespace CalcEngine.Tokenise;

public class Tokeniser
{

    public Tokeniser()
    {
    }

    public IEnumerable<Token> Tokenise(string expression)
    {
        var tokeniserState = new TokeniserState();
        for (int i = 0; i < expression.Length; i++)
        {
            if (tokeniserState.State == StateName.Default)
            {
                if (ProcessDefault(expression, i, ref tokeniserState) is Token result)
                {
                    yield return result;
                }
            }
            else if (tokeniserState.State == StateName.Number)
            {
                if (ProcessNumber(expression, i, ref tokeniserState) is NumberLiteralToken result)
                {
                    yield return result;
                    if (ProcessDefault(expression, i, ref tokeniserState) is Token defaultResult)
                    {
                        yield return defaultResult;
                    }
                }
            }
            else if (tokeniserState.State == StateName.Identifier)
            {
                if (ProcessIdentifier(expression, i, ref tokeniserState) is IdentifierToken result)
                {
                    yield return result;
                    if (ProcessDefault(expression, i, ref tokeniserState) is Token defaultResult)
                    {
                        yield return defaultResult;
                    }
                }
            }
        }
        if (tokeniserState.State == StateName.Number)
        {
            yield return tokeniserState.CompleteNumber(expression);
        }
        else if (tokeniserState.State == StateName.Identifier)
        {
            yield return tokeniserState.CompleteIdentifier(expression);
        }
    }

    private static Token? ProcessDefault(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (char.IsWhiteSpace(c))
        {
            return null;
        }
        else if (char.IsDigit(c))
        {
            state.InitialiseNumber(i);
            return null;
        }
        else if (c == '+')
        {
            return new OperatorToken(Operator.Addition, i, 1);
        }
        else if (c == '-')
        {
            return new OperatorToken(Operator.Subtraction, i, 1);
        }
        else if (c == '/')
        {
            return new OperatorToken(Operator.Division, i, 1);
        }
        else if (c == '*')
        {
            return new OperatorToken(Operator.Multiplication, i, 1);
        }
        else if (c == '(')
        {
            return new OperatorToken(Operator.OpenParen, i, 1);
        }
        else if (c == ')')
        {
            return new OperatorToken(Operator.CloseParen, i, 1);
        }
        else if (c == ',')
        {
            return new OperatorToken(Operator.Comma, i, 1);
        }
        else if (char.IsLetter(c))
        {
            state.InitialiseIdentifier(i);
            return null;
        }
        else
        {
            throw new InvalidCharacterException(c, i);
        }
    }

    private static NumberLiteralToken? ProcessNumber(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '.')
        {
            if (state.HadExponent || state.HadDecimal)
            {
                throw new InvalidCharacterException(c, i);
            }
            else
            {
                state.AddDecimal();
                return null;
            }
        }
        else if (c == 'e')
        {
            if (state.HadExponent)
            {
                throw new InvalidCharacterException(c, i);
            }
            else
            {
                state.AddExponent();
                return null;
            }
        }
        else if (char.IsDigit(c))
        {
            state.Continue();
            return null;
        }
        else
        {
            return state.CompleteNumber(expression);
        }
    }

    private static IdentifierToken? ProcessIdentifier(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (char.IsLetterOrDigit(c))
        {
            state.Continue();
            return null;
        }
        else
        {
            return state.CompleteIdentifier(expression);
        }
    }
}

public enum StateName
{
    Default, Number, Identifier
}

public struct TokeniserState
{
    public StateName State { get; private set; }
    public bool HadDecimal { get; private set; }
    public bool HadExponent { get; private set; }

    private int _start;
    private int _length;

    public TokeniserState()
    {
        State = StateName.Default;
        _start = 0;
        _length = 0;
        HadDecimal = false;
        HadExponent = false;
    }

    public void Continue()
    {
        _length++;
    }

    public void AddDecimal()
    {
        HadDecimal = true;
        _length++;
    }

    public void AddExponent()
    {
        HadExponent = true;
        _length++;
    }

    public void InitialiseNumber(int start)
    {
        _start = start;
        _length = 1;
        HadDecimal = false;
        HadExponent = false;
        State = StateName.Number;
    }

    public void InitialiseIdentifier(int start)
    {
        _start = start;
        _length = 1;
        HadDecimal = false;
        HadExponent = false;
        State = StateName.Identifier;
    }

    public NumberLiteralToken CompleteNumber(string expression)
    {
        State = StateName.Default;
        double value = double.Parse(expression.AsSpan(_start, _length));
        var numberToken = new NumberLiteralToken(value, _start, _length);
        return numberToken;
    }

    public IdentifierToken CompleteIdentifier(string expression)
    {
        State = StateName.Default;
        var identifierToken = new IdentifierToken(expression.Substring(_start, _length), _start, _length);
        return identifierToken;
    }
}
