using CalcEngine.Tokenise;
using Xunit;

namespace CalcEngine.Tests;

public class TokeniserTests
{
    [Fact]
    public void Literal()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("1");
        Assert.Equal(new[] { new NumberLiteralToken(1, 0, 1) }, tokens);
    }

    [Fact]
    public void NegativeLiteral()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("-1");
        Assert.Equal(new Token[] { new OperatorToken(Operator.Subtraction, 0, 1), new NumberLiteralToken(1, 1, 1) }, tokens);
    }

    [Fact]
    public void Variable()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("a");
        Assert.Equal(new Token[] { new IdentifierToken("a", 0, 1) }, tokens);
    }
}
