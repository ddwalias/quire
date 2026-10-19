using System.Reflection;
using Xunit;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatDimDifferentialTests
{
    private static Array Dims(Side s, params string[] names)
    {
        var array = Array.CreateInstance(s.Dim,names.Length);
        for(int i=0;i<names.Length;i++) array.SetValue(s.D(names[i]),i);
        return array;
    }

    [SkippableFact]
    public void Dimension_arrays() => Scenario.Compare((s,log)=>
    {
        var t=s.T("Mcd.MatDim");
        foreach(var names in new[]{Array.Empty<string>(),new[]{"L"},new[]{"L","L"},new[]{"L","T"},new[]{"poly","poly"},new[]{"identity","poly"}})
        {
            var dims=Dims(s,names);
            log.Log(s,"Uniform",()=>Call(t,"Uniform",dims));
            log.Log(s,"Invert",()=>Call(t,"Invert",dims));
            log.Log(s,"Mult scalar",()=>CallSig(t,"Mult",["Dim","Dim[]"],s.D("L"),dims));
            log.Log(s,"Product",()=>Call(t,"Product",s.D("T"),dims));
            log.Log(s,"Push",()=>Call(t,"Push",s.D("L"),dims));
            foreach(var rhs in new[]{Array.Empty<string>(),new[]{"L"},new[]{"L","T"},new[]{"L2","v"},new[]{"poly","poly"},new[]{"L","T","N"}})
            {
                var d2=Dims(s,rhs);
                log.Log(s,"Eq_b arrays",()=>CallSig(t,"Eq_b",["Dim[]","Dim[]"],dims,d2));
                log.Log(s,"IsScalarMultiple",()=>Call(t,"IsScalarMultiple",dims,d2));
                log.Log(s,"Mult arrays",()=>CallSig(t,"Mult",["Dim[]","Dim[]"],dims,d2));
            }
        }
        log.Log(s,"Uniform null",()=>Call(t,"Uniform",new object?[]{null}));
        log.Log(s,"Invert null",()=>Call(t,"Invert",new object?[]{null}));
        log.Log(s,"Mult scalar null",()=>CallSig(t,"Mult",["Dim","Dim[]"],s.D("L"),null));
        log.Log(s,"Product null",()=>Call(t,"Product",s.D("L"),null));
        log.Log(s,"Push null",()=>Call(t,"Push",s.D("L"),null));
        log.Log(s,"CMult null",()=>Call(t,"CMult",null,null));
    });

    public static string DescribeDimension(Side s, object? d)
    {
        if(d==null) return "null";
        string result=$"{d.GetType().Name} {Get(d,"Rows")}x{Get(d,"Cols")} uniform {s.DimText(Get(d,"UniformDim"))} linear {ReferenceEquals(Get(d,"IsLinear"),d)}";
        int rows=(int)Get(d,"Rows")!, cols=(int)Get(d,"Cols")!;
        if(rows*cols>36) return result;
        for(int r=0;r<rows;r++) for(int c=0;c<cols;c++)
            result+=$" ({r},{c})="+s.DimText(Call(d,"dim",r,c));
        return result;
    }

    [SkippableFact]
    public void Nested_dimensions_and_dispatch() => Scenario.Compare((s,log)=>
    {
        var t=s.T("Mcd.MatDim");
        var uniform=Nested(t,"uniform_dim"); var row=Nested(t,"r_dim"); var col=Nested(t,"c_dim"); var rc=Nested(t,"rc_dim");
        var dimensions=new List<object>();
        dimensions.Add(Call(t,"make",s.D("L"))!);
        dimensions.Add(Call(uniform,"make",s.D("T"),2,3)!);
        dimensions.Add(Call(uniform,"make",null,3,2)!);
        dimensions.Add(Call(row,"make",Dims(s,"L","T"),3)!);
        dimensions.Add(Call(col,"make",Dims(s,"L","T"),3)!);
        dimensions.Add(Call(row,"make",Dims(s,"L","L"),2)!);
        dimensions.Add(Call(col,"make",Dims(s,"L","L"),2)!);
        dimensions.Add(Call(row,"make",Call(t,"Invert",Dims(s,"L","T")),2)!);
        dimensions.Add(Call(col,"make",Call(t,"Invert",Dims(s,"L","T")),2)!);
        dimensions.Add(Call(rc,"make_rc",Dims(s,"L","T"),Dims(s,"L","T"))!);
        dimensions.Add(Call(rc,"make_rc",Dims(s,"L","T"),Call(t,"Invert",Dims(s,"L","T")))!);
        foreach(var d in dimensions)
        {
            log.Add(DescribeDimension(s,d));
            foreach(var p in new[]{"RowDimensions","ColDimensions"}) log.Log(s,p,()=>Get(d,p));
            log.Log(s,"row array",()=>Call(d,"RowDimensionArray",0));
            log.Log(s,"col array",()=>Call(d,"ColDimensionArray",0));
            log.Add("element matrix "+DescribeDimension(s,Call(d,"matdim",0,0)));
            log.Add("transpose "+DescribeDimension(s,Call(t,"Transpose",d)));
            log.Log(s,"det",()=>Call(t,"Det",d,2));
            foreach(int power in new[]{-2,-1,0,1,2,3})
                log.Add("power "+power+" "+Outcome(()=>Call(t,"Pwr",d,power),o=>DescribeDimension(s,o)));
            foreach(var other in dimensions)
            {
                log.Log(s,"Eq_b",()=>CallSig(t,"Eq_b",["MatDim","MatDim"],d,other));
                log.Add("MMult "+Outcome(()=>Call(t,"MMult",d,other),o=>DescribeDimension(s,o)));
                log.Log(s,"DMult",()=>Call(t,"DMult",d,other));
                log.Log(s,"Eq",()=>CallSig(t,"Eq",["MatDim","MatDim","UInt32"],d,other,7u));
            }
            foreach(var dn in Side.DimNames)
            {
                log.Log(s,"Eq_b scalar",()=>CallSig(t,"Eq_b",["MatDim","Dim"],d,s.D(dn)));
                log.Log(s,"Eq scalar",()=>CallSig(t,"Eq",["MatDim","Dim","UInt32"],d,s.D(dn),5u));
            }
        }
        foreach(var names in new[]{new[]{"L","L"},new[]{"L","T"},new[]{"poly","poly"},Array.Empty<string>()})
        foreach(int size in new[]{0,1,3})
        foreach(var axis in new[]{"ROW","COL"})
        {
            object axisValue=Enum.Parse(Nested(t,"RC"),axis);
            log.Add("factory "+Outcome(()=>Call(t,"make",axisValue,Dims(s,names),size),o=>DescribeDimension(s,o)));
            log.Add("row factory "+Outcome(()=>CallSig(t,"makeRowDimension",["Dim","Dim[]","Int32"],s.D("L"),Dims(s,names),size),o=>DescribeDimension(s,o)));
            log.Add("col factory "+Outcome(()=>CallSig(t,"makeColDimension",["Dim","Dim[]","Int32"],s.D("L"),Dims(s,names),size),o=>DescribeDimension(s,o)));
        }
        foreach(var rows in new[]{new[]{"L","T"},new[]{"L","L"}})
        foreach(var cols in new[]{new[]{"L","T"},new[]{"T","T"}})
            log.Add("two factors "+DescribeDimension(s,Call(t,"make",Dims(s,rows),Dims(s,cols))));
    });

    private const string Contract = "Mcd.MatDim";
    private static readonly Guid MathcadMvid = new("17a0933b-4988-4c18-963d-563998321cc8");

    private static string Show(Side side, object? value)
    {
        if (value == null) return "null";
        if (value is Array arr)
        {
            var items = new List<string>();
            foreach (var item in arr) items.Add(Show(side, item));
            return "[" + string.Join(", ", items) + "]";
        }
        if (side.Dim.IsInstanceOfType(value)) return side.DimText(value);
        if (side.T("Mcd.MatDim").IsInstanceOfType(value)) return DescribeDimension(side, value);
        return Reflect.Text(value);
    }

    [SkippableFact]
    public void Direct_static_array_operations_and_pwr_match_mathcad()
    {
        Runtime.Sync();

        var qMatDim = sideT("Mcd.MatDim", Side.Quire);
        var mMatDim = sideT("Mcd.MatDim", Side.Mathcad);
        var qDim = Side.Quire.Dim;
        var mDim = Side.Mathcad.Dim;
        var qDimArr = qDim.MakeArrayType();
        var mDimArr = mDim.MakeArrayType();

        // 1. Uniform
        var qUniform = qMatDim.GetMethod("Uniform", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDimArr], null)!;
        var mUniform = mMatDim.GetMethod("Uniform", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDimArr], null)!;
        Assert.Equal(0x06000629, mUniform.MetadataToken);
        Assert.Equal(MathcadMvid, mUniform.Module.ModuleVersionId);

        MethodComparison.Compare("Uniform.singleton", Contract,
            qUniform, null, [Dims(Side.Quire, "L")],
            mUniform, null, [Dims(Side.Mathcad, "L")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Uniform.equal", Contract,
            qUniform, null, [Dims(Side.Quire, "L", "L")],
            mUniform, null, [Dims(Side.Mathcad, "L", "L")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Uniform.distinct", Contract,
            qUniform, null, [Dims(Side.Quire, "L", "T")],
            mUniform, null, [Dims(Side.Mathcad, "L", "T")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Uniform.null", Contract,
            qUniform, null, [null],
            mUniform, null, [null],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 2. IsScalarMultiple
        var qIsScalar = qMatDim.GetMethod("IsScalarMultiple", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDimArr, qDimArr], null)!;
        var mIsScalar = mMatDim.GetMethod("IsScalarMultiple", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDimArr, mDimArr], null)!;
        Assert.Equal(0x0600062b, mIsScalar.MetadataToken);
        Assert.Equal(MathcadMvid, mIsScalar.Module.ModuleVersionId);

        MethodComparison.Compare("IsScalarMultiple.same", Contract,
            qIsScalar, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "L", "T")],
            mIsScalar, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "L", "T")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("IsScalarMultiple.multiple", Contract,
            qIsScalar, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "L2", "v")],
            mIsScalar, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "L2", "v")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("IsScalarMultiple.nonmultiple", Contract,
            qIsScalar, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "L", "L")],
            mIsScalar, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "L", "L")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 3. Mult(Dim[], Dim[])
        var qMultArr = qMatDim.GetMethod("Mult", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDimArr, qDimArr], null)!;
        var mMultArr = mMatDim.GetMethod("Mult", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDimArr, mDimArr], null)!;
        Assert.Equal(0x0600062c, mMultArr.MetadataToken);
        Assert.Equal(MathcadMvid, mMultArr.Module.ModuleVersionId);

        MethodComparison.Compare("Mult.arrays_constant", Contract,
            qMultArr, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "T", "L")],
            mMultArr, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "T", "L")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Mult.arrays_varying", Contract,
            qMultArr, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "L", "T")],
            mMultArr, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "L", "T")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 4. Mult(Dim, Dim[])
        var qMultScalar = qMatDim.GetMethod("Mult", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDim, qDimArr], null)!;
        var mMultScalar = mMatDim.GetMethod("Mult", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDim, mDimArr], null)!;
        Assert.Equal(0x0600062d, mMultScalar.MetadataToken);
        Assert.Equal(MathcadMvid, mMultScalar.Module.ModuleVersionId);

        MethodComparison.Compare("Mult.scalar", Contract,
            qMultScalar, null, [Side.Quire.D("L"), Dims(Side.Quire, "L", "T")],
            mMultScalar, null, [Side.Mathcad.D("L"), Dims(Side.Mathcad, "L", "T")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 5. Invert(Dim[])
        var qInvert = qMatDim.GetMethod("Invert", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDimArr], null)!;
        var mInvert = mMatDim.GetMethod("Invert", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDimArr], null)!;
        Assert.Equal(0x0600062e, mInvert.MetadataToken);
        Assert.Equal(MathcadMvid, mInvert.Module.ModuleVersionId);

        MethodComparison.Compare("Invert.vector", Contract,
            qInvert, null, [Dims(Side.Quire, "L", "T")],
            mInvert, null, [Dims(Side.Mathcad, "L", "T")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 6. Product(Dim, Dim[])
        var qProduct = qMatDim.GetMethod("Product", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDim, qDimArr], null)!;
        var mProduct = mMatDim.GetMethod("Product", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDim, mDimArr], null)!;
        Assert.Equal(0x0600062f, mProduct.MetadataToken);
        Assert.Equal(MathcadMvid, mProduct.Module.ModuleVersionId);

        MethodComparison.Compare("Product.vector", Contract,
            qProduct, null, [Side.Quire.D("T"), Dims(Side.Quire, "L", "L")],
            mProduct, null, [Side.Mathcad.D("T"), Dims(Side.Mathcad, "L", "L")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 7. Pwr(MatDim, int)
        var qPwr = qMatDim.GetMethod("Pwr", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qMatDim, typeof(int)], null)!;
        var mPwr = mMatDim.GetMethod("Pwr", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mMatDim, typeof(int)], null)!;
        Assert.Equal(0x0600061f, mPwr.MetadataToken);
        Assert.Equal(MathcadMvid, mPwr.Module.ModuleVersionId);

        var qUniform2x2 = Call(qMatDim, "make", Side.Quire.D("L"))!;
        var mUniform2x2 = Call(mMatDim, "make", Side.Mathcad.D("L"))!;

        MethodComparison.Compare("Pwr.positive", Contract,
            qPwr, null, [qUniform2x2, 2],
            mPwr, null, [mUniform2x2, 2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Pwr.zero", Contract,
            qPwr, null, [qUniform2x2, 0],
            mPwr, null, [mUniform2x2, 0],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Pwr.minus_one", Contract,
            qPwr, null, [qUniform2x2, -1],
            mPwr, null, [mUniform2x2, -1],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("Pwr.minus_two", Contract,
            qPwr, null, [qUniform2x2, -2],
            mPwr, null, [mUniform2x2, -2],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        // 8. make(Dim[], Dim[])
        var qMakeTwo = qMatDim.GetMethod("make", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [qDimArr, qDimArr], null)!;
        var mMakeTwo = mMatDim.GetMethod("make", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly, null, [mDimArr, mDimArr], null)!;
        Assert.Equal(0x06000627, mMakeTwo.MetadataToken);
        Assert.Equal(MathcadMvid, mMakeTwo.Module.ModuleVersionId);

        MethodComparison.Compare("make.uniform_rows", Contract,
            qMakeTwo, null, [Dims(Side.Quire, "L", "L"), Dims(Side.Quire, "T", "T")],
            mMakeTwo, null, [Dims(Side.Mathcad, "L", "L"), Dims(Side.Mathcad, "T", "T")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("make.uniform_cols", Contract,
            qMakeTwo, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "v", "v")],
            mMakeTwo, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "v", "v")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));

        MethodComparison.Compare("make.nonuniform_both", Contract,
            qMakeTwo, null, [Dims(Side.Quire, "L", "T"), Dims(Side.Quire, "v", "N")],
            mMakeTwo, null, [Dims(Side.Mathcad, "L", "T"), Dims(Side.Mathcad, "v", "N")],
            v => Show(Side.Quire, v), v => Show(Side.Mathcad, v));
    }

    private static System.Type sideT(string name, Side side) => side.T(name);
}
