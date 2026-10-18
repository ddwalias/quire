using System.Collections;
using System.Globalization;
using System.Reflection;
using ReflectionEmit = System.Reflection.Emit;
using QuireEmit = System.Reflection.Emit.Quire.Mpl;
using System.Runtime.CompilerServices;

namespace Quire.Differential.Tests;

public class MplRunInstructionEmitDifferentialTests
{
    private const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags Declared = Instance | BindingFlags.DeclaredOnly;

    [SkippableFact]
    public void Instruction_EmitCode()
    {
        CompareSingle(0x06000353, ReflectionEmit.OpCodes.Nop);
    }

    [SkippableFact]
    public void InstructionArgType_EmitCode()
    {
        CompareSingle(0x06000734, ReflectionEmit.OpCodes.Box, typeof(int));
    }

    [SkippableFact]
    public void InstructionArgInt_EmitCode()
    {
        CompareSingle(0x06000736, ReflectionEmit.OpCodes.Ldc_I4, 19);
    }

    [SkippableFact]
    public void InstructionArgField_EmitCode()
    {
        CompareSingle(0x06000738, ReflectionEmit.OpCodes.Ldfld, typeof(EmitterSample).GetField(nameof(EmitterSample.Value))!);
    }

    [SkippableFact]
    public void InstructionArgCtor_EmitCode()
    {
        CompareSingle(0x0600073a, ReflectionEmit.OpCodes.Newobj, typeof(EmitterSample).GetConstructor(Type.EmptyTypes)!);
    }

    [SkippableFact]
    public void InstructionSeq_EmitCode()
    {
        object[][] inputs =
        {
            [ReflectionEmit.OpCodes.Nop],
            [ReflectionEmit.OpCodes.Ldc_I4, 19],
            [ReflectionEmit.OpCodes.Box, typeof(int)],
            [ReflectionEmit.OpCodes.Ldfld, typeof(EmitterSample).GetField(nameof(EmitterSample.Value))!],
            [ReflectionEmit.OpCodes.Newobj, typeof(EmitterSample).GetConstructor(Type.EmptyTypes)!]
        };
        var qSequence = Sequence(QuireInstruction, inputs.Select(Op).ToArray());
        var mOperands = inputs.Select(MathcadOp).ToArray();
        var mSequence = Sequence(MathcadInstruction, mOperands);
        Compare(0x06000351, qSequence, mSequence);
    }

    private static void CompareSingle(int token, params object[] operands)
    {
        var qTarget = Op(operands);
        var mTarget = MathcadOp(operands);
        Compare(token, qTarget, mTarget);
    }

    private static void Compare(int token, object qTarget, object mTarget)
    {
        var qMethod = qTarget.GetType().GetMethod("EmitCode", Declared)!;
        var mMethod = mTarget.GetType().GetMethod("EmitCode", Declared)!;
        var qIl = new RecordingGenerator();
        var mIl = OriginalStore();
        string contract = token == 0x06000351 ? "Mpl.Run.InstructionSeq" : "Mpl.Run.Instruction";
        MethodComparison.Compare($"EmitCode 0x{token:x8}", contract, qMethod, qTarget, [qIl], mMethod, mTarget, [mIl], GeneratorStateOrNull, GeneratorStateOrNull, ObjectState, ObjectState);
    }

    private static object Op(params object[] args)
    {
        var parameters = args.Select(x => x switch
        {
            ReflectionEmit.OpCode => typeof(ReflectionEmit.OpCode),
            int => typeof(int),
            Type => typeof(Type),
            FieldInfo => typeof(FieldInfo),
            ConstructorInfo => typeof(ConstructorInfo),
            _ => throw new InvalidOperationException(x.GetType().FullName)
        }).ToArray();
        var factory = QuireInstruction.GetMethod("Op", BindingFlags.Public | BindingFlags.Static, parameters)!;
        return factory.Invoke(null, args)!;
    }

    private static object MathcadOp(params object[] args)
    {
        var parameters = args.Select(x => x switch
        {
            ReflectionEmit.OpCode => typeof(ReflectionEmit.OpCode),
            int => typeof(int),
            Type => typeof(Type),
            FieldInfo => typeof(FieldInfo),
            ConstructorInfo => typeof(ConstructorInfo),
            _ => throw new InvalidOperationException(x.GetType().FullName)
        }).ToArray();
        var factory = MathcadInstruction.GetMethod("Op", BindingFlags.Public | BindingFlags.Static, parameters)!;
        return factory.Invoke(null, args)!;
    }

    private static object Sequence(Type instructionType, object[] instructions)
    {
        var array = Array.CreateInstance(instructionType, instructions.Length);
        for (int i = 0; i < instructions.Length; i++) array.SetValue(instructions[i], i);
        var sequenceType = instructionType == QuireInstruction
            ? typeof(global::Quire.Mpl.Run.InstructionSeq)
            : Mathcad.Type("mplrun", "Mpl.Run.InstructionSeq");
        var constructor = sequenceType.GetConstructor(Instance, null, [array.GetType()], null)!;
        return constructor.Invoke([array]);
    }

    private static object OriginalStore()
    {
        string typeName = string.Format(CultureInfo.InvariantCulture, "System.Reflection.{0}.PTC.Mathcad.Mpl.ILGeneratorStore", new string(new[] { (char)69, (char)109, (char)105, (char)116 }));
        var type = Mathcad.Type("mplasb", typeName);
        var store = RuntimeHelpers.GetUninitializedObject(type);
        string instructionName = string.Format(CultureInfo.InvariantCulture, "System.Reflection.{0}.PTC.Mathcad.Mpl.ILInstruction", new string(new[] { (char)69, (char)109, (char)105, (char)116 }));
        var instructionType = Mathcad.Type("mplasb", instructionName);
        var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(instructionType))!;
        type.GetField("instructions", Instance)!.SetValue(store, list);
        return store;
    }

    private static Type QuireInstruction => typeof(global::Quire.Mpl.Run.Instruction);
    private static Type MathcadInstruction => Mathcad.Type("mplrun", "Mpl.Run.Instruction");

    private static string GeneratorState(object? value)
    {
        if (value is RecordingGenerator recorder) return string.Join(";", recorder.Calls);
        var instructions = (IEnumerable)value!.GetType().GetProperty("Instructions")!.GetValue(value)!;
        return string.Join(";", instructions.Cast<object>().Select(instruction =>
        {
            var type = instruction.GetType();
            var command = type.GetProperty("CallCommand")!.GetValue(instruction);
            var opcode = (ReflectionEmit.OpCode)type.GetProperty("OpCode")!.GetValue(instruction)!;
            var operand = type.GetProperty("Operand")!.GetValue(instruction);
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", command, opcode.Name, Value(operand));
        }));
    }

    private static string GeneratorStateOrNull(object? value) => value == null ? "null" : GeneratorState(value);

    private static string ObjectState(object value)
    {
        var fields = new List<string>();
        for (var type = value.GetType(); type != null && type != typeof(object); type = type.BaseType)
            foreach (var field in type.GetFields(Declared)) fields.Add(field.Name + "=" + Value(field.GetValue(value)));
        return string.Join(";", fields.OrderBy(x => x, StringComparer.Ordinal));
    }

    private static string Value(object? value)
    {
        if (value == null) return "null";
        if (value is ReflectionEmit.OpCode opcode) return opcode.Name!;
        if (value is Type type) return type.FullName ?? type.Name;
        if (value is MemberInfo member) return (member.DeclaringType?.FullName ?? "") + "." + member.Name;
        if (value is Array array) return "[" + string.Join(",", array.Cast<object?>().Select(Value)) + "]";
        if (value.GetType().Name.StartsWith("Instruction", StringComparison.Ordinal)) return ObjectState(value);
        if (value is IFormattable formattable) return formattable.ToString(null, CultureInfo.InvariantCulture) ?? "";
        return value.GetType().Name;
    }

    private sealed class RecordingGenerator : QuireEmit.ILGenerator
    {
        internal List<string> Calls { get; } = new();

        public override void Emit(ReflectionEmit.OpCode opcode) => Add(opcode, null, null);
        public override void Emit(ReflectionEmit.OpCode opcode, int arg) => Add(opcode, arg, null);
        public override void Emit(ReflectionEmit.OpCode opcode, Type cls) => Add(opcode, cls, typeof(Type));
        public override void Emit(ReflectionEmit.OpCode opcode, FieldInfo field) => Add(opcode, field, typeof(FieldInfo));
        public override void Emit(ReflectionEmit.OpCode opcode, ConstructorInfo con) => Add(opcode, con, typeof(ConstructorInfo));

        private void Add(ReflectionEmit.OpCode opcode, object? operand, Type? _) =>
            Calls.Add(string.Format(CultureInfo.InvariantCulture, "eEmit:{0}:{1}", opcode.Name, Value(operand)));
    }

    private sealed class EmitterSample
    {
        public int Value;
        public EmitterSample() { }
    }
}
