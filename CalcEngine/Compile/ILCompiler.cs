using System.Reflection;
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
    private delegate double F(object[] parameters);
    private static readonly Type[] _methodArgs = new[] { typeof(object[]) };
    [ThreadStatic]
    private static DynamicMethod? _method;
    private static readonly MethodInfo _convertDouble = typeof(Convert).GetMethod(nameof(Convert.ToDouble), new[] { typeof(object) })!;

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
        _method ??= new("", typeof(double), _methodArgs);

        ILGenerator il = _method.GetILGenerator();
        GenerateIl(parsed.Expressions[parsed.Root], parsed.Expressions, il);
        il.Emit(OpCodes.Ret);

        var result = new ExpressionResult(_method.CreateDelegate<Func<object[], double>>(), parsed.Variables);
        if (_cache is not null)
        {
            _cache.AddExpression(expression, result);
        }
        return result;
    }

    private void GenerateIl(Expr expr, IReadOnlyList<Expr> expressions, ILGenerator il)
    {
        switch (expr)
        {
            case FunctionCallExpression function: FunctionCall(function, expressions, il); break;
            case InfixExpression infix: Infix(infix, expressions, il); break;
            case NegativeExpression negative: Negative(negative, expressions, il); break;
            case NumberLiteralExpression numberLiteral: NumberLiteral(numberLiteral, il); break;
            case VariableExpression variable: Variable(variable, il); break;
            default: throw new ArgumentOutOfRangeException(nameof(expr));
        };
    }

    private void Variable(VariableExpression variable, ILGenerator il)
    {
        il.Emit(OpCodes.Ldarg, 0);
        il.Emit(OpCodes.Ldc_I4, variable.Index);
        il.Emit(OpCodes.Ldelem_Ref);
        il.Emit(OpCodes.Call, _convertDouble);
    }

    private void NumberLiteral(NumberLiteralExpression numberLiteral, ILGenerator il)
    {
        il.Emit(OpCodes.Ldc_R8, numberLiteral.Value);
    }

    private void Negative(NegativeExpression negative, IReadOnlyList<Expr> expressions, ILGenerator il)
    {
        GenerateIl(expressions[negative.Expression], expressions, il);
        il.Emit(OpCodes.Neg);
    }

    private void Infix(InfixExpression infix, IReadOnlyList<Expr> expressions, ILGenerator il)
    {
        GenerateIl(expressions[infix.Left], expressions, il);
        GenerateIl(expressions[infix.Right], expressions, il);
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

    private void FunctionCall(FunctionCallExpression function, IReadOnlyList<Expr> expressions, ILGenerator il)
    {
        foreach (var argument in function.Arguments)
        {
            GenerateIl(expressions[argument], expressions, il);
        }
        il.EmitCall(OpCodes.Call, _functions.GetFunction(function.FunctionName), null);
    }
}
