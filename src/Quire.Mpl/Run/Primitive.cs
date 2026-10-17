#nullable disable
using Quire.Mcd;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>Mpl's primitive comparisons: 1 when the comparison holds, else 0 (docs/mirror/evidence/Mpl.Run.Primitive.md).</summary>
public partial class Primitive
{
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int ceq_i4(int x, int y) => x == y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cne_i4(int x, int y) => x != y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int clt_i4(int x, int y) => x < y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cle_i4(int x, int y) => x <= y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cgt_i4(int x, int y) => x > y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cge_i4(int x, int y) => x >= y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int ceq_f8(double x, double y) => x == y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cne_f8(double x, double y) => x != y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int clt_f8(double x, double y) => x < y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cle_f8(double x, double y) => x <= y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cgt_f8(double x, double y) => x > y ? 1 : 0;
    [MirrorVerified("PrimitiveDifferentialTests")]
    public static int cge_f8(double x, double y) => x >= y ? 1 : 0;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object Throw(Exception e) => throw e;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_matrix(object o) => o as MatVal;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_number(object o) => o as NumVal;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_string(object o) => o as string;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_function(object o) => o as Function;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object as_range(object o) => o as RngVal;

    [MirrorVerified("PrimitivePredicateDifferentialTests")]
    public bool IsOpCodePrimitive() { if (m_code == null) return m_opcodes != null; return false; }

    [MirrorVerified("PrimitivePredicateDifferentialTests")]
    public bool IsCodePrimitive() => m_code != null;

    [MirrorVerified("PrimitivePredicateDifferentialTests")]
    public bool IsMethodPrimitive() { if (m_method != null && m_code == null) return m_opcodes == null; return false; }

    [MirrorVerified("PrimitivePredicateDifferentialTests")]
    public bool IsForeignPrimitive() => m_foreign != null;

    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static int neg_i4(int x) => -x;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static int add_i4(int x, int y) => x + y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static int sub_i4(int x, int y) => x - y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static int mul_i4(int x, int y) => x * y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static int div_i4(int x, int y) => x / y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static int rem_i4(int x, int y) => x % y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static double neg_f8(double x) => -x;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static double add_f8(double x, double y) => x + y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static double sub_f8(double x, double y) => x - y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static double mul_f8(double x, double y) => x * y;
    [MirrorVerified("PrimitiveArithmeticDifferentialTests")]
    public static double div_f8(double x, double y) => x / y;

    [MirrorVerified("PrimitiveReferenceDifferentialTests")]
    public static object or_ref(object x, object y) { if (x == null) return y; return x; }
    [MirrorVerified("PrimitiveReferenceDifferentialTests")]
    public static object and_ref(object x, object y) { if (x != null) return y; return null; }

    [MirrorVerified("PrimitiveConversionDifferentialTests")]
    public static int conv_i4(double x) => (int)x;
    [MirrorVerified("PrimitiveConversionDifferentialTests")]
    public static double conv_f8(int x) => x;

    [MirrorVerified("PrimitiveArrayDifferentialTests")]
    public static int[] arr_i4(int size) => new int[size];
    [MirrorVerified("PrimitiveArrayDifferentialTests")]
    public static double[] arr_f8(int size) => new double[size];
    [MirrorVerified("PrimitiveArrayDifferentialTests")]
    public static object[] arr_ref(int size) => new object[size];

    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static int ldelem_i4(int[] arr, int index) => arr[index];
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static void stelem_i4_void(int[] arr, int index, int val) { arr[index] = val; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static int[] stelem_i4(int[] arr, int index, int val) { arr[index] = val; return arr; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static double ldelem_f8(double[] arr, int index) => arr[index];
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static void stelem_f8_void(double[] arr, int index, double val) { arr[index] = val; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static double[] stelem_f8(double[] arr, int index, double val) { arr[index] = val; return arr; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static object ldelem_ref(object[] arr, int index) => arr[index];
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static void stelem_ref_void(object[] arr, int index, object val) { arr[index] = val; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static object[] stelem_ref(object[] arr, int index, object val) { arr[index] = val; return arr; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static void stelem_vec(object val, object[] clo, int index) { clo[index] = val; }
    [MirrorVerified("PrimitiveArrayAccessDifferentialTests")]
    public static int ldlen(object[] arr) => arr.Length;

    [MirrorVerified("PrimitiveBitwiseDifferentialTests")]
    public static int xor(int x, int y) => x ^ y;
    [MirrorVerified("PrimitiveBitwiseDifferentialTests")]
    public static int shl(int x, int y) => x << y;
    [MirrorVerified("PrimitiveBitwiseDifferentialTests")]
    public static int shr(int x, int y) => x >> y;
    [MirrorVerified("PrimitiveBitwiseDifferentialTests")]
    public static uint shr_u4(uint x, int y) => x >> y;
    [MirrorVerified("PrimitiveBitwiseDifferentialTests")]
    public static double nrt_f8(double x, double y) => Math.Pow(y, 1.0 / x);

    [MirrorVerified("PrimitiveValueDifferentialTests")]
    public static object iv(int i) => new NumVal(i, 0.0);

    [MirrorVerified("PrimitiveValueDifferentialTests")]
    public static object with_bool(object o)
    {
        NumVal numVal = o as NumVal;
        if (numVal == null) AppException.Raise("must be scalar");
        if (numVal.Data.rp == 0.0 && numVal.Data.ip == 0.0) return null;
        return True.singleton;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, System.Reflection.Emit.OpCode code, System.Reflection.MethodInfo mi)
    {
        m_name = nm;
        m_opcodes = new System.Reflection.Emit.OpCode[1] { code };
        m_method = mi;
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, System.Reflection.Emit.OpCode[] codes, System.Reflection.MethodInfo mi)
    {
        m_name = nm;
        m_opcodes = codes;
        m_method = mi;
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, System.Reflection.Emit.OpCode code)
    {
        m_name = nm;
        m_opcodes = new System.Reflection.Emit.OpCode[1] { code };
        m_method = typeof(Primitive).GetMethod(nm);
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, Code code)
    {
        m_name = nm;
        m_code = code;
        m_method = typeof(Primitive).GetMethod(nm);
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, System.Reflection.Emit.OpCode[] codes)
    {
        m_name = nm;
        m_opcodes = codes;
        m_method = typeof(Primitive).GetMethod(nm);
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, System.Reflection.MethodInfo mi)
    {
        m_name = nm;
        m_opcodes = null;
        m_method = mi;
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm)
    {
        m_name = nm;
        m_opcodes = null;
        m_method = typeof(Primitive).GetMethod(nm);
        m_foreign = null;
    }

    [MirrorVerified("PrimitiveConstructorDifferentialTests")]
    private Primitive(string nm, string def)
    {
        m_name = nm;
        m_opcodes = null;
        m_method = null;
        m_foreign = def;
    }

    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static object Call(int prim, object x) => m_primitive[prim].m_method.Invoke(null, new object[1] { x });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static object Call(int prim, object x, object y) => m_primitive[prim].m_method.Invoke(null, new object[2] { x, y });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static object Call(int prim, object x, object y, object z) => m_primitive[prim].m_method.Invoke(null, new object[3] { x, y, z });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static int Call(int prim, int x) => (int)m_primitive[prim].m_method.Invoke(null, new object[1] { x });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static int Call(int prim, int x, int y) => (int)m_primitive[prim].m_method.Invoke(null, new object[2] { x, y });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static double Call(int prim, double x) => (double)m_primitive[prim].m_method.Invoke(null, new object[1] { x });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static double Call(int prim, double x, double y) => (double)m_primitive[prim].m_method.Invoke(null, new object[2] { x, y });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static int Call_c(int prim, double x) => (int)m_primitive[prim].m_method.Invoke(null, new object[1] { x });
    [MirrorVerified("PrimitiveCallDifferentialTests")]
    public static double Call_c(int prim, int x) => (double)m_primitive[prim].m_method.Invoke(null, new object[1] { x });

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object identity(object o) => o;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object dup(object o) => throw new AppException("may not capture pseudo function");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object build(object k, object z) => throw new AppException("build: must inline");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object dfoldr(object f, object n, object xs) => throw new AppException("dfoldr: must inline");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static void conv_void(object o) { }

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object invalid(object o) => throw new AppException("compiler error");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object mpl_type(object o) => throw new AppException("compiler error");

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object rv(double r) => new NumVal(r, 0.0);

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object rrv(double r, double i) => new NumVal(r, i);

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object seq(object x, object y) => y;

    [MirrorVerified("MplRunPrimitiveMechanicalDifferentialTests")]
    public static object seq_void(object x, object y) => throw new AppException("compiler error");
}
