using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Mcd;

public class MatValIndexedTests
{
    [Fact]
    public void Checked_access_reports_original_argument_positions()
    {
        var vector=new MatVal(new double[]{1,2});
        Assert.Equal(2,((NumVal)vector.mcd_get_1(1)).Real);
        Assert.Equal(2u,Assert.Throws<AppException>(()=>vector.mcd_get_1(-1)).Argument);
        Assert.Equal(uint.MaxValue,Assert.Throws<AppException>(()=>vector.mcd_sym_get_1(-1)).Argument);
        Assert.Equal(1u,Assert.Throws<AppException>(()=>new MatVal(1,2).mcd_get_1(0)).Argument);
        Assert.Equal(2u,Assert.Throws<AppException>(()=>vector.mcd_get_2(0,1)).Argument);
        Assert.Throws<DivideByZeroException>(()=>new MatVal(0).iterator_get(0));
    }
    [Fact]
    public void Axis_queries_classify_pointer_objects_and_strict_imaginary_threshold()
    {
        var m=new MatVal(2,2,true);
        m.set_2(0,0,3.0);
        m.set_2i(0,0,0.1);
        m.set_ptrs_2(1,0,new StrVal("text"));
        Assert.Equal(ElementType.Complex,m.get_type_2(0,0,0.1));
        Assert.Equal(ElementType.Double,m.get_type_2(0,0,0.2));
        Assert.Equal(new[]{ElementType.Complex,ElementType.String},m.element_types(MatAxis.Col,0,double.NaN));
        Assert.Equal(1.5,m.get_doubles(MatAxis.Row,0,0.2,2).First());
        Assert.IsType<StrVal>(m.get_data(MatAxis.Col,0,0.2,2).Last());
    }
    [Fact]
    public void Scalar_division_copies_immutable_storage_and_preserves_number_units()
    {
        new RuntimeOptions().SyncToThread();
        var m=new MatVal(new double[]{8,4}) { Immutable=true };
        var divided=m/2;
        Assert.NotSame(m,divided);
        Assert.Equal(new double[]{8,4},m.m_rarr);
        Assert.Equal(new double[]{4,2},divided.m_rarr);
        var dimension=Dim.Parse("V{Length 60000}");
        var number=new NumVal(8,4,dimension);
        var scaled=Assert.IsType<NumVal>(Utilities.Divide(number,2));
        Assert.Same(dimension,scaled.m_dim);
        Assert.Equal(4,scaled.Real);
        Assert.Equal(2,scaled.Imag);
        Assert.Equal("illegal factor",Assert.Throws<AppException>(()=>Utilities.Divide("text",0)).Message);
        Assert.Same(number,Utilities.Multiply(number,1));
    }
    [Fact]
    public void Native_shape_declaration_has_original_offsets_and_default_pointers()
    {
        Assert.Equal(32,System.Runtime.InteropServices.Marshal.SizeOf<mat_val>());
        Assert.Equal(4,System.Runtime.InteropServices.Marshal.OffsetOf<mat_val>(nameof(mat_val.m_flags)).ToInt32());
        Assert.Equal(16,System.Runtime.InteropServices.Marshal.OffsetOf<mat_val>(nameof(mat_val.m_rv)).ToInt32());
        Assert.Equal(24,System.Runtime.InteropServices.Marshal.OffsetOf<mat_val>(nameof(mat_val.m_iv)).ToInt32());
        Assert.Equal(nint.Zero,new MatVal(2).Data.m_rv);
        Assert.Equal(nint.Zero,new MatVal(2).Data.m_iv);
    }
}
