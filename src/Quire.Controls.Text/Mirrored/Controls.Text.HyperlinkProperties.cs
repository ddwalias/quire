#nullable disable
#pragma warning disable

using System;
using System.Windows;

namespace Quire.Controls.Text;

public static partial class HyperlinkProperties
{
    private static readonly Type _typeOfThis = typeof(HyperlinkProperties);

    public static readonly DependencyProperty LinkVisitedProperty =
        DependencyProperty.RegisterAttached("LinkVisited", typeof(bool), _typeOfThis, new FrameworkPropertyMetadata(false));

    public static bool GetLinkVisited(DependencyObject dObj) => (bool)dObj.GetValue(LinkVisitedProperty);

    public static void SetLinkVisited(DependencyObject dObj, bool value) => dObj.SetValue(LinkVisitedProperty, value);
}
