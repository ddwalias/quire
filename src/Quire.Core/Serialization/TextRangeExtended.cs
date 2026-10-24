#nullable disable
using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Wpf;
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

    public void LoadInternal(Stream st)
    {
        MemoryStream memoryStream = new MemoryStream();
        st.CopyTo(memoryStream);
        bool isSysDefFont = Fonts.SystemFontFamilies.Contains(new FontFamily("Mathcad UniMath Prime"));
        bool isSysConFont = Fonts.SystemFontFamilies.Contains(new FontFamily("Mathcad UniMath"));
        string[] styleNames;
        using (Package package = Package.Open(memoryStream, FileMode.Open, FileAccess.ReadWrite))
        {
            Uri partUri = new Uri("/Xaml/Document.xaml", UriKind.Relative);
            PackagePart part = package.GetPart(partUri);
            string input;
            using (StreamReader streamReader = new StreamReader(part.GetStream()))
            {
                input = streamReader.ReadToEnd();
            }
            MatchCollection matchCollection = Regex.Matches(input, "<(Run|Ptc.Controls.Text:(SubscriptRun|SuperscriptRun))[^>]*>");
            styleNames = new string[matchCollection.Count];
            for (int index = 0; index < matchCollection.Count; index++)
            {
                MatchCollection matchCollection2 = Regex.Matches(matchCollection[index].Value, " style=\"(?<style>[^.\"]+)\"");
                if (matchCollection2.Count > 0)
                {
                    styleNames[index] = matchCollection2[0].Groups["style"].Value;
                }
            }
            input = Regex.Replace(input, " style=\"(?<style>[^.\"]+)\"", "");
            using (Stream stream = part.GetStream(FileMode.Create, FileAccess.Write))
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(input);
            }
        }
        this.LoadWithCustomContext(memoryStream);
        if (styleNames.Length > 0)
        {
            int i = 0;
            this.EnumerateRunsInRange().Each((Run run) =>
            {
                if (i < styleNames.Length && styleNames[i].IsNotEmpty())
                {
                    string text = styleNames[i];
                    TextElementAttachedProperties.SetStyleKey(run, text);
                    if (run.FontFamily.Source.Contains("#"))
                    {
                        run.FontFamily = new FontFamily(new Uri("pack://application:,,,"), run.FontFamily.Source);
                    }
                    if (!isSysDefFont && run.FontFamily.Source.Equals("Mathcad UniMath Prime"))
                    {
                        run.FontFamily = new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath Prime"));
                    }
                    if (!isSysConFont && run.FontFamily.Source.Equals("Mathcad UniMath"))
                    {
                        run.FontFamily = new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath"));
                    }
                    run.Tag = text;
                }
                i++;
            });
        }
    }

    public void SaveInternal(Stream st, bool preserveTextElements, bool isConversion)
    {
        MemoryStream memoryStream = new MemoryStream();
        base.Save((Stream)memoryStream, System.Windows.DataFormats.XamlPackage, preserveTextElements);
        using (Package package = Package.Open(memoryStream, FileMode.Open, FileAccess.ReadWrite))
        {
            Uri partUri = new Uri("/Xaml/Document.xaml", UriKind.Relative);
            PackagePart part = package.GetPart(partUri);
            string xamlText;
            using (StreamReader streamReader = new StreamReader(part.GetStream()))
            {
                xamlText = streamReader.ReadToEnd();
            }
            global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder(xamlText);
            int j = 0;
            int jOffset = 0;
            this.EnumerateRunsInRange().Each((Run run) =>
            {
                string text = TextElementAttachedProperties.GetStyleKey(run) ?? (string)run.Tag;
                string text2 = (run is ICustomFontSizeOwner)
                    ? (run.GetType().Namespace + ":" + run.GetType().Name)
                    : run.GetType().Name;
                string text3 = "<" + text2;
                j = xamlText.IndexOf(text3, j + 1, StringComparison.Ordinal);
                while (xamlText.Substring(j, "<Run> </Run>".Length) == "<Run> </Run>" && run.Text != " ")
                {
                    j = xamlText.IndexOf(text3, j + 1, StringComparison.Ordinal);
                }
                if (!text.IsNullOrEmpty() && DebugTools.DevAssert(j > -1, null).Passed)
                {
                    string text4 = " style=\"" + text + "\"";
                    sb.Insert(j + jOffset + text3.Length, text4);
                    jOffset += text4.Length;
                }
                if (isConversion && DebugTools.DevAssert(j > -1, null).Passed)
                {
                    WriteExplicitFontAttributes(sb, xamlText, run, text3, j, ref jOffset);
                }
            });
            using (StreamWriter streamWriter = new StreamWriter(part.GetStream(FileMode.Create, FileAccess.Write)))
            {
                streamWriter.Write(sb.ToString());
                streamWriter.Flush();
            }
        }
        memoryStream.WriteTo(st);
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
