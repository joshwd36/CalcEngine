using System.Linq.Expressions;
using System.Reflection.Emit;
using CalcEngine.Expressions;
using CalcEngine.Functions;
using CalcEngine.Parse;
using CalcEngine.Tokenise;

namespace CalcEngine.Compile;

public class ILCompiler : ICompiler
{
    private readonly Parser _parser;
    private readonly FunctionRegistry _functions;
    private ExpressionCache? _cache;

    public bool UseCache
    {
        get { return _cache != null; }
        set { if (value) _cache = new ExpressionCache(); else _cache = null; }
    }

    public ILCompiler(FunctionRegistry? functions = null, ExpressionCache? cache = null)
    {
        _parser = new Parser();
        _functions = functions ?? new FunctionRegistry(true);
        _cache = cache;
    }

    public ExpressionResult Compile(string expression)
    {
        if (_cache is not null && _cache.TryGetExpression(expression) is ExpressionResult cached)
        {
            return cached;
        }
        var parsed = _parser.Parse(expression);

        var methodArgs = new Type[parsed.Variables.Count];
        var delegateArgs = new Type[parsed.Variables.Count + 1];
        var doubleType = typeof(double);
        for (int i = 0; i < parsed.Variables.Count; i++)
        {
            methodArgs[i] = doubleType;
            delegateArgs[i] = doubleType;
        }
        delegateArgs[parsed.Variables.Count] = doubleType;
        var method = new DynamicMethod("", typeof(double), methodArgs);

        ILGenerator il = method.GetILGenerator();
        GenerateIl(parsed.Expression, il);
        il.Emit(OpCodes.Ret);

        var delegateType = Expression.GetDelegateType(delegateArgs);

        var result = new ExpressionResult(method.CreateDelegate(delegateType), parsed.Variables);
        if (_cache is not null)
        {
            _cache.AddExpression(expression, result);
        }
        return result;
    }

    private void GenerateIl(Expr expr, ILGenerator il)
    {
        switch (expr)
        {
            case FunctionCallExpression function: FunctionCall(function, il); break;
            case InfixExpression infix: Infix(infix, il); break;
            case NegativeExpression negative: Negative(negative, il); break;
            case NumberLiteralExpression numberLiteral: NumberLiteral(numberLiteral, il); break;
            case VariableExpression variable: Variable(variable, il); break;
            default: throw new ArgumentOutOfRangeException(nameof(expr));
        };
    }

    private void Variable(VariableExpression variable, ILGenerator il)
    {
        il.Emit(OpCodes.Ldarg, variable.Index);
    }

    private void NumberLiteral(NumberLiteralExpression numberLiteral, ILGenerator il)
    {
        il.Emit(OpCodes.Ldc_R8, numberLiteral.Value);
    }

    private void Negative(NegativeExpression negative, ILGenerator il)
    {
        GenerateIl(negative.Expression, il);
        il.Emit(OpCodes.Neg);
    }

    private void Infix(InfixExpression infix, ILGenerator il)
    {
        GenerateIl(infix.Left, il);
        GenerateIl(infix.Right, il);
        var opCode = infix.Operator switch
        {
            Operator.Addition => OpCodes.Add,
            Operator.Subtraction => OpCodes.Sub,
            Operator.Multiplication => OpCodes.Mul,
            Operator.Division => OpCodes.Div,
            _ => throw new ArgumentOutOfRangeException(nameof(infix))
        };
        il.Emit(opCode);
    }

    private void FunctionCall(FunctionCallExpression function, ILGenerator il)
    {
        foreach (var argument in function.Arguments)
        {
            GenerateIl(argument, il);
        }
        il.EmitCall(OpCodes.Call, _functions.GetFunction(function.FunctionName), null);
    }
}
