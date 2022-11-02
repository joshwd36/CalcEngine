using CalcEngine.Expressions;
using CalcEngine.Parse;
using CalcEngine.Tokenise;
using Xunit;

namespace CalcEngine.Tests;

public class ParserTests
{
    [Fact]
    public void SingleNumber()
    {
        var parser = new Parser();

        var expression = parser.Parse("1");
        Assert.Equal(new NumberLiteralExpression(1), expression.Expression);
        Assert.Equal("(1)", expression.Expression.ToString());
    }

    [Fact]
    public void AdditionAndSubtraction()
    {
        var parser = new Parser();

        var expression = parser.Parse("1 + 2 * 3");
        Assert.Equal(new InfixExpression(new NumberLiteralExpression(1), Operator.Addition, new InfixExpression(new NumberLiteralExpression(2), Operator.Multiplication, new NumberLiteralExpression(3))), expression.Expression);
        Assert.Equal("((1) + ((2) * (3)))", expression.Expression.ToString());
    }

    [Fact]
    public void NegativeLiteral()
    {
        var parser = new Parser();

        var expression = parser.Parse("-1");
        Assert.Equal(new NegativeExpression(new NumberLiteralExpression(1)), expression.Expression);
        Assert.Equal("(-(1))", expression.Expression.ToString());
    }
}
