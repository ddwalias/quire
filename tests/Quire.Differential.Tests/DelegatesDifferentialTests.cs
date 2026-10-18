using System.Reflection;
using System.Text;
using Quire.Mpl.Run;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class DelegatesDifferentialTests
{
    private const string Contract = "Mpl.Run.Delegates";
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static Type QDelegatesType => typeof(Delegates);
    private static Type MDelegatesType => Mathcad.Type("mplrun", "Mpl.Run.Delegates");

    private static Type[] QGtypes => (Type[])QDelegatesType.GetField("gtypes", Static)!.GetValue(null)!;
    private static Type[] MGtypes => (Type[])MDelegatesType.GetField("gtypes", Static)!.GetValue(null)!;

    private static Type[] QVtypes => (Type[])QDelegatesType.GetField("vtypes", Static)!.GetValue(null)!;
    private static Type[] MVtypes => (Type[])MDelegatesType.GetField("vtypes", Static)!.GetValue(null)!;

    private static MethodInfo Method(Side side, string name, BindingFlags flags, Type[]? types = null)
    {
        Type t = side == Side.Quire ? QDelegatesType : MDelegatesType;
        if (types == null)
            return t.GetMethod(name, flags) ?? throw new HarnessException($"Missing {side.Name} Delegates.{name}");
        return t.GetMethod(name, flags, types) ?? throw new HarnessException($"Missing {side.Name} Delegates.{name}");
    }

    private static Type MakeFType(Side side, int arity, Type[] typeArgs)
    {
        Type[] gtypes = side == Side.Quire ? QGtypes : MGtypes;
        return gtypes[arity].MakeGenericType(typeArgs);
    }

    private static Delegate CreateFDelegate(Side side, int arity, Type[] typeArgs, MethodInfo method)
    {
        Type fType = MakeFType(side, arity, typeArgs);
        return Delegate.CreateDelegate(fType, method);
    }

    private static string Show(object? value) => MplRunSides.Describe(value);

    private static string ShowDelegateOrValue(object? value)
    {
        if (value == null) return "null";
        if (value is Delegate d)
        {
            var pars = d.Method.GetParameters();
            object[] invokeArgs = new object[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                Type pt = pars[i].ParameterType;
                if (pt == typeof(int)) invokeArgs[i] = 10 + i;
                else if (pt == typeof(double)) invokeArgs[i] = 2.5 + i;
                else if (pt == typeof(bool)) invokeArgs[i] = true;
                else invokeArgs[i] = "arg" + i;
            }
            string methodName = d.Method.Name.StartsWith('<') && d.Method.Name.Contains('>')
                ? d.Method.Name.Substring(1, d.Method.Name.IndexOf('>') - 1)
                : d.Method.Name;
            try
            {
                object? res = d.DynamicInvoke(invokeArgs);
                return $"{d.GetType().Name}[{methodName}]->{res}";
            }
            catch (Exception ex)
            {
                return $"{d.GetType().Name}[{methodName}]->throws {ex.InnerException?.GetType().Name ?? ex.GetType().Name}";
            }
        }

        if (value is MethodBase mb)
        {
            return $"{mb.DeclaringType?.Name}.{mb.Name}";
        }

        var applyMethod = value.GetType().GetMethod("Apply", Instance, [typeof(object[])]);
        var purityProp = value.GetType().GetProperty("Purity", Instance);
        if (applyMethod != null && purityProp != null)
        {
            int p = (int)purityProp.GetValue(value)!;
            object[] appArgs = new object[p];
            for (int i = 0; i < p; i++) appArgs[i] = "farg" + i;
            try
            {
                object? res = applyMethod.Invoke(value, [appArgs]);
                return $"{value.GetType().Name}[purity={p}]->{res}";
            }
            catch (Exception ex)
            {
                return $"{value.GetType().Name}[purity={p}]->throws {ex.InnerException?.GetType().Name ?? ex.GetType().Name}";
            }
        }

        return Show(value);
    }

    private static string RenderStaticInitialization(Side side)
    {
        Type t = side == Side.Quire ? QDelegatesType : MDelegatesType;
        var gtypes = (Type[])t.GetField("gtypes", Static)!.GetValue(null)!;
        var vtypes = (Type[])t.GetField("vtypes", Static)!.GetValue(null)!;
        var imethods = (MethodInfo[])t.GetField("imethods", Static)!.GetValue(null)!;
        var cloMethod = (MethodInfo[])t.GetField("clo_method", Static)!.GetValue(null)!;

        var sb = new StringBuilder();
        sb.Append("gtypes:");
        for (int i = 0; i < gtypes.Length; i++)
            sb.Append(gtypes[i]?.Name ?? "null").Append(',');
        sb.Append(";vtypes:");
        for (int i = 0; i < vtypes.Length; i++)
            sb.Append(vtypes[i]?.Name ?? "null").Append(',');
        sb.Append(";imethods:");
        for (int i = 0; i < imethods.Length; i++)
            sb.Append(imethods[i]?.Name ?? "null").Append(',');
        sb.Append(";clo_method:");
        for (int i = 0; i < cloMethod.Length; i++)
            sb.Append(cloMethod[i]?.Name ?? "null").Append(',');
        return sb.ToString();
    }

    public static class Targets
    {
        public static double TargetM1(int a, string b) => a + (b?.Length ?? 0) * 1.5;
        public static string TargetM0() => "target_m0";
        public static int TargetAdd(int a, int b) => a + b;
        public static int Inv1(string s) => s.Length;
        public static int Inv2(string s, int i) => s.Length + i;
        public static int Inv3(string s, int i, double d) => s.Length + i + (int)d;
        public static int Inv4(string s, int i, double d, bool b) => s.Length + i + (int)d + (b ? 100 : 0);
        public static object Clo1(string clo, object a) => $"{clo}:{a}";
        public static object Clo2(string clo, object a, object b) => $"{clo}:{a},{b}";
        public static object Clo3(string clo, object a, object b, object c) => $"{clo}:{a},{b},{c}";
        public static object Clo4(string clo, object a, object b, object c, object d) => $"{clo}:{a},{b},{c},{d}";
        public static string WrapF1(int x) => $"w1:{x}";
        public static string WrapF2(int x, string y) => $"w2:{x},{y}";
        public static string WrapF3(int x, string y, double z) => $"w3:{x},{y},{z}";
        public static string Identity(string s) => s;
        public static string ObjectIdentity(object o) => $"obj:{o}";
        public static int ValueReturn(object o) => 42;
        public static string ValueArg(int i) => $"val:{i}";
        public static string StringReturnObjectArg(object x) => $"sret:{x}";
    }

    [SkippableFact]
    public void Delegates_Constructor_and_Static_Initialization()
    {
        Runtime.Sync();
        var qCtor = QDelegatesType.GetConstructor(Instance, null, Type.EmptyTypes, null)!;
        var mCtor = MDelegatesType.GetConstructor(Instance, null, Type.EmptyTypes, null)!;
        Assert.Equal(0x06000342, mCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mCtor.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.ctor", Contract,
            qCtor, null, [],
            mCtor, null, [],
            ShowDelegateOrValue, ShowDelegateOrValue);

        var qCctor = QDelegatesType.TypeInitializer!;
        var mCctor = MDelegatesType.TypeInitializer!;
        Assert.Equal(0x06000319, mCctor.MetadataToken);
        Assert.Equal(MathcadMvid, mCctor.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.cctor", Contract,
            qCctor, null, [],
            mCctor, null, [],
            quireShow: _ => RenderStaticInitialization(Side.Quire),
            originalShow: _ => RenderStaticInitialization(Side.Mathcad));

        // Check PI_1 and PI_2 static fields
        object qPi1 = QDelegatesType.GetField("PI_1", Static)!.GetValue(null)!;
        object mPi1 = MDelegatesType.GetField("PI_1", Static)!.GetValue(null)!;
        var qApply1 = qPi1.GetType().GetMethod("Apply", Instance, [typeof(object[])])!;
        var mApply1 = mPi1.GetType().GetMethod("Apply", Instance, [typeof(object[])])!;
        Assert.Equal("first", qApply1.Invoke(qPi1, [new object[] { "first", "second" }])?.ToString());
        Assert.Equal("first", mApply1.Invoke(mPi1, [new object[] { "first", "second" }])?.ToString());

        object qPi2 = QDelegatesType.GetField("PI_2", Static)!.GetValue(null)!;
        object mPi2 = MDelegatesType.GetField("PI_2", Static)!.GetValue(null)!;
        var qApply2 = qPi2.GetType().GetMethod("Apply", Instance, [typeof(object[])])!;
        var mApply2 = mPi2.GetType().GetMethod("Apply", Instance, [typeof(object[])])!;
        Assert.Equal("second", qApply2.Invoke(qPi2, [new object[] { "first", "second" }])?.ToString());
        Assert.Equal("second", mApply2.Invoke(mPi2, [new object[] { "first", "second" }])?.ToString());
    }

    [SkippableFact]
    public void Delegates_fparams_overloads_and_clo_params()
    {
        Runtime.Sync();

        // 1. clo_params: token 0x0600031a
        var qCloParams = Method(Side.Quire, "clo_params", Static, [typeof(Type[])]);
        var mCloParams = Method(Side.Mathcad, "clo_params", Static, [typeof(Type[])]);
        Assert.Equal(0x0600031a, mCloParams.MetadataToken);
        Assert.Equal(MathcadMvid, mCloParams.Module.ModuleVersionId);

        Type[] ftys4 = [typeof(int), typeof(object), typeof(string), typeof(double)];
        MethodComparison.Compare("Delegates.clo_params.arity4", Contract,
            qCloParams, null, [ftys4],
            mCloParams, null, [ftys4],
            ShowDelegateOrValue, ShowDelegateOrValue);

        Type[] ftys2 = [typeof(void), typeof(object)];
        MethodComparison.Compare("Delegates.clo_params.arity2", Contract,
            qCloParams, null, [ftys2],
            mCloParams, null, [ftys2],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. fparams(Type, Type[]): token 0x0600031b
        var qFparamsTypes = Method(Side.Quire, "fparams", Static, [typeof(Type), typeof(Type[])]);
        var mFparamsTypes = Method(Side.Mathcad, "fparams", Static, [typeof(Type), typeof(Type[])]);
        Assert.Equal(0x0600031b, mFparamsTypes.MetadataToken);
        Assert.Equal(MathcadMvid, mFparamsTypes.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.fparams.types", Contract,
            qFparamsTypes, null, [typeof(int), new Type[] { typeof(string), typeof(double) }],
            mFparamsTypes, null, [typeof(int), new Type[] { typeof(string), typeof(double) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.fparams.empty_types", Contract,
            qFparamsTypes, null, [typeof(void), Array.Empty<Type>()],
            mFparamsTypes, null, [typeof(void), Array.Empty<Type>()],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. fparams(MethodInfo): token 0x0600031c
        var qFparamsMethod = Method(Side.Quire, "fparams", Static, [typeof(MethodInfo)]);
        var mFparamsMethod = Method(Side.Mathcad, "fparams", Static, [typeof(MethodInfo)]);
        Assert.Equal(0x0600031c, mFparamsMethod.MetadataToken);
        Assert.Equal(MathcadMvid, mFparamsMethod.Module.ModuleVersionId);

        MethodInfo targetM1 = typeof(Targets).GetMethod(nameof(Targets.TargetM1), Static)!;
        MethodComparison.Compare("Delegates.fparams.method_with_args", Contract,
            qFparamsMethod, null, [targetM1],
            mFparamsMethod, null, [targetM1],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodInfo targetM0 = typeof(Targets).GetMethod(nameof(Targets.TargetM0), Static)!;
        MethodComparison.Compare("Delegates.fparams.method_no_args", Contract,
            qFparamsMethod, null, [targetM0],
            mFparamsMethod, null, [targetM0],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. fparams(Delegate): token 0x0600031d
        var qFparamsDel = Method(Side.Quire, "fparams", Static, [typeof(Delegate)]);
        var mFparamsDel = Method(Side.Mathcad, "fparams", Static, [typeof(Delegate)]);
        Assert.Equal(0x0600031d, mFparamsDel.MetadataToken);
        Assert.Equal(MathcadMvid, mFparamsDel.Module.ModuleVersionId);

        Func<int, string, double> testFunc = (a, b) => 1.0;
        MethodComparison.Compare("Delegates.fparams.delegate_func", Contract,
            qFparamsDel, null, [testFunc],
            mFparamsDel, null, [testFunc],
            ShowDelegateOrValue, ShowDelegateOrValue);

        Action<int> testAction = a => { };
        MethodComparison.Compare("Delegates.fparams.delegate_action", Contract,
            qFparamsDel, null, [testAction],
            mFparamsDel, null, [testAction],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_Type_and_Constructor_Helpers()
    {
        Runtime.Sync();

        // 1. FType: token 0x06000334
        var qFType = Method(Side.Quire, "FType", Static, [typeof(Type[])]);
        var mFType = Method(Side.Mathcad, "FType", Static, [typeof(Type[])]);
        Assert.Equal(0x06000334, mFType.MetadataToken);
        Assert.Equal(MathcadMvid, mFType.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.FType.arity2", Contract,
            qFType, null, [new Type[] { typeof(int), typeof(string) }],
            mFType, null, [new Type[] { typeof(int), typeof(string) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.FType.arity3", Contract,
            qFType, null, [new Type[] { typeof(object), typeof(int), typeof(double) }],
            mFType, null, [new Type[] { typeof(object), typeof(int), typeof(double) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. VType: token 0x06000335
        var qVType = Method(Side.Quire, "VType", Static, [typeof(Type[])]);
        var mVType = Method(Side.Mathcad, "VType", Static, [typeof(Type[])]);
        Assert.Equal(0x06000335, mVType.MetadataToken);
        Assert.Equal(MathcadMvid, mVType.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.VType.arity0", Contract,
            qVType, null, [Array.Empty<Type>()],
            mVType, null, [Array.Empty<Type>()],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.VType.arity1", Contract,
            qVType, null, [new Type[] { typeof(int) }],
            mVType, null, [new Type[] { typeof(int) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.VType.arity2", Contract,
            qVType, null, [new Type[] { typeof(int), typeof(string) }],
            mVType, null, [new Type[] { typeof(int), typeof(string) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. MethodToDelegateType: token 0x06000337
        var qM2DType = Method(Side.Quire, "MethodToDelegateType", Static, [typeof(MethodInfo)]);
        var mM2DType = Method(Side.Mathcad, "MethodToDelegateType", Static, [typeof(MethodInfo)]);
        Assert.Equal(0x06000337, mM2DType.MetadataToken);
        Assert.Equal(MathcadMvid, mM2DType.Module.ModuleVersionId);

        MethodInfo addMethod = typeof(Targets).GetMethod(nameof(Targets.TargetAdd), Static)!;
        MethodComparison.Compare("Delegates.MethodToDelegateType", Contract,
            qM2DType, null, [addMethod],
            mM2DType, null, [addMethod],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. MethodToDelegateCtor: token 0x06000338
        var qM2DCtor = Method(Side.Quire, "MethodToDelegateCtor", Static, [typeof(MethodInfo)]);
        var mM2DCtor = Method(Side.Mathcad, "MethodToDelegateCtor", Static, [typeof(MethodInfo)]);
        Assert.Equal(0x06000338, mM2DCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mM2DCtor.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.MethodToDelegateCtor", Contract,
            qM2DCtor, null, [addMethod],
            mM2DCtor, null, [addMethod],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 5. MethodToDelegate: token 0x06000336
        var qM2D = Method(Side.Quire, "MethodToDelegate", Static, [typeof(MethodInfo)]);
        var mM2D = Method(Side.Mathcad, "MethodToDelegate", Static, [typeof(MethodInfo)]);
        Assert.Equal(0x06000336, mM2D.MetadataToken);
        Assert.Equal(MathcadMvid, mM2D.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.MethodToDelegate", Contract,
            qM2D, null, [addMethod],
            mM2D, null, [addMethod],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 6. ParameterTypesToDelegateType: token 0x06000339
        var qP2DType = Method(Side.Quire, "ParameterTypesToDelegateType", Static, [typeof(Type), typeof(Type[])]);
        var mP2DType = Method(Side.Mathcad, "ParameterTypesToDelegateType", Static, [typeof(Type), typeof(Type[])]);
        Assert.Equal(0x06000339, mP2DType.MetadataToken);
        Assert.Equal(MathcadMvid, mP2DType.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.ParameterTypesToDelegateType.func", Contract,
            qP2DType, null, [typeof(int), new Type[] { typeof(string) }],
            mP2DType, null, [typeof(int), new Type[] { typeof(string) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.ParameterTypesToDelegateType.action", Contract,
            qP2DType, null, [typeof(void), new Type[] { typeof(string) }],
            mP2DType, null, [typeof(void), new Type[] { typeof(string) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.ParameterTypesToDelegateType.void_empty", Contract,
            qP2DType, null, [typeof(void), Array.Empty<Type>()],
            mP2DType, null, [typeof(void), Array.Empty<Type>()],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 7. ParameterTypesToDelegateCtor: token 0x0600033a
        var qP2DCtor = Method(Side.Quire, "ParameterTypesToDelegateCtor", Static, [typeof(Type), typeof(Type[])]);
        var mP2DCtor = Method(Side.Mathcad, "ParameterTypesToDelegateCtor", Static, [typeof(Type), typeof(Type[])]);
        Assert.Equal(0x0600033a, mP2DCtor.MetadataToken);
        Assert.Equal(MathcadMvid, mP2DCtor.Module.ModuleVersionId);

        MethodComparison.Compare("Delegates.ParameterTypesToDelegateCtor.func", Contract,
            qP2DCtor, null, [typeof(int), new Type[] { typeof(string) }],
            mP2DCtor, null, [typeof(int), new Type[] { typeof(string) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.ParameterTypesToDelegateCtor.action", Contract,
            qP2DCtor, null, [typeof(void), new Type[] { typeof(string) }],
            mP2DCtor, null, [typeof(void), new Type[] { typeof(string) }],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.ParameterTypesToDelegateCtor.void_empty", Contract,
            qP2DCtor, null, [typeof(void), Array.Empty<Type>()],
            mP2DCtor, null, [typeof(void), Array.Empty<Type>()],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_Invoke_and_Premium_Helpers()
    {
        Runtime.Sync();

        // 1. Invoke1: token 0x0600031e
        var qInv1Def = Method(Side.Quire, "Invoke1", Static);
        var mInv1Def = Method(Side.Mathcad, "Invoke1", Static);
        Assert.Equal(0x0600031e, mInv1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv1Def.Module.ModuleVersionId);

        var qInv1 = qInv1Def.MakeGenericMethod(typeof(int), typeof(string));
        var mInv1 = mInv1Def.MakeGenericMethod(typeof(int), typeof(string));
        MethodInfo mTargetInv1 = typeof(Targets).GetMethod(nameof(Targets.Inv1), Static)!;
        Delegate qDelInv1 = CreateFDelegate(Side.Quire, 2, [typeof(int), typeof(string)], mTargetInv1);
        Delegate mDelInv1 = CreateFDelegate(Side.Mathcad, 2, [typeof(int), typeof(string)], mTargetInv1);

        MethodComparison.Compare("Delegates.Invoke1", Contract,
            qInv1, null, [qDelInv1, "sample_test"],
            mInv1, null, [mDelInv1, "sample_test"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. Invoke2: token 0x0600031f
        var qInv2Def = Method(Side.Quire, "Invoke2", Static);
        var mInv2Def = Method(Side.Mathcad, "Invoke2", Static);
        Assert.Equal(0x0600031f, mInv2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv2Def.Module.ModuleVersionId);

        var qInv2 = qInv2Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int));
        var mInv2 = mInv2Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int));
        MethodInfo mTargetInv2 = typeof(Targets).GetMethod(nameof(Targets.Inv2), Static)!;
        Delegate qDelInv2 = CreateFDelegate(Side.Quire, 3, [typeof(int), typeof(string), typeof(int)], mTargetInv2);
        Delegate mDelInv2 = CreateFDelegate(Side.Mathcad, 3, [typeof(int), typeof(string), typeof(int)], mTargetInv2);

        MethodComparison.Compare("Delegates.Invoke2", Contract,
            qInv2, null, [qDelInv2, "test", 42],
            mInv2, null, [mDelInv2, "test", 42],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. Invoke3: token 0x06000320
        var qInv3Def = Method(Side.Quire, "Invoke3", Static);
        var mInv3Def = Method(Side.Mathcad, "Invoke3", Static);
        Assert.Equal(0x06000320, mInv3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv3Def.Module.ModuleVersionId);

        var qInv3 = qInv3Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double));
        var mInv3 = mInv3Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double));
        MethodInfo mTargetInv3 = typeof(Targets).GetMethod(nameof(Targets.Inv3), Static)!;
        Delegate qDelInv3 = CreateFDelegate(Side.Quire, 4, [typeof(int), typeof(string), typeof(int), typeof(double)], mTargetInv3);
        Delegate mDelInv3 = CreateFDelegate(Side.Mathcad, 4, [typeof(int), typeof(string), typeof(int), typeof(double)], mTargetInv3);

        MethodComparison.Compare("Delegates.Invoke3", Contract,
            qInv3, null, [qDelInv3, "test", 42, 3.14],
            mInv3, null, [mDelInv3, "test", 42, 3.14],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. Invoke4: token 0x06000321
        var qInv4Def = Method(Side.Quire, "Invoke4", Static);
        var mInv4Def = Method(Side.Mathcad, "Invoke4", Static);
        Assert.Equal(0x06000321, mInv4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mInv4Def.Module.ModuleVersionId);

        var qInv4 = qInv4Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double), typeof(bool));
        var mInv4 = mInv4Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double), typeof(bool));
        MethodInfo mTargetInv4 = typeof(Targets).GetMethod(nameof(Targets.Inv4), Static)!;
        Delegate qDelInv4 = CreateFDelegate(Side.Quire, 5, [typeof(int), typeof(string), typeof(int), typeof(double), typeof(bool)], mTargetInv4);
        Delegate mDelInv4 = CreateFDelegate(Side.Mathcad, 5, [typeof(int), typeof(string), typeof(int), typeof(double), typeof(bool)], mTargetInv4);

        MethodComparison.Compare("Delegates.Invoke4", Contract,
            qInv4, null, [qDelInv4, "test", 42, 3.14, true],
            mInv4, null, [mDelInv4, "test", 42, 3.14, true],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 5. Premium_1: token 0x06000322
        var qPrem1Def = Method(Side.Quire, "Premium_1", Static);
        var mPrem1Def = Method(Side.Mathcad, "Premium_1", Static);
        Assert.Equal(0x06000322, mPrem1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mPrem1Def.Module.ModuleVersionId);

        var qPrem1 = qPrem1Def.MakeGenericMethod(typeof(int), typeof(string));
        var mPrem1 = mPrem1Def.MakeGenericMethod(typeof(int), typeof(string));

        MethodComparison.Compare("Delegates.Premium_1", Contract,
            qPrem1, null, [qDelInv1],
            mPrem1, null, [mDelInv1],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 6. Premium_2: token 0x06000323
        var qPrem2Def = Method(Side.Quire, "Premium_2", Static);
        var mPrem2Def = Method(Side.Mathcad, "Premium_2", Static);
        Assert.Equal(0x06000323, mPrem2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mPrem2Def.Module.ModuleVersionId);

        var qPrem2 = qPrem2Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int));
        var mPrem2 = mPrem2Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int));

        MethodComparison.Compare("Delegates.Premium_2", Contract,
            qPrem2, null, [qDelInv2],
            mPrem2, null, [mDelInv2],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 7. Premium_3: token 0x06000324
        var qPrem3Def = Method(Side.Quire, "Premium_3", Static);
        var mPrem3Def = Method(Side.Mathcad, "Premium_3", Static);
        Assert.Equal(0x06000324, mPrem3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mPrem3Def.Module.ModuleVersionId);

        var qPrem3 = qPrem3Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double));
        var mPrem3 = mPrem3Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double));

        MethodComparison.Compare("Delegates.Premium_3", Contract,
            qPrem3, null, [qDelInv3],
            mPrem3, null, [mDelInv3],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 8. Premium_4: token 0x06000325
        var qPrem4Def = Method(Side.Quire, "Premium_4", Static);
        var mPrem4Def = Method(Side.Mathcad, "Premium_4", Static);
        Assert.Equal(0x06000325, mPrem4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mPrem4Def.Module.ModuleVersionId);

        var qPrem4 = qPrem4Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double), typeof(bool));
        var mPrem4 = mPrem4Def.MakeGenericMethod(typeof(int), typeof(string), typeof(int), typeof(double), typeof(bool));

        MethodComparison.Compare("Delegates.Premium_4", Contract,
            qPrem4, null, [qDelInv4],
            mPrem4, null, [mDelInv4],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_Closure_Private_Helpers()
    {
        Runtime.Sync();

        // 1. Closure_1: token 0x06000326
        var qClo1Def = Method(Side.Quire, "Closure_1", Static);
        var mClo1Def = Method(Side.Mathcad, "Closure_1", Static);
        Assert.Equal(0x06000326, mClo1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mClo1Def.Module.ModuleVersionId);

        var qClo1 = qClo1Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object));
        var mClo1 = mClo1Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object));
        MethodInfo mTargetClo1 = typeof(Targets).GetMethod(nameof(Targets.Clo1), Static)!;
        Delegate qDelClo1 = CreateFDelegate(Side.Quire, 3, [typeof(object), typeof(string), typeof(object)], mTargetClo1);
        Delegate mDelClo1 = CreateFDelegate(Side.Mathcad, 3, [typeof(object), typeof(string), typeof(object)], mTargetClo1);

        MethodComparison.Compare("Delegates.Closure_1", Contract,
            qClo1, null, [qDelClo1, "prefix1"],
            mClo1, null, [mDelClo1, "prefix1"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. Closure_2: token 0x06000327
        var qClo2Def = Method(Side.Quire, "Closure_2", Static);
        var mClo2Def = Method(Side.Mathcad, "Closure_2", Static);
        Assert.Equal(0x06000327, mClo2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mClo2Def.Module.ModuleVersionId);

        var qClo2 = qClo2Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object), typeof(object));
        var mClo2 = mClo2Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object), typeof(object));
        MethodInfo mTargetClo2 = typeof(Targets).GetMethod(nameof(Targets.Clo2), Static)!;
        Delegate qDelClo2 = CreateFDelegate(Side.Quire, 4, [typeof(object), typeof(string), typeof(object), typeof(object)], mTargetClo2);
        Delegate mDelClo2 = CreateFDelegate(Side.Mathcad, 4, [typeof(object), typeof(string), typeof(object), typeof(object)], mTargetClo2);

        MethodComparison.Compare("Delegates.Closure_2", Contract,
            qClo2, null, [qDelClo2, "prefix2"],
            mClo2, null, [mDelClo2, "prefix2"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. Closure_3: token 0x06000328
        var qClo3Def = Method(Side.Quire, "Closure_3", Static);
        var mClo3Def = Method(Side.Mathcad, "Closure_3", Static);
        Assert.Equal(0x06000328, mClo3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mClo3Def.Module.ModuleVersionId);

        var qClo3 = qClo3Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object), typeof(object), typeof(object));
        var mClo3 = mClo3Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object), typeof(object), typeof(object));
        MethodInfo mTargetClo3 = typeof(Targets).GetMethod(nameof(Targets.Clo3), Static)!;
        Delegate qDelClo3 = CreateFDelegate(Side.Quire, 5, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object)], mTargetClo3);
        Delegate mDelClo3 = CreateFDelegate(Side.Mathcad, 5, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object)], mTargetClo3);

        MethodComparison.Compare("Delegates.Closure_3", Contract,
            qClo3, null, [qDelClo3, "prefix3"],
            mClo3, null, [mDelClo3, "prefix3"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. Closure_4: token 0x06000329
        var qClo4Def = Method(Side.Quire, "Closure_4", Static);
        var mClo4Def = Method(Side.Mathcad, "Closure_4", Static);
        Assert.Equal(0x06000329, mClo4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mClo4Def.Module.ModuleVersionId);

        var qClo4 = qClo4Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object));
        var mClo4 = mClo4Def.MakeGenericMethod(typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object));
        MethodInfo mTargetClo4 = typeof(Targets).GetMethod(nameof(Targets.Clo4), Static)!;
        Delegate qDelClo4 = CreateFDelegate(Side.Quire, 6, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)], mTargetClo4);
        Delegate mDelClo4 = CreateFDelegate(Side.Mathcad, 6, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)], mTargetClo4);

        MethodComparison.Compare("Delegates.Closure_4", Contract,
            qClo4, null, [qDelClo4, "prefix4"],
            mClo4, null, [mDelClo4, "prefix4"],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_DelegateClosure_and_Purity_Dispatch()
    {
        Runtime.Sync();

        MethodInfo mTargetClo1 = typeof(Targets).GetMethod(nameof(Targets.Clo1), Static)!;
        MethodInfo mTargetClo2 = typeof(Targets).GetMethod(nameof(Targets.Clo2), Static)!;
        MethodInfo mTargetClo3 = typeof(Targets).GetMethod(nameof(Targets.Clo3), Static)!;
        MethodInfo mTargetClo4 = typeof(Targets).GetMethod(nameof(Targets.Clo4), Static)!;

        Delegate qDelClo1 = CreateFDelegate(Side.Quire, 3, [typeof(object), typeof(string), typeof(object)], mTargetClo1);
        Delegate mDelClo1 = CreateFDelegate(Side.Mathcad, 3, [typeof(object), typeof(string), typeof(object)], mTargetClo1);
        Delegate qDelClo2 = CreateFDelegate(Side.Quire, 4, [typeof(object), typeof(string), typeof(object), typeof(object)], mTargetClo2);
        Delegate mDelClo2 = CreateFDelegate(Side.Mathcad, 4, [typeof(object), typeof(string), typeof(object), typeof(object)], mTargetClo2);
        Delegate qDelClo3 = CreateFDelegate(Side.Quire, 5, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object)], mTargetClo3);
        Delegate mDelClo3 = CreateFDelegate(Side.Mathcad, 5, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object)], mTargetClo3);
        Delegate qDelClo4 = CreateFDelegate(Side.Quire, 6, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)], mTargetClo4);
        Delegate mDelClo4 = CreateFDelegate(Side.Mathcad, 6, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)], mTargetClo4);

        Func<object, object> nonMatching = x => x;

        // 1. DelegateClosure_1: token 0x0600032a
        var qDC1Def = Method(Side.Quire, "DelegateClosure_1", Static);
        var mDC1Def = Method(Side.Mathcad, "DelegateClosure_1", Static);
        Assert.Equal(0x0600032a, mDC1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mDC1Def.Module.ModuleVersionId);

        var qDC1 = qDC1Def.MakeGenericMethod(typeof(string));
        var mDC1 = mDC1Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.DelegateClosure_1.match", Contract,
            qDC1, null, [qDelClo1, "clo_arg1"],
            mDC1, null, [mDelClo1, "clo_arg1"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure_1.non_match", Contract,
            qDC1, null, [nonMatching, "clo_arg1"],
            mDC1, null, [nonMatching, "clo_arg1"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. DelegateClosure_2: token 0x0600032c
        var qDC2Def = Method(Side.Quire, "DelegateClosure_2", Static);
        var mDC2Def = Method(Side.Mathcad, "DelegateClosure_2", Static);
        Assert.Equal(0x0600032c, mDC2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mDC2Def.Module.ModuleVersionId);

        var qDC2 = qDC2Def.MakeGenericMethod(typeof(string));
        var mDC2 = mDC2Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.DelegateClosure_2.match", Contract,
            qDC2, null, [qDelClo2, "clo_arg2"],
            mDC2, null, [mDelClo2, "clo_arg2"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure_2.non_match", Contract,
            qDC2, null, [nonMatching, "clo_arg2"],
            mDC2, null, [nonMatching, "clo_arg2"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. DelegateClosure_3: token 0x0600032e
        var qDC3Def = Method(Side.Quire, "DelegateClosure_3", Static);
        var mDC3Def = Method(Side.Mathcad, "DelegateClosure_3", Static);
        Assert.Equal(0x0600032e, mDC3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mDC3Def.Module.ModuleVersionId);

        var qDC3 = qDC3Def.MakeGenericMethod(typeof(string));
        var mDC3 = mDC3Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.DelegateClosure_3.match", Contract,
            qDC3, null, [qDelClo3, "clo_arg3"],
            mDC3, null, [mDelClo3, "clo_arg3"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure_3.non_match", Contract,
            qDC3, null, [nonMatching, "clo_arg3"],
            mDC3, null, [nonMatching, "clo_arg3"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. DelegateClosure_4: token 0x06000330
        var qDC4Def = Method(Side.Quire, "DelegateClosure_4", Static);
        var mDC4Def = Method(Side.Mathcad, "DelegateClosure_4", Static);
        Assert.Equal(0x06000330, mDC4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mDC4Def.Module.ModuleVersionId);

        var qDC4 = qDC4Def.MakeGenericMethod(typeof(string));
        var mDC4 = mDC4Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.DelegateClosure_4.match", Contract,
            qDC4, null, [qDelClo4, "clo_arg4"],
            mDC4, null, [mDelClo4, "clo_arg4"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure_4.non_match", Contract,
            qDC4, null, [nonMatching, "clo_arg4"],
            mDC4, null, [nonMatching, "clo_arg4"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 5. DelegateClosure (purity switch): token 0x06000332
        var qDCDef = Method(Side.Quire, "DelegateClosure", Static);
        var mDCDef = Method(Side.Mathcad, "DelegateClosure", Static);
        Assert.Equal(0x06000332, mDCDef.MetadataToken);
        Assert.Equal(MathcadMvid, mDCDef.Module.ModuleVersionId);

        var qDC = qDCDef.MakeGenericMethod(typeof(string));
        var mDC = mDCDef.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.DelegateClosure.purity2", Contract,
            qDC, null, [qDelClo1, "clo2", 2],
            mDC, null, [mDelClo1, "clo2", 2],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure.purity3", Contract,
            qDC, null, [qDelClo2, "clo3", 3],
            mDC, null, [mDelClo2, "clo3", 3],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure.purity4", Contract,
            qDC, null, [qDelClo3, "clo4", 4],
            mDC, null, [mDelClo3, "clo4", 4],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure.purity5", Contract,
            qDC, null, [qDelClo4, "clo5", 5],
            mDC, null, [mDelClo4, "clo5", 5],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure.purity_default_low", Contract,
            qDC, null, [qDelClo1, "clo", 1],
            mDC, null, [mDelClo1, "clo", 1],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.DelegateClosure.purity_default_high", Contract,
            qDC, null, [qDelClo1, "clo", 6],
            mDC, null, [mDelClo1, "clo", 6],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_FunctionClosure_Helpers()
    {
        Runtime.Sync();

        MethodInfo mTargetClo1 = typeof(Targets).GetMethod(nameof(Targets.Clo1), Static)!;
        MethodInfo mTargetClo2 = typeof(Targets).GetMethod(nameof(Targets.Clo2), Static)!;
        MethodInfo mTargetClo3 = typeof(Targets).GetMethod(nameof(Targets.Clo3), Static)!;
        MethodInfo mTargetClo4 = typeof(Targets).GetMethod(nameof(Targets.Clo4), Static)!;

        Delegate qDelClo1 = CreateFDelegate(Side.Quire, 3, [typeof(object), typeof(string), typeof(object)], mTargetClo1);
        Delegate mDelClo1 = CreateFDelegate(Side.Mathcad, 3, [typeof(object), typeof(string), typeof(object)], mTargetClo1);
        Delegate qDelClo2 = CreateFDelegate(Side.Quire, 4, [typeof(object), typeof(string), typeof(object), typeof(object)], mTargetClo2);
        Delegate mDelClo2 = CreateFDelegate(Side.Mathcad, 4, [typeof(object), typeof(string), typeof(object), typeof(object)], mTargetClo2);
        Delegate qDelClo3 = CreateFDelegate(Side.Quire, 5, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object)], mTargetClo3);
        Delegate mDelClo3 = CreateFDelegate(Side.Mathcad, 5, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object)], mTargetClo3);
        Delegate qDelClo4 = CreateFDelegate(Side.Quire, 6, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)], mTargetClo4);
        Delegate mDelClo4 = CreateFDelegate(Side.Mathcad, 6, [typeof(object), typeof(string), typeof(object), typeof(object), typeof(object), typeof(object)], mTargetClo4);

        Func<string, string, string> fb1 = (clo, a) => $"fb1:{clo}:{a}";
        Func<string, string, string, string> fb2 = (clo, a, b) => $"fb2:{clo}:{a}:{b}";
        Func<string, string, string, string, string> fb3 = (clo, a, b, c) => $"fb3:{clo}:{a}:{b}:{c}";
        Func<string, string, string, string, string, string> fb4 = (clo, a, b, c, d) => $"fb4:{clo}:{a}:{b}:{c}:{d}";

        // 1. FunctionClosure_1: token 0x0600032b
        var qFC1Def = Method(Side.Quire, "FunctionClosure_1", Static);
        var mFC1Def = Method(Side.Mathcad, "FunctionClosure_1", Static);
        Assert.Equal(0x0600032b, mFC1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mFC1Def.Module.ModuleVersionId);

        var qFC1 = qFC1Def.MakeGenericMethod(typeof(string));
        var mFC1 = mFC1Def.MakeGenericMethod(typeof(string));

        // Fast path: DelegateFunction
        MethodComparison.Compare("Delegates.FunctionClosure_1.fast", Contract,
            qFC1, null, [qDelClo1, "fclo1"],
            mFC1, null, [mDelClo1, "fclo1"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // Fallback path: Fn_clo
        MethodComparison.Compare("Delegates.FunctionClosure_1.fallback", Contract,
            qFC1, null, [fb1, "fclo1"],
            mFC1, null, [fb1, "fclo1"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. FunctionClosure_2: token 0x0600032d
        var qFC2Def = Method(Side.Quire, "FunctionClosure_2", Static);
        var mFC2Def = Method(Side.Mathcad, "FunctionClosure_2", Static);
        Assert.Equal(0x0600032d, mFC2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mFC2Def.Module.ModuleVersionId);

        var qFC2 = qFC2Def.MakeGenericMethod(typeof(string));
        var mFC2 = mFC2Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.FunctionClosure_2.fast", Contract,
            qFC2, null, [qDelClo2, "fclo2"],
            mFC2, null, [mDelClo2, "fclo2"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.FunctionClosure_2.fallback", Contract,
            qFC2, null, [fb2, "fclo2"],
            mFC2, null, [fb2, "fclo2"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. FunctionClosure_3: token 0x0600032f
        var qFC3Def = Method(Side.Quire, "FunctionClosure_3", Static);
        var mFC3Def = Method(Side.Mathcad, "FunctionClosure_3", Static);
        Assert.Equal(0x0600032f, mFC3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mFC3Def.Module.ModuleVersionId);

        var qFC3 = qFC3Def.MakeGenericMethod(typeof(string));
        var mFC3 = mFC3Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.FunctionClosure_3.fast", Contract,
            qFC3, null, [qDelClo3, "fclo3"],
            mFC3, null, [mDelClo3, "fclo3"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.FunctionClosure_3.fallback", Contract,
            qFC3, null, [fb3, "fclo3"],
            mFC3, null, [fb3, "fclo3"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. FunctionClosure_4: token 0x06000331
        var qFC4Def = Method(Side.Quire, "FunctionClosure_4", Static);
        var mFC4Def = Method(Side.Mathcad, "FunctionClosure_4", Static);
        Assert.Equal(0x06000331, mFC4Def.MetadataToken);
        Assert.Equal(MathcadMvid, mFC4Def.Module.ModuleVersionId);

        var qFC4 = qFC4Def.MakeGenericMethod(typeof(string));
        var mFC4 = mFC4Def.MakeGenericMethod(typeof(string));

        MethodComparison.Compare("Delegates.FunctionClosure_4.fast", Contract,
            qFC4, null, [qDelClo4, "fclo4"],
            mFC4, null, [mDelClo4, "fclo4"],
            ShowDelegateOrValue, ShowDelegateOrValue);

        MethodComparison.Compare("Delegates.FunctionClosure_4.fallback", Contract,
            qFC4, null, [fb4, "fclo4"],
            mFC4, null, [fb4, "fclo4"],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_toFDelegate()
    {
        Runtime.Sync();

        var qToF = Method(Side.Quire, "toFDelegate", Static);
        var mToF = Method(Side.Mathcad, "toFDelegate", Static);
        Assert.Equal(0x06000333, mToF.MetadataToken);
        Assert.Equal(MathcadMvid, mToF.Module.ModuleVersionId);

        Type qF2 = MakeFType(Side.Quire, 2, [typeof(object), typeof(object)]);
        Type mF2 = MakeFType(Side.Mathcad, 2, [typeof(object), typeof(object)]);

        // 1. Same type: already F<object, object>
        MethodInfo mObjId = typeof(Targets).GetMethod(nameof(Targets.ObjectIdentity), Static)!;
        Delegate qAlreadyF = Delegate.CreateDelegate(qF2, mObjId);
        Delegate mAlreadyF = Delegate.CreateDelegate(mF2, mObjId);

        MethodComparison.Compare("Delegates.toFDelegate.already_f", Contract,
            qToF, null, [qF2, qAlreadyF],
            mToF, null, [mF2, mAlreadyF],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. Return type is value type (not class): returns null
        Func<object, int> valueRet = Targets.ValueReturn;
        MethodComparison.Compare("Delegates.toFDelegate.value_return", Contract,
            qToF, null, [qF2, valueRet],
            mToF, null, [mF2, valueRet],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. Reference return, object arg: Func<object, object> converted to F<object, object>
        Func<object, object> funcObj = Targets.ObjectIdentity;
        MethodComparison.Compare("Delegates.toFDelegate.func_object", Contract,
            qToF, null, [qF2, funcObj],
            mToF, null, [mF2, funcObj],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 4. Reference return, class arg: Func<string, string> converted to F<object, object>
        Func<string, string> funcString = Targets.Identity;
        MethodComparison.Compare("Delegates.toFDelegate.func_class", Contract,
            qToF, null, [qF2, funcString],
            mToF, null, [mF2, funcString],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 5. Reference return, value type arg: Func<int, string> returns null
        Func<int, string> funcValArg = Targets.ValueArg;
        MethodComparison.Compare("Delegates.toFDelegate.value_arg", Contract,
            qToF, null, [qF2, funcValArg],
            mToF, null, [mF2, funcValArg],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 6. Non-object class return, object arg: Func<object, string>
        Func<object, string> funcClassRet = Targets.StringReturnObjectArg;
        MethodComparison.Compare("Delegates.toFDelegate.class_ret_obj_arg", Contract,
            qToF, null, [qF2, funcClassRet],
            mToF, null, [mF2, funcClassRet],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_Wrap_and_Projections()
    {
        Runtime.Sync();

        // 1. Wrap1: token 0x0600033b
        var qWrap1Def = Method(Side.Quire, "Wrap1", Static);
        var mWrap1Def = Method(Side.Mathcad, "Wrap1", Static);
        Assert.Equal(0x0600033b, mWrap1Def.MetadataToken);
        Assert.Equal(MathcadMvid, mWrap1Def.Module.ModuleVersionId);

        var qWrap1 = qWrap1Def.MakeGenericMethod(typeof(string), typeof(int));
        var mWrap1 = mWrap1Def.MakeGenericMethod(typeof(string), typeof(int));
        MethodInfo mWrapF1 = typeof(Targets).GetMethod(nameof(Targets.WrapF1), Static)!;
        Delegate qF1 = CreateFDelegate(Side.Quire, 2, [typeof(string), typeof(int)], mWrapF1);
        Delegate mF1 = CreateFDelegate(Side.Mathcad, 2, [typeof(string), typeof(int)], mWrapF1);

        MethodComparison.Compare("Delegates.Wrap1", Contract,
            qWrap1, null, [qF1],
            mWrap1, null, [mF1],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 2. Wrap2: token 0x0600033c
        var qWrap2Def = Method(Side.Quire, "Wrap2", Static);
        var mWrap2Def = Method(Side.Mathcad, "Wrap2", Static);
        Assert.Equal(0x0600033c, mWrap2Def.MetadataToken);
        Assert.Equal(MathcadMvid, mWrap2Def.Module.ModuleVersionId);

        var qWrap2 = qWrap2Def.MakeGenericMethod(typeof(string), typeof(int), typeof(string));
        var mWrap2 = mWrap2Def.MakeGenericMethod(typeof(string), typeof(int), typeof(string));
        MethodInfo mWrapF2 = typeof(Targets).GetMethod(nameof(Targets.WrapF2), Static)!;
        Delegate qF2 = CreateFDelegate(Side.Quire, 3, [typeof(string), typeof(int), typeof(string)], mWrapF2);
        Delegate mF2 = CreateFDelegate(Side.Mathcad, 3, [typeof(string), typeof(int), typeof(string)], mWrapF2);

        MethodComparison.Compare("Delegates.Wrap2", Contract,
            qWrap2, null, [qF2],
            mWrap2, null, [mF2],
            ShowDelegateOrValue, ShowDelegateOrValue);

        // 3. Wrap3: token 0x0600033d
        var qWrap3Def = Method(Side.Quire, "Wrap3", Static);
        var mWrap3Def = Method(Side.Mathcad, "Wrap3", Static);
        Assert.Equal(0x0600033d, mWrap3Def.MetadataToken);
        Assert.Equal(MathcadMvid, mWrap3Def.Module.ModuleVersionId);

        var qWrap3 = qWrap3Def.MakeGenericMethod(typeof(string), typeof(int), typeof(string), typeof(double));
        var mWrap3 = mWrap3Def.MakeGenericMethod(typeof(string), typeof(int), typeof(string), typeof(double));
        MethodInfo mWrapF3 = typeof(Targets).GetMethod(nameof(Targets.WrapF3), Static)!;
        Delegate qF3 = CreateFDelegate(Side.Quire, 4, [typeof(string), typeof(int), typeof(string), typeof(double)], mWrapF3);
        Delegate mF3 = CreateFDelegate(Side.Mathcad, 4, [typeof(string), typeof(int), typeof(string), typeof(double)], mWrapF3);

        MethodComparison.Compare("Delegates.Wrap3", Contract,
            qWrap3, null, [qF3],
            mWrap3, null, [mF3],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_project21()
    {
        Runtime.Sync();
        var qProj21Def = Method(Side.Quire, "project21", Static);
        var mProj21Def = Method(Side.Mathcad, "project21", Static);
        Assert.Equal(0x0600033e, mProj21Def.MetadataToken);
        Assert.Equal(MathcadMvid, mProj21Def.Module.ModuleVersionId);

        var qProj21 = qProj21Def.MakeGenericMethod(typeof(string), typeof(int));
        var mProj21 = mProj21Def.MakeGenericMethod(typeof(string), typeof(int));

        MethodComparison.Compare("Delegates.project21", Contract,
            qProj21, null, [],
            mProj21, null, [],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_project22()
    {
        Runtime.Sync();
        var qProj22Def = Method(Side.Quire, "project22", Static);
        var mProj22Def = Method(Side.Mathcad, "project22", Static);
        Assert.Equal(0x0600033f, mProj22Def.MetadataToken);
        Assert.Equal(MathcadMvid, mProj22Def.Module.ModuleVersionId);

        var qProj22 = qProj22Def.MakeGenericMethod(typeof(string), typeof(int));
        var mProj22 = mProj22Def.MakeGenericMethod(typeof(string), typeof(int));

        MethodComparison.Compare("Delegates.project22", Contract,
            qProj22, null, [],
            mProj22, null, [],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_pi_21()
    {
        Runtime.Sync();
        var qPi21Def = Method(Side.Quire, "pi_21", Static);
        var mPi21Def = Method(Side.Mathcad, "pi_21", Static);
        Assert.Equal(0x06000340, mPi21Def.MetadataToken);
        Assert.Equal(MathcadMvid, mPi21Def.Module.ModuleVersionId);

        var qPi21 = qPi21Def.MakeGenericMethod(typeof(string), typeof(int));
        var mPi21 = mPi21Def.MakeGenericMethod(typeof(string), typeof(int));

        MethodComparison.Compare("Delegates.pi_21", Contract,
            qPi21, null, [],
            mPi21, null, [],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }

    [SkippableFact]
    public void Delegates_pi_22()
    {
        Runtime.Sync();
        var qPi22Def = Method(Side.Quire, "pi_22", Static);
        var mPi22Def = Method(Side.Mathcad, "pi_22", Static);
        Assert.Equal(0x06000341, mPi22Def.MetadataToken);
        Assert.Equal(MathcadMvid, mPi22Def.Module.ModuleVersionId);

        var qPi22 = qPi22Def.MakeGenericMethod(typeof(string), typeof(int));
        var mPi22 = mPi22Def.MakeGenericMethod(typeof(string), typeof(int));

        MethodComparison.Compare("Delegates.pi_22", Contract,
            qPi22, null, [],
            mPi22, null, [],
            ShowDelegateOrValue, ShowDelegateOrValue);
    }
}
