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
        Assert.Equal(3, result.Execute(new List<object> { 2 }));
        Assert.Equal(3, result.Execute(2));
        var function = (Func<double, double>)result.Function;
        Assert.Equal(3, function(2));
    }

    [Fact]
    public void BasicCalculationWithMultipleParameters()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a * b");
        Assert.Equal(-9, result.Execute(new Dictionary<string, object> { { "a", 2 }, { "b", -5 } }));
        Assert.Equal(-9, result.Execute(new List<object> { 2, -5 }));
        Assert.Equal(-9, result.Execute(2, -5));
        var function = (Func<double, double, double>)result.Function;
        Assert.Equal(-9, function(2, -5));
    }

    [Fact]
    public void BasicCalculationWithRepeatedParameters()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("1 + a * a");
        Assert.Equal(5, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(5, result.Execute(new List<object> { 2 }));
        Assert.Equal(5, result.Execute(2));
        var function = (Func<double, double>)result.Function;
        Assert.Equal(5, function(2));
    }
}
