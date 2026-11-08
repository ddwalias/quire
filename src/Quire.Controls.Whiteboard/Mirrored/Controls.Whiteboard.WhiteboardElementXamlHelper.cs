#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardElementXamlHelper
{
    public WhiteboardElementXamlHelper() { }

    public override UIElement GetPrintView(UIElement element)
    {
        if (element is not IWhiteboardControl)
        {
            return null;
        }

        UIElement printView = base.GetPrintView(element);
        IWhiteboardControl control = printView as IWhiteboardControl;
        if (control == null)
        {
            return null;
        }

        control.ClearVisualState();
        return printView;
    }
}
