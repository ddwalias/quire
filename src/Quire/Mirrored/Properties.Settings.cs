#nullable disable

using Quire.Controls;

namespace Quire.Properties;

public sealed partial class Settings
{
    public static Settings Default => defaultInstance;

    public bool UpgradeSettings
    {
        get
        {
            return (bool?)this["UpgradeSettings"] == true;
        }

        set
        {
            this["UpgradeSettings"] = value;
        }
    }

    public string DefaultWindowsLayout
    {
        get
        {
            return ((string)this["DefaultWindowsLayout"]) ?? null;
        }
    }

    public bool IsHelpDefault
    {
        get
        {
            return (bool?)this["IsHelpDefault"] == true;
        }

        set
        {
            this["IsHelpDefault"] = value;
        }
    }

    public string HelpPath
    {
        get
        {
            return ((string)this["HelpPath"]) ?? null;
        }

        set
        {
            this["HelpPath"] = value;
        }
    }

    public string CWD
    {
        get
        {
            return ((string)this["CWD"]) ?? null;
        }

        set
        {
            this["CWD"] = value;
        }
    }

    public double Zoom
    {
        get
        {
            return ((double?)this["Zoom"]).GetValueOrDefault();
        }

        set
        {
            this["Zoom"] = value;
        }
    }

    public double WinHeight
    {
        get
        {
            return ((double?)this["WinHeight"]).GetValueOrDefault();
        }

        set
        {
            this["WinHeight"] = value;
        }
    }

    public double WinWidth
    {
        get
        {
            return ((double?)this["WinWidth"]).GetValueOrDefault();
        }

        set
        {
            this["WinWidth"] = value;
        }
    }

    public double WinLeft
    {
        get
        {
            return ((double?)this["WinLeft"]).GetValueOrDefault();
        }

        set
        {
            this["WinLeft"] = value;
        }
    }

    public double WinTop
    {
        get
        {
            return ((double?)this["WinTop"]).GetValueOrDefault();
        }

        set
        {
            this["WinTop"] = value;
        }
    }

    public bool DocOptionsEnabled
    {
        get
        {
            return (bool?)this["DocOptionsEnabled"] == true;
        }

        set
        {
            this["DocOptionsEnabled"] = value;
        }
    }

    public string QuickAccessToolbarLayout
    {
        get
        {
            return ((string)this["QuickAccessToolbarLayout"]) ?? null;
        }

        set
        {
            this["QuickAccessToolbarLayout"] = value;
        }
    }

    public bool EnableGettingstartedTab
    {
        get
        {
            return (bool?)this["EnableGettingstartedTab"] == true;
        }

        set
        {
            this["EnableGettingstartedTab"] = value;
        }
    }

    public int MaxNumberOfThreads
    {
        get
        {
            return ((int?)this["MaxNumberOfThreads"]).GetValueOrDefault();
        }

        set
        {
            this["MaxNumberOfThreads"] = value;
        }
    }

    public bool WinMaximized
    {
        get
        {
            return (bool?)this["WinMaximized"] == true;
        }

        set
        {
            this["WinMaximized"] = value;
        }
    }

    public bool OptOutQualityAgent
    {
        get
        {
            return (bool?)this["OptOutQualityAgent"] == true;
        }

        set
        {
            this["OptOutQualityAgent"] = value;
        }
    }

    public bool EnableRssFeed
    {
        get
        {
            return (bool?)this["EnableRssFeed"] == true;
        }

        set
        {
            this["EnableRssFeed"] = value;
        }
    }

    public int NumberOfThreads
    {
        get
        {
            return ((int?)this["NumberOfThreads"]).GetValueOrDefault();
        }

        set
        {
            this["NumberOfThreads"] = value;
        }
    }

    public string MyTemplatesLocation
    {
        get
        {
            return ((string)this["MyTemplatesLocation"]) ?? null;
        }

        set
        {
            this["MyTemplatesLocation"] = value;
        }
    }

    public string SharedTemplatesLocation
    {
        get
        {
            return ((string)this["SharedTemplatesLocation"]) ?? null;
        }

        set
        {
            this["SharedTemplatesLocation"] = value;
        }
    }

    public bool EnableCustomMyTemplatesLocation
    {
        get
        {
            return (bool?)this["EnableCustomMyTemplatesLocation"] == true;
        }

        set
        {
            this["EnableCustomMyTemplatesLocation"] = value;
        }
    }

    public bool EnableCustomSharedTemplatesLocation
    {
        get
        {
            return (bool?)this["EnableCustomSharedTemplatesLocation"] == true;
        }

        set
        {
            this["EnableCustomSharedTemplatesLocation"] = value;
        }
    }

    public string PathToDefaultTemplateForNewDocument
    {
        get
        {
            return ((string)this["PathToDefaultTemplateForNewDocument"]) ?? null;
        }

        set
        {
            this["PathToDefaultTemplateForNewDocument"] = value;
        }
    }

    public bool EnableCustomPathToDefaultTemplate
    {
        get
        {
            return (bool?)this["EnableCustomPathToDefaultTemplate"] == true;
        }

        set
        {
            this["EnableCustomPathToDefaultTemplate"] = value;
        }
    }

    public Controls.MruObservableCollection MyMRUItems
    {
        get
        {
            return ((MruObservableCollection)this["MyMRUItems"]) ?? null;
        }

        set
        {
            this["MyMRUItems"] = value;
        }
    }

    public bool ShowNativeProofingLanguage
    {
        get
        {
            return (bool?)this["ShowNativeProofingLanguage"] == true;
        }

        set
        {
            this["ShowNativeProofingLanguage"] = value;
        }
    }

    public int SymbolicResultMaximumSize
    {
        get
        {
            return ((int?)this["SymbolicResultMaximumSize"]).GetValueOrDefault();
        }

        set
        {
            this["SymbolicResultMaximumSize"] = value;
        }
    }

    public int DelayRecalculationInMsecAfterCloseExcel
    {
        get
        {
            return ((int?)this["DelayRecalculationInMsecAfterCloseExcel"]).GetValueOrDefault();
        }

        set
        {
            this["DelayRecalculationInMsecAfterCloseExcel"] = value;
        }
    }

    public string DisplayWarningsFilter
    {
        get
        {
            return ((string)this["DisplayWarningsFilter"]) ?? null;
        }

        set
        {
            this["DisplayWarningsFilter"] = value;
        }
    }

    public bool IsShowWarningsEnabled
    {
        get
        {
            return (bool?)this["IsShowWarningsEnabled"] == true;
        }

        set
        {
            this["IsShowWarningsEnabled"] = value;
        }
    }

    public bool ShowScriptableControlsWarningDialog
    {
        get
        {
            return (bool?)this["ShowScriptableControlsWarningDialog"] == true;
        }

        set
        {
            this["ShowScriptableControlsWarningDialog"] = value;
        }
    }

    public bool DisableScriptableControls
    {
        get
        {
            return (bool?)this["DisableScriptableControls"] == true;
        }

        set
        {
            this["DisableScriptableControls"] = value;
        }
    }

    public bool IsScriptingRunWorksheetRecalculate
    {
        get
        {
            return (bool?)this["IsScriptingRunWorksheetRecalculate"] == true;
        }

        set
        {
            this["IsScriptingRunWorksheetRecalculate"] = value;
        }
    }

    public bool IsScriptingRunWorksheetOpen
    {
        get
        {
            return (bool?)this["IsScriptingRunWorksheetOpen"] == true;
        }

        set
        {
            this["IsScriptingRunWorksheetOpen"] = value;
        }
    }

    public bool EnableCustomPythonDirectoryPath
    {
        get
        {
            return (bool?)this["EnableCustomPythonDirectoryPath"] == true;
        }

        set
        {
            this["EnableCustomPythonDirectoryPath"] = value;
        }
    }

    public string PythonDirectoryPath
    {
        get
        {
            return ((string)this["PythonDirectoryPath"]) ?? null;
        }

        set
        {
            this["PythonDirectoryPath"] = value;
        }
    }

    public Settings() { }
}
