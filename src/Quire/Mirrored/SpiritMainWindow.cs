#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Freemium;
using Quire.FunctionalitiesLimitation;
using Quire.IO;
using Quire.Services;

namespace Quire;

public partial class SpiritMainWindow
{
    public const string FileTypeNotSupportedKey = "Application.FileTypeNotSupported";
    public const string FileNotCompatibleKey = "Application.FileNotCompatibleKey";
    public const string LogFileIsLocatedAtKey = "Application.LogFileIsLocatedAt";
    public const string MathcadPrimeWorksheetSaveFilterKey = "Application.QuireWorksheetSaveFilter";
    public const string MathcadPrimeTemplateSaveFilterKey = "Application.QuireTemplateSaveFilter";
    public const string XPSDocumentSaveFilterKey = "Application.XPSDocumentSaveFilter";
    public const string RtfDocumentSaveFilterKey = "Application.RtfDocumentSaveFilter";
    public const string PdfDocumentSaveFilterKey = "Application.PdfDocumentSaveFilter";
    public const string MathcadPrimeWorksheetsOpenFilterKey = "Application.QuireWorksheetsOpenFilter";
    public const string MathcadPrimeTemplatesOpenFilterKey = "Application.QuireTemplatesOpenFilter";
    public const string AllMathcadPrimeFilesOpenFilterKey = "Application.AllQuireFilesOpenFilter";
    public const string TabPrefixForTemplateFile = "Application.TabPrefixForTemplateFile";
    public const string FileOpenDialogForTemplatesTitleKey = "Application.FileOpenDialogForTemplatesTitle";
    public const string FileOpenDialogForOpenTemplatesTitleKey = "Application.FileOpenDialogForOpenTemplatesTitle";
    public const string UserTemplatesDefaultLocationKey = "Application.UserTemplatesDefaultLocation";
    public const string SharedTemplatesDefaultLocationKey = "Application.SharedTemplatesDefaultLocation";
    public const string BuiltInTemplatesDefaultLocationKey = "Application.BuiltInTemplatesDefaultLocation";
    public const string UserDefinedDefaultTemplateDoesNotExists = "Application.UserDefinedDefaultTemplateDoesNotExists";
    public const string DefaultTemplateFileOpenTitle = "Application.DefaultTemplateFileOpenTitle";
    public const string ApplicationSaveChangesRequestKey = "Application.DoYouWantToSaveTheChanges";
    public const string ApplicationSynchronizationEnabledKey = "Application.SynchronizationEnabled";
    public const string ApplicationCalculationProgressKey = "Application.CalculationProgress";
    public const string ApplicationWaitForCalculationFinishedKey = "Application.WaitForCalculationFinished";
    public const string ApplicationValuesCouldNotBeSyncKey = "Application.ValuesCouldNotBeSync";
    public const string ApplicationSyncingInputOutputKey = "Application.SyncingInputOutput";
    public const string ApplicationInputsDoNotMatchKey = "Application.InputsDoNotMatch";
    public const string ApplicationInputAliasDuplicatedKey = "Application.InputAliasDuplicated";
    public const string ApplicationOutputAliasDuplicatedKey = "Application.OutputAliasDuplicated";
    public const string ApplicationSaveAndSyncCompletedKey = "Application.SaveAndSyncCompleted";
    public const string ApplicationSaveAndSyncCompletedWithConflictsKey = "Application.SaveAndSyncCompletedWithConflicts";
    public const string ApplicationSavedWithoutSyncKey = "Application.SavedWithoutSync";
    public const string ApplicationSaveAndSyncAbortedKey = "Application.SaveAndSyncAborted";
    public const string ApplicationSaveFailedButSyncComplete = "Application.SaveFailedButSyncComplete";
    public const global::Quire.IO.NavigateUrlErrorWarningKeys ApplicationFailedToOpenUrl = (global::Quire.IO.NavigateUrlErrorWarningKeys)(0);
    public const global::Quire.IO.NavigateUrlErrorWarningKeys ApplicationDontOpenWhileSaveAs = (global::Quire.IO.NavigateUrlErrorWarningKeys)(6);
    public const string EngineeringNotebookOpenError = "Application.EngineeringNotebookOpenError";
    public const string TemplateFileVersionNotCompatible = "Application.TemplateFileVersionNotCompatible";
    private const string DefaultTemplateName = "Default.mctx";
    private const string DefaultCulture = "en-US";
    private const string DefaultCultureTwoLetters = "en";
    public static global::Quire.AddonsCollection Addons;
    public readonly global::Quire.QualityAgentManager.IQualityAgentManager QualityAgentManagerInstance;
    public readonly global::Quire.OptOutQualityAgentHandler OptOutQualityAgentHandler;
    private readonly global::System.Collections.Generic.Dictionary<global::System.Windows.Window, global::System.Windows.Visibility> _visibilityDictionary;
    private bool _handlingException;
    private readonly global::Quire.Wpf.ObservableRangeCollection<global::Quire.Controls.Core.IEngineeringDocument> _openDocumentsList;
    private global::System.Nullable<global::System.Windows.Thickness> _lastCustomMarginValueSetting;
    private readonly global::Quire.Controls.Help.HelpManager _helpManager;
    private bool _anyDocumentRequestToUpdateInputsEnabled;
    private string _defaultQuickAccessToolbarLayoutValue;
    public const string InUseByAnotherResourceKey = "Application.InUseByAnother";
    public const string FileIsReadonlyResourceKey = "Application.FileIsReadOnly";
    private global::Quire.FunctionBrowserWindow _functionBrowserWindow;
    private global::System.Diagnostics.Process _converterProcess;
    private int _nextUntitledIndex;
    private bool _useDocumentTabs;
    private bool _isShowWarningsEnabled;
    private global::Quire.Controls.Core.WarningsTypesToShow _displayWarningsFilter;
    private readonly global::Quire.FindUserProperties _findProperties;
    internal global::Quire.SpiritMainWindow _this;
    internal global::System.Windows.Shapes.Ellipse CalcIndicator;
    internal global::System.Windows.Controls.TextBlock AutoCalcIndicator;
    internal global::Quire.FindControl FindBar;
    internal global::Fluent.DropDownButton _pageModeCheckButton;
    internal global::Fluent.DropDownButton _draftModeCheckButton;
    private global::Quire.Zoom _zoomControl;
    internal global::System.Windows.Controls.DockPanel EngineNotificationBar;
    internal global::AvalonDock.DockingManager appDock;

    public string GetPathToDefaultTemplateForNewDocument()
    {
        return global::Quire.Properties.Settings.Default.PathToDefaultTemplateForNewDocument;
    }

    public void SetPathToDefaultTemplateForNewDocument(string filePath)
    {
        global::Quire.Properties.Settings.Default.PathToDefaultTemplateForNewDocument = filePath;
        global::Quire.Properties.Settings.Default.Save();
    }

    public string GetDefaultTemplatesLocation()
    {
        return System.IO.Path.GetFullPath(System.IO.Path.Combine(GetMathcadPrimeInstallDirectoryLocalPath(), (string)TryFindResource("Application.BuiltInTemplatesDefaultLocation"), GetCultureForDefaultTemplatesAndSplashScreen()));
    }

    public bool GetEnableCustomPathToDefaultTemplate()
    {
        return global::Quire.Properties.Settings.Default.EnableCustomPathToDefaultTemplate;
    }

    public string GetDefaultBuiltInTemplatesLocation()
    {
        return DefaultBuiltInTemplatesLocation;
    }

    public string GetMyTemplatesLocation()
    {
        return global::Quire.Properties.Settings.Default.MyTemplatesLocation;
    }

    public void SetMyTemplatesLocation(string folderPath)
    {
        UpdateTemplatesFolderLocationForKey("Application.UserTemplatesDefaultLocation", GetMyTemplatesLocation());
        if (string.Equals(GetMyTemplatesLocation(), folderPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        global::Quire.Properties.Settings.Default.MyTemplatesLocation = folderPath;
        global::Quire.Properties.Settings.Default.Save();
    }

    public string GetSharedTemplatesLocation()
    {
        return global::Quire.Properties.Settings.Default.SharedTemplatesLocation;
    }

    public void SetSharedTemplatesLocation(string folderPath)
    {
        UpdateTemplatesFolderLocationForKey("Application.SharedTemplatesDefaultLocation", GetSharedTemplatesLocation());
        if (string.Equals(GetSharedTemplatesLocation(), folderPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        global::Quire.Properties.Settings.Default.SharedTemplatesLocation = folderPath;
        global::Quire.Properties.Settings.Default.Save();
    }

    private string GetFullPathForTemplatesLocationByKey(string templateKey)
    {
        string pathToTemplatesFolderByKey = GetPathToTemplatesFolderByKey(templateKey);
        string path = TryFindResource(templateKey) as string;
        return System.IO.Path.GetFullPath(System.IO.Path.Combine(pathToTemplatesFolderByKey, path));
    }

    private string GetPathToTemplatesFolderByKey(string templateKey)
    {
        if (!IsGivenKeyMyTemplatesKey(templateKey))
        {
            return FileUtils.GetCommonConfigFolderPath();
        }

        return FileUtils.GetLocalApplicationDataPath();
    }

    private void UpdateTemplatesFolderLocationForKey(string userTemplatesDefaultLocationKey, string templatesLocation)
    {
        if ((string)TryFindResource(userTemplatesDefaultLocationKey) != null)
        {
            TemplatesFolderLocations[(string)TryFindResource(userTemplatesDefaultLocationKey)] = templatesLocation;
        }
    }

    public bool IsGivenKeyMyTemplatesKey(string templateKey)
    {
        return string.Equals("Application.UserTemplatesDefaultLocation", templateKey);
    }

    public void SetPythonDirectoryPath(string pythonDirectoryPath)
    {
        global::Quire.Properties.Settings.Default.PythonDirectoryPath = pythonDirectoryPath;
        global::Quire.Properties.Settings.Default.Save();
    }

    public static string ReviseFileExtension(string fileName, string extension)
    {
        string currentExtension = System.IO.Path.GetExtension(fileName);
        if (!string.IsNullOrEmpty(currentExtension) && currentExtension.ToLower() == extension)
        {
            return fileName;
        }

        return System.IO.Path.ChangeExtension(fileName, currentExtension + extension);
    }

    private static void OnZoomChanged(double zoom)
    {
        global::Quire.Properties.Settings.Default.Zoom = zoom;
        global::Quire.Properties.Settings.Default.Save();
    }

    private void OnDocumentModified(IEngineeringDocument engineeringDocument)
    {
        if (Addons != null && engineeringDocument.IsDirty && !string.IsNullOrEmpty(engineeringDocument.FileName))
        {
            Addons.CallAddonsOnDocumentModified(engineeringDocument.FilePath);
        }

        DocumentModified(engineeringDocument);
    }

    private void SetAndDisplayWarning(string warningMessage, string problematicEntity)
    {
        string displayedMessage = string.Format("{0}{1}{2}", warningMessage, Environment.NewLine, problematicEntity);
        DisplayWarning(displayedMessage);
    }

    internal void ResetUntitledIndex()
    {
        _nextUntitledIndex = 0;
    }

    private void UpdateCurrentShowWarningsFilter()
    {
        CurrentShowWarningsFilter = IsShowWarningsEnabled ? DisplayWarningsFilter : WarningsTypesToShow.None;
    }

    private void ExitApplication(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }

    internal void UpdateTemplateLocationsIfNeeded()
    {
        SetMyTemplatesLocation(global::Quire.Properties.Settings.Default.MyTemplatesLocation);
        SetSharedTemplatesLocation(global::Quire.Properties.Settings.Default.SharedTemplatesLocation);
    }

    private void OpenUrl(string url, bool useBrowser)
    {
        if (!UrlUtils.TryOpenUrl(url, useBrowser))
        {
            ShowFailedToOpenUrlMessage(url);
        }
    }

    private void FirePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string GetEngineVersion()
    {
        if (ActiveDocument != null)
        {
            return ActiveDocument.GetEngineVersion();
        }

        return string.Empty;
    }

    private static string GetMathcadPrimeInstallDirectoryLocalPath()
    {
        return FileUtils.GetAssemblyFolder(Assembly.GetExecutingAssembly());
    }

    internal static string GetCultureForDefaultTemplatesAndSplashScreen()
    {
        string cultureLanguage = LanguageUtils.GetCultureLanguage(replaceHypenByUnderscore: false);
        if (cultureLanguage.Equals("en"))
        {
            return "en-US";
        }

        return cultureLanguage;
    }

    private void SetupWindowDimensions()
    {
        SetupMainWindowDimensions(this);
    }

    private void DisplayFreemiumAboutBox(object sender, ExecutedRoutedEventArgs e)
    {
        FreemiumAboutBox.Instance.Show(e.Parameter, this);
    }

    private bool FindIsNotAllowedInFreemiumMode()
    {
        if (FindProperties.FindTextSubscriptPositions.IsNotEmpty())
        {
            return !FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.ToggleSubscript);
        }

        return false;
    }

    private bool ReplaceIsNotAllowedInFreemiumMode()
    {
        if (FindProperties.FindTextSubscriptPositions.IsNotEmpty() || FindProperties.ReplaceTextSubscriptPositions.IsNotEmpty())
        {
            return !FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.ToggleSubscript);
        }

        return false;
    }

    public event global::System.Action<string, string, string, string> DocumentRenamed;
    public event global::System.Action<string, string> DocumentClosed;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument> DocumentSaved;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument> DocumentModified;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::System.Collections.Generic.List<string>, global::System.Collections.Generic.List<string>> DocumentInputsOutputsSelected;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts> DocumentInputsOutputsStatesGeneratedWhenSave;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts> DocumentInputsOutputsStatesGeneratingBeforeSave;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::Quire.Controls.Core.IORegions.IValuesSetter> RequestToUpdateInputs;
    public event global::System.Action OptionsAboutDialogOpening;
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    public global::Quire.IntegrationCommands IntegrationCommands { get; set; }
    public bool IsComInstance { get; set; }
    public global::System.Collections.Generic.Dictionary<string, string> TemplatesFolderLocations { get; private set; }
    public global::System.Windows.Input.CommandBinding HelpCommandBinding { get; private set; }
    public string DefaultPathToDefaultTemplateForNewDocument { get; private set; }
    public string DefaultMyTemplatesLocation { get; private set; }
    public string DefaultSharedTemplatesLocation { get; private set; }
    public string DefaultBuiltInTemplatesLocation { get; private set; }

    public global::Quire.PageMarginSelectorType PageMarginSelectorType
    {
        get => (global::Quire.PageMarginSelectorType)GetValue(PageMarginSelectorTypeProperty);
        set => SetValue(PageMarginSelectorTypeProperty, value);
    }

    public bool ShowLicenseError
    {
        get => (bool)GetValue(ShowLicenseErrorProperty);
        set => SetValue(ShowLicenseErrorProperty, value);
    }

    public global::Quire.Controls.Core.IEngineeringDocument ActiveDocument
    {
        get => (global::Quire.Controls.Core.IEngineeringDocument)GetValue(ActiveDocumentProperty);
        set => SetValue(ActiveDocumentProperty, value);
    }

    private bool IsDefaultTemplateNewDocExists =>
        global::System.IO.File.Exists(GetPathToDefaultTemplateForNewDocument());

    public global::Quire.MruFiles.MruFileManager MruFileManager { get; } = new global::Quire.MruFiles.MruFileManager();
    internal global::Quire.Zoom ZoomControl => _zoomControl;
    public global::Quire.InputOutputDesignationWindow WorksheetInputOutputWindow { get; set; }

    internal bool UseDocumentTabs
    {
        get => _useDocumentTabs;
        set => _useDocumentTabs = value;
    }

    internal global::Quire.Controls.Core.WarningsTypesToShow CurrentShowWarningsFilter
    {
        get => global::Quire.Controls.Core.AttachedProperties.GetWarningDisplayFilter((global::System.Windows.DependencyObject)this);
        set
        {
            if (CurrentShowWarningsFilter == value)
            {
                return;
            }
            global::Quire.Controls.Core.AttachedProperties.SetWarningDisplayFilter((global::System.Windows.DependencyObject)this, value);
        }
    }

    public global::Quire.FindUserProperties FindProperties => _findProperties;

    public global::System.Nullable<global::System.Windows.Thickness> LastCustomMarginValueSetting
    {
        get => _lastCustomMarginValueSetting;
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool AnyDocumentRequestToUpdateInputsEnabled
    {
        get => _anyDocumentRequestToUpdateInputsEnabled;
        private set
        {
            if (_anyDocumentRequestToUpdateInputsEnabled == value)
            {
                return;
            }
            _anyDocumentRequestToUpdateInputsEnabled = value;
            FirePropertyChanged("AnyDocumentRequestToUpdateInputsEnabled");
        }
    }

    public bool IsShowWarningsEnabled
    {
        get => _isShowWarningsEnabled;
        private set
        {
            if (IsShowWarningsEnabled == value)
            {
                return;
            }
            _isShowWarningsEnabled = value;
            global::Quire.Properties.Settings.Default.IsShowWarningsEnabled = value;
            global::Quire.Properties.Settings.Default.Save();
            FirePropertyChanged("IsShowWarningsEnabled");
            UpdateCurrentShowWarningsFilter();
        }
    }

    public global::Quire.Controls.Core.WarningsTypesToShow DisplayWarningsFilter
    {
        get => _displayWarningsFilter;
        private set
        {
            if (DisplayWarningsFilter == value)
            {
                return;
            }
            _displayWarningsFilter = value;
            global::Quire.Properties.Settings.Default.DisplayWarningsFilter = value.ToString();
            global::Quire.Properties.Settings.Default.Save();
            FirePropertyChanged("DisplayWarningsFilter");
            UpdateCurrentShowWarningsFilter();
        }
    }

    public global::Quire.Wpf.IMessageBoxManager MessageBoxManager =>
        global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Wpf.IMessageBoxManager>();

    protected global::Quire.Wpf.IDialogManager DialogManager =>
        global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Wpf.IDialogManager>();

    private static global::Quire.Services.IExceptionLogger ExLogger =>
        global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Controls.Core.IPrimeExceptionLogger>();

    public global::System.Collections.Generic.IList<global::Quire.Controls.Core.IEngineeringDocument> OpenDocumentsList =>
        _openDocumentsList;

    private global::Quire.Controls.EngineeringDocument ActiveEngineeringDoc =>
        ActiveDocument as global::Quire.Controls.EngineeringDocument;

    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        // Release IL uses the self-assembly pack URI spelling for this XAML resource locator.
        global::System.Uri resourceLocator = new global::System.Uri("/<self>;component/spiritmainwindow.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal global::System.Delegate _CreateDelegate(global::System.Type delegateType, string handler)
    {
        return global::System.Delegate.CreateDelegate(delegateType, this, handler);
    }
}
