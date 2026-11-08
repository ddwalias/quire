#nullable disable
#pragma warning disable
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.ViewModel;

internal partial class OleObjectResizeData : IResizeData
{
    public Size OldSize { get; private set; }

    public double DiagonalForRedo { get; private set; }

    public double DiagonalForUndo { get; private set; }

    public OleObjectResizeData(Size oldSize, double undoDiagonal, double redoDiagonal)
    {
        OldSize = oldSize;
        DiagonalForUndo = undoDiagonal;
        DiagonalForRedo = redoDiagonal;
    }
}
