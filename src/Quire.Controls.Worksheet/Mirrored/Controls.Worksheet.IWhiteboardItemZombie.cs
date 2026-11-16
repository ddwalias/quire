using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial interface IWhiteboardItemZombie
{
    IWhiteboardItem Item { get; }

    Size ItemSize { get; set; }

    Rect ItemBounds { get; set; }

    double ItemVerticalAlignmentOffset { get; set; }

    void UpdateZombieRect();

    void ResetVisualBrush();

    void Hide();

    void Show();
}
