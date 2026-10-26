#nullable disable
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial class FrameworkElementXamlHelper : DefaultPageElementXamlHelper
{
    public override UIElement GetPrintView(UIElement element)
    {
        UIElement printView = base.GetPrintView(element);
        if (printView is FrameworkElement frameworkElement)
        {
            frameworkElement.Loaded += (sender, e) =>
            {
                ClearBitmapEffects(sender as DependencyObject);
            };
        }
        return printView;
    }

    private static void ClearBitmapEffects(DependencyObject visual)
    {
        visual.ClearValue(UIElement.BitmapEffectProperty);
        int childrenCount = VisualTreeHelper.GetChildrenCount(visual);
        for (int i = 0; i < childrenCount; i++)
        {
            ClearBitmapEffects(VisualTreeHelper.GetChild(visual, i));
        }
    }

    public FrameworkElementXamlHelper() { }
}
