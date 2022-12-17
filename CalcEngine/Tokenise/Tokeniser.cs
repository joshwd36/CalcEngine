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
                if (ProcessDefault(expression, i, ref tokeniserState) is OperatorToken result)
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
                if (ProcessIdentifier(expression, i, ref tokeniserState) is Token result)
                {
                    yield return result;
                    if (ProcessDefault(expression, i, ref tokeniserState) is Token defaultResult)
                    {
                        yield return defaultResult;
                    }
                }
            }
            else if (tokeniserState.State == StateName.StringLiteral)
            {
                if (ProcessString(expression, i, ref tokeniserState) is StringLiteralToken result)
                {
                    yield return result;
                }
            }
            else if (tokeniserState.State == StateName.And)
            {
                yield return ProcessAnd(expression, i, ref tokeniserState);
            }
            else if (tokeniserState.State == StateName.Or)
            {
                yield return ProcessOr(expression, i, ref tokeniserState);
            }
            else if (tokeniserState.State == StateName.Equal)
            {
                yield return ProcessEqual(expression, i, ref tokeniserState);
            }
            else if (tokeniserState.State == StateName.NotEqual)
            {
                if (ProcessNotEqual(expression, i, ref tokeniserState) is OperatorToken result)
                {
                    yield return result;
                    if (result is OperatorToken { Operator: Operator.Not } && ProcessDefault(expression, i, ref tokeniserState) is Token defaultResult)
                    {
                        yield return defaultResult;
                    }
                }
            }
            else if (tokeniserState.State == StateName.LessThan)
            {
                if (ProcessLessThan(expression, i, ref tokeniserState) is OperatorToken result)
                {
                    yield return result;
                    if (result is OperatorToken { Operator: Operator.LessThan } && ProcessDefault(expression, i, ref tokeniserState) is Token defaultResult)
                    {
                        yield return defaultResult;
                    }
                }
            }
            else if (tokeniserState.State == StateName.GreaterThan)
            {
                if (ProcessGreaterThan(expression, i, ref tokeniserState) is OperatorToken result)
                {
                    yield return result;
                    if (result is OperatorToken { Operator: Operator.GreaterThan } && ProcessDefault(expression, i, ref tokeniserState) is Token defaultResult)
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
        else if (tokeniserState.State == StateName.LessThan)
        {
            yield return tokeniserState.CompleteLessThan();
        }
        else if (tokeniserState.State == StateName.GreaterThan)
        {
            yield return tokeniserState.CompleteGreaterThan();
        }
    }

    private static OperatorToken? ProcessDefault(string expression, int i, ref TokeniserState state)
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
        else if (c == '\'')
        {
            state.InitialiseString(i, QuoteStyle.Single);
            return null;
        }
        else if (c == '"')
        {
            state.InitialiseString(i, QuoteStyle.Double);
            return null;
        }
        else if (c == '&')
        {
            state.InitialiseAnd(i);
            return null;
        }
        else if (c == '|')
        {
            state.InitialiseOr(i);
            return null;
        }
        else if (c == '=')
        {
            state.InitialiseEqual(i);
            return null;
        }
        else if (c == '!')
        {
            state.InitialiseNotEqual(i);
            return null;
        }
        else if (c == '<')
        {
            state.InitialiseLessThan(i);
            return null;
        }
        else if (c == '>')
        {
            state.InitialiseGreaterThan(i);
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
        else if (c == '%')
        {
            return new OperatorToken(Operator.Remainder, i, 1);
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

    private static Token? ProcessIdentifier(string expression, int i, ref TokeniserState state)
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

    private static StringLiteralToken? ProcessString(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if ((state.QuoteStyle == QuoteStyle.Single && c == '\'') || (state.QuoteStyle == QuoteStyle.Double && c == '"'))
        {
            return state.CompleteString(expression);
        }
        else
        {
            state.Continue();
            return null;
        }
    }

    private static OperatorToken ProcessAnd(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '&')
        {
            return state.CompleteAnd();
        }
        else
        {
            throw new InvalidCharacterException(c, i);
        }
    }

    private static OperatorToken ProcessOr(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '|')
        {
            return state.CompleteOr();
        }
        else
        {
            throw new InvalidCharacterException(c, i);
        }
    }

    private static OperatorToken ProcessEqual(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '=')
        {
            return state.CompleteEqual();
        }
        else
        {
            throw new InvalidCharacterException(c, i);
        }
    }

    private static OperatorToken ProcessNotEqual(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '=')
        {
            return state.CompleteNotEqual();
        }
        else
        {
            return state.CompleteNot();
        }
    }

    private static OperatorToken ProcessLessThan(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '=')
        {
            return state.CompleteLessThanEqual();
        }
        else
        {
            return state.CompleteLessThan();
        }
    }

    private static OperatorToken ProcessGreaterThan(string expression, int i, ref TokeniserState state)
    {
        char c = expression[i];
        if (c == '=')
        {
            return state.CompleteGreaterThanEqual();
        }
        else
        {
            return state.CompleteGreaterThan();
        }
    }
}

public enum StateName
{
    Default, Number, Identifier, StringLiteral, And, Or, Equal, NotEqual, GreaterThan, LessThan
}

public struct TokeniserState
{
    public StateName State { get; private set; }
    public bool HadDecimal { get; private set; }
    public bool HadExponent { get; private set; }
    public QuoteStyle QuoteStyle { get; private set; }

    private int _start;
    public int _length;

    public TokeniserState()
    {
        State = StateName.Default;
        _start = 0;
        _length = 0;
        HadDecimal = false;
        HadExponent = false;
        QuoteStyle = QuoteStyle.Single;
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
        State = StateName.Identifier;
    }

    public void InitialiseString(int start, QuoteStyle quoteStyle)
    {
        _start = start;
        _length = 1;
        QuoteStyle = quoteStyle;
        State = StateName.StringLiteral;
    }

    public void InitialiseAnd(int start)
    {
        _start = start;
        _length = 1;
        State = StateName.And;
    }

    public void InitialiseOr(int start)
    {
        _start = start;
        _length = 1;
        State = StateName.Or;
    }

    public void InitialiseEqual(int start)
    {
        _start = start;
        _length = 1;
        State = StateName.Equal;
    }

    public void InitialiseNotEqual(int start)
    {
        _start = start;
        _length = 1;
        State = StateName.NotEqual;
    }

    public void InitialiseLessThan(int start)
    {
        _start = start;
        _length = 1;
        State = StateName.LessThan;
    }

    public void InitialiseGreaterThan(int start)
    {
        _start = start;
        _length = 1;
        State = StateName.GreaterThan;
    }

    public NumberLiteralToken CompleteNumber(string expression)
    {
        State = StateName.Default;
        double value = double.Parse(expression.AsSpan(_start, _length));
        var numberToken = new NumberLiteralToken(value, _start, _length);
        return numberToken;
    }

    public Token CompleteIdentifier(string expression)
    {
        State = StateName.Default;
        string value = expression.Substring(_start, _length);
        if (value == "true")
        {
            return new BoolLiteralToken(true, _start, _length);
        }
        else if (value == "false")
        {
            return new BoolLiteralToken(false, _start, _length);
        }
        else
        {
            var identifierToken = new IdentifierToken(value, _start, _length);
            return identifierToken;
        }
    }

    public StringLiteralToken CompleteString(string expression)
    {
        State = StateName.Default;
        string value = expression.Substring(_start + 1, _length - 1);
        return new StringLiteralToken(value, _start, _length + 1);
    }

    public OperatorToken CompleteNot()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.Not, _start, 1);
    }

    public OperatorToken CompleteAnd()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.And, _start, 2);
    }

    public OperatorToken CompleteOr()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.Or, _start, 2);
    }

    public OperatorToken CompleteEqual()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.Equal, _start, 2);
    }

    public OperatorToken CompleteNotEqual()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.NotEqual, _start, 2);
    }

    public OperatorToken CompleteLessThan()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.LessThan, _start, 1);
    }

    public OperatorToken CompleteLessThanEqual()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.LessThanEqual, _start, 2);
    }

    public OperatorToken CompleteGreaterThan()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.GreaterThan, _start, 1);
    }

    public OperatorToken CompleteGreaterThanEqual()
    {
        State = StateName.Default;
        return new OperatorToken(Operator.GreaterThanEqual, _start, 2);
    }
}

public enum QuoteStyle
{
    Single, Double
}
