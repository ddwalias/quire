#nullable disable
using System.Reflection;
using System.Reflection.Emit;
using Quire.Mirror;

namespace Quire.Mpl.Run;

internal partial class Instruction
{
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
        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        private ConstructorInfo m_ctor;

        [MirrorVerified("MplRunInstructionMechanicalDifferentialTests")]
        public InstructionArgCtor(OpCode op, ConstructorInfo ci) : base(op)
        {
            m_ctor = ci;
        }
    }
}
