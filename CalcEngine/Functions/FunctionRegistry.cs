using CalcEngine.Check;
using System.Reflection;

namespace CalcEngine.Functions;

public class FunctionRegistry
{
    private readonly Dictionary<string, FunctionEntry> _functions;

    private static string IfString(bool condition, string ifTrue, string ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    private static double IfNumber(bool condition, double ifTrue, double ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    private static bool IfBool(bool condition, bool ifTrue, bool ifFalse)
    {
        return condition ? ifTrue : ifFalse;
    }

    public FunctionRegistry(bool addDefault = true)
    {
        _functions = new Dictionary<string, FunctionEntry>();
        if (addDefault)
        {
            AddFunction("sqrt", typeof(Math).GetMethod(nameof(Math.Sqrt))!);
            AddFunction("pow", typeof(Math).GetMethod(nameof(Math.Pow))!);
            AddFunction("IfString", typeof(FunctionRegistry).GetMethod(nameof(IfString), BindingFlags.NonPublic | BindingFlags.Static)!);
            AddFunction("IfNumber", typeof(FunctionRegistry).GetMethod(nameof(IfNumber), BindingFlags.NonPublic | BindingFlags.Static)!);
            AddFunction("IfBool", typeof(FunctionRegistry).GetMethod(nameof(IfBool), BindingFlags.NonPublic | BindingFlags.Static)!);
        }
    }

    public void AddFunction(string name, MethodInfo method)
    {
        List<ExprType> parameterTypes = method.GetParameters().Select(p => GetDataType(p.ParameterType)).ToList();
        ExprType returnType = GetDataType(method.ReturnType);
        _functions.Add(name, new FunctionEntry(method, parameterTypes, returnType));
    }

    private static ExprType GetDataType(Type type)
    {
        if (type == typeof(double))
        {
            return ExprType.Number;
        }
        else if (type == typeof(string))
        {
            return ExprType.String;
        }
        else if (type == typeof(bool))
        {
            return ExprType.Bool;
        }
        else
        {
            throw new Exception($"Cannot use method with type {type}");
        }
    }

    public void AddFunction<T1>(string name, Func<T1> method)
    {
        AddFunction(name, method.Method);
    }

    public void AddFunction<T1, T2>(string name, Func<T1, T2> method)
    {
        AddFunction(name, method.Method);
    }

    public void AddFunction<T1, T2, T3>(string name, Func<T1, T2, T3> method)
    {
        AddFunction(name, method.Method);
    }

    public void AddFunction<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4> method)
    {
        AddFunction(name, method.Method);
    }

    public void AddFunction<T1, T2, T3, T4, T5>(string name, Func<T1, T2, T3, T4, T5> method)
    {
        AddFunction(name, method.Method);
    }

    public void AddFunction<T1, T2, T3, T4, T5, T6>(string name, Func<T1, T2, T3, T4, T5, T6> method)
    {
        AddFunction(name, method.Method);
    }

    public FunctionEntry GetFunction(string name)
    {
        if (_functions.TryGetValue(name, out var value))
        {
            return value;
        }
        else
        {
            throw new InvalidFunctionException(name);
        }
    }
}
