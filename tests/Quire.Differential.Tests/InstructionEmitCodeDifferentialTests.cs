using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using static Quire.Differential.Tests.MplRunSides;
using QuireInstruction = Quire.Mpl.Run.Instruction;

namespace Quire.Differential.Tests;

public class InstructionEmitCodeDifferentialTests
{
    private const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags AllInstance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    [SkippableFact]
    public void instruction_emit_overloads_forward_opcode_and_operand()
    {
        Runtime.Sync();
        Type quireType = typeof(QuireInstruction);
        Type originalType = M("Mpl.Run.Instruction");
        Type quireGeneratorType = typeof(System.Reflection.Emit.Quire.Mpl.ILGenerator);
        Type originalGeneratorType = Mathcad.Type("mplasb", "System.Reflection.Emit.PTC.Mathcad.Mpl.ILGenerator");
        Type quireRecorderType = CreateRecorderType(quireGeneratorType);
        Type originalRecorderType = CreateRecorderType(originalGeneratorType);

        FieldInfo field = typeof(EmitCodeFixture).GetField(nameof(EmitCodeFixture.Field), AllStatic)!;
        ConstructorInfo constructor = typeof(EmitCodeFixture).GetConstructor(Type.EmptyTypes)!;
        CompareInstruction(quireType, originalType, quireRecorderType, originalRecorderType,
            "no operand", new Type[] { typeof(OpCode) }, new object?[] { OpCodes.Nop });
        CompareInstruction(quireType, originalType, quireRecorderType, originalRecorderType,
            "type operand", new Type[] { typeof(OpCode), typeof(Type) }, new object?[] { OpCodes.Box, typeof(string) });
        CompareInstruction(quireType, originalType, quireRecorderType, originalRecorderType,
            "integer operand", new Type[] { typeof(OpCode), typeof(int) }, new object?[] { OpCodes.Ldc_I4, 731 });
        CompareInstruction(quireType, originalType, quireRecorderType, originalRecorderType,
            "field operand", new Type[] { typeof(OpCode), typeof(FieldInfo) }, new object?[] { OpCodes.Ldfld, field });
        CompareInstruction(quireType, originalType, quireRecorderType, originalRecorderType,
            "constructor operand", new Type[] { typeof(OpCode), typeof(ConstructorInfo) }, new object?[] { OpCodes.Newobj, constructor });
    }

    [SkippableFact]
    public void instruction_sequence_emits_each_instruction_in_order()
    {
        Runtime.Sync();
        Type quireType = typeof(QuireInstruction);
        Type originalType = M("Mpl.Run.Instruction");
        Type quireSeqType = typeof(Quire.Mpl.Run.InstructionSeq);
        Type originalSeqType = M("Mpl.Run.InstructionSeq");
        Type quireGeneratorType = typeof(System.Reflection.Emit.Quire.Mpl.ILGenerator);
        Type originalGeneratorType = Mathcad.Type("mplasb", "System.Reflection.Emit.PTC.Mathcad.Mpl.ILGenerator");
        Type quireRecorderType = CreateRecorderType(quireGeneratorType);
        Type originalRecorderType = CreateRecorderType(originalGeneratorType);
        FieldInfo field = typeof(EmitCodeFixture).GetField(nameof(EmitCodeFixture.Field), AllStatic)!;
        ConstructorInfo constructor = typeof(EmitCodeFixture).GetConstructor(Type.EmptyTypes)!;
        (object qSequence, object mSequence) = CreateSequences(quireType, originalType, quireSeqType, originalSeqType,
            field, constructor);
        MethodInfo qEmit = DeclaredEmit(quireSeqType);
        MethodInfo mEmit = DeclaredEmit(originalSeqType);
        object qRecorder = Activator.CreateInstance(quireRecorderType)!;
        object mRecorder = Activator.CreateInstance(originalRecorderType)!;
        MethodComparison.Compare("sequence with all operand forms", "Mpl.Run.InstructionSeq", qEmit, qSequence,
            new object?[] { qRecorder }, mEmit, mSequence, new object?[] { mRecorder },
            RenderRecorder, RenderRecorder, RenderSequence, RenderSequence);
    }

    private static void CompareInstruction(Type quireType, Type originalType, Type quireRecorderType,
        Type originalRecorderType, string caseId, Type[] signature, object?[] values)
    {
        MethodInfo qFactory = quireType.GetMethod("Op", AllStatic, null, signature, null)!;
        MethodInfo mFactory = originalType.GetMethod("Op", AllStatic, null, signature, null)!;
        object qInstruction = qFactory.Invoke(null, values)!;
        object mInstruction = mFactory.Invoke(null, values)!;
        MethodInfo qEmit = DeclaredEmit(qInstruction.GetType());
        MethodInfo mEmit = DeclaredEmit(mInstruction.GetType());
        object qRecorder = Activator.CreateInstance(quireRecorderType)!;
        object mRecorder = Activator.CreateInstance(originalRecorderType)!;
        MethodComparison.Compare(caseId, "Mpl.Run.Instruction", qEmit, qInstruction,
            new object?[] { qRecorder }, mEmit, mInstruction, new object?[] { mRecorder },
            RenderRecorder, RenderRecorder, RenderInstruction, RenderInstruction);
    }

    private static (object, object) CreateSequences(Type quireInstruction, Type originalInstruction,
        Type quireSequence, Type originalSequence, FieldInfo field, ConstructorInfo constructor)
    {
        Type[][] signatures =
        {
            new Type[] { typeof(OpCode) }, new Type[] { typeof(OpCode), typeof(Type) },
            new Type[] { typeof(OpCode), typeof(int) }, new Type[] { typeof(OpCode), typeof(FieldInfo) },
            new Type[] { typeof(OpCode), typeof(ConstructorInfo) }
        };
        object?[][] values =
        {
            new object?[] { OpCodes.Nop }, new object?[] { OpCodes.Box, typeof(string) },
            new object?[] { OpCodes.Ldc_I4, 731 }, new object?[] { OpCodes.Ldfld, field },
            new object?[] { OpCodes.Newobj, constructor }
        };
        Array qItems = Array.CreateInstance(quireInstruction, signatures.Length);
        Array mItems = Array.CreateInstance(originalInstruction, signatures.Length);
        for (int i = 0; i < signatures.Length; i++)
        {
            qItems.SetValue(quireInstruction.GetMethod("Op", AllStatic, null, signatures[i], null)!.Invoke(null, values[i]), i);
            mItems.SetValue(originalInstruction.GetMethod("Op", AllStatic, null, signatures[i], null)!.Invoke(null, values[i]), i);
        }
        ConstructorInfo qConstructor = quireSequence.GetConstructor(AllInstance | BindingFlags.DeclaredOnly, null,
            new Type[] { qItems.GetType() }, null)!;
        ConstructorInfo mConstructor = originalSequence.GetConstructor(AllInstance | BindingFlags.DeclaredOnly, null,
            new Type[] { mItems.GetType() }, null)!;
        return (qConstructor.Invoke(new object?[] { qItems }), mConstructor.Invoke(new object?[] { mItems }));
    }

    private static MethodInfo DeclaredEmit(Type type) =>
        type.GetMethod("EmitCode", AllInstance | BindingFlags.DeclaredOnly)!;

    private static Type CreateRecorderType(Type baseType)
    {
        AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName("InstructionEmitRecorder" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.Run);
        ModuleBuilder module = assembly.DefineDynamicModule("main");
        TypeBuilder type = module.DefineType("RecordingILGenerator", TypeAttributes.Public | TypeAttributes.Sealed,
            baseType);
        FieldBuilder calls = type.DefineField("Calls", typeof(List<string>), FieldAttributes.Public);
        ConstructorInfo baseCtor = baseType.GetConstructor(Type.EmptyTypes)!;
        ConstructorBuilder ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
            Type.EmptyTypes);
        ILGenerator ctorIl = ctor.GetILGenerator();
        ctorIl.Emit(OpCodes.Ldarg_0);
        ctorIl.Emit(OpCodes.Call, baseCtor);
        ctorIl.Emit(OpCodes.Ldarg_0);
        ctorIl.Emit(OpCodes.Newobj, typeof(List<string>).GetConstructor(Type.EmptyTypes)!);
        ctorIl.Emit(OpCodes.Stfld, calls);
        ctorIl.Emit(OpCodes.Ret);

        DefineRecorderOverride(type, calls, baseType, new Type[] { typeof(OpCode) }, "none");
        DefineRecorderOverride(type, calls, baseType, new Type[] { typeof(OpCode), typeof(Type) }, "type");
        DefineRecorderOverride(type, calls, baseType, new Type[] { typeof(OpCode), typeof(int) }, "int");
        DefineRecorderOverride(type, calls, baseType, new Type[] { typeof(OpCode), typeof(FieldInfo) }, "field");
        DefineRecorderOverride(type, calls, baseType, new Type[] { typeof(OpCode), typeof(ConstructorInfo) }, "ctor");
        return type.CreateType()!;
    }

    private static void DefineRecorderOverride(TypeBuilder type, FieldBuilder calls, Type baseType, Type[] signature,
        string kind)
    {
        MethodInfo baseMethod = baseType.GetMethod("Emit", BindingFlags.Instance | BindingFlags.Public, null,
            signature, null)!;
        MethodBuilder method = type.DefineMethod(baseMethod.Name,
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
            typeof(void), signature);
        ILGenerator il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, calls);
        il.Emit(OpCodes.Ldstr, kind);
        il.Emit(OpCodes.Ldarg_1);
        if (signature.Length > 1)
        {
            il.Emit(OpCodes.Ldarg_2);
            if (signature[1].IsValueType) il.Emit(OpCodes.Box, signature[1]);
        }
        else
        {
            il.Emit(OpCodes.Ldnull);
        }
        il.Emit(OpCodes.Call, typeof(InstructionEmitCodeDifferentialTests).GetMethod(nameof(Record),
            BindingFlags.Static | BindingFlags.Public)!);
        il.Emit(OpCodes.Ret);
        type.DefineMethodOverride(method, baseMethod);
    }

    public static void Record(List<string> calls, string kind, OpCode opcode, object? operand) =>
        calls.Add(kind + ":" + opcode.Value.ToString("X4") + ":" + OperandText(operand));

    private static string RenderRecorder(object? value)
    {
        if (value == null) return "null";
        FieldInfo calls = value.GetType().GetField("Calls", BindingFlags.Public | BindingFlags.Instance)!;
        return string.Join("|", (List<string>)calls.GetValue(value)!);
    }

    private static string RenderInstruction(object value)
    {
        List<string> fields = new();
        for (Type? type = value.GetType(); type != null; type = type.BaseType)
        {
            foreach (FieldInfo field in type.GetFields(AllInstance | BindingFlags.DeclaredOnly))
            {
                if (field.IsStatic) continue;
                object? fieldValue = field.GetValue(value);
                fields.Add(field.Name + "=" + (fieldValue is OpCode opcode
                    ? opcode.Value.ToString("X4") : OperandText(fieldValue)));
            }
        }
        fields.Sort(StringComparer.Ordinal);
        return string.Join(";", fields);
    }

    private static string RenderSequence(object value)
    {
        FieldInfo operations = value.GetType().GetField("m_ops", AllInstance | BindingFlags.DeclaredOnly)!;
        IEnumerable items = (IEnumerable)operations.GetValue(value)!;
        return string.Join("|", items.Cast<object>().Select(RenderInstruction));
    }

    private static string OperandText(object? operand) => operand switch
    {
        null => "null",
        Type type => type.AssemblyQualifiedName!,
        MemberInfo member => member.DeclaringType!.FullName + "." + member.Name,
        _ => operand.ToString()!,
    };

    private sealed class EmitCodeFixture
    {
        public static int Field;
        public EmitCodeFixture() { }
    }
}
