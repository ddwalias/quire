#nullable disable
using System;
using System.Windows;
using System.Windows.Markup;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EngineErrorMessages : ResourceDictionary, IComponentConnector
{
    private static readonly EngineErrorMessages _instance = new EngineErrorMessages();

    public const string FreemiumErrorCode = "premium%_only";

    public const string IllegalRange = "illegal_range";

    private bool _contentLoaded;

    public EngineErrorMessages()
    {
        InitializeComponent();
    }

    public static string GetStringByKey(string key)
    {
        try
        {
            if (_instance.Contains(key))
            {
                return _instance[key].ToString();
            }
            return _instance["nonlisted_error"]?.ToString() + " " + EscapeForFormatString(key) + ".";
        }
        catch (Exception ex)
        {
            return string.Format("Exception retrieving message for \"{0}\", Exception \"{1}\" : \"{2}\"", key, ex.GetType(), ex.Message);
        }
    }

    public static string GetDisplayMessage(EngineError error)
    {
        if (error == null || string.IsNullOrEmpty(error.ErrorCode))
        {
            return "";
        }
        string stringByKey = GetStringByKey(error.ErrorCode);
        if (error.ErrorMessageArguments != null)
        {
            try
            {
                object[] array = error.ErrorMessageArguments.ToArray();
                return string.Format(stringByKey, array);
            }
            catch (FormatException ex)
            {
                DebugTools.FailAssertion("Exception \"{0}\" thrown when formatting error message: \"{1}\" with {2} arguments", ex.Message, stringByKey, error.ErrorMessageArguments.Count);
            }
        }
        return stringByKey;
    }

    [MirrorVerified("PtcCalculationCoreEngineErrorMessagesMechanicalDifferentialTests")]
    private static string EscapeForFormatString(string original) => original.Replace("{", "{{").Replace("}", "}}");

    public static void MergeDictionary(ResourceDictionary dictionary)
    {
        _instance.MergedDictionaries.Add(dictionary);
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri uri = new Uri("/Ptc.Calculation.Core;component/engineerrormessages.xaml", UriKind.Relative);
        Application.LoadComponent(this, uri);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => _contentLoaded = true;
}
