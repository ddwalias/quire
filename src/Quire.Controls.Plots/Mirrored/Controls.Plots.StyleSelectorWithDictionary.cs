#nullable disable
#pragma warning disable

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Quire.Controls.Plots;

[ContentProperty("Dictionary")]
public partial class StyleSelectorWithDictionary
{
    public IDictionary Dictionary { get; set; }

    public override Style SelectStyle(object item, DependencyObject container)
    {
        return Dictionary[item.GetType()] as Style;
    }

    public StyleSelectorWithDictionary()
    {
    }
}
