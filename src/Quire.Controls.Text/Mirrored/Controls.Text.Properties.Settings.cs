#nullable disable
#pragma warning disable

using System.Configuration;
using System.Diagnostics;

namespace Quire.Controls.Text.Properties;

internal sealed partial class Settings
{
    private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

    public static Settings Default => defaultInstance;

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue(@"%LOCALAPPDATA%\PTC\Mathcad Prime\SpellCheck\MathcadDictionary.lex")]
    public string DictionaryPath
    {
        get => (string)this["DictionaryPath"] ?? null;
        set => this["DictionaryPath"] = value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("True")]
    public bool UseWindowsDictionary
    {
        get => ((bool?)this["UseWindowsDictionary"]).GetValueOrDefault();
        set => this["UseWindowsDictionary"] = value;
    }

    public Settings()
    {
    }
}
