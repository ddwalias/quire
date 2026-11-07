#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IPageElementXamlHelper
{
    UIElement GetPrintView(UIElement element);
}
