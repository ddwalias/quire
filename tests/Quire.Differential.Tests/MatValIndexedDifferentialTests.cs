using System.Collections;
using static Quire.Differential.Tests.Reflect;

namespace Quire.Differential.Tests;

public class MatValIndexedDifferentialTests
{
    [SkippableFact]
    public void Checked_access_and_classification() => Scenario.Compare((s,log)=>
    {
        var t=s.T("Mcd.MatVal");
        var m=New(t,3,2,true,4);
        Set(m,"m_dim",s.D("L"));
        CallSig(m,"set_2",["Int32","Int32","Double"],0,0,2.0);
        Call(m,"set_2i",0,0,0.1);
        CallSig(m,"set_2",["Int32","Int32","Double"],1,0,double.NaN);
        Call(m,"set_ptrs_2",2,0,s.Str("text"));
        Call(m,"set_ptrs_2",0,1,s.Num(1,0,"T"));
        Call(m,"set_ptrs_2",1,1,s.Num(1,double.NaN,"T"));
        Call(m,"set_ptrs_2",2,1,null);
        foreach(int i in new[]{-1,0,1,2,3})
        {
            foreach(var method in new[]{"matrix_get_1","sym_matrix_get_1","aref_1","iterator_get"})
                log.Log(s,method,()=>Call(m,method,i));
            log.Log(s,"mcd_get_1 signed",()=>CallSig(m,"mcd_get_1",["Int32"],i));
            log.Log(s,"mcd_get_1 unsigned",()=>CallSig(m,"mcd_get_1",["UInt32"],unchecked((uint)i)));
            foreach(int j in new[]{-1,0,1,2})
            {
                foreach(var method in new[]{"matrix_get_2","sym_matrix_get_2","aref_2"}) log.Log(s,method,()=>Call(m,method,i,j));
                log.Log(s,"mcd_get_2 signed",()=>CallSig(m,"mcd_get_2",["Int32","Int32"],i,j));
                log.Log(s,"mcd_get_2 unsigned",()=>CallSig(m,"mcd_get_2",["UInt32","UInt32"],unchecked((uint)i),unchecked((uint)j)));
                foreach(double threshold in new[]{double.NaN,0,0.1,0.2,double.PositiveInfinity})
                    log.Log(s,"type",()=>Call(m,"get_type_2",i,j,threshold));
            }
        }
        foreach(var axis in new[]{"Row","Col"})
        foreach(int index in new[]{-1,0,1,2})
        foreach(double threshold in new[]{double.NaN,0,0.1,0.2})
        {
            object a=Enum.Parse(s.T("Mcd.MatAxis"),axis);
            log.Log(s,"types",()=>Call(m,"element_types",a,index,threshold));
            log.Log(s,"ptrs",()=>Call(m,"get_ptrs",a,index));
            foreach(double scale in new[]{0,2})
            {
                log.Log(s,"doubles",()=>Call(m,"get_doubles",a,index,threshold,scale));
                log.Log(s,"data",()=>Call(m,"get_data",a,index,threshold,scale));
            }
        }
        foreach(var obj in new object?[]{null,s.Num(1,0,"identity"),m})
        {
            log.Log(s,"static getm1",()=>Call(t,"mcd_getm_1",obj,0));
            log.Log(s,"static getm2",()=>Call(t,"mcd_getm_2",obj,0,0));
            log.Log(s,"static get1",()=>CallSig(t,"mcd_get_1",["Object","Object"],obj,s.Num(0,0,"identity")));
        }
        var vector=New(t,new double[]{1,2,3});
        foreach(int i in new[]{-1,0,2,3}) log.Log(s,"vector index",()=>Call(vector,"DynCall",s.Num(i,0,"identity")));
        log.Log(s,"matrix DynCall",()=>Call(m,"DynCall",s.Num(0,0,"identity"),s.Num(1,0,"identity")));
        log.Log(s,"PackObjects",()=>Call(m,"PackObjects"));
        log.Log(s,"PackObjects offset",()=>Call(m,"PackObjects",2));
    });

    [SkippableFact]
    public void Scalar_division_and_multiplication() => Scenario.Compare((s,log)=>
    {
        foreach(bool packed in new[]{true,false})
        foreach(bool immutable in new[]{false,true})
        foreach(bool pointer in new[]{false,true})
        foreach(double scalar in new[]{1,2,0,double.NaN,double.PositiveInfinity,-2})
        {
            var m=New(s.T("Mcd.MatVal"),2,1,true,packed?2:3);
            CallSig(m,"set_1",["Int32","Double"],0,8.0);
            Call(m,"set_2i",0,0,4.0);
            CallSig(m,"set_1",["Int32","Double"],1,6.0);
            if(pointer) Call(m,"set_ptrs_1",1,s.Num(6,2,"L"));
            Set(m,"Immutable",immutable);
            object? result=null;
            log.Log(s,"matrix divide",()=> { result=Call(s.T("Mcd.MatVal"),"op_Division",m,scalar); return ReferenceEquals(result,m); });
            if(result!=null) log.Add("result "+MatValDifferentialTests.Storage(s,result));
            log.Add("input "+MatValDifferentialTests.Storage(s,m));
        }
        foreach(var value in new object?[]{null,"text",s.Num(6,2,"L"),New(s.T("Mcd.MatVal"),new double[]{6,2})})
        foreach(double scalar in new[]{1,2,0,double.PositiveInfinity,double.NegativeInfinity,double.NaN})
        {
            log.Log(s,"Utilities.Divide",()=>Call(s.T("Mcd.Utilities"),"Divide",value,scalar));
            log.Log(s,"Utilities.Multiply",()=>Call(s.T("Mcd.Utilities"),"Multiply",value,scalar));
        }
    });
}
