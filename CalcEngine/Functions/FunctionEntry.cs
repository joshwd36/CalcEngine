using CalcEngine.Check;

namespace CalcEngine.Functions;

public record FunctionEntry(Delegate Delegate, Type[] DelegateArgs, IReadOnlyList<ExprType> ArgumentTypes, ExprType ReturnType);
