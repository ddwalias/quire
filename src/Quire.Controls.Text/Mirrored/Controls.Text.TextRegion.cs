#nullable disable
#pragma warning disable

using System.Windows;

namespace Quire.Controls.Text;

public partial class TextRegion
{
    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if ("Style" == dp.Name || "Template" == dp.Name)
        {
            return false;
        }

        return base.ShouldSerializeProperty(dp);
    }
}
