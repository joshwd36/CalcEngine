namespace CalcEngine.Tokenise;

public class Tokeniser
{

    public Tokeniser()
    {
    }

    public IEnumerable<Token> Tokenise(string expression)
    {
        ITokeniserState state = EmptyState.Singleton;
        var tokeniserState = new TokeniserState();
        for (int i = 0; i < expression.Length; i++)
        {
            var (newState, tokens) = state.Parse(tokeniserState, expression, i);
            foreach (var t in tokens)
            {
                yield return t;
            }
            state = newState;
        }
        if (state.Complete(expression) is Token finalToken)
        {
            yield return finalToken;
        }
    }
}

public class TokeniserState
{
    public EmptyState EmptyState { get; }
    public NumericState NumericState { get; }
    public IdentifierState IdentifierState { get; }

    public TokeniserState()
    {
        EmptyState = new EmptyState();
        NumericState = new NumericState();
        IdentifierState = new IdentifierState();
    }
}

public interface ITokeniserState
{
    (ITokeniserState, IEnumerable<Token>) Parse(TokeniserState tokeniserState, string expression, int i);
    Token? Complete(string expression);
}

public class EmptyState : ITokeniserState
{
    public static EmptyState Singleton { get; } = new EmptyState();

    public Token? Complete(string expression)
    {
        return null;
    }

    public (ITokeniserState, IEnumerable<Token>) Parse(TokeniserState tokeniserState, string expression, int i)
    {
        char c = expression[i];
        return c switch
        {
            char when char.IsDigit(c) => (NumericState.Create(tokeniserState, i), Enumerable.Empty<Token>()),
            char when char.IsWhiteSpace(c) => (EmptyState.Singleton, Enumerable.Empty<Token>()),
            '+' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.Addition, i, 1) }),
            '-' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.Subtraction, i, 1) }),
            '/' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.Division, i, 1) }),
            '*' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.Multiplication, i, 1) }),
            '(' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.OpenParen, i, 1) }),
            ')' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.CloseParen, i, 1) }),
            ',' => (EmptyState.Singleton, new[] { new OperatorToken(Operator.Comma, i, 1) }),
            char when char.IsLetter(c) => (IdentifierState.Create(i), Enumerable.Empty<Token>()),
            _ => throw new InvalidCharacterException(c, i)
        };
    }
}

public class NumericState : ITokeniserState
{
    private int _start;
    private int _length;
    private bool _hadDecimal;
    private bool _hadExponent;

    public static NumericState Create(TokeniserState tokeniserState, int i)
    {
        tokeniserState.NumericState._start = i;
        tokeniserState.NumericState._hadDecimal = false;
        tokeniserState.NumericState._hadExponent = false;
        tokeniserState.NumericState._length = 1;
        return tokeniserState.NumericState;
    }

    public (ITokeniserState, IEnumerable<Token>) Parse(TokeniserState tokeniserState, string expression, int i)
    {
        char c = expression[i];
        return c switch
        {
            char when char.IsDigit(c) => Continue(),
            '.' when !_hadDecimal && !_hadExponent => AddDecimal(),
            'e' when !_hadExponent => AddExponent(),
            _ => Complete(tokeniserState, expression, i)
        };
    }

    private (ITokeniserState, IEnumerable<Token>) AddDecimal()
    {
        _hadDecimal = true;
        return Continue();
    }

    private (ITokeniserState, IEnumerable<Token>) AddExponent()
    {
        _hadExponent = true;
        return Continue();
    }

    private (ITokeniserState, IEnumerable<Token>) Continue()
    {
        _length++;
        return (this, Enumerable.Empty<Token>());
    }

    private (ITokeniserState, IEnumerable<Token>) Complete(TokeniserState tokeniserState, string expression, int i)
    {
        Token numberToken = Complete(expression);
        var (nextState, nextTokens) = EmptyState.Singleton.Parse(tokeniserState, expression, i);
        return (nextState, nextTokens.Prepend(numberToken));
    }

    public Token Complete(string expression)
    {
        double value = double.Parse(expression.AsSpan().Slice(_start, _length));
        var numberToken = new NumberLiteralToken(value, _start, _length);
        return numberToken;
    }
}

public class IdentifierState : ITokeniserState
{
    private readonly static IdentifierState _singleton = new IdentifierState();
    private int _start;
    private int _length;

    public static IdentifierState Create(int i)
    {
        _singleton._start = i;
        _singleton._length = 1;
        return _singleton;
    }

    public Token Complete(string expression)
    {
        var identifierToken = new IdentifierToken(expression.Substring(_start, _length).ToString(), _start, _length);
        return identifierToken;
    }

    public (ITokeniserState, IEnumerable<Token>) Parse(TokeniserState tokeniserState, string expression, int i)
    {
        char c = expression[i];
        return c switch
        {
            char when char.IsLetterOrDigit(c) => Continue(),
            _ => Complete(tokeniserState, expression, i),
        };
    }

    private (ITokeniserState, IEnumerable<Token>) Complete(TokeniserState tokeniserState, string expression, int i)
    {
        Token identifierToken = Complete(expression);
        var (nextState, nextTokens) = EmptyState.Singleton.Parse(tokeniserState, expression, i);
        return (nextState, nextTokens.Prepend(identifierToken));
    }

    private (ITokeniserState, IEnumerable<Token>) Continue()
    {
        _length++;
        return (this, Enumerable.Empty<Token>());
    }
}
