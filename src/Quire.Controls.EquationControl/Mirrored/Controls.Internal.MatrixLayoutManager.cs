namespace Quire.Controls.Internal;

internal partial class MatrixLayoutManager
{
    public override object Clone()
    {
        return new MatrixLayoutManager(this);
    }
}
