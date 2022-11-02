namespace CalcEngine.Tokenise;

public abstract record Token
{
    protected Token(int start, int length)
    {
        Start = start;
        Length = length;
    }

    public int Start { get; }
    public int Length { get; }
}

public record OperatorToken : Token
{
    public OperatorToken(Operator op, int start, int length) : base(start, length)
    {
        Operator = op;
    }

    public Operator Operator { get; }
}

public enum Operator
{
    Addition,
    Subtraction,
    Division,
    Multiplication,
    OpenParen,
    CloseParen,
    Comma,
}

public record NumberLiteralToken : Token
{
    public NumberLiteralToken(double value, int start, int length) : base(start, length)
    {
        Value = value;
    }

    public double Value { get; }
}

public record IdentifierToken : Token
{
    public IdentifierToken(string name, int start, int length) : base(start, length)
    {
        Name = name;
    }

    public string Name { get; }
}
