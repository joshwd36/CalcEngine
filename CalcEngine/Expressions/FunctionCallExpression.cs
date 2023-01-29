using CalcEngine.Check;
using CalcEngine.Functions;

namespace CalcEngine.Expressions;

public record FunctionCallExpression(string FunctionName, IReadOnlyList<Expr> Arguments, int ConstantIndex) : Expr
{
    public override string Format(IReadOnlyList<string> variables)
    {
        var joined = string.Join(", ", Arguments.Select(a => a.Format(variables)));
        return $"({FunctionName}({joined}))";
    }

    public override TypedExpr TypeCheck(ExprType expectedType, TypedVariable[] typedVariables, IReadOnlyList<string> variables, object[] constants, FunctionRegistry functionRegistry)
    {
        ExprType[] argumentTypes = new ExprType[Arguments.Count];
        TypedExpr[] typedArgs = new TypedExpr[Arguments.Count];
        List<int> anys = new();
        for (int i = 0; i < Arguments.Count; i++)
        {
            typedArgs[i] = Arguments[i].TypeCheck(ExprType.Any, typedVariables, variables, constants, functionRegistry);
            argumentTypes[i] = typedArgs[i].Type;
            if (typedArgs[i].Type == ExprType.Any)
            {
                anys.Add(i);
            }
        }
        FunctionEntry function = functionRegistry.GetFunction(FunctionName, argumentTypes);
        if (expectedType == ExprType.Any || function.ReturnType == expectedType)
        {
            foreach (int i in anys)
            {
                ExprType functionArgType = function.ArgumentTypes[i];
                typedArgs[i] = Arguments[i].TypeCheck(functionArgType, typedVariables, variables, constants, functionRegistry);
            }
            if (Arguments.Count != function.ArgumentTypes.Count)
            {
                throw new Exception($"Function {FunctionName} expects {function.ArgumentTypes.Count} arguments, found {Arguments.Count}");
            }
            constants[ConstantIndex] = function.Delegate;
            return new TypedFunctionCallExpr(function, typedArgs, function.ReturnType, ConstantIndex);
        }
        else
        {
            throw new InvalidTypeException(expectedType, function.ReturnType);
        }
    }
}
