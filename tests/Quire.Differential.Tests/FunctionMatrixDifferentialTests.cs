using static Quire.Differential.Tests.Reflect;
using System.Reflection;
using System.Reflection.Emit;

namespace Quire.Differential.Tests;

public class FunctionMatrixDifferentialTests
{
    [SkippableFact]
    public void Cons_construction_append_and_malformed_tails() => Scenario.Compare((s, log) =>
    {
        var t = s.T("Mpl.Run.Cons");
        var tail = New(t, 2, null);
        var head = New(t, 1, tail);
        log.Log(s, "head", () => Get(head, "head"));
        log.Log(s, "tail identity", () => ReferenceEquals(tail, Get(head, "tail")));
        log.Log(s, "length", () => Call(t, "Length", head));
        log.Log(s, "format", () => head.ToString());
        var append = New(t, 3, null);
        log.Log(s, "append identity", () => ReferenceEquals(head, Call(t, "destructive_append", head, append)));
        log.Log(s, "appended", () => head.ToString());
        log.Log(s, "null append identity", () => ReferenceEquals(append, Call(t, "destructive_append", null, append)));
        foreach (object? value in new object?[] { null, 7, "bad" })
        {
            var malformed = New(t, 1, value);
            log.Log(s, "malformed length", () => Call(t, "Length", malformed));
            log.Log(s, "malformed format", () => malformed.ToString());
            log.Log(s, "malformed append", () => Call(t, "destructive_append", malformed, append));
        }
        log.Log(s, "null head", () => New(t, null, null).ToString());
    });
    private static object Probe(Side side, int purity, int captured, int minimum, bool varargs)
    {
        var baseType = side.T("Mpl.Run.Function");
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("FunctionProbe" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndCollect);
        var type = assembly.DefineDynamicModule("probe").DefineType("Probe", TypeAttributes.Public, baseType);
        var ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, [typeof(int), typeof(int), typeof(int), typeof(bool)]);
        var body = ctor.GetILGenerator();
        body.Emit(OpCodes.Ldarg_0); body.Emit(OpCodes.Ldarg_1); body.Emit(OpCodes.Ldarg_2);
        body.Emit(OpCodes.Call, baseType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, [typeof(int), typeof(int)], null)!);
        body.Emit(OpCodes.Ldarg_0); body.Emit(OpCodes.Ldarg_3);
        body.Emit(OpCodes.Stfld, baseType.GetField("m_minargs", BindingFlags.Instance | BindingFlags.NonPublic)!);
        body.Emit(OpCodes.Ldarg_0); body.Emit(OpCodes.Ldarg_S, (byte)4);
        body.Emit(OpCodes.Stfld, baseType.GetField("m_varargs", BindingFlags.Instance | BindingFlags.NonPublic)!);
        body.Emit(OpCodes.Ret);
        var method = type.DefineMethod("invVec", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig);
        var result = method.DefineGenericParameters("R")[0];
        method.SetReturnType(typeof(void));
        method.SetParameters(result.MakeByRefType(), typeof(object[]));
        method.DefineParameter(1, ParameterAttributes.Out, "result");
        body = method.GetILGenerator();
        body.Emit(OpCodes.Ldarg_1); body.Emit(OpCodes.Ldarg_2); body.Emit(OpCodes.Ldlen); body.Emit(OpCodes.Conv_I4);
        body.Emit(OpCodes.Box, typeof(int)); body.Emit(OpCodes.Unbox_Any, result); body.Emit(OpCodes.Stobj, result); body.Emit(OpCodes.Ret);
        type.DefineMethodOverride(method, baseType.GetMethod("invVec")!);
        return New(type.CreateType()!, purity, captured, minimum, varargs);
    }
    [SkippableFact]
    public void Function_full_application_optional_padding_and_delegates() => Scenario.Compare((s, log) =>
    {
        foreach (var setup in new[] { (3, 0, 3, false), (5, 2, 1, false), (17, 0, 16, false), (19, 0, 17, false), (2, 0, 2, true) })
        {
            var f = Probe(s, setup.Item1, setup.Item2, setup.Item3, setup.Item4);
            log.Log(s, "name", () => Get(f, "Name"));
            log.Log(s, "format", () => f.ToString());
            Set(f, "Name", "probe");
            log.Log(s, "named format", () => f.ToString());
            log.Log(s, "purity", () => Get(f, "Purity"));
            log.Log(s, "minargs", () => Get(f, "MinArgs"));
            foreach (int count in new[] { -1, 0, 1, 2, 3, 16, 17, 19 })
            {
                log.Log(s, "fully " + count, () => Call(f, "IsFullyApplied", count));
                log.Log(s, "assert " + count, () => Call(f, "AssertFullApp", count, 7u));
                if (count >= setup.Item3 || setup.Item4)
                    log.Log(s, "apply " + count, () => CallSig(f, "Apply", ["Object[]"], (object)new object[Math.Max(0, count)]));
            }
            log.Log(s, "delegate1", () => ((Delegate)Call(f, "AsDelegate_1")!).DynamicInvoke(42));
            log.Log(s, "delegate2", () => ((Delegate)Call(f, "AsDelegate_2")!).DynamicInvoke(42, 43));
        }
    });
    [SkippableFact]
    public void Function_backed_mapping_and_vectorize() => Scenario.Compare((s, log) =>
    {
        var matrixType = s.T("Mcd.MatVal");
        var source = New(matrixType, new double[] { 1, 2, 3, 4 }, null, 2, 2);
        var unary = Call(matrixType, "map", Probe(s, 1, 0, 1, false), source)!;
        log.Add("unary " + MatValDifferentialTests.Storage(s, unary));
        var binary = Call(matrixType, "map", Probe(s, 2, 0, 2, false), source, source)!;
        log.Add("binary " + MatValDifferentialTests.Storage(s, binary));
        foreach (int count in new[] { 1, 2, 3, 5, 16, 17, 19 })
        {
            object[] args = Enumerable.Range(0, count).Select(i => i == 0 ? source : s.Num(i, 0, "poly")).ToArray();
            var result = Call(matrixType, "vectorize", Probe(s, count, 0, count, false), (object)args)!;
            log.Add("vectorized " + count + " " + MatValDifferentialTests.Storage(s, result));
            log.Add("final argument " + s.Show(args[0]));
        }
    });
}
