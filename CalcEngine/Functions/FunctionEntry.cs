using CalcEngine.Check;
using System.Reflection;

namespace CalcEngine.Functions;

public record FunctionEntry(MethodInfo MethodInfo, IReadOnlyList<ExprType> ArgumentTypes, ExprType ReturnType);
