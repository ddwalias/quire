#nullable disable
#pragma warning disable

using System.Windows.Input;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.ExcelComponent;

public static partial class OleBaseComponentCommands
{
    public static readonly RoutedUICommand NewOleComponentExpression = new RoutedUICommand("NewOleComponentExpression", "New Ole Component Expression", typeof(OleBaseComponentControl));
}
