#nullable disable

using System.Windows.Markup;
using System.Windows.Media;

namespace Quire;

public partial class FontDataObject
{
    public FontFamily FontFamily { get; }

    public string LocalizedName { get; }

    public FontDataObject(FontFamily fontFamily, XmlLanguage lang)
    {
        FontFamily = fontFamily;
        LocalizedName = fontFamily.FamilyNames.TryGetValue(lang, out var value) ? value : fontFamily.Source;
    }

    public override string ToString()
    {
        return LocalizedName;
    }
}
