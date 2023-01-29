using CalcEngine.Compile;
using System;
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

        var result = compiler.Compile("ifString(a == 2, 'Hello', 'World')");
        Assert.Equal("Hello", result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal("Hello", result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void GenericIfString()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("if(a == 2, 'Hello', 'World')");
        Assert.Equal("Hello", result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal("Hello", result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void GenericIfBool()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("if(a == 5, false, true)");
        Assert.Equal(true, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(true, result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void GenericIfNumber()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("if(a == 5, 3, 2)");
        Assert.Equal(2.0, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(2.0, result.Execute(new object[] { 2.0 }));
    }

    [Fact]
    public void GenericIfNumberVariable()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("if(a == 5, b, 2)");
        Assert.Equal(2.0, result.Execute(new Dictionary<string, object> { { "a", 2 }, { "b", 3 } }));
        Assert.Equal(2.0, result.Execute(new object[] { 2.0, 3 }));
    }

    [Fact]
    public void Pi()
    {
        var compiler = new ILCompiler();

        var result = compiler.Compile("2 * pi()");
        Assert.Equal(2 * Math.PI, result.Execute(new Dictionary<string, object> { { "a", 2 } }));
        Assert.Equal(2 * Math.PI, result.Execute(new object[] { 2.0 }));
    }
}
