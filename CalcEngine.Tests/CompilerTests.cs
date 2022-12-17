using CalcEngine.Compile;
using System.Collections.Generic;
using Xunit;

namespace CalcEngine.Tests;

public class CompilerTests
{
    [Fact]
    public void BasicAddition()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + 2");
        Assert.Equal(3.0, result.Execute(new Dictionary<string, object>()));
    }

    [Fact]
    public void BasicAdditionWithParameter()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a");
        Assert.Equal(3.0, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(3.0, result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void BasicCalculationWithMultipleParameters()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a * b");
        Assert.Equal(-9.0, result.Execute(new Dictionary<string, object> { { "a", 2 }, { "b", -5 } }));
        Assert.Equal(-9.0, result.Execute(new object[] { 2.0, -5.0 }));
    }

    [Fact]
    public void BasicCalculationWithRepeatedParameters()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a * a");
        Assert.Equal(5.0, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(5.0, result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void BasicCalculationWithFunction()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + pow(a, 2)");
        Assert.Equal(5.0, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(5.0, result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void IfFunctionString()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("IfString(a == 2, 'Hello', 'World')");
        Assert.Equal("Hello", result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal("Hello", result.Execute(new object[] { 2.0 }));
    }
}
