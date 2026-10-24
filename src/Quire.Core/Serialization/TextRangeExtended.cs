#nullable disable
using System.Windows.Documents;
using Quire.Mirror;

namespace Quire.Serialization;

public partial class TextRangeExtended
{
    [MirrorVerified("PtcSerializationTextRangeExtendedMechanicalDifferentialTests")]
    public TextRangeExtended(TextPointer tpStart, TextPointer tpEnd) : base(tpStart, tpEnd) { }

    public void Save(System.IO.Stream st, string format, bool preserveTextElements, bool isConversion = false)
    {
        if (format != System.Windows.DataFormats.XamlPackage)
        {
            base.Save(st, format, preserveTextElements);
        }
        else
        {
            SaveInternal(st, preserveTextElements, isConversion);
        }
    }

    public new void Load(System.IO.Stream st, string format)
    {
        if (format != System.Windows.DataFormats.XamlPackage)
        {
            base.Load(st, format);
        }
        else
        {
            LoadInternal(st);
        }
    }
}
