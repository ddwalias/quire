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

    private void WriteExplicitFontAttributes(global::System.Text.StringBuilder sb, string xamlText, Run run, string runString, int j, ref int jOffset)
    {
        int num = xamlText.IndexOf('>', j);
        string text = xamlText.Substring(j, num - j + 1);
        if (!text.Contains("FontFamily="))
        {
            string text2 = " FontFamily=\"" + run.FontFamily.Source + "\"";
            sb.Insert(j + jOffset + runString.Length, text2);
            jOffset += text2.Length;
        }
        if (!text.Contains("FontSize="))
        {
            string text3 = $" FontSize=\"{run.FontSize}\"";
            sb.Insert(j + jOffset + runString.Length, text3);
            jOffset += text3.Length;
        }
    }
}
