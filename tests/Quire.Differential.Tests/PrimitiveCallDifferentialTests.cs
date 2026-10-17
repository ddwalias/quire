using System.Reflection;
using System.Runtime.CompilerServices;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.MplRunSides;

namespace Quire.Differential.Tests;

public class PrimitiveCallDifferentialTests
{
    private const string Contract = "Mpl.Run.Primitive";
    private const int PrimitiveIndex = 1;
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.Static;
    private static readonly System.Type[] ObjectCall = [typeof(int), typeof(object)];
    private static readonly System.Type[] ObjectCall2 = [typeof(int), typeof(object), typeof(object)];
    private static readonly System.Type[] ObjectCall3 = [typeof(int), typeof(object), typeof(object), typeof(object)];
    private static readonly System.Type[] IntCall = [typeof(int), typeof(int)];
    private static readonly System.Type[] IntCall2 = [typeof(int), typeof(int), typeof(int)];
    private static readonly System.Type[] DoubleCall = [typeof(int), typeof(double)];
    private static readonly System.Type[] DoubleCall2 = [typeof(int), typeof(double), typeof(double)];

    [SkippableFact]
    public void Object_call_overloads_forward_arguments_and_return_values()
    {
        object qIdentity = new();
        object mIdentity = new();
        CompareCall("Call", ObjectCall, "identity", [typeof(object)], [qIdentity], [mIdentity],
            value => ReferenceEquals(value, qIdentity) ? "same argument" : Reflect.Text(value),
            value => ReferenceEquals(value, mIdentity) ? "same argument" : Reflect.Text(value));
        object qFirst = new();
        object qSecond = new();
        object mFirst = new();
        object mSecond = new();
        CompareCall("Call", ObjectCall2, "or_ref", [typeof(object), typeof(object)],
            [null, qSecond], [null, mSecond],
            value => ReferenceEquals(value, qSecond) ? "second argument" : Reflect.Text(value),
            value => ReferenceEquals(value, mSecond) ? "second argument" : Reflect.Text(value));
        CompareCall("Call", ObjectCall2, "or_ref", [typeof(object), typeof(object)],
            [qFirst, qSecond], [mFirst, mSecond],
            value => ReferenceEquals(value, qFirst) ? "first argument" : Reflect.Text(value),
            value => ReferenceEquals(value, mFirst) ? "first argument" : Reflect.Text(value));

        object[] qValues = ["before"];
        object[] mValues = ["before"];
        CompareCall("Call", ObjectCall3, "stelem_ref", [typeof(object[]), typeof(int), typeof(object)],
            [qValues, 0, "after"], [mValues, 0, "after"],
            value => ReferenceEquals(value, qValues) ? "same array " + Reflect.Text(value) : Reflect.Text(value),
            value => ReferenceEquals(value, mValues) ? "same array " + Reflect.Text(value) : Reflect.Text(value));
    }

    [SkippableFact]
    public void Numeric_call_overloads_box_invoke_and_unbox_the_expected_types()
    {
        CompareCall("Call", IntCall, "neg_i4", [typeof(int)], [int.MinValue], [int.MinValue]);
        CompareCall("Call", IntCall2, "add_i4", [typeof(int), typeof(int)],
            [int.MaxValue, 1], [int.MaxValue, 1]);
        CompareCall("Call", IntCall2, "sub_i4", [typeof(int), typeof(int)], [11, 3], [11, 3]);
        CompareCall("Call", DoubleCall, "neg_f8", [typeof(double)], [-0.0], [-0.0]);
        CompareCall("Call", DoubleCall2, "sub_f8", [typeof(double), typeof(double)], [4.5, 1.25], [4.5, 1.25]);
        CompareCall("Call_c", DoubleCall, "conv_i4", [typeof(double)], [42.75], [42.75]);
        CompareCall("Call_c", IntCall, "conv_f8", [typeof(int)], [int.MinValue], [int.MinValue]);
    }

    private static void CompareCall(string callName, System.Type[] callParameters, string targetName,
        System.Type[] targetParameters, object?[] quireTargetArgs, object?[] originalTargetArgs,
        Func<object?, string>? quireShow = null, Func<object?, string>? originalShow = null)
    {
        var quireType = typeof(Primitive);
        var originalType = M("Mpl.Run.Primitive");
        var quireMethod = quireType.GetMethod(callName, Static, null, callParameters, null)!;
        var originalMethod = originalType.GetMethod(callName, Static, null,
            targetParameters.Prepend(typeof(int)).ToArray(), null)!;
        var quireTarget = FindTarget(quireType, targetName, targetParameters);
        var originalTarget = FindTarget(originalType, targetName,
            targetParameters);
        object? quireArrayBefore = Reflect.Get(quireType, "m_primitive");
        object? originalArrayBefore = Reflect.Get(originalType, "m_primitive");
        try
        {
            Reflect.Set(quireType, "m_primitive", BuildPrimitiveArray(quireType, quireTarget));
            Reflect.Set(originalType, "m_primitive", BuildPrimitiveArray(originalType, originalTarget));
            MethodComparison.Compare(callName + " -> " + targetName, Contract, quireMethod, null,
                PrependIndex(quireTargetArgs), originalMethod, null, PrependIndex(originalTargetArgs),
                quireShow ?? Reflect.Text, originalShow ?? Reflect.Text);
        }
        finally
        {
            Reflect.Set(quireType, "m_primitive", quireArrayBefore);
            Reflect.Set(originalType, "m_primitive", originalArrayBefore);
        }
    }

    private static MethodInfo FindTarget(System.Type type, string name, System.Type[] parameters)
    {
        return type.GetMethod(name, Static, null, parameters, null)!;
    }

    private static object BuildPrimitiveArray(System.Type primitiveType, MethodInfo target)
    {
        var items = Array.CreateInstance(primitiveType, PrimitiveIndex + 1);
        object primitive = RuntimeHelpers.GetUninitializedObject(primitiveType);
        Reflect.Set(primitive, "m_method", target);
        items.SetValue(primitive, PrimitiveIndex);
        return items;
    }

    private static object?[] PrependIndex(object?[] arguments) => [PrimitiveIndex, .. arguments];

}
