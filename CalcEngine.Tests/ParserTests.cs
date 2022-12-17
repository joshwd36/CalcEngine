using CalcEngine.Parse;
using Xunit;

namespace CalcEngine.Tests;

public class ParserTests
{
    [Fact]
    public void SingleNumber()
    {
        var parser = new Parser();

        var expression = parser.Parse("1");
        Assert.Equal("(1)", expression.ToString());
    }

    [Fact]
    public void AdditionAndSubtraction()
    {
        var parser = new Parser();

        var expression = parser.Parse("1 + 2 * 3");
        Assert.Equal("((1) + ((2) * (3)))", expression.ToString());
    }

    [Fact]
    public void NegativeLiteral()
    {
        var parser = new Parser();

        var expression = parser.Parse("-1");
        Assert.Equal("(-(1))", expression.ToString());
    }
}
