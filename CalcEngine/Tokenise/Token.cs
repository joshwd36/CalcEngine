namespace CalcEngine.Tokenise;

public abstract record Token(int Start, int Length);

public record OperatorToken(Operator Operator, int Start, int Length) : Token(Start, Length);

public enum Operator
{
    Addition,
    Subtraction,
    Division,
    Multiplication,
    Remainder,
    OpenParen,
    CloseParen,
    Comma,
    Not,
    And,
    Or,
    Equal,
    NotEqual,
    LessThan,
    LessThanEqual,
    GreaterThan,
    GreaterThanEqual,
}

public record NumberLiteralToken(double Value, int Start, int Length) : Token(Start, Length);

public record IdentifierToken(string Name, int Start, int Length) : Token(Start, Length);

public record StringLiteralToken(string Value, int Start, int Length) : Token(Start, Length);

public record BoolLiteralToken(bool Value, int Start, int Length) : Token(Start, Length);
