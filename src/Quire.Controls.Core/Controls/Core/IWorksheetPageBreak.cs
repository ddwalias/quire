#nullable disable
namespace Quire.Controls.Core;

public partial interface IWorksheetPageBreak
{
    double VerticalPosition { set; }
    int NextPage { get; set; }
}
