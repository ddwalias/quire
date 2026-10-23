using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public static partial class StackBehaviourExtensions
{
    public static int GetPushCount(this StackBehaviour behaviour, object operand = null)
    {
        switch (behaviour)
        {
            case StackBehaviour.Push0:
                return 0;
            case StackBehaviour.Push1:
                return 1;
            case StackBehaviour.Push1_push1:
                return 2;
            case StackBehaviour.Pushi:
            case StackBehaviour.Pushi8:
            case StackBehaviour.Pushr4:
            case StackBehaviour.Pushr8:
            case StackBehaviour.Pushref:
                return 1;
            case StackBehaviour.Varpush:
                if (operand is MethodInfo methodInfo)
                {
                    return (!(methodInfo.ReturnType == typeof(void))) ? 1 : 0;
                }
                if (operand is ConstructorInfo)
                {
                    return 1;
                }
                return 1;
            default:
                return 0;
        }
    }

    public static int GetPopCount(this StackBehaviour behaviour, OpCode opCode, object operand = null)
    {
        // Up-front locals: the Release IL differs when they move to first use.
        MethodInfo methodInfo;
        ConstructorInfo constructorInfo;
        MethodInfo methodOperand;
        int num;
        switch (behaviour)
        {
            case StackBehaviour.Pop0:
                return 0;
            case StackBehaviour.Pop1:
                return 1;
            case StackBehaviour.Pop1_pop1:
                return 2;
            case StackBehaviour.Popi:
            case StackBehaviour.Popref:
                return 1;
            case StackBehaviour.Popref_pop1:
            case StackBehaviour.Popref_popi:
                return 2;
            case StackBehaviour.Popref_popi_popi:
            case StackBehaviour.Popref_popi_popi8:
            case StackBehaviour.Popref_popi_popr4:
            case StackBehaviour.Popref_popi_popr8:
            case StackBehaviour.Popref_popi_popref:
                return 3;
            case StackBehaviour.Varpop:
                if (IsDelegateInvocationOpCode(opCode))
                {
                    methodOperand = operand as MethodInfo;
                    if (methodOperand is not null && IsDelegateInvoke(methodOperand))
                    {
                        return 1 + methodOperand.GetParameters().Length;
                    }
                }
                methodInfo = operand as MethodInfo;
                if (methodInfo is not null)
                {
                    num = methodInfo.GetParameters().Length;
                    if (!methodInfo.IsStatic && methodInfo.DeclaringType != null)
                    {
                        num++;
                    }
                    return num;
                }
                constructorInfo = operand as ConstructorInfo;
                if (constructorInfo is not null)
                {
                    return constructorInfo.GetParameters().Length;
                }
                return 0;
            default:
                return 0;
        }
    }

    private static bool IsDelegateInvoke(MethodInfo method)
    {
        if (method.DeclaringType != null && method.Name == "Invoke")
        {
            return typeof(Delegate).IsAssignableFrom(method.DeclaringType);
        }
        return false;
    }

    private static bool IsDelegateInvocationOpCode(OpCode opCode)
    {
        if (!(opCode == OpCodes.Callvirt) && !(opCode == OpCodes.Call) && !(opCode == OpCodes.Ldftn))
        {
            return opCode == OpCodes.Ldvirtftn;
        }
        return true;
    }
}
