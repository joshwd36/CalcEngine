using System;
using System.Collections.Generic;
using CalcEngine.Compile;
using Xunit;

namespace CalcEngine.Tests;

public class CompilerTests
{
    [Fact]
    public void BasicAddition()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + 2");
        Assert.Equal(3, result.Execute(new Dictionary<string, object>()));
    }

    [Fact]
    public void BasicAdditionWithParameter()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a");
        Assert.Equal(3, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(3, result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void BasicCalculationWithMultipleParameters()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a * b");
        Assert.Equal(-9, result.Execute(new Dictionary<string, object> { { "a", 2 }, { "b", -5 } }));
        Assert.Equal(-9, result.Execute(new object[] { 2.0, -5.0 }));
    }

    [Fact]
    public void BasicCalculationWithRepeatedParameters()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a * a");
        Assert.Equal(5, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(5, result.Execute(new object[] { 2.0 }));
    }
}
