#nullable disable
using System.Reflection;
using System.Reflection.Emit;
using Quire.Mirror;

namespace Quire.Mpl.Run;

internal partial class Instruction
{
    [MirrorVerified("InstructionEmitCodeDifferentialTests")]
    public override void EmitCode(System.Reflection.Emit.Quire.Mpl.ILGenerator il)
    {
        il.Emit(m_op);
    }

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    protected OpCode m_op;

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    protected Instruction(OpCode op)
    {
        m_op = op;
    }

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    public static Instruction Op(OpCode op) => new Instruction(op);

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    public static Instruction Op(OpCode op, Type ty) => new InstructionArgType(op, ty);

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    public static Instruction Op(OpCode op, int i) => new InstructionArgInt(op, i);

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    public static Instruction Op(OpCode op, FieldInfo f) => new InstructionArgField(op, f);

    [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
    public static Instruction Op(OpCode op, ConstructorInfo c) => new InstructionArgCtor(op, c);

    partial class InstructionArgType
    {
        [MirrorVerified("InstructionEmitCodeDifferentialTests")]
        public override void EmitCode(System.Reflection.Emit.Quire.Mpl.ILGenerator il)
        {
            il.Emit(m_op, m_type);
        }

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        private Type m_type;

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        public InstructionArgType(OpCode op, Type ty) : base(op)
        {
            m_type = ty;
        }
    }

    partial class InstructionArgInt
    {
        [MirrorVerified("InstructionEmitCodeDifferentialTests")]
        public override void EmitCode(System.Reflection.Emit.Quire.Mpl.ILGenerator il)
        {
            il.Emit(m_op, m_int);
        }

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        protected int m_int;

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        public InstructionArgInt(OpCode op, int i) : base(op)
        {
            m_int = i;
        }
    }

    partial class InstructionArgField
    {
        [MirrorVerified("InstructionEmitCodeDifferentialTests")]
        public override void EmitCode(System.Reflection.Emit.Quire.Mpl.ILGenerator il)
        {
            il.Emit(m_op, m_field);
        }

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        private FieldInfo m_field;

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        public InstructionArgField(OpCode op, FieldInfo fi) : base(op)
        {
            m_field = fi;
        }
    }

    partial class InstructionArgCtor
    {
        [MirrorVerified("InstructionEmitCodeDifferentialTests")]
        public override void EmitCode(System.Reflection.Emit.Quire.Mpl.ILGenerator il)
        {
            il.Emit(m_op, m_ctor);
        }

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        private ConstructorInfo m_ctor;

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        public InstructionArgCtor(OpCode op, ConstructorInfo ci) : base(op)
        {
            m_ctor = ci;
        }
    }
}
