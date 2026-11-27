using Quire.Controls.Core;

namespace Quire.EquationEditor.Tree;

public partial class MatrixNodeInfo
{
    public MatrixFormat MatrixFormat
    {
        get => a_;
        set
        {
            a_ = value;
            a_.FormatChanged += a;
        }
    }
}
