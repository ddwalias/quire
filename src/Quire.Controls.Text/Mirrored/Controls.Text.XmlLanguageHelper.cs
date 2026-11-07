#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows.Markup;

namespace Quire.Controls.Text;

public static partial class XmlLanguageHelper
{
    public static readonly XmlLanguage FallbackXmlLanguage = XmlLanguage.GetLanguage("en-US");

    private static readonly CultureInfo FallbackCultureInfo = FallbackXmlLanguage.GetSpecificCulture();

    public static CultureInfo ConvertXmlLanguageToCultureInfo(XmlLanguage lang)
    {
        if (lang == null)
        {
            return null;
        }
        CultureInfo culture;
        try
        {
            culture = lang.GetSpecificCulture();
        }
        catch (Exception ex)
        {
            culture = FallbackCultureInfo;
            DebugTools.FailAssertion(ex);
        }
        return culture;
    }

    public static XmlLanguage ConvertCultureInfoToXmlLanguage(CultureInfo culture)
    {
        XmlLanguage language;
        try
        {
            language = XmlLanguage.GetLanguage(culture.Name);
        }
        catch (Exception ex)
        {
            language = FallbackXmlLanguage;
            DebugTools.FailAssertion(ex);
        }
        return language;
    }
}
