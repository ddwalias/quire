#nullable disable
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Core;
using Quire.IO;
using Quire.Mirror;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Help;

public partial class HelpManager
{
    private readonly string _rootHelpPath;
    private readonly Uri _defaultHelpPath;
    private readonly Uri _defaultWebServerHelpPath;
    private readonly IHelpSettings _helpSettings;
    private readonly ApplicationVariant.ApplicationType _currentApplicationType;

    public const string DefaultRelativeHelpPathDir = "./Help";
    public const string TutorialPath = "index.html#page/PTC_Mathcad_Help/Tutorials/gs_tutorial/about_the_getting_started_tutorial.html";
    private const string DefaultEngNbLocalHelpEnvVarName = "PTC_PMA_HC_URL_13";

    private string _engNbLocalHelpEnvVarName = "PTC_PMA_HC_URL_13";
    private string _helpServerPath;

    [MirrorVerified("PtcControlsHelpHelpManagerMechanicalDifferentialTests")]
    public string ActiveToolTipUid { get; set; }

    public string EngNbLocalHelpEnvironmentVariableName
    {
        get => _engNbLocalHelpEnvVarName;
        set
        {
            string engNbLocalHelpEnvVarName = (string.IsNullOrWhiteSpace(value) ? "PTC_PMA_HC_URL_13" : value);
            _engNbLocalHelpEnvVarName = engNbLocalHelpEnvVarName;
            HelpServerPath = GetServerPath();
        }
    }

    public string HelpServerPath
    {
        get => _helpServerPath;
        set
        {
            if (_helpServerPath == value)
            {
                return;
            }
            _helpServerPath = value;
            OnPropertyChanged("HelpServerPath");
        }
    }

    [MirrorVerified("PtcControlsHelpHelpManagerMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    public HelpManager(ApplicationVariant.ApplicationType applicationType, IHelpSettings helpSettings, string rootHelpPath)
    {
        _helpSettings = helpSettings;
        _currentApplicationType = applicationType;
        _helpSettings.PropertyChanged += OnHelpSettingsPropertyChanged;
        _rootHelpPath = rootHelpPath;
        string cultureLanguage = LanguageUtils.GetCultureLanguage(replaceHypenByUnderscore: true);
        _defaultWebServerHelpPath = GetWebPathPerVariant(_currentApplicationType, cultureLanguage);
        switch (applicationType)
        {
        case ApplicationVariant.ApplicationType.EngNBPlus:
            _defaultHelpPath = new Uri(string.Empty, UriKind.Relative);
            break;
        case ApplicationVariant.ApplicationType.EngNB:
            if (rootHelpPath.IsNullOrEmpty())
            {
                _defaultHelpPath = new Uri(string.Empty, UriKind.Relative);
            }
            else
            {
                _defaultHelpPath = new Uri(Path.Combine(Path.Combine(rootHelpPath, "./Help"), cultureLanguage));
            }
            break;
        case ApplicationVariant.ApplicationType.MathcadOnPrem:
        default:
            _defaultHelpPath = new Uri(Path.Combine(Path.Combine(rootHelpPath, "./Help"), cultureLanguage));
            break;
        }
        HelpServerPath = GetServerPath();
    }

    private static Uri GetWebPathPerVariant(ApplicationVariant.ApplicationType currentApplicationType, string cultureLanguage)
    {
        switch (currentApplicationType == ApplicationVariant.ApplicationType.EngNB || currentApplicationType == ApplicationVariant.ApplicationType.EngNBPlus)
        {
        case false:
            return new Uri(Urls.DefaultMathcadPrimeWebServerHelpPathDir + cultureLanguage + "/" + Urls.DefaultUrlParam);
        default:
            return new Uri(Urls.DefaultEngNbWebServerHelpPathDir + cultureLanguage + "/" + Urls.DefaultUrlParam);
        }
    }

    public string GetTutorialPath()
    {
        return HelpServerPath + TutorialPath;
    }

    public string ConstructTopicPath(string helpIdKey, IWhiteboardItem activeItem, UIElement sourceElement)
    {
        string helpUid = BuildHelpUidKey(helpIdKey, activeItem, sourceElement);
        string helpId = null;
        HelpIdDictionary instance = ResourceProvider<HelpIdDictionary>.Instance;
        if (!string.IsNullOrEmpty(helpUid) && instance.Contains(helpUid))
        {
            helpId = instance[helpUid].ToString();
        }
        if (helpUid != null && helpId.IsNullOrEmpty() && _helpSettings.DocOptionsEnabled)
        {
            string text = string.Format("There is no Entry for: {0}UID: {1}{0}HELPID: {2}", Environment.NewLine, helpUid, helpId);
            AppServiceRegistry.GetService<IMessageBoxManager>().ShowError(Application.Current.MainWindow, text, WpfUtils.ApplicationFullName);
            WpfClipboard.Default.SetText(text);
            ActiveToolTipUid = null;
        }
        if (string.IsNullOrEmpty(helpId))
        {
            helpId = instance["Ribbon.Tab.GettingStarted.Help.Help"].ToString();
        }
        return string.Format("{0}index.html#context/PTC_Mathcad_Help/{1}", HelpServerPath, helpId);
    }

    private string BuildHelpUidKey(string helpIdKey, IWhiteboardItem activeItem, UIElement sourceElement)
    {
        helpIdKey = helpIdKey ?? ActiveToolTipUid;
        if (helpIdKey != null)
        {
            return helpIdKey;
        }
        if (activeItem is UIElement uIElement && (sourceElement == null || sourceElement.IsAncestorOf(uIElement)))
        {
            if (IOTag.HasInputTag(uIElement))
            {
                helpIdKey = "Ribbon.Tab.Document.MCAD.ToggleInputRegion.Tooltip";
            }
            else if (IOTag.HasOutputTag(uIElement))
            {
                helpIdKey = "Ribbon.Tab.Document.MCAD.ToggleOutputRegion.Tooltip";
            }
            else
            {
                helpIdKey = activeItem.GetHelpContext();
            }
        }
        if (helpIdKey != null)
        {
            return helpIdKey;
        }
        helpIdKey = (sourceElement as IHelpContext)?.GetHelpContext() ?? sourceElement?.Uid;
        return helpIdKey;
    }

    private string GetServerPath()
    {
        Uri absoluteUri;
        switch (_currentApplicationType)
        {
        case ApplicationVariant.ApplicationType.EngNB:
        {
            string environmentVariable = Environment.GetEnvironmentVariable(_engNbLocalHelpEnvVarName);
            string cultureLanguage = LanguageUtils.GetCultureLanguage(replaceHypenByUnderscore: true);
            string text = (string.IsNullOrWhiteSpace(environmentVariable) ? string.Empty : (environmentVariable.TrimEnd('/') + "/EngNbkHelp"));
            if (!text.IsNullOrEmpty())
            {
                absoluteUri = new Uri(Path.Combine(Path.Combine(text, "./Help"), cultureLanguage));
            }
            else
            {
                if (string.IsNullOrEmpty(_defaultHelpPath?.OriginalString))
                {
                    absoluteUri = _defaultWebServerHelpPath;
                }
                else
                {
                    absoluteUri = (Directory.Exists(_defaultHelpPath?.LocalPath) ? _defaultHelpPath : _defaultWebServerHelpPath);
                }
            }
            break;
        }
        case ApplicationVariant.ApplicationType.EngNBPlus:
            absoluteUri = _defaultWebServerHelpPath;
            break;
        case ApplicationVariant.ApplicationType.MathcadOnPrem:
        default:
            if (_helpSettings.IsHelpDefault || !UrlUtils.TryCreateAbsoluteUri(_rootHelpPath, _helpSettings.HelpPath, out absoluteUri))
            {
                absoluteUri = (Directory.Exists(_defaultHelpPath?.LocalPath) ? _defaultHelpPath : _defaultWebServerHelpPath);
            }
            break;
        }
        string absoluteUrl = absoluteUri.AbsoluteUri;
        if (!absoluteUrl.EndsWith("/"))
        {
            absoluteUrl += "/";
        }
        return absoluteUrl;
    }

    private void OnHelpSettingsPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "IsHelpDefault" || args.PropertyName == "HelpPath")
        {
            HelpServerPath = GetServerPath();
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
