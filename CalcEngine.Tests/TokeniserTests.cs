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

    [Fact]
    public void SingleString()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("'Hello World!'");
        Assert.Equal(new Token[] { new StringLiteralToken("Hello World!", 0, 14) }, tokens);
    }

    [Fact]
    public void DoubleString()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("\"Hello World!\"");
        Assert.Equal(new Token[] { new StringLiteralToken("Hello World!", 0, 14) }, tokens);
    }

    [Fact]
    public void DoubleStringWithSingle()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("\"Hello m'world!\"");
        Assert.Equal(new Token[] { new StringLiteralToken("Hello m'world!", 0, 16) }, tokens);
    }

    [Fact]
    public void BoolTrue()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("true");
        Assert.Equal(new Token[] { new BoolLiteralToken(true, 0, 4) }, tokens);
    }

    [Fact]
    public void BoolFalse()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("false");
        Assert.Equal(new Token[] { new BoolLiteralToken(false, 0, 5) }, tokens);
    }

    [Fact]
    public void OperatorAnd()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("&&");
        Assert.Equal(new Token[] { new OperatorToken(Operator.And, 0, 2) }, tokens);
    }

    [Fact]
    public void OperatorOr()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("||");
        Assert.Equal(new Token[] { new OperatorToken(Operator.Or, 0, 2) }, tokens);
    }

    [Fact]
    public void OperatorEqual()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("==");
        Assert.Equal(new Token[] { new OperatorToken(Operator.Equal, 0, 2) }, tokens);
    }

    [Fact]
    public void OperatorLessThan()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("<");
        Assert.Equal(new Token[] { new OperatorToken(Operator.LessThan, 0, 1) }, tokens);
    }

    [Fact]
    public void OperatorLessThanEqual()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("<=");
        Assert.Equal(new Token[] { new OperatorToken(Operator.LessThanEqual, 0, 2) }, tokens);
    }

    [Fact]
    public void OperatorGreaterThan()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise(">");
        Assert.Equal(new Token[] { new OperatorToken(Operator.GreaterThan, 0, 1) }, tokens);
    }

    [Fact]
    public void OperatorGreaterThanEqual()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise(">=");
        Assert.Equal(new Token[] { new OperatorToken(Operator.GreaterThanEqual, 0, 2) }, tokens);
    }

    [Fact]
    public void OperatorLessThanNumber()
    {
        var tokeniser = new Tokeniser();

        var tokens = tokeniser.Tokenise("<5");
        Assert.Equal(new Token[] { new OperatorToken(Operator.LessThan, 0, 1), new NumberLiteralToken(5, 1, 1) }, tokens);
    }
}
