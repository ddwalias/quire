#nullable disable
#pragma warning disable

using System.ComponentModel;
using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.Text;

public partial class Paragraph
{
    private Size PageSize => WorksheetPropertiesManager.GetWorksheetProperties(this).PageSizeValue;

    private Thickness PageMargin => WorksheetPropertiesManager.GetWorksheetProperties(this).PageMargin;

    public double InnerWidth
    {
        get
        {
            if (_innerWidth == 0)
            {
                _innerWidth = MinWidth;
            }

            return _innerWidth;
        }
        set
        {
            if (_innerWidth != value)
            {
                _innerWidth = value;
                FirePropertyChanged(new PropertyChangedEventArgs(nameof(InnerWidth)));
            }
        }
    }

    public bool NotHookupSizeChangedHandlerUntilLoaded { get; set; }

    private static object CoerceCanvasLeft(DependencyObject d, object basevalue)
    {
        DebugTools.DevAssert(basevalue is double, null);
        double canvasLeft = (double)basevalue;
        if (!canvasLeft.Equals(0.0))
        {
            canvasLeft = 0.0;
        }

        return canvasLeft;
    }

    private static void CanvasLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    public void FireHookupSizeChangedHandler()
    {
        HookupSizeChangedHandler();
    }
}
