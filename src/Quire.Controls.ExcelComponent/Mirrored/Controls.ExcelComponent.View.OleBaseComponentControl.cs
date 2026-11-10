using System.Windows;

namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleBaseComponentControl
{
    public static double GetItemHeight(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.HeightProperty);
    }

    public static void SetItemHeight(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.HeightProperty, (object)value);
    }

    public static double GetOleComponentLocationLeft(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationLeftProperty);
    }

    public static void SetOleComponentLocationLeft(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationLeftProperty, (object)value);
    }

    public Size CoerceNewSize(Size newSize, global::Quire.Controls.Core.IResizeData resizeData)
    {
        return newSize;
    }

    public override string GetBlockEquationTypeAttributeName(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        return string.Empty;
    }
}
