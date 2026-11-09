#nullable disable
#pragma warning disable

using System.Windows;

namespace Quire.Controls.Text;

public partial class TextRegion
{
    public TextRegion()
    {
        InitializeComponent();
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if ("Style" == dp.Name || "Template" == dp.Name)
        {
            return false;
        }

        return base.ShouldSerializeProperty(dp);
    }

    public TextRegion CloneTextRegion()
    {
        TextRegion textRegion = new TextRegion();
        textRegion.CloneContent(this, null);
        textRegion.Width = Width;
        return textRegion;
    }
}
