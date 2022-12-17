using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record FunctionCallExpression(string FunctionName, IReadOnlyList<int> Arguments) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        var joined = string.Join(", ", Arguments.Select(a => expressions[a].Format(expressions, variables)));
        return $"({FunctionName}({joined}))";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        FunctionEntry function = functionRegistry.GetFunction(FunctionName);
        if (expectedType == ExprType.Any || function.ReturnType == expectedType)
        {
            if (Arguments.Count != function.ArgumentTypes.Count)
            {
                throw new Exception($"Function {FunctionName} expects {function.ArgumentTypes.Count} arguments, found {Arguments.Count}");
            }
            for (int i = 0; i < Arguments.Count; i++)
            {
                int argumentIndex = Arguments[i];
                ExprType argumentExpectedType = function.ArgumentTypes[i];

                typedExpressions[argumentIndex] = parseResult.Expressions[argumentIndex].TypeCheck(argumentExpectedType, typedExpressions, typedVariables, parseResult, functionRegistry);
            }
            return new TypedFunctionCallExpr(function.MethodInfo, Arguments, function.ReturnType);
        }
        else
        {
            throw new InvalidTypeException(expectedType, function.ReturnType);
        }
    }
}
