using System.Windows;

namespace Quire.Controls;

public partial class FlexingStackPanel
{
    public global::System.Windows.Controls.Orientation Orientation
    {
        get => (global::System.Windows.Controls.Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static void SetSizeType(UIElement element, SizeType value)
    {
        element.SetValue(SizeTypeProperty, value);
    }

    public static SizeType GetSizeType(UIElement element)
    {
        return (SizeType)element.GetValue(SizeTypeProperty);
    }

    public FlexingStackPanel() { }
}
