using CalcEngine.Check;
using CalcEngine.Functions;
using CalcEngine.Parse;

namespace CalcEngine.Expressions;

public record FunctionCallExpression(string FunctionName, IReadOnlyList<int> Arguments, int ConstantIndex) : Expr
{
    public override string Format(IReadOnlyList<Expr> expressions, IReadOnlyList<string> variables)
    {
        var joined = string.Join(", ", Arguments.Select(a => expressions[a].Format(expressions, variables)));
        return $"({FunctionName}({joined}))";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedExpr[] typedExpressions, TypedVariable[] typedVariables, object[] constants, ParseResult parseResult, FunctionRegistry functionRegistry)
    {
        ExprType[] argumentTypes = new ExprType[Arguments.Count];
        List<int> anys = new();
        for (int i = 0; i < Arguments.Count; i++)
        {
            int argumentIndex = Arguments[i];

            TypedExpr typedArgument = parseResult.Expressions[argumentIndex].TypeCheck(ExprType.Any, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            typedExpressions[argumentIndex] = typedArgument;
            argumentTypes[i] = typedArgument.Type;
            if (typedArgument.Type == ExprType.Any)
            {
                anys.Add(i);
            }
        }
        FunctionEntry function = functionRegistry.GetFunction(FunctionName, argumentTypes);
        if (expectedType == ExprType.Any || function.ReturnType == expectedType)
        {
            foreach (int i in anys)
            {
                int argumentIndex = Arguments[i];
                ExprType functionArgType = function.ArgumentTypes[i];
                typedExpressions[argumentIndex] = parseResult.Expressions[argumentIndex].TypeCheck(functionArgType, typedExpressions, typedVariables, constants, parseResult, functionRegistry);
            }
            if (Arguments.Count != function.ArgumentTypes.Count)
            {
                throw new Exception($"Function {FunctionName} expects {function.ArgumentTypes.Count} arguments, found {Arguments.Count}");
            }
            constants[ConstantIndex] = function.Delegate;
            return new TypedFunctionCallExpr(function, Arguments, function.ReturnType, ConstantIndex);
        }
        else
        {
            throw new InvalidTypeException(expectedType, function.ReturnType);
        }
    }
}
