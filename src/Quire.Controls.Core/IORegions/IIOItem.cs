#nullable disable
using System.Windows;

namespace Quire.Controls.Core.IORegions;

public partial interface IIOItem
{
    DependencyObject Region { get; }
    string IdSexp { get; }
    string Alias { get; set; }
    bool IsInput { get; }
    void ShowInWorksheet();
}
