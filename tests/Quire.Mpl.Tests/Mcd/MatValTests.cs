using System.Collections;
using Quire.Mcd;
using Quire.Mpl.Internal;

namespace Quire.Mpl.Tests.Mcd;

public class MatValTests
{
    public MatValTests() => RuntimeThread.Ensure();
    [Fact]
    public void Constructors_preserve_storage_shape_and_dimension_rules()
    {
        var m = new MatVal(2, 3, true, 4);
        Assert.Equal(12, m.m_rarr.Length);
        Assert.Equal(12, m.m_iarr.Length);
        Assert.Null(m.m_dim);
        Assert.Equal(Flags.MatrixTag, m.Data.m_flags);
        Assert.Equal(6, m.Isize);
        Assert.False(m.Packed);
        var re = new double[12];
        var im = new double[12];
        var ptrs = new Hashtable();
        var view = new MatVal(re, im, 2, 3, 4, ptrs, Flags.MatrixTag | Flags.Shared, 1);
        Assert.Same(re, view.m_rarr);
        Assert.Same(im, view.m_iarr);
        Assert.Same(ptrs, view.m_parr);
        Assert.Same(Dim.identity, view.m_dim);
        Assert.Equal(Flags.MatrixTag | Flags.Shared, view.Data.m_flags);
        Assert.Equal(1, view.Offset);
        Assert.Equal(10, view.Index(1, 2));
        Assert.Equal(2, view.Index(1));
        Assert.Equal(1, view.RowOf(10));
        Assert.Equal(2, view.ColOf(10));
        Assert.Equal(131, view.GetHashCode());
        Assert.Null(new MatVal().m_rarr);
        Assert.Same(Dim.identity, new MatVal().m_dim);
        Assert.Empty(new MatVal(-2, 0).m_rarr);
        Assert.Single(new MatVal(2, 1, false, 1).m_rarr);
        Assert.Throws<OverflowException>(() => new MatVal(-1, 1));
        Assert.Same(re, new MatVal(re).m_rarr);
        Assert.Same(im, new MatVal(re, im).m_iarr);
        Assert.Equal(1, new MatVal(re, im, 2).Cols);
        Assert.Equal(4, new MatVal(re, im, 2, 3, 4).Anr);
        Assert.Equal(2, new MatVal(re, im, 2, 3).Anr);
        Assert.Same(ptrs, new MatVal(re, im, 2, 3, 4, ptrs).m_parr);
        Assert.Throws<NullReferenceException>(() => new MatVal((double[])null!));
    }

    [Fact]
    public void Pointer_nan_uses_high_word_and_preserves_payload()
    {
        Assert.Equal(-1, BitConverter.DoubleToInt64Bits(MatVal.PtrNan));
        Assert.Equal(-1, BitConverter.DoubleToInt64Bits(MatVal.ThePtrNan()));
        Assert.Equal(2146959360u, MatVal.NanMask);
        foreach (long bits in new[] { -1L, unchecked((long)0xFFFFFFFF00000000UL), unchecked((long)0x7FF8000000000000UL), 0L })
        {
            double value = BitConverter.Int64BitsToDouble(bits);
            Assert.Equal((uint)((ulong)bits >> 32) == uint.MaxValue, MatVal.IsPtrNan(ref value));
            Assert.Equal(bits, BitConverter.DoubleToInt64Bits(value));
            var matrix = new MatVal(new[] { value });
            var privateQuery = typeof(MatVal).GetMethod("IsPtrNan", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
            Assert.Equal((uint)((ulong)bits >> 32) == uint.MaxValue, privateQuery.Invoke(matrix, new object[] { 0, 0 }));
        }
    }

    [Fact]
    public void Imaginary_query_discards_all_zero_storage_and_scans_padding()
    {
        var m = new MatVal(1, 1, true, 2);
        var im = m.m_iarr;
        Assert.False(m.HasImag);
        Assert.Null(m.m_iarr);
        m.AllocImaginary();
        Assert.NotSame(im, m.m_iarr);
        m.m_iarr![1] = double.NaN;
        Assert.True(m.HasImag);
        m.AllocImaginary();
        Assert.All(m.m_iarr, value => Assert.Equal(0, value));
        Assert.False(m.HasImag);
    }

    [Fact]
    public void Flags_preserve_tag_and_independent_bits()
    {
        var m = new MatVal(2);
        m.Immutable = true;
        m.Shared = true;
        Assert.True(m.Immutable);
        Assert.True(m.Shared);
        Assert.Equal(Flags.MatrixTag | Flags.CopyOnWrite | Flags.Shared, m.Data.m_flags);
        m.Immutable = false;
        Assert.True(m.Shared);
        m.Shared = false;
        Assert.Equal(Flags.MatrixTag, m.Data.m_flags);
    }
}
