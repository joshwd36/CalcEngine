using System.Linq.Expressions;
using CalcEngine.Expressions;
using CalcEngine.Functions;
using CalcEngine.Parse;
using CalcEngine.Tokenise;

namespace CalcEngine.Compile;

public class Compiler : ICompiler
{
    private readonly Parser _parser;
    private readonly FunctionRegistry _functions;
    private readonly ExpressionCache _cache;
    public bool UseCache { get; set; } = true;

    public Compiler(FunctionRegistry? functions = null, ExpressionCache? cache = null)
    {
        _parser = new Parser();
        _functions = functions ?? new FunctionRegistry(true);
        _cache = cache ?? new ExpressionCache();
    }

    public ExpressionResult Compile(string expression)
    {
        if (UseCache && _cache.TryGetExpression(expression) is ExpressionResult cached)
        {
            return cached;
        }
        var parsed = _parser.Parse(expression);

        var parameters = new ParameterExpression[parsed.Variables.Count];
        var generated = GenerateExpression(parsed.Expression, parameters);
        while (generated.CanReduce)
        {
            generated = generated.Reduce();
        }
        var lambda = Expression.Lambda(generated, parameters);
        var result = new ExpressionResult(lambda.Compile(), parsed.Variables);
        if (UseCache)
        {
            _cache.AddExpression(expression, result);
        }
        return result;
    }

    private Expression GenerateExpression(Expr expr, ParameterExpression[] parameters)
    {
        return expr switch
        {
            FunctionCallExpression function => FunctionCall(function, parameters),
            InfixExpression infix => Infix(infix, parameters),
            NegativeExpression negative => Negative(negative, parameters),
            NumberLiteralExpression numberLiteral => NumberLiteral(numberLiteral),
            VariableExpression variable => Variable(variable, parameters),
            _ => throw new ArgumentOutOfRangeException(nameof(expr)),
        };
    }

    private Expression FunctionCall(FunctionCallExpression expression, ParameterExpression[] parameters)
    {
        var function = _functions.GetFunction(expression.FunctionName);
        return Expression.Call(function, expression.Arguments.Select(a => GenerateExpression(a, parameters)));
    }

    private Expression Infix(InfixExpression expression, ParameterExpression[] parameters)
    {
        Expression left = GenerateExpression(expression.Left, parameters);
        Expression right = GenerateExpression(expression.Right, parameters);
        return expression.Operator switch
        {
            Operator.Addition => Expression.Add(left, right),
            Operator.Subtraction => Expression.Subtract(left, right),
            Operator.Multiplication => Expression.Multiply(left, right),
            Operator.Division => Expression.Divide(left, right),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    private Expression Negative(NegativeExpression expression, ParameterExpression[] parameters)
    {
        return Expression.Negate(GenerateExpression(expression.Expression, parameters));
    }

    private static Expression NumberLiteral(NumberLiteralExpression expression)
    {
        return Expression.Constant(expression.Value, typeof(double));
    }

    private static Expression Variable(VariableExpression expression, ParameterExpression[] parameters)
    {
        if (parameters[expression.Index] is ParameterExpression existing)
        {
            return existing;
        }
        else
        {
            var parameter = Expression.Parameter(typeof(double), expression.Identifier);
            parameters[expression.Index] = parameter;
            return parameter;
        }
    }
}
