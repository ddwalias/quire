#nullable disable
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Win32;
using Quire;
using Quire.Mirror;

namespace Quire.Controls;

public static partial class LanguageUtils
{
    public const string FallbackLanguage = "en";

    public const string ChinesePrefix = "zh-";

    private const string ChartAppLangRegKeyName = "LANG";

    private static readonly string[] _supportedLanguages = new string[10] { "en", "de", "es", "it", "fr", "ru", "ko", "ja", "zh-CN", "zh-TW" };

    private static readonly Dictionary<string, string> _mathcadToChartAppL10N = new Dictionary<string, string>
    {
        { "en", "usascii" },
        { "de", "german" },
        { "es", "spanish" },
        { "it", "italian" },
        { "fr", "french" },
        { "ru", "russian" },
        { "ko", "korean" },
        { "ja", "japanese" },
        { "zh-CN", "chinese_cn" },
        { "zh-TW", "chinese_tw" },
    };

    public static string GetCultureLanguage(bool replaceHypenByUnderscore)
    {
        string language;
        if (_supportedLanguages.Contains(CultureInfo.CurrentUICulture.Name))
        {
            language = CultureInfo.CurrentUICulture.Name;
        }
        else if (_supportedLanguages.Contains(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName))
        {
            language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        }
        else
        {
            language = FallbackLanguage;
        }
        if (!replaceHypenByUnderscore)
        {
            return language;
        }
        return language.Replace('-', '_');
    }

    public static void SetChartAppL10N()
    {
        if (DebugTools.DevAssert(_mathcadToChartAppL10N.TryGetValue(GetCultureLanguage(false), out string language), null, Array.Empty<object>()).Failed)
        {
            language = _mathcadToChartAppL10N[FallbackLanguage];
        }

        RegistryKey registryKey = null;
        try
        {
            try
            {
                registryKey = Registry.CurrentUser.CreateSubKey(ChartAppRegKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (registryKey != null)
                {
                    registryKey.SetValue(ChartAppLangRegKeyName, language);
                }
            }
            catch (System.Exception exception)
            {
                DebugTools.FailAssertionDoNotThrow(exception);
            }
        }
        finally
        {
            if (registryKey != null)
            {
                registryKey.Close();
            }
        }
    }
}
