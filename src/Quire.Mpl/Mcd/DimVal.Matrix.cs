#nullable disable
using Quire.Mirror;
using Quire.Mpl.Run;
namespace Quire.Mcd;
public abstract partial class DimVal
{
    [MirrorAnalyzed("Mcd.MatDim")]
    public virtual MatDim MatrixDimension => MatDim.make(m_dim);
    [MirrorAnalyzed("Mcd.MatDim")]
    public static MatDim MatDimOf(object o) => (o as DimVal).MatrixDimension;

    [MirrorAnalyzed("Mcd.MatVal")]
    public virtual MatVal extend_1(int i)
    {
        if(i<0) AppException.Raise("index out of bounds",2u);
        return new MatVal(i+1);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public virtual MatVal extend_2(int i,int j)
    {
        if(i<0) AppException.Raise("index out of bounds",2u);
        if(j<0) AppException.Raise("index out of bounds",3u);
        return new MatVal(i+1,j+1);
    }
    [MirrorAnalyzed("Mcd.MatVal")]
    public virtual MatVal extend_1u(int i) => new MatVal(i+1);
    [MirrorAnalyzed("Mcd.MatVal")]
    public virtual MatVal extend_2u(int i,int j) => new MatVal(i+1,j+1);
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set0_1(int i) { var m=extend_1(i); m.set_1(i,0.0); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set0_2(int i,int j) { var m=extend_2(i,j); m.set_2(i,j,0.0); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_1(int i,double re) { var m=extend_1(i); m.set_1(i,re,Dim.identity); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_1(int i,object v) => matrix_set_1(i,v as DimVal);
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_1(int i,DimVal v) { v.asImmutable(); var m=extend_1(i); m.set_1(i,v); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_1_ob(int i,object v) { var m=extend_1(i); m.set_1(i,v); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_2(int i,int j,double re) { var m=extend_2(i,j); m.set_2(i,j,re,Dim.identity); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_2(int i,int j,double re,double im) { var m=extend_2(i,j); m.set_2(i,j,re,im,Dim.identity); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_2i(int i,int j,double im) { var m=extend_2(i,j); m.set_2i(i,j,im); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_2(int i,int j,object v) => matrix_set_2(i,j,v as DimVal);
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_2(int i,int j,DimVal v) { v.asImmutable(); var m=extend_2(i,j); m.set_2(i,j,v); return m; }
    [MirrorAnalyzed("Mcd.MatVal")]
    public MatVal matrix_set_2_ob(int i,int j,object v) { var m=extend_2(i,j); m.set_2(i,j,v); return m; }
}
