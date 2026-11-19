#nullable disable

using System.ComponentModel;
using System.Globalization;

namespace Quire;

public partial class ProofingLanguageDataItem
{
    private CultureInfo _culture;

    private bool _isChecked;

    public ProofingLanguageDataItem(CultureInfo culture)
    {
        _culture = culture;
    }

    public ProofingLanguageDataItem()
    {
    }

    public override string ToString()
    {
        return (string)Header;
    }

    public CultureInfo Culture
    {
        get
        {
            return _culture;
        }
        set
        {
            _culture = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Header"));
        }
    }

    public object Value => Culture?.Name;

    public object Header => GetCultureName(Culture);

    public bool IsChecked
    {
        get
        {
            return _isChecked;
        }
        set
        {
            if (_isChecked != value)
            {
                _isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    internal static string GetCultureName(CultureInfo culture)
    {
        if (global::Quire.DebugTools.AssertNotNull(culture, null).Failed)
        {
            return null;
        }
        string text = global::Quire.Properties.Settings.Default.ShowNativeProofingLanguage ? culture.NativeName : culture.DisplayName;
        if (!text.IsNotEmpty())
        {
            return culture.EnglishName;
        }
        return text;
    }
}
