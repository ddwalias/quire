using System.Collections;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValDifferentialTests
{
    private static void Describe(Side s, List<string> log, object m)
    {
        foreach (var p in new[] { "Rows", "Cols", "Anr", "Offset", "Isize", "Packed", "Immutable", "Shared" })
            log.Log(s, p, () => Get(m, p));
        log.Add("dim " + s.DimText(Get(m, "m_dim")));
        log.Add("flags " + Convert.ToInt64(Get(Get(m, "Data")!, "m_flags")));
        log.Log(s,"native real pointer",()=>Get(Get(m,"Data")!,"m_rv"));
        log.Log(s,"native imaginary pointer",()=>Get(Get(m,"Data")!,"m_iv"));
        log.Add("real " + s.Show(Get(m, "m_rarr")));
        log.Add("imag " + s.Show(Get(m, "m_iarr")));
        log.Log(s, "hash", () => m.GetHashCode());
        foreach (int i in new[] { -1, 0, 1, 7 })
        {
            log.Log(s, "Index1", () => Call(m, "Index", i));
            log.Log(s, "Index2", () => Call(m, "Index", i, 2));
            log.Log(s, "RowOf", () => Call(m, "RowOf", i));
            log.Log(s, "ColOf", () => Call(m, "ColOf", i));
        }
    }

    [SkippableFact]
    public void Constructors_and_storage() => Scenario.Compare((s, log) =>
    {
        var t = s.T("Mcd.MatVal");
        foreach (object?[] args in new object?[][]
        {
            [], [2], [2,3], [2,3,true], [2,3,true,4], [-2,0], [2,1,false,1], [-1,1],
            [new double[]{1,2,3}], [new double[]{1,2},new double[]{3,4}],
            [new double[]{1},null,2], [new double[]{1},null,2,3],
            [new double[]{1},null,2,3,4], [new double[]{1},null,2,3,4,new Hashtable()],
            [new double[12],new double[12],2,3,4,new Hashtable(),Enum.ToObject(s.T("Mcd.Flags"),2146697220),1],
        })
        {
            object m;
            try { m = New(t, args); }
            catch(Exception e) { log.Add(Thrown(e)); continue; }
            Describe(s, log, m);
        }
    });

    [SkippableFact]
    public void Imaginary_flags_and_nan() => Scenario.Compare((s, log) =>
    {
        var t = s.T("Mcd.MatVal");
        log.Log(s,"PtrNan",()=>Get(t,"PtrNan"));
        log.Log(s,"ThePtrNan",()=>Call(t,"ThePtrNan"));
        log.Log(s,"NanMask",()=>Get(t,"NanMask"));
        foreach (long bits in new[]{-1L,unchecked((long)0xFFFFFFFF00000000UL),unchecked((long)0x7FF8000000000000UL),0L})
        {
            object?[] args = [BitConverter.Int64BitsToDouble(bits)];
            log.Log(s,"IsPtrNan",()=>CallSig(t,"IsPtrNan",["Double&"],args));
            log.Add("payload " + Side.Bits((double)args[0]!));
        }
        var m = New(t,1,1,true,2);
        foreach (var p in new[]{"Immutable","Shared"})
        {
            Set(m,p,true);
            log.Log(s,p,()=>Get(m,p));
            log.Add("flags "+Convert.ToInt64(Get(Get(m,"Data")!,"m_flags")));
            Set(m,p,false);
        }
        log.Log(s,"HasImag zeros",()=>Get(m,"HasImag"));
        log.Add("imag "+s.Show(Get(m,"m_iarr")));
        Call(m,"AllocImaginary");
        ((double[])Get(m,"m_iarr")!)[1]=double.NaN;
        log.Log(s,"HasImag padding",()=>Get(m,"HasImag"));
        Call(m,"AllocImaginary");
        log.Add("imag "+s.Show(Get(m,"m_iarr")));
        log.Log(s,"HasImag reset",()=>Get(m,"HasImag"));
    });

    public static string Storage(Side s, object m)
    {
        var data=Get(m,"Data")!;
        string text=$"{Get(m,"Rows")}x{Get(m,"Cols")} stride {Get(m,"Anr")} offset {Get(m,"Offset")} flags {Convert.ToInt64(Get(data,"m_flags"))} dim {s.DimText(Get(m,"m_dim"))} real {s.Show(Get(m,"m_rarr"))} imag {s.Show(Get(m,"m_iarr"))}";
        var table=Get(m,"m_parr") as Hashtable;
        text+=" ptrs "+(table==null?"null":string.Join(";",table.Keys.Cast<int>().Order().Select(k=>k+":"+s.Show(table[k]))));
        return text;
    }
    private static object Matrix(Side s,string dimension="poly",int rows=2,int cols=2,int stride=3,int offset=1)
    {
        var m=New(s.T("Mcd.MatVal"),new double[offset+stride*cols],new double[offset+stride*cols],rows,cols,stride,new Hashtable(),Enum.ToObject(s.T("Mcd.Flags"),2146697216),offset);
        Set(m,"m_dim",s.D(dimension));
        return m;
    }
    private static void State(Side s,List<string> log,string label,object m) => log.Add(label+" "+Storage(s,m));

    [SkippableFact]
    public void Numeric_setters_and_raw_access() => Scenario.Compare((s,log)=>
    {
        foreach(var matrixDim in Side.DimNames)
        foreach(var numberDim in Side.DimNames)
        foreach(int overload in Enumerable.Range(0,6))
        {
            var m=Matrix(s,matrixDim);
            string label=$"{matrixDim} <- {numberDim} setter {overload}";
            object value=s.Num(2,3,numberDim);
            switch(overload)
            {
                case 0: CallSig(m,"set_1",["Int32","Object"],1,value); break;
                case 1: CallSig(m,"set_2",["Int32","Int32","Object"],1,1,value); break;
                case 2: Call(m,"set_1",1,2.0,s.D(numberDim)); break;
                case 3: Call(m,"set_2",1,1,2.0,s.D(numberDim)); break;
                case 4: Call(m,"set_1",1,2.0,3.0,s.D(numberDim)); break;
                case 5: Call(m,"set_2",1,1,2.0,3.0,s.D(numberDim)); break;
            }
            State(s,log,label,m);
            foreach(int index in new[]{-2,-1,0,1,2,3,4,6,7})
            {
                log.Log(s,"get_1",()=>Call(m,"get_1",index));
                log.Log(s,"get_real",()=>CallSig(m,"get_real",["Int32"],index));
                log.Log(s,"get_imag",()=>CallSig(m,"get_imag",["Int32"],index));
                log.Log(s,"get_ptr",()=>Call(m,"get_1_ptr",index));
                log.Log(s,"get_dim",()=>Call(m,"get_1_dim",index));
                log.Log(s,"mixed",()=>CallSig(m,"get_real_mixed",["Int32"],index));
            }
            var scratch=s.Num(-123,-456,"T");
            log.Log(s,"get_na",()=>Call(m,"get_1_na",scratch,1));
            log.Add("scratch "+s.Show(scratch));
            log.Log(s,"raw overwrite",()=>CallSig(m,"set_1",["Int32","Double"],1,9.0));
            State(s,log,"after overwrite",m);
            log.Log(s,"get_1 after overwrite",()=>Call(m,"get_1",1));
            log.Log(s,"get_matrixdim",()=>MatDimDifferentialTests.DescribeDimension(s,Call(m,"get_1_matdim",1)));
        }
    });

    [SkippableFact]
    public void Pointer_queries_canonicalization_and_packing() => Scenario.Compare((s,log)=>
    {
        var kinds=new[]{"numbers","mixed","zeros","strings","nested","missing","stale"};
        foreach(var kind in kinds)
        foreach(bool shared in new[]{false,true})
        foreach(var query in new[]{"HasPtrs","RepHasPtrs","IsUniformScalar","IsUniform","UniformDimension","PhysicalDimension","IsDimless","IsScalarMatrix","IsRealMatrix","IsRealMatrixMixed","IsRealVector","MatrixDimension","Canonicalize"})
        {
            var m=Matrix(s,"identity");
            Set(m,"Shared",shared);
            for(int c=0;c<2;c++) for(int r=0;r<2;r++)
            {
                object? value=kind switch
                {
                    "numbers"=>s.Num(r+c,0,"L"),
                    "mixed"=>s.Num(r+c,r,r==c?"L":"T"),
                    "zeros"=>s.Num(0,0,"mcd_zero"),
                    "strings"=>"hello",
                    "nested"=>New(s.T("Mcd.MatVal"),1),
                    _=>null,
                };
                Call(m,"set_ptrs_2",r,c,value);
            }
            if(kind=="stale") ((double[])Get(m,"m_rarr")!)[(int)Call(m,"Index",0,0)!]=9;
            if(kind=="missing") Set(m,"m_parr",null);
            string label=$"{kind} shared {shared} {query}";
            if(query=="Canonicalize") log.Log(s,label,()=>Call(m,query));
            else if(query=="MatrixDimension") log.Log(s,label,()=>MatDimDifferentialTests.DescribeDimension(s,Get(m,query)));
            else log.Log(s,label,()=>Get(m,query));
            State(s,log,"state",m);
            log.Log(s,"PackReal",()=>Call(m,"PackReal"));
            log.Log(s,"PackImag",()=>Call(m,"PackImag"));
            log.Log(s,"PackReal col",()=>Call(m,"PackReal",1));
            log.Log(s,"PackImag col",()=>Call(m,"PackImag",1));
            log.Log(s,"PackReal sub",()=>Call(m,"PackReal",0,2,1,2));
            log.Log(s,"PackImag sub",()=>Call(m,"PackImag",0,2,1,2));
            log.Log(s,"ColDimArray",()=>Call(m,"ColDimArray",0));
            log.Log(s,"RowDimArray",()=>Call(m,"RowDimArray",0));
            log.Log(s,"get missing",()=>Call(m,"get_2",0,0));
            State(s,log,"after get",m);
        }
        var matrix=Matrix(s);
        Call(matrix,"set_ptrs_2",0,0,"text");
        log.Log(s,"mixed string handle",()=>Call(matrix,"get_real_mixed_handle",0,0,17u));
        Call(matrix,"set_ptrs_2",0,1,matrix);
        State(s,log,"self pointer snapshot",matrix);
        var child=Get(matrix,"m_parr") as Hashtable;
        var nested=child![(int)Call(matrix,"Index",0,1)!]!;
        log.Log(s,"nested immutable",()=>Get(nested,"Immutable"));
        log.Log(s,"self copied",()=>!ReferenceEquals(matrix,nested));
        log.Log(s,"contains text value",()=>Call(matrix,"contains",child[(int)Call(matrix,"Index",0,0)!]));
    });

    [SkippableFact]
    public void Native_struct_layout_and_dimension_rows() => Scenario.Compare((s,log)=>
    {
        var t=s.T("Mcd.mat_val");
        log.Log(s,"native struct size",()=>System.Runtime.InteropServices.Marshal.SizeOf(t));
        foreach(var f in new[]{"m_anr","m_flags","m_rows","m_cols","m_rv","m_iv"})
            log.Log(s,f+" offset",()=>System.Runtime.InteropServices.Marshal.OffsetOf(t,f));
        foreach(var dim in Side.DimNames)
        foreach(int stride in new[]{0,1,3})
        {
            var m=Matrix(s,dim,2,2,stride,0);
            log.Log(s,"ColDimArray raw",()=>Call(m,"ColDimArray",0));
            log.Log(s,"RowDimArray raw",()=>Call(m,"RowDimArray",0));
        }
    });
}
