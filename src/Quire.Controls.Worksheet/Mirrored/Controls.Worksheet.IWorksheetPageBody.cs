using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetPageBody
{
    Brush WaterMark { get; set; }

    IObservableRangeCollection<IWhiteboardItemZombie> Zombies { get; }

    Geometry Clip { get; set; }

    double GetItemHeight(IWhiteboardItem item);
}
