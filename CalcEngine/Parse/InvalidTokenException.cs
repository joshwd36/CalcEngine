using CalcEngine.Tokenise;

namespace CalcEngine.Parse;

public class InvalidTokenException : Exception
{
    public Token Token { get; }

    public InvalidTokenException(Token token) : base($"Unexpected token {token} at position {token.Start}")
    {
        Token = token;
    }
}
