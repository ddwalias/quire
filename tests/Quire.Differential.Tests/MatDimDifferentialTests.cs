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
}
