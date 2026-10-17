#nullable disable
using System.Reflection;
using System.Reflection.Emit;
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Primitive
{
    [MirrorVerified("PrimitiveInitializerDifferentialTests")]
    public static Primitive[] m_primitive;

    [MirrorVerified("PrimitiveInitializerDifferentialTests")]
    public static MethodInfo m_stelem_vec;

    [MirrorVerified("PrimitiveInitializerDifferentialTests")]
    static Primitive()
    {
        m_stelem_vec = typeof(Primitive).GetMethod("stelem_vec");
        Code[] args = new Code[3]
        {
            Code.NullCode,
            new CodeArg(Instruction.Op(OpCodes.Dup)),
            Code.NullCode
        };
        Code code = new CodeFunction(Instruction.Op(OpCodes.Stelem_Ref), args);
        Code code2 = new CodeFunction(Instruction.Op(OpCodes.Stelem_I4), args);
        Code code3 = new CodeFunction(Instruction.Op(OpCodes.Stelem_R8), args);
        Code code4 = new CodeFunction(Code.NullCode, new Code[2]
        {
            Code.NullCode,
            new CodeArg(Instruction.Op(OpCodes.Pop))
        });
        System.Type mathType = typeof(Math);
        mathType.GetMethod("Pow");
        m_primitive = new Primitive[88]
        {
            new Primitive("or_ref"),
            new Primitive("and_ref"),
            new Primitive("Throw", OpCodes.Throw),
            new Primitive("invalid"),
            new Primitive("neg_i4", OpCodes.Neg),
            new Primitive("add_i4", OpCodes.Add),
            new Primitive("sub_i4", OpCodes.Sub),
            new Primitive("mul_i4", OpCodes.Mul),
            new Primitive("div_i4", OpCodes.Div),
            new Primitive("rem_i4", OpCodes.Rem),
            new Primitive("ceq_i4", OpCodes.Ceq),
            new Primitive("cne_i4", new OpCode[3] { OpCodes.Ceq, OpCodes.Ldc_I4_1, OpCodes.Xor }),
            new Primitive("clt_i4", OpCodes.Clt),
            new Primitive("cle_i4", new OpCode[3] { OpCodes.Cgt, OpCodes.Ldc_I4_1, OpCodes.Xor }),
            new Primitive("cgt_i4", OpCodes.Cgt),
            new Primitive("cge_i4", new OpCode[3] { OpCodes.Clt, OpCodes.Ldc_I4_1, OpCodes.Xor }),
            new Primitive("neg_f8", OpCodes.Neg),
            new Primitive("add_f8", OpCodes.Add),
            new Primitive("sub_f8", OpCodes.Sub),
            new Primitive("mul_f8", OpCodes.Mul),
            new Primitive("div_f8", new OpCode[2] { OpCodes.Div, OpCodes.Ckfinite }),
            new Primitive("ceq_f8", OpCodes.Ceq),
            new Primitive("cne_f8", new OpCode[3] { OpCodes.Ceq, OpCodes.Ldc_I4_1, OpCodes.Xor }),
            new Primitive("clt_f8", OpCodes.Clt),
            new Primitive("cle_f8", new OpCode[3] { OpCodes.Cgt_Un, OpCodes.Ldc_I4_1, OpCodes.Xor }),
            new Primitive("cgt_f8", OpCodes.Cgt),
            new Primitive("cge_f8", new OpCode[3] { OpCodes.Clt_Un, OpCodes.Ldc_I4_1, OpCodes.Xor }),
            new Primitive("conv_i4", OpCodes.Conv_Ovf_I4),
            new Primitive("conv_f8", OpCodes.Conv_R8),
            new Primitive("identity", new OpCode[0]),
            new Primitive("dup", OpCodes.Dup),
            new Primitive("arr_i4", Instruction.Op(OpCodes.Newarr, typeof(int))),
            new Primitive("arr_f8", Instruction.Op(OpCodes.Newarr, typeof(double))),
            new Primitive("arr_ref", Instruction.Op(OpCodes.Newarr, typeof(object))),
            new Primitive("ldelem_i4", OpCodes.Ldelem_I4),
            new Primitive("stelem_i4_void", OpCodes.Stelem_I4),
            new Primitive("stelem_i4", code2),
            new Primitive("ldelem_f8", OpCodes.Ldelem_R8),
            new Primitive("stelem_f8_void", OpCodes.Stelem_R8),
            new Primitive("stelem_f8", code3),
            new Primitive("ldelem_ref", OpCodes.Ldelem_Ref),
            new Primitive("stelem_ref_void", OpCodes.Stelem_Ref),
            new Primitive("stelem_ref", code),
            new Primitive("ldlen", OpCodes.Ldlen),
            new Primitive("build"),
            new Primitive("dfoldr"),
            new Primitive("identity", new OpCode[0]),
            new Primitive("conv_void", OpCodes.Pop),
            new Primitive("as_matrix", Instruction.Op(OpCodes.Isinst, typeof(MatVal))),
            new Primitive("as_number", Instruction.Op(OpCodes.Isinst, typeof(NumVal))),
            new Primitive("as_string", Instruction.Op(OpCodes.Isinst, typeof(string))),
            new Primitive("mpl_type"),
            new Primitive("iv", typeof(NumVal).GetMethod("IntDv")),
            new Primitive("rv", typeof(NumVal).GetMethod("RealDv")),
            new Primitive("rrv", typeof(NumVal).GetMethod("NumDv")),
            new Primitive("with_bool"),
            new Primitive("as_function", Instruction.Op(OpCodes.Isinst, typeof(Function))),
            new Primitive("xor", OpCodes.Xor),
            new Primitive("shl", OpCodes.Shl),
            new Primitive("shr", OpCodes.Shr),
            new Primitive("shr_u4", OpCodes.Shr_Un),
            new Primitive("pwr_f8", "System.Math::Pow"),
            new Primitive("nrt_f8"),
            new Primitive("pwr_v", "[efimatrix_]Efi.Matrix::pwr"),
            new Primitive("nrt_v", "[eficore_]Efi.Core::nthroot"),
            new Primitive("add_v", "[eficore_]Efi.Core::add"),
            new Primitive("sub_v", "[eficore_]Efi.Core::sub"),
            new Primitive("mul_v", "[efimatrix_]Efi.Matrix::mult"),
            new Primitive("div_v", "[efimatrix_]Efi.Matrix::div"),
            new Primitive("neg_v", "[eficore_]Efi.Core::neg"),
            new Primitive("bv", "[mplrun]Mcd.NumVal::McdBool"),
            new Primitive("as_range", Instruction.Op(OpCodes.Isinst, typeof(RngVal))),
            new Primitive("as_i4", "instance[mplrun]Mcd.DimVal::asDimlessInt()"),
            new Primitive("ibv", "[mplrun]Mcd.NumVal::ibv"),
            new Primitive("seq_void", new OpCode[0]),
            new Primitive("seq", code4),
            new Primitive("as_dimval", Instruction.Op(OpCodes.Isinst, typeof(DimVal))),
            new Primitive("set1_v", "instance[mplrun]Mcd.DimVal::matrix_set_1(System.Int32,System.Object)"),
            new Primitive("set1_re", "instance[mplrun]Mcd.DimVal::matrix_set_1(System.Int32,System.Double)"),
            new Primitive("vb_bld", "[mplrun]Mpl.Run.VecBuilder::make(System.Int32)"),
            new Primitive("vb_psh1", "instance[mplrun]Mpl.Run.VecBuilder::Push1(System.Object)"),
            new Primitive("vb_psh4", "instance[mplrun]Mpl.Run.VecBuilder::Push4(System.Object,System.Object,System.Object,System.Object)"),
            new Primitive("vb_list", "instance[mplrun]Mpl.Run.VecBuilder::AsList()"),
            new Primitive("vb_vec", "instance[mplrun]Mpl.Run.VecBuilder::AsVector()"),
            new Primitive("mb_psh", "instance[mplrun]Mcd.MatVal::PushRaw(System.Object)"),
            new Primitive("mb_pshF", "instance[mplrun]Mcd.MatVal::PushRaw(System.Double)"),
            new Primitive("mb_pshI", "instance[mplrun]Mcd.MatVal::PushRaw(System.Int32)"),
            new Primitive("mb_psh0", "instance[mplrun]Mcd.MatVal::PushZero()")
        };
    }
}
