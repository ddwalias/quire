#nullable disable

using Quire.MessageBoxes;
using Quire.Controls.Core;
using Quire.Wpf;
using System;
using System.Reflection;
using Microsoft.Win32;
using Quire.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Controls;
using Quire.Freemium;
using Quire.FunctionalitiesLimitation;
using Quire.Services;

namespace Quire;

public partial class SpiritMainWindow
{
    // Mathcad nested types precede the outer method stream.
    [global::Quire.Mirror.MirrorOf("Spirit.SpiritMainWindow+PropertyNames, MathcadPrime")]

    public static partial class PropertyNames

    {

        public const string AnyDocumentRequestToUpdateInputsEnabled = "AnyDocumentRequestToUpdateInputsEnabled";

    }
    [global::Quire.Mirror.MirrorOf("Spirit.SpiritMainWindow+SaveAndSyncFinalMessage, MathcadPrime")]

    private enum SaveAndSyncFinalMessage

    {

        NoMessage = 0,

        InfoMessage = 1,

        WarningMessage = 2,

    }
    [global::Quire.Mirror.MirrorOf("Spirit.SpiritMainWindow+SaveStatus, MathcadPrime")]

    private partial struct SaveStatus

    {

        public global::System.Exception Exception;

        public bool CancelSave;

        public bool OK { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    }
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
    public const string InUseByAnotherResourceKey = "Application.InUseByAnother";
    public const string FileIsReadonlyResourceKey = "Application.FileIsReadOnly";
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
    [global::Quire.Mirror.MirrorStub]

    private static readonly global::System.Type TypeOfThis;
    [global::Quire.Mirror.MirrorStub]

    public static global::System.Windows.DependencyProperty ActiveDocumentProperty;
    [global::Quire.Mirror.MirrorStub]

    public static readonly global::System.Windows.DependencyProperty PageMarginSelectorTypeProperty;
    [global::Quire.Mirror.MirrorStub]

    public static readonly global::System.Windows.DependencyProperty ShowLicenseErrorProperty;
    [global::Quire.Mirror.MirrorStub]

    private bool _contentLoaded;
    // The first four Mathcad events precede the closure-bearing methods.
    public event global::System.Action<string, string, string, string> DocumentRenamed;
    public event global::System.Action<string, string> DocumentClosed;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument> DocumentSaved;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument> DocumentModified;
    public global::Quire.IntegrationCommands IntegrationCommands { get; set; }
    public bool IsComInstance { get; set; }
    public static bool IsFeLinesInstance { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Collections.Generic.Dictionary<string, string> TemplatesFolderLocations { get; private set; }
    public global::Quire.PageMarginSelectorType PageMarginSelectorType

    {

        get => (global::Quire.PageMarginSelectorType)GetValue(PageMarginSelectorTypeProperty);

        set => SetValue(PageMarginSelectorTypeProperty, value);

    }
    public global::System.Action<string, global::System.Exception> OpenFileExceptionHandler { private get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, string, global::System.Exception, global::Quire.Controls.Core.IDocumentSaveOptions> SaveFileExceptionHandler { private get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::Quire.Wpf.IMessageBoxManager MessageBoxManager =>

        global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Wpf.IMessageBoxManager>();
    protected global::Quire.Wpf.IDialogManager DialogManager =>

        global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Wpf.IDialogManager>();
    public global::System.Windows.Input.CommandBinding HelpCommandBinding { get; private set; }
    private static global::Quire.Services.IExceptionLogger ExLogger =>

        global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Controls.Core.IPrimeExceptionLogger>();
    private void HandlePrepareSnapshotStart(global::Quire.Controls.EngineeringDocument _)

    {

        global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;

    }
    private void HandlePrepareSnapshotEnd(global::Quire.Controls.EngineeringDocument _)

    {

        global::System.Windows.Input.Mouse.OverrideCursor = null;

    }
    private T ShowMessage<T>(global::System.Func<global::System.Windows.Window, string, T> messageDisplayDelegate)

    {

        if (!MessageBoxManager.IsDisabled)

        {

            Activate();

        }

        return messageDisplayDelegate(this, global::Quire.Wpf.WpfUtils.ApplicationFullName);

    }
    public global::System.Collections.Generic.IList<global::Quire.Controls.Core.IEngineeringDocument> OpenDocumentsList =>

        _openDocumentsList;
    public global::System.Nullable<global::System.Windows.Thickness> LastCustomMarginValueSetting

    {

        get => _lastCustomMarginValueSetting;

        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    }
    public static bool IsWorksheetConverterInstalled { get; }
    // Compile-order member; a real initializer is required for type loading.
    static SpiritMainWindow()
    {
        TypeOfThis = typeof(SpiritMainWindow);
        ActiveDocumentProperty = global::System.Windows.DependencyProperty.Register(
            "ActiveDocument",
            typeof(global::Quire.Controls.Core.IEngineeringDocument),
            TypeOfThis,
            new global::System.Windows.FrameworkPropertyMetadata((global::System.Windows.PropertyChangedCallback)OnActiveDocumentChanged));
        PageMarginSelectorTypeProperty = global::System.Windows.DependencyProperty.Register(
            "PageMarginSelectorType",
            typeof(global::Quire.PageMarginSelectorType),
            TypeOfThis);
        ShowLicenseErrorProperty = global::System.Windows.DependencyProperty.Register(
            "ShowLicenseError",
            typeof(bool),
            TypeOfThis);
        if (global::Quire.DebugTools.IsDebug)
        {
            string assemblyName = global::System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            global::Quire.DebugTools.DevAssert(assemblyName == "MathcadPrime", null);
        }
        try
        {
            string installDirectory = GetMathcadPrimeInstallDirectoryLocalPath();
            IsWorksheetConverterInstalled = global::System.IO.File.Exists(
                global::System.IO.Path.Combine(installDirectory ?? string.Empty, "WorksheetConverter.exe"));
        }
        catch (global::System.Exception exception)
        {
            global::Quire.DebugTools.FailAssertion(exception.Message);
        }
    }
    private void OnEngNotebookConnectionStatusChanged()

    {

        global::Quire.AppCommands.Exit.Execute(null, this);

    }
    internal static void EnsureLicenseManagerInstance() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
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
    public void OpenStartPage()

    {

        if (global::Quire.License.LicenseManager.Instance.LicenseType == global::Quire.License.LicenseType.NoLicense)

        {

            string cultureLanguage = global::Quire.Controls.LanguageUtils.GetCultureLanguage(replaceHypenByUnderscore: false);

            // Mathcad's Release IL uses String.Format's composite-format overload for this URL.

            string url = string.Format("www.ptc.com/{0}/mathcad/start-page", cultureLanguage);

            OpenUrl(url, useBrowser: true);

        }

    }
    private void AnyDocumentRequestToUpdateInputsEnabledCheck()
    {
        AnyDocumentRequestToUpdateInputsEnabled = OpenDocumentsList.Any(engineeringDocument => engineeringDocument.ReadonlyOptions.RequestToUpdateInputsEnabled);
    }
    private void OpenDocumentsListChanged(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs eventArgs)

    {

        if (eventArgs.Action == global::System.Collections.Specialized.NotifyCollectionChangedAction.Move)

        {

            return;

        }

        if (eventArgs.OldItems != null)

        {

            foreach (global::Quire.Controls.Core.IEngineeringDocument oldItem in eventArgs.OldItems)

            {

                oldItem.PropertyChanged -= DocumentPropertyChanged;

            }

        }

        if (eventArgs.NewItems != null)

        {

            foreach (global::Quire.Controls.Core.IEngineeringDocument newItem in eventArgs.NewItems)

            {

                newItem.PropertyChanged += DocumentPropertyChanged;

            }

        }

        AnyDocumentRequestToUpdateInputsEnabledCheck();

        global::Quire.DebugTools.DevAssert(eventArgs.OldItems != null || eventArgs.NewItems != null, null);

    }
    private void DocumentPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs eventArgs)

    {

        if (eventArgs.PropertyName == "ReadonlyOptions")

        {

            AnyDocumentRequestToUpdateInputsEnabledCheck();

        }

    }
    public string DefaultPathToDefaultTemplateForNewDocument { get; private set; }
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
    public string DefaultMyTemplatesLocation { get; private set; }
    public string DefaultSharedTemplatesLocation { get; private set; }
    public string DefaultBuiltInTemplatesLocation { get; private set; }
    private bool IsDefaultTemplateNewDocExists =>

        global::System.IO.File.Exists(GetPathToDefaultTemplateForNewDocument());
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
    private void InitializeTemplatesLocations()

    {

        TemplatesFolderLocations = new global::System.Collections.Generic.Dictionary<string, string>();

        if (string.Equals(GetMyTemplatesLocation(), "DEFAULT"))

        {

            SetMyTemplatesLocation(DefaultMyTemplatesLocation);

        }

        if (string.Equals(GetSharedTemplatesLocation(), "DEFAULT"))

        {

            SetSharedTemplatesLocation(DefaultSharedTemplatesLocation);

        }

        if (global::Quire.UserSettingsProvider.IsConfigCopiedForMOR && !IsDefaultTemplateNewDocExists)

        {

            global::Quire.Properties.Settings.Default.EnableCustomPathToDefaultTemplate = false;

        }

        if (!GetEnableCustomPathToDefaultTemplate() || string.Equals(GetPathToDefaultTemplateForNewDocument(), "DEFAULT"))

        {

            SetPathToDefaultTemplateForNewDocument(DefaultPathToDefaultTemplateForNewDocument);

        }

        UpdateTemplatesFolderLocationForKey("Application.UserTemplatesDefaultLocation", GetMyTemplatesLocation());

        UpdateTemplatesFolderLocationForKey("Application.SharedTemplatesDefaultLocation", GetSharedTemplatesLocation());

        UpdateTemplatesFolderLocationForKey("Application.BuiltInTemplatesDefaultLocation", GetDefaultBuiltInTemplatesLocation());

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
    internal static void SetupMainWindowDimensions(global::System.Windows.Window window)

    {

        global::Quire.Properties.Settings settings = global::Quire.Properties.Settings.Default;

        if (settings.WinHeight.IsValid())

        {

            window.Height = settings.WinHeight;

        }



        if (settings.WinWidth.IsValid())

        {

            window.Width = settings.WinWidth;

        }



        if (settings.WinLeft.IsValid() && settings.WinLeft + window.Width > 10.0)

        {

            window.Left = settings.WinLeft;

            if (window.Left > global::System.Windows.SystemParameters.VirtualScreenWidth)

            {

                window.WindowStartupLocation = global::System.Windows.WindowStartupLocation.CenterScreen;

            }

        }



        if (settings.WinTop.IsValid() && settings.WinTop + window.Height > 10.0)

        {

            window.Top = settings.WinTop;

        }



        if (settings.WinMaximized)

        {

            window.WindowState = global::System.Windows.WindowState.Maximized;

        }

    }
    private global::System.Collections.Generic.IEnumerable<string> GetOpenFilesList()
    {
        return OpenDocumentsList.Select(item => item.FilePath);
    }
    // These five fields follow GetOpenFilesList to preserve the Mathcad lambda ordinal.
    private string _defaultQuickAccessToolbarLayoutValue;
    // These five fields follow GetOpenFilesList to preserve the Mathcad lambda ordinal.
    private global::Quire.FunctionBrowserWindow _functionBrowserWindow;
    // These five fields follow GetOpenFilesList to preserve the Mathcad lambda ordinal.
    private global::System.Diagnostics.Process _converterProcess;
    // These five fields follow GetOpenFilesList to preserve the Mathcad lambda ordinal.
    private int _nextUntitledIndex;
    private void UnhandledDispatcherExceptionHandler(

        object sender,

        global::System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)

    {

        if (_handlingException)

        {

            e.Handled = true;

            return;

        }



        _handlingException = true;

        string[] message =

        [

            "An Unhandled exception occurred on the dispatcher thread. See",

            "{0}",

            "for more information. Prime will attempt to save any modified files and close.",

            "Please restart the application."

        ];

        if (!e.Handled)

        {

            try

            {

                HandleException(e.Exception, string.Join(global::System.Environment.NewLine, message));

                global::Quire.Controls.Core.IEngineeringDocument activeDocument = ActiveDocument;

                if (AttemptToSaveWorksheetCopy(activeDocument))

                {

                    CloseDocument(activeDocument, forceClose: true);

                }

            }

            catch (global::System.Exception exception)

            {

                global::Quire.DebugTools.FailAssertion(exception.Message);

            }

        }



        e.Handled = true;

        Close();

        _handlingException = false;

    }
    private void HandleException(Exception exception, string message)

    {

        if (exception != null)

        {

            string logFile = global::Quire.Services.AppServiceRegistry.GetService<global::Quire.Controls.Core.IPrimeExceptionLogger>().LogException(exception);

            try

            {

                global::Quire.Wpf.WpfUtils.DoEvents();

            }

            catch (Exception ex)

            {

                global::Quire.DebugTools.FailAssertion(ex.Message);

            }

            MessageBoxManager.ShowExceptionMessage(string.Format(message, logFile));

        }

    }
    private bool AttemptToSaveWorksheetCopy(global::Quire.Controls.Core.IEngineeringDocument doc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UnhandledExceptionHandler(object sender, global::System.UnhandledExceptionEventArgs args)

    {

        if (_handlingException)

        {

            return;

        }



        _handlingException = true;

        string[] message =

        [

            "An Unhandled exception occurred. See",

            "{0}",

            "for more information. Prime will attempt to save any modified files and close.",

            "Please restart the application."

        ];

        global::System.Exception exception = args.ExceptionObject as global::System.Exception;

        HandleException(exception, string.Join(global::System.Environment.NewLine, message));

        if (((global::System.Windows.Threading.DispatcherObject)this).Dispatcher.CheckAccess())

        {

            Close();

        }

        else

        {

            ((global::System.Windows.Threading.DispatcherObject)this).Dispatcher.BeginInvoke(

                (global::System.Delegate)new global::System.Action(Close),

                (global::System.Windows.Threading.DispatcherPriority)10,

                global::System.Array.Empty<object>());

        }



        _handlingException = false;

    }
    private static void SpiritOptionPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)

    {

        if (e.PropertyName.Equals("EnableGettingStartedTab"))

        {

            global::Quire.Properties.Settings.Default.EnableGettingstartedTab = global::Quire.SpiritOptions.Instance.EnableGettingStartedTab;

        }

        global::Quire.Properties.Settings.Default.Save();

    }
    private void HelpManagerPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs args)

    {

        if (args.PropertyName == "HelpServerPath")

        {

            foreach (global::Quire.Controls.Core.IEngineeringDocument openDocument in OpenDocumentsList)

            {

                openDocument.HelpServerPath = _helpManager.HelpServerPath;

            }

        }

    }
    private void IsVisibleChangedEventHandler(

        object sender,

        global::System.Windows.DependencyPropertyChangedEventArgs e)

    {

        if ((bool)e.NewValue)

        {

            foreach (global::System.Collections.Generic.KeyValuePair<

                         global::System.Windows.Window,

                         global::System.Windows.Visibility> item in _visibilityDictionary)

            {

                item.Key.Visibility = item.Value;

            }

        }

        else

        {

            _visibilityDictionary.Clear();

            foreach (global::System.Windows.Window ownedWindow in OwnedWindows)

            {

                _visibilityDictionary[ownedWindow] = ownedWindow.Visibility;

                ownedWindow.Visibility = global::System.Windows.Visibility.Hidden;

            }

        }

    }
    private void ContextMenuOpeningEventHandler(object sender, global::System.Windows.Controls.ContextMenuEventArgs e)

    {

        global::System.Windows.FrameworkElement source = e.Source as global::System.Windows.FrameworkElement;

        if (source == null)

        {

            return;

        }

        global::System.Windows.Controls.ContextMenu contextMenu = source.ContextMenu;

        if (contextMenu != null && contextMenu.Name == "SystemQatCustomizeContextMenu")

        {

            for (int i = 0; i < contextMenu.Items.Count; i++)

            {

                global::System.Windows.Controls.MenuItem menuItem = contextMenu.Items.GetItemAt(i) as global::System.Windows.Controls.MenuItem;

                if (menuItem != null && menuItem.Name == "SystemCustomizeQatMenuItem")

                {

                    contextMenu.Items.RemoveAt(i);

                    ContextMenuOpening -= ContextMenuOpeningEventHandler;

                    break;

                }

            }

        }

    }
    private void HookupLicense(bool isEmbedding)

    {

        global::Quire.License.LicenseManager.Instance.LicenseTypeChanged += OnLicenseStatusChanged;

        global::Quire.License.LicenseManager.Instance.PropertyChanged += OnLicensePropertyChanged;

        if (!isEmbedding)

        {

            global::Quire.License.LicenseManager.Instance.LaunchLicenseWizardAtStartupIfNeeded();

        }

        UpdateShowLicenseErrorProperty();

    }
    private void UnhookLicense()

    {

        global::Quire.License.LicenseManager.Instance.LicenseTypeChanged -= OnLicenseStatusChanged;

        global::Quire.License.LicenseManager.Instance.PropertyChanged -= OnLicensePropertyChanged;

    }
    private void OnLicensePropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs args)

    {

        if (args.PropertyName == "DaysLeft")

        {

            UpdateShowLicenseErrorProperty();

        }

    }
    public bool ShowLicenseError

    {

        get => (bool)GetValue(ShowLicenseErrorProperty);

        set => SetValue(ShowLicenseErrorProperty, value);

    }
    private void OnLicenseStatusChanged(object sender, global::System.EventArgs eventArgs)

    {

        UpdateShowLicenseErrorProperty();

    }
    private void UpdateShowLicenseErrorProperty()

    {

        global::Quire.License.LicenseType licenseType = global::Quire.License.LicenseManager.Instance.LicenseType;

        int daysLeft = global::Quire.License.LicenseManager.Instance.DaysLeft ?? -1;

        ((global::System.Windows.DependencyObject)this).SetValue(

            ShowLicenseErrorProperty,

            licenseType == global::Quire.License.LicenseType.NoLicense ||

            licenseType == global::Quire.License.LicenseType.Trial ||

            (daysLeft >= 0 && daysLeft < 30));

    }
    private void HandleWindowLoaded(bool isEmbedding) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnFunctionalityServiceLicenseTypeChanged()

    {

        OnFunctionalityServiceLicenseTypeChanged(null, global::System.EventArgs.Empty);

    }
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    private static void OnRibbonControlBeforeAddToQat(object sender, global::Quire.DevComponents.WpfRibbon.QatOperationAddEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnFunctionalityServiceLicenseTypeChanged(object sender, global::System.EventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleWindowUnloaded()

    {

        UnhookWndProc();

    }
    private void HookupWndProc()

    {

        global::System.Windows.Interop.HwndSource hwndSource = global::System.Windows.Interop.HwndSource.FromHwnd(new global::System.Windows.Interop.WindowInteropHelper(this).Handle);

        if (hwndSource != null)

        {

            hwndSource.AddHook(WndProc);

        }

    }
    private void UnhookWndProc()

    {

        nint handle = new global::System.Windows.Interop.WindowInteropHelper(this).Handle;

        if (handle != global::System.IntPtr.Zero)

        {

            global::System.Windows.Interop.HwndSource hwndSource = global::System.Windows.Interop.HwndSource.FromHwnd(handle);

            if (hwndSource != null)

            {

                hwndSource.RemoveHook(WndProc);

            }

        }

    }
    private nint WndProc(nint hwnd, int msg, nint wparam, nint lparam, ref bool handled)

    {

        if ((long)msg == 271)

        {

            global::System.Windows.IInputElement focusedElementInSameScope =

                global::Quire.Wpf.WpfUtils.GetFocusedElementInSameScope(this);

            global::Quire.Controls.Core.IImeClient imeClient =

                global::Quire.Wpf.WpfUtils.TryFindAncestorIncludingSelf<global::Quire.Controls.Core.IImeClient>(

                    focusedElementInSameScope as global::System.Windows.DependencyObject,

                    includeLogical: false);

            if (imeClient != null)

            {

                global::Quire.Controls.Core.ImeHandler.PositioningIme(imeClient);

            }

        }



        return global::System.IntPtr.Zero;

    }
    private static void InitializeSettings()

    {

        global::System.Version version = global::System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        if (version == null)

        {

            return;

        }

        global::System.Version version2 = new global::System.Version(version.Major, 0, 0, 0);

        global::Quire.Properties.Settings.Default.Reload();

        if (!IsFeLinesInstance && version > version2)

        {

            global::Quire.UserSettingsProvider.CopyPrevUserConfig(version);

        }

        if (global::Quire.Properties.Settings.Default.UpgradeSettings)

        {

            global::Quire.Properties.Settings.Default.Upgrade();

            global::Quire.Properties.Settings.Default.UpgradeSettings = false;

            global::Quire.Properties.Settings.Default.Save();

        }

    }
    public object GetXamlObject(string fileName)

    {

        object result;

        try

        {

            global::System.Xml.XmlReader reader = global::System.Xml.XmlReader.Create(fileName);

            try

            {

                result = global::System.Windows.Markup.XamlReader.Load(reader);

            }

            finally

            {

                if (reader != null)

                {

                    ((global::System.IDisposable)reader).Dispose();

                }

            }

        }

        catch (global::System.Exception)

        {

            result = null;

        }



        return result;

    }
    private bool HandleMissingWriteRightsOnSaveException(

        global::Quire.Controls.Core.IEngineeringDocument doc,

        global::System.Exception exception,

        string fileName,

        global::Quire.Controls.Core.IDocumentSaveOptions options)

    {

        bool handled = ClassifyWriteFailure(exception, fileName, out string displayedMessage);

        if (handled)

        {

            DisplayWarning(displayedMessage);

            SaveDocumentWithFilenameAndExtension(doc, null, global::System.IO.Path.GetExtension(fileName), options);

        }

        return handled;

    }
    private bool ClassifyWriteFailure(

        global::System.Exception exception,

        string fileName,

        out string displayedMessage)

    {

        displayedMessage = null;

        bool handled;

        global::System.Exception exceptionCopy = exception;

        switch (exceptionCopy)

        {

            case global::Quire.IO.FileIsUsedByAnotherProcessException:

            {

                displayedMessage = exception.InnerException?.Message;

                if (displayedMessage.IsNullOrEmpty())

                {

                    displayedMessage = TryFindResource("Application.InUseByAnother") as string;

                }



                handled = true;

                break;

            }

            case global::System.UnauthorizedAccessException:

            {

                global::System.Nullable<bool> isReadonly = IsReadonly(fileName);

                if (isReadonly.GetValueOrDefault())

                {

                    displayedMessage = TryFindResource("Application.FileIsReadOnly") as string;

                    handled = true;

                }

                else

                {

                    // Mathcad sends a false nullable result through the shared default assignment.
                    // lint-ok: Mathcad Release IL shares this branch with the switch's default assignment.
                    goto default;

                }



                break;

            }

            default:

                handled = false;

                break;

        }



        return handled;

    }
    private void DisplayWarning(string displayedMessage) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void DisplayCriticalWarning(string displayedMessage) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.Controls.Core.IEngineeringDocument ActiveDocument

    {

        get => (global::Quire.Controls.Core.IEngineeringDocument)GetValue(ActiveDocumentProperty);

        set => SetValue(ActiveDocumentProperty, value);

    }
    private global::Quire.Controls.EngineeringDocument ActiveEngineeringDoc =>

        ActiveDocument as global::Quire.Controls.EngineeringDocument;
    public global::Quire.MruFiles.MruFileManager MruFileManager { get; } = new global::Quire.MruFiles.MruFileManager();
    internal global::Quire.Zoom ZoomControl => _zoomControl;
    [global::Quire.Mirror.MirrorStub]
    private void BindCommands() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleDocumentZoomIn(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        int steps = (int)e.Parameter;

        ZoomControl.ChangeZoomBySteps(steps);

    }
    private void CanImportExcel(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)

    {

        e.CanExecute = global::Quire.Controls.EquationCommands.InsertFunction.CanExecute(null, global::Quire.Wpf.WpfUtils.GetFocusedElementInSameScope(this));

    }
    private void DisplayTutorial(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        OpenUrl(_helpManager.GetTutorialPath(), useBrowser: true);

    }
    private void CanUpdateInputs(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)

    {

        if (ActiveDocument != null)

        {

            e.CanExecute = ActiveDocument.ReadonlyOptions.RequestToUpdateInputsEnabled;

        }

    }
    private bool AreAliasesValidForUpdateInputs(global::Quire.Controls.Core.IEngineeringDocument doc, global::System.Collections.Generic.IEnumerable<string> aliases)
    {
        return aliases.Select(alias => doc.InputOutputRegionManager.Inputs.GetItem(alias, doc.ReadonlyOptions.CaseInsensitiveAliasComparisonEnabled)).All(ioItem => ioItem != null);
    }
    // This field follows the capturing lambda to preserve the Mathcad closure ordinal.
    private bool _useDocumentTabs;
    internal void UpdateInputsEngNote(object sender)

    {

        UpdateInputs(sender, null);

    }
    private void UpdateInputs(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal void SetContextDataEngNote(string[] contextData)

    {

        global::Quire.Controls.Help.EngNoteContextData.Parse(contextData);

        _helpManager.EngNbLocalHelpEnvironmentVariableName = global::Quire.Controls.Help.EngNoteContextData.HelpEnvVarName;

    }
    private void HandleOpenFuncitonBrowser(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::System.Runtime.InteropServices.DllImport("user32.dll")]

    private static extern bool SetForegroundWindow(nint hWnd);
    [global::Quire.Mirror.MirrorStub]
    private void HandleLaunchConverter(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void CanLaunchConverter(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)

    {

        e.CanExecute = IsWorksheetConverterInstalled;

    }
    private void HandleCloseEngineeringDocument(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        global::Quire.Controls.Core.IEngineeringDocument engineeringDocument = e.Parameter as global::Quire.Controls.Core.IEngineeringDocument;

        if (engineeringDocument == null)

        {

            engineeringDocument = ActiveDocument;

        }

        HandleCloseEngineeringDocument(engineeringDocument);

    }
    private void HandleCloseTabs(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    private global::System.Predicate<global::Quire.Controls.Core.IEngineeringDocument> GetFilterForCloseTabsEnum(global::Quire.CloseTabCommandEnum commandEnum) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleMoveTab(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    private global::Quire.DevComponents.WpfDock.IDockWindowGroup MainDockWindowGroup => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CanExecuteMoveTabCommand(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal bool HandleCloseEngineeringDocument(global::Quire.Controls.Core.IEngineeringDocument doc)

    {

        return CloseDocument(doc);

    }
    private void LaunchImportExcel(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        global::Quire.ImportExcel importExcel = new global::Quire.ImportExcel

        {

            Owner = this

        };

        bool? result = importExcel.ShowDialog();

        if (result.HasValue && result.Value)

        {

            InsertReadExcel(

                ActiveDocument,

                importExcel.FileName,

                importExcel.SelectedRange,

                e.OriginalSource as global::System.Windows.IInputElement);

        }

    }
    public global::Quire.InputOutputDesignationWindow WorksheetInputOutputWindow { get; set; }
    private static void HandleOpenInputOutputDesignationWindow(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        if (sender is SpiritMainWindow)

        {

            (sender as SpiritMainWindow).OpenWorksheetInputOutputWindow();

        }

    }
    private void OpenWorksheetInputOutputWindow() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleDockWindowClosedEvent(object sender, global::System.EventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void AddHandlersForWorksheetInputOutputWindowEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetBindingForWorksheetInputOutputWindowProperties()

    {

        global::System.Windows.Data.BindingOperations.SetBinding(

            (global::System.Windows.DependencyObject)WorksheetInputOutputWindow,

            InputOutputDesignationWindow.EngineeringDocumentProperty,

            new global::System.Windows.Data.Binding

            {

                Source = this,

                Path = new global::System.Windows.PropertyPath(ActiveDocumentProperty)

            });

    }
    private static void InsertReadExcel(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName,

        string range,

        global::System.Windows.IInputElement originalSource)

    {

        string path = FileUtils.EvaluateRelativePath(

            document.DocCalculationVariables.CurrentWorkingDirectory,

            fileName,

            stripSingleLeadingDot: false);

        string[] args = new string[2] { path, range };

        global::Quire.Controls.EquationCommands.InsertFunction.Execute(

            new global::Quire.Controls.InsertFunctionCommandParameter("READEXCEL", args),

            originalSource);

    }
    private void DisplayHelp(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        string stringParameter = GetStringParameter(args);

        global::System.Windows.UIElement sourceElement = args.Source as global::System.Windows.UIElement;

        global::Quire.Controls.Core.IWhiteboardItem activeItem = ActiveEngineeringDoc.Worksheet.ActiveItem;

        string url = _helpManager.ConstructTopicPath(stringParameter, activeItem, sourceElement);

        OpenUrl(url, useBrowser: true);

    }
    private void ToolTipOpened(object sender, global::System.Windows.RoutedEventArgs e)

    {

        global::System.Windows.Controls.ToolTip toolTip = e.Source as global::System.Windows.Controls.ToolTip;

        if (global::Quire.DebugTools.AssertNotNull(toolTip, null).Failed)

        {

            return;

        }

        _helpManager.ActiveToolTipUid = toolTip.Uid;

        global::Quire.Wpf.DependencyPropertyChangeManager.AddPropertyChangedHandler(

            global::System.Windows.UIElement.IsVisibleProperty,

            (global::System.Windows.DependencyObject)toolTip,

            HandleTooltipIsVisibleChanged);

    }
    private void HandleTooltipIsVisibleChanged(object obj, global::System.Windows.DependencyPropertyChangedEventArgs args)

    {

        global::System.Windows.Controls.ToolTip toolTip = obj as global::System.Windows.Controls.ToolTip;

        if (global::Quire.DebugTools.AssertNotNull(toolTip, null).Failed)

        {

            return;

        }

        if (_helpManager.ActiveToolTipUid == toolTip.Uid)

        {

            _helpManager.ActiveToolTipUid = null;

        }

        global::Quire.Wpf.DependencyPropertyChangeManager.RemovePropertyChangedHandler(

            global::System.Windows.UIElement.IsVisibleProperty,

            toolTip,

            HandleTooltipIsVisibleChanged);

    }
    private static void HandleShowPropertyGrid(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    [global::Quire.Mirror.MirrorStub]
    private void HandleShowSaveAsError(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool CloseDocument(global::Quire.Controls.Core.IEngineeringDocument doc, bool forceClose = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    private void RemoveTabAndDocument(global::Quire.DevComponents.WpfDock.DockWindow dockWindow) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void RemoveTabAndDocument(global::AvalonDock.Layout.LayoutAnchorable dockWindow) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleNewDocument(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions =

            e.Parameter as global::Quire.Controls.Core.DocumentReadonlyOptions;

        NewDocument(readonlyOptions);

    }
    private bool SaveDocumentWithFilenameAndExtension(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName,

        string extension,

        global::Quire.Controls.Core.IDocumentSaveOptions options)

    {

        if (!options.IsHandlingException)

        {

            Addons.CallAddonsWhenDocumentSaving(document.FilePath);

        }

        else

        {

            global::Quire.DebugTools.DevAssert(fileName.IsNotEmpty(), null);

        }



        if (string.IsNullOrEmpty(fileName))

        {

            fileName = AskForSaveDefaultFileName(document, extension);

        }



        extension = global::System.IO.Path.GetExtension(fileName) ?? extension ?? ".mcdx";

        document.IsDocumentATemplate = extension.Equals(

            ".mctx",

            global::System.StringComparison.OrdinalIgnoreCase);

        return TrySaveFile(document, fileName, options);

    }
    private void HandleSaveDocumentWithExtension(string extension)

    {

        global::Quire.Controls.Core.IEngineeringDocument activeDocument = ActiveDocument;

        if (activeDocument.IsPrinting && !AskCancelPrintInProgressDialog())

        {

            return;

        }



        SaveDocumentWithFilenameAndExtension(

            activeDocument,

            null,

            extension,

            global::Quire.Controls.RegularSaveOptions.Empty);

    }
    private void HandleSaveDocumentAs(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        SaveDocumentWithFilename(ActiveDocument, e.Parameter as string);

    }
    internal bool SaveDocumentWithFilename(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName)

    {

        return SaveDocumentWithFilenameAndExtension(

            document,

            fileName,

            null,

            global::Quire.Controls.RegularSaveOptions.Empty);

    }
    internal bool SaveDocumentAsNonPrimeFormat_ForApiOnly(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName)

    {

        return SaveDocumentWithFilenameAndExtension(

            document,

            fileName,

            null,

            global::Quire.Controls.RegularSaveOptions.NonPrimeFormatForApi);

    }
    private void HandleSaveDocumentCopy(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        SaveDocumentWithFilenameAndExtension(

            ActiveDocument,

            e.Parameter as string,

            null,

            global::Quire.Controls.DetachedCopyOptions.Empty);

    }
    private int ShouldPromptForFileNameOnSave(global::Quire.Controls.Core.IEngineeringDocument doc)
    {
        int result = doc.IsNew ? 1 : 0;
        if (result == 0 && !doc.IsNetworkDriveDisconnected && !doc.CanBeSavedWithOriginalName)
        {
            result = 1;
            if (ShowMessage((global::System.Windows.Window window, string title) => MessageBoxManager.ShowQuestion(window, (string)TryFindResource("Application.FileIsReadOnly"), title, global::System.Windows.MessageBoxButton.OKCancel)) != global::System.Windows.MessageBoxResult.OK)
            {
                result = -1;
            }
        }
        return result;
    }
    public bool SaveDocumentAsPdf_ForApiOnly(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string pdfFileName)

    {

        return document.SaveAsPdf_ForApiOnly(pdfFileName);

    }
    internal bool SaveAndPushEngNote(

        global::Quire.Controls.Core.IEngineeringDocument engineeringDocument)

    {

        return SaveDocument(

            engineeringDocument,

            global::Quire.Controls.RegularSaveOptions.WithStatesGeneration);

    }
    private bool SaveDocument(

        global::Quire.Controls.Core.IEngineeringDocument document,

        global::Quire.Controls.RegularSaveOptions options)

    {

        bool result = false;

        string fileName = document.FilePath;

        if (global::Quire.DebugTools.DevAssert(!options.IsHandlingException, null).Passed)

        {

            Addons.CallAddonsWhenDocumentSaving(fileName);

        }



        int promptForFileName = ShouldPromptForFileNameOnSave(document);

        if (promptForFileName != -1)

        {

            if (promptForFileName == 1 && !document.IsNetworkDriveDisconnected)

            {

                fileName = AskForSaveDefaultFileName(document, null);

            }



            if (!string.IsNullOrEmpty(fileName))

            {

                result = TrySaveFile(document, fileName, options);

            }

        }



        return result;

    }
    private void HandleSaveDocumentWithInputsOutputsStates(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs eventArgs)

    {

        SaveDocument(ActiveDocument, global::Quire.Controls.RegularSaveOptions.WithStatesGeneration);

    }
    private void HandleSaveDocumentWithoutInputsOutputsStates(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs eventArgs)

    {

        SaveDocumentWithoutInputsOutputsStates(ActiveDocument);

    }
    internal bool SaveDocumentWithoutInputsOutputsStates(

        global::Quire.Controls.Core.IEngineeringDocument document)

    {

        return SaveDocument(document, global::Quire.Controls.RegularSaveOptions.Empty);

    }
    private string AskForSaveDefaultFileName(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string defaultFileExtensionForDialog)

    {

        if (defaultFileExtensionForDialog == null)

        {

            defaultFileExtensionForDialog = ".mcdx";

            if (!string.IsNullOrEmpty(document.FileName))

            {

                string extension = global::System.IO.Path.GetExtension(document.FileName);

                if (!string.IsNullOrEmpty(extension) && extension.Equals(

                        ".mctx",

                        global::System.StringComparison.OrdinalIgnoreCase))

                {

                    defaultFileExtensionForDialog = ".mctx";

                }

            }

        }



        return AskForSaveFileName(

            document,

            GetFileSaveOpenFilters(null, save: true),

            defaultFileExtensionForDialog);

    }
    internal global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair>

        GetFileSaveOpenFilters(string extension, bool save)

    {

        return GetFileSaveOpenFilters(

            extension,

            TryFindResource,

            global::Quire.Services.FunctionalityService.Instance,

            global::Quire.Properties.Settings.Default.DocOptionsEnabled,

            save);

    }
    internal static global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair>

        GetFileSaveOpenFilters(

            string extension,

            global::System.Func<object, object> tryFindResource,

            global::Quire.Services.IFunctionalityService functionalityService,

            bool docOptionsEnabled,

            bool save)

    {

        global::System.Collections.Generic.List<global::Quire.Controls.Core.FileDialogFilterPair> filters =

            new global::System.Collections.Generic.List<global::Quire.Controls.Core.FileDialogFilterPair>();

        if (extension == ".mcdx" || extension == null)

        {

            filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

            {

                FilterString = string.Format(

                    "{0} (*{1})|*{1}",

                    tryFindResource(save

                        ? "Application.MathcadPrimeWorksheetSaveFilter"

                        : "Application.MathcadPrimeWorksheetsOpenFilter"),

                    ".mcdx"),

                Extension = ".mcdx"

            });

        }



        if (save)

        {

            if (extension == ".mctx" ||

                (extension == null && functionalityService.IsFunctionalityEnabled(

                    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.SaveEngineeringDocumentAsMCTXCommand)))

            {

                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

                {

                    FilterString = string.Format(

                        "{0} (*{1})|*{1}",

                        tryFindResource("Application.MathcadPrimeTemplateSaveFilter"),

                        ".mctx"),

                    Extension = ".mctx"

                });

            }



            if (extension == ".rtf" ||

                (extension == null && functionalityService.IsFunctionalityEnabled(

                    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.SaveEngineeringDocumentAsRtfCommand)))

            {

                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

                {

                    FilterString = string.Format(

                        "{0} (*{1})|*{1}",

                        tryFindResource("Application.RtfDocumentSaveFilter"),

                        ".rtf"),

                    Extension = ".rtf"

                });

            }



            if (extension == ".pdf" ||

                (extension == null && functionalityService.IsFunctionalityEnabled(

                    global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.SaveEngineeringDocumentAsPdfCommand)))

            {

                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

                {

                    FilterString = string.Format(

                        "{0} (*{1})|*{1}",

                        tryFindResource("Application.PdfDocumentSaveFilter"),

                        ".pdf"),

                    Extension = ".pdf"

                });

            }



            if (extension == ".xps" || extension == null)

            {

                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

                {

                    FilterString = string.Format(

                        "{0} (*{1})|*{1}",

                        tryFindResource("Application.XPSDocumentSaveFilter"),

                        ".xps"),

                    Extension = ".xps"

                });

            }



            if (extension == ".dita" || ((extension == null) & docOptionsEnabled))

            {

                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

                {

                    FilterString = string.Format("{0} (*{1})|*{1}", "Dita files", ".dita"),

                    Extension = ".dita"

                });

            }

        }

        else if (extension == ".mctx" ||

                 (extension == null && functionalityService.IsFunctionalityEnabled(

                     global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey.OpenTemplateDocumentCommand)))

        {

            filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

            {

                FilterString = string.Format(

                    "{0} (*{1})|*{1}",

                    tryFindResource("Application.MathcadPrimeTemplatesOpenFilter"),

                    ".mctx"),

                Extension = ".mctx"

            });

            if (extension == null)

            {

                filters.Add(new global::Quire.Controls.Core.FileDialogFilterPair

                {

                    FilterString = string.Format(

                        "{0} (*{1};*{2})|*{1};*{2}",

                        tryFindResource("Application.AllMathcadPrimeFilesOpenFilter"),

                        ".mcdx",

                        ".mctx"),

                    Extension = ".*"

                });

            }

        }



        return filters;

    }
    private int GetFilterIdxForExtensionOrDefault(

        global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair> filterArray,

        string fileExtension)

    {

        int index = -1;

        int defaultIndex = -1;

        for (int i = 0; i < filterArray.Count; i++)

        {

            if (filterArray[i].Extension == fileExtension)

            {

                index = i;

                break;

            }

            if (filterArray[i].Extension == ".mcdx")

            {

                defaultIndex = i;

            }

        }

        if (index >= 0)

        {

            return index + 1;

        }

        return defaultIndex + 1;

    }
    private string AskForSaveFileName(

        global::Quire.Controls.Core.IEngineeringDocument doc,

        global::System.Collections.Generic.IList<global::Quire.Controls.Core.FileDialogFilterPair> filterList,

        string defaultFileExtension)

    {

        if (MessageBoxManager.IsDisabled)

        {

            return null;

        }



        global::System.Windows.Forms.SaveFileDialog sfd = new global::System.Windows.Forms.SaveFileDialog();

        if (defaultFileExtension.Equals(".mctx", global::System.StringComparison.OrdinalIgnoreCase))

        {

            string myTemplatesLocation = GetMyTemplatesLocation();

            if (!global::System.IO.Directory.Exists(myTemplatesLocation))

            {

                if (!IsDirectoryOneOfDefaultTemplateLocations(myTemplatesLocation))

                {

                    SetAndDisplayWarning(TryFindResource("Application.DirectoryDoesNotExist") as string, myTemplatesLocation);

                    return null;

                }



                if (global::Quire.Services.AppServiceRegistry.GetService<global::Quire.IO.IFileOperationsProvider>().CreateDirectory(myTemplatesLocation) != null)

                {

                    SetAndDisplayWarning(TryFindResource("Application.UnableCreateDirectory") as string, myTemplatesLocation);

                    return null;

                }

            }



            sfd.InitialDirectory = myTemplatesLocation;

        }

        else if (global::System.IO.Directory.Exists(doc.CWD))

        {

            sfd.InitialDirectory = doc.CWD;

        }

        else if (global::System.IO.Directory.Exists(global::Quire.Properties.Settings.Default.CWD))

        {

            sfd.InitialDirectory = global::Quire.Properties.Settings.Default.CWD;

        }



        Addons.CallAddonsOnSaveDialogOpening(ref sfd);

        if (filterList == null)

        {

            return null;

        }



        sfd.Filter = filterList.ToFilterString();

        sfd.FilterIndex = GetFilterIdxForExtensionOrDefault(filterList, defaultFileExtension);

        sfd.FileName = FilePathToNameConverter.Convert(doc.DisplayFilePath, doc.DocumentName);

        try

        {

            if (DialogManager.ShowDialog(sfd) == global::System.Windows.Forms.DialogResult.OK)

            {

                return ReviseFileExtension(sfd.FileName, filterList[sfd.FilterIndex - 1].Extension);

            }

        }

        catch (global::System.IO.PathTooLongException ex)

        {

            global::System.Windows.MessageBox.Show(

                this,

                ex.Message,

                ex.Source,

                global::System.Windows.MessageBoxButton.OK,

                global::System.Windows.MessageBoxImage.Exclamation,

                global::System.Windows.MessageBoxResult.OK,

                global::System.Windows.MessageBoxOptions.None);

        }



        return null;

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
    private static global::System.Collections.Generic.IList<T> ExtractCaseInsensitiveDuplicates<T>(global::System.Collections.Generic.IEnumerable<T> allItems, out global::System.Collections.Generic.IList<string> duplicates) where T : global::Quire.Controls.Core.IORegions.IItemAlias => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::Quire.Controls.Core.IORegions.IInputOutputItemsStates GenerateInputOutputStatesOnSave(global::Quire.Controls.Core.IEngineeringDocument doc, string dlgTitle, out bool cancelSave, out global::Quire.SpiritMainWindow.SaveAndSyncFinalMessage needsFinalMessage) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool TrySaveFile(global::Quire.Controls.Core.IEngineeringDocument doc, string fileName, global::Quire.Controls.Core.IDocumentSaveOptions options) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void OnCWDChanged(string directory)

    {

        if (Addons?.CallAddonsToCheckIfCwdShouldBeUpdated(directory) ?? true)

        {

            global::Quire.Properties.Settings.Default.CWD = directory;

            global::Quire.Properties.Settings.Default.Save();

        }

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
    private static bool? IsReadonly(string fileName)

    {

        try

        {

            return (global::System.IO.File.GetAttributes(fileName) & global::System.IO.FileAttributes.ReadOnly) != 0;

        }

        catch

        {

            return null;

        }

    }
    private void HandleOpenDocument(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        string fileName;

        string tag = string.Empty;

        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions = null;

        if (args.Parameter is global::Quire.OpenEngineeringDocumentParameter documentParameter)

        {

            fileName = documentParameter.FilePath;

            readonlyOptions = documentParameter.ReadonlyOptions;

        }

        else

        {

            fileName = args.Parameter as string;

        }



        if (UrlUtils.HasTag(fileName, out string pathWithoutTag, out string tagName))

        {

            tag = tagName;

            fileName = pathWithoutTag;

        }



        if (!UseDocumentTabs && !AskToSaveDocument(ActiveDocument))

        {

            return;

        }



        if (string.IsNullOrEmpty(fileName))

        {

            fileName = LaunchOpenFileDialogAndGetFileName(((global::System.Windows.Input.RoutedCommand)args.Command).Name);

            Addons.CallAddonsOnOpenDialogClosing();

        }



        fileName = Addons.CallAddonsToGetResolvedFileNameForOpen(fileName);

        if (!string.IsNullOrEmpty(fileName))

        {

            if (!global::System.IO.File.Exists(fileName))

            {

                OpenFileExceptionHandler(fileName, new global::System.IO.FileNotFoundException(string.Empty, fileName));

            }

            else

            {

                OpenFile(fileName, readonlyOptions, tag);

            }

        }

    }
    private void HandleNewDocumentFromExisting(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        string templateFileName = GetTemplateFileName(args, isNewFromTemplate: true);

        if (string.IsNullOrEmpty(templateFileName))

        {

            return;

        }



        global::Quire.Controls.Core.IEngineeringDocument document;

        if (!global::System.IO.File.Exists(templateFileName))

        {

            OpenFileExceptionHandler(

                templateFileName,

                new global::System.IO.FileNotFoundException(string.Empty, templateFileName));

        }

        else if (HandleClonningAndOpenningWorksheetInNewDocument(templateFileName, null, out document) &&

                 document.ReadonlyOptions.FileLocationHistoryEnabled)

        {

            MruFileManager.AddMRUFileItemIfNeeded(templateFileName);

        }

    }
    private bool HandleClonningAndOpenningWorksheetInNewDocument(

        string fileName,

        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions,

        out global::Quire.Controls.Core.IEngineeringDocument document)

    {

        bool result = true;

        document = NewDocument(incrementCounter: true, readonlyOptions, isTemplate: false);

        global::System.Exception exception = CloneWorksheetAndOpenInDocument(document, fileName);

        if (exception != null)

        {

            CloseDocument(document);

            result = false;

            OpenFileExceptionHandler(fileName, exception);

        }



        return result;

    }
    private string GetTemplateFileName(

        global::System.Windows.Input.ExecutedRoutedEventArgs args,

        bool isNewFromTemplate = false)

    {

        string parameter = args.Parameter as string;

        string fileName = string.Empty;

        if (!string.IsNullOrEmpty(parameter))

        {

            fileName = TemplatesFolderLocations.ContainsKey(parameter)

                ? LaunchOpenFileDialogAndGetFileNameForTemplates(

                    TemplatesFolderLocations[parameter],

                    isNewFromTemplate)

                : parameter;

        }



        return fileName;

    }
    private void HandleOpenTemplateDocument(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        if (!UseDocumentTabs && !AskToSaveDocument(ActiveDocument))

        {

            return;

        }



        string templateFileName = GetTemplateFileName(args);

        if (string.IsNullOrEmpty(templateFileName))

        {

            return;

        }



        if (!string.IsNullOrEmpty(templateFileName))

        {

            if (!global::System.IO.File.Exists(templateFileName))

            {

                OpenFileExceptionHandler(

                    templateFileName,

                    new global::System.IO.FileNotFoundException(string.Empty, templateFileName));

            }

            else

            {

                OpenFile(templateFileName, null);

            }

        }

    }
    private bool IsDirectoryOneOfDefaultTemplateLocations(string location)

    {

        if (!string.Equals(location, DefaultMyTemplatesLocation, global::System.StringComparison.OrdinalIgnoreCase))

        {

            return string.Equals(location, DefaultSharedTemplatesLocation, global::System.StringComparison.OrdinalIgnoreCase);

        }

        return true;

    }
    public string LaunchOpenFileDialogAndGetFileNameForTemplates(

        string templatesFolderLocation,

        bool overrideDialogTitle,

        string title = null)

    {

        string result = string.Empty;

        string dialogTitle = title ?? (overrideDialogTitle

            ? TryFindResource("Application.FileOpenDialogForTemplatesTitle") as string

            : TryFindResource("Application.FileOpenDialogForOpenTemplatesTitle") as string);

        string filterString = GetFileSaveOpenFilters(".mctx", save: false)[0].FilterString;

        if (!global::System.IO.Directory.Exists(templatesFolderLocation))

        {

            if (!IsDirectoryOneOfDefaultTemplateLocations(templatesFolderLocation))

            {

                SetAndDisplayWarning(

                    TryFindResource("Application.DirectoryDoesNotExist") as string,

                    templatesFolderLocation);

                return null;

            }



            if (global::Quire.Services.AppServiceRegistry

                    .GetService<global::Quire.IO.IFileOperationsProvider>()

                    .CreateDirectory(templatesFolderLocation) != null)

            {

                SetAndDisplayWarning(

                    TryFindResource("Application.UnableCreateDirectory") as string,

                    templatesFolderLocation);

                return null;

            }

        }



        if (global::Quire.SystemInfo.OsBeforeXp())

        {

            var openFileDialog = new global::Microsoft.Win32.OpenFileDialog

            {

                Filter = filterString,

                Title = dialogTitle,

                InitialDirectory = templatesFolderLocation

            };

            if (DialogManager.ShowDialog(openFileDialog) == true)

            {

                result = openFileDialog.FileName;

            }

        }

        else

        {

            var openFileDialog = new global::System.Windows.Forms.OpenFileDialog

            {

                Filter = filterString,

                Title = dialogTitle,

                InitialDirectory = templatesFolderLocation,

                RestoreDirectory = true

            };

            if (DialogManager.ShowDialog(openFileDialog) ==

                global::System.Windows.Forms.DialogResult.OK)

            {

                result = openFileDialog.FileName;

            }

        }



        return result;

    }
    private void SetAndDisplayWarning(string warningMessage, string problematicEntity)

    {

        string displayedMessage = string.Format("{0}{1}{2}", warningMessage, Environment.NewLine, problematicEntity);

        DisplayWarning(displayedMessage);

    }
    private global::System.Exception CloneWorksheetAndOpenInDocument(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName)

    {

        global::Quire.Controls.Core.FileLoadResult fileLoadResult;

        try

        {

            global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;

            document.IsCalculationsEnabled = false;

            document.IsAutoCalcOn = false;

            fileLoadResult = document.NewFromDocument(fileName);

            document.IsCalculationsEnabled = true;

            document.IsAutoCalcOn = document.DocCalculationOptions.IsAutoCalcOn;

            if (fileLoadResult.ErrorsOccurredWhenLoadingRegions)

            {

                string logFile = ExLogger.LogExceptions(fileLoadResult.DeserializationExceptions);

                DisplayCriticalWarningWithLogFile(

                    (string)TryFindResource("Application.SomeRegionsCorrupted"),

                    logFile);

            }

        }

        finally

        {

            global::System.Windows.Input.Mouse.OverrideCursor = null;

        }



        return fileLoadResult.LoadException;

    }
    private string LaunchOpenFileDialogAndGetFileName(string commandName)

    {

        if (commandName.Equals("OpenFromWC"))

        {

            return Addons.CallAddonsToOpenFromWindchill();

        }



        string fileName = string.Empty;

        string filter = GetFileSaveOpenFilters(null, save: false).ToFilterString();

        global::System.Windows.Forms.OpenFileDialog dialog = new global::System.Windows.Forms.OpenFileDialog();

        Addons.CallAddonsOnOpenDialogOpening(ref dialog);

        dialog.Filter = filter;

        if (DialogManager.ShowDialog(dialog) == global::System.Windows.Forms.DialogResult.OK)

        {

            fileName = dialog.FileName;

        }



        return fileName;

    }
    protected override void OnDrop(global::System.Windows.DragEventArgs e)

    {

        if (e.Data.GetDataPresent(global::System.Windows.DataFormats.FileDrop))

        {

            if (e.Data.GetData(global::System.Windows.DataFormats.FileDrop) is string[] files && files.Length == 1)

            {

                AppCommands.OpenEngineeringDocument.Execute(files[0], ActiveDocument as global::System.Windows.IInputElement);

            }

        }

        else

        {

            base.OnDrop(e);

        }

    }
    private void OpenFile(

        string fileName,

        global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions,

        string tag = "")

    {

        string extension = global::System.IO.Path.GetExtension(fileName);

        if (UseDocumentTabs)

        {

            foreach (global::Quire.Controls.Core.IEngineeringDocument openDocument in OpenDocumentsList)

            {

                if (openDocument.FilePath != null &&

                    global::System.IO.Path.GetFullPath(openDocument.FilePath).Equals(

                        global::System.IO.Path.GetFullPath(fileName),

                        global::System.StringComparison.InvariantCultureIgnoreCase))

                {

                    ActiveDocument = openDocument;

                    ScrollToTagInDoc(openDocument, tag);

                    return;

                }

            }

        }



        global::Quire.Controls.Core.IEngineeringDocument document =

            NewDocument(

                incrementCounter: false,

                readonlyOptions,

                string.Equals(extension, ".mctx", global::System.StringComparison.OrdinalIgnoreCase));

        TryOpenFileInDocument(document, fileName, tag);

    }
    private void ScrollToTagInDoc(global::Quire.Controls.Core.IEngineeringDocument document, string tag)

    {

        if (tag.IsNotEmpty())

        {

            (document.Worksheet as global::Quire.Controls.Worksheet.WorksheetControl)?.BringFirstTagItemIntoView(tag);

        }

    }
    private bool TryOpenFileInDocument(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName,

        string tag = "")

    {

        global::System.Exception exception = OpenFileInDocument(document, fileName, tag);

        if (exception != null)

        {

            CloseDocument(document);

            OpenFileExceptionHandler(fileName, exception);

            return false;

        }



        return true;

    }
    private void HandleOpenFileFailure(string fileName, global::System.Exception result)

    {

        bool logException = false;

        string displayedMessage;

        if (result is global::System.IO.FileNotFoundException)

        {

            displayedMessage = (string)TryFindResource("Application.FileDoesNotExist") + "\n" + fileName;

        }

        else if (result is global::System.FormatException)

        {

            logException = true;

            displayedMessage = (string)TryFindResource("Application.FileTypeNotSupported");

        }

        else if (result is global::Quire.Core.Serialization.IncompatibleFormatWorksheetException)

        {

            displayedMessage =

                (TryFindResource("Application.FileNotCompatibleKey") as string) ?? result.Message;

        }

        else if (result is global::Quire.Controls.PreviousVersionWorksheetException)

        {

            displayedMessage = (string)TryFindResource("Application.OpenPreviousVersionWorksheet");

        }

        else

        {

            logException = true;

            displayedMessage = (string)TryFindResource("Application.FileTypeNotSupported");

        }



        if (logException)

        {

            result.Data.Add("File being opened", fileName);

            string logFile = ExLogger.LogException(result);

            DisplayCriticalWarningWithLogFile(displayedMessage, logFile);

        }

        else

        {

            DisplayWarning(displayedMessage);

        }

    }
    private void HandleSaveFileFailure(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName,

        global::System.Exception result,

        global::Quire.Controls.Core.IDocumentSaveOptions options)

    {

        if (MessageBoxManager.IsDisabled)

        {

            return;

        }



        if (HandleMissingWriteRightsOnSaveException(document, result, fileName, options))

        {

            return;

        }



        if (result is global::Quire.IO.PrintToFileException printToFileException)

        {

            string message;

            if (printToFileException.ErrorCode == global::Quire.IO.PrintFileErrorCode.TakeInnerExceptionByOS)

            {

                message = printToFileException.InnerException?.Message;

            }

            else

            {

                message = TryFindResource(printToFileException.ErrorCode) as string;

                if (message.IsNotEmpty())

                {

                    string formattedMessage = null;

                    try

                    {

                        formattedMessage = string.Format(message, printToFileException.MessageArgs);

                    }

                    catch

                    {

                        global::Quire.DebugTools.FailAssertion(null, global::System.Array.Empty<object>());

                    }



                    message = formattedMessage ?? message;

                }

            }



            DisplayWarning(

                global::Quire.DebugTools.DevAssert(message.IsNotEmpty(), null).Passed

                    ? message

                    : (result.Message ?? string.Empty));

        }

        else if (result is global::Quire.IO.FileFormatBlockedByFreemiumException)

        {

            if (!MessageBoxManager.IsDisabled)

            {

                global::Quire.DebugTools.FailAssertionDoNotThrow(null, global::System.Array.Empty<object>());

            }

        }

        else if (result is global::System.ApplicationException)

        {

            DisplayWarning(result.Message);

        }

        else

        {

            result.Data.Add("File being saved", fileName);

            string logFile = ExLogger.LogException(result);

            DisplayCriticalWarningWithLogFile(

                (string)TryFindResource("Application.FileTypeNotSupported"),

                logFile);

        }

    }
    private void DisplayCriticalWarningWithLogFile(string displayedMessage, string logFile)

    {

        DisplayCriticalWarning(string.Format(

            "{0}{1}{2}{1}{3}",

            displayedMessage,

            Environment.NewLine,

            (string)FindResource("Application.LogFileIsLocatedAt"),

            logFile));

    }
    private global::System.Exception OpenFileInDocument(

        global::Quire.Controls.Core.IEngineeringDocument document,

        string fileName,

        string tag)

    {

        global::Quire.Controls.Core.FileLoadResult fileLoadResult;

        try

        {

            global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;

            document.IsCalculationsEnabled = false;

            document.IsAutoCalcOn = false;

            fileLoadResult = document.OpenFileAndUpdateCWDState(fileName);

            if (fileLoadResult.LoadException == null)

            {

                if (document.ReadonlyOptions.FileLocationHistoryEnabled)

                {

                    MruFileManager.AddMRUFileItemIfNeeded(fileName);

                }



                Addons.CallAddonsWhenDocumentOpened(document, fileName);

            }



            document.IsAutoCalcOn = document.DocCalculationOptions.IsAutoCalcOn;

            document.IsCalculationsEnabled = true;

            document.IsDirty = false;

            if (fileLoadResult.ErrorsOccurredWhenLoadingRegions)

            {

                string logFile = ExLogger.LogExceptions(fileLoadResult.DeserializationExceptions);

                DisplayCriticalWarningWithLogFile(

                    (string)TryFindResource("Application.SomeRegionsCorrupted"),

                    logFile);

            }

        }

        finally

        {

            global::System.Windows.Input.Mouse.OverrideCursor = null;

            ScrollToTagInDoc(document, tag);

        }



        return fileLoadResult.LoadException;

    }
    internal void ResetUntitledIndex()

    {

        _nextUntitledIndex = 0;

    }
    internal bool UseDocumentTabs

    {

        get => _useDocumentTabs;

        set => _useDocumentTabs = value;

    }
    private void UpdateCurrentShowWarningsFilter()

    {

        CurrentShowWarningsFilter = IsShowWarningsEnabled ? DisplayWarningsFilter : WarningsTypesToShow.None;

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
    private global::Quire.Controls.Core.IEngineeringDocument NewDocument(global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions = default) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool CanGetDefaultBuiltInTemplateIfPossible(ref string templateName, out bool isTemplateIncompatible)

    {

        templateName = GetPathToDefaultTemplateForNewDocument();

        return CanUseTemplateForNewDocument(templateName, out isTemplateIncompatible);

    }
    private bool CanGetDefaultBuildInTemplateFromDefaultLocationIfPossible(ref string templateName)

    {

        templateName = DefaultPathToDefaultTemplateForNewDocument;

        return DoesTemplateFileExists(templateName);

    }
    private bool DoesTemplateFileExists(string templateName)

    {

        if (!string.IsNullOrEmpty(templateName))

        {

            return global::Quire.Services.AppServiceRegistry.GetService<global::Quire.IO.IFileOperationsProvider>().FileExists(templateName);

        }

        return false;

    }
    private bool CanUseTemplateForNewDocument(string templateName, out bool isTemplateIncompatible)

    {

        isTemplateIncompatible = false;

        if (!DoesTemplateFileExists(templateName))

        {

            return false;

        }



        if (!global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable.TryGetFormatVersionFromFile(

                templateName,

                out global::System.Version formatVersion))

        {

            return false;

        }



        isTemplateIncompatible = !global::Quire.Core.Serialization.ApplicationPropertiesXmlSerializable

            .IsFormatVersionCompatible(formatVersion);

        return !isTemplateIncompatible;

    }
    private string ChooseDefaultTemplateIfAnyForNewDocument()

    {

        string templateName = string.Empty;

        if (CanGetDefaultBuiltInTemplateIfPossible(ref templateName, out bool isTemplateIncompatible))

        {

            return templateName;

        }



        if (!string.IsNullOrEmpty(templateName) &&

            !templateName.Equals(DefaultPathToDefaultTemplateForNewDocument))

        {

            if (isTemplateIncompatible)

            {

                SetAndDisplayWarning(

                    TryFindResource("Application.TemplateFileVersionNotCompatible") as string,

                    templateName);

            }

            else

            {

                SetAndDisplayWarning(

                    TryFindResource("Application.UserDefinedDefaultTemplateDoesNotExists") as string,

                    templateName);

            }

        }



        if (CanGetDefaultBuildInTemplateFromDefaultLocationIfPossible(ref templateName))

        {

            return templateName;

        }



        return null;

    }
    private global::Quire.Controls.Core.IEngineeringDocument NewDocument(bool incrementCounter, global::Quire.Controls.Core.DocumentReadonlyOptions readonlyOptions, bool isTemplate) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ReplaceActiveTabWithDocument(global::Quire.Controls.EngineeringDocument newDoc, bool incrementCounter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ActiveTabChanged(object sender, global::System.Windows.Controls.SelectionChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal int GetIndexOfTabForDocument(global::Quire.Controls.Core.IEngineeringDocument doc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    internal static global::AvalonDock.Layout.LayoutAnchorable GetTabForDocument(

        global::Quire.Controls.Core.IEngineeringDocument document)

    {

        return global::Quire.Wpf.WpfUtils.FindLogicalAncestor<global::AvalonDock.Layout.LayoutAnchorable>(

            (global::System.Windows.DependencyObject)(document is global::System.Windows.DependencyObject

                ? document

                : null));

    }
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    public static global::Quire.Controls.Core.IEngineeringDocument GetDocumentForTab(global::Quire.DevComponents.WpfDock.DockWindow dockWindow) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public static global::Quire.Controls.Core.IEngineeringDocument GetDocumentForTab(

        global::AvalonDock.Layout.LayoutAnchorable dockWindow)

    {

        if (dockWindow != null && dockWindow.Content is global::Quire.Controls.ScrollViewerGrid scrollViewerGrid)

        {

            return scrollViewerGrid.EngineeringDocumentInstance;

        }



        return null;

    }
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    public static string GetApplicationFullWindowTitle(global::Quire.License.LicenseType licenseType, global::System.Windows.FrameworkElement resourcesElement, string titleText) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CreateNewTabWithDocument(global::Quire.Controls.EngineeringDocument content, bool incrementCounter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void LaunchCustomMarginsDialog(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        if (DebugTools.DevAssert(args.Parameter is global::System.Windows.FrameworkElement, null).Failed)

        {

            return;

        }



        CustomMarginsDialog.ShowDialog(

            this,

            (global::System.Windows.FrameworkElement)args.Parameter,

            ActiveEngineeringDoc.Worksheet.PageLayout);

    }
    private void LaunchSystemUnitDialog(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        SystemUnitEditDialog.ShowDialog(

            this,

            args.Parameter as global::System.ComponentModel.ICollectionView,

            global::Quire.Controls.VisualCGSUnitDescriptorTable.Instance,

            global::Quire.Calculation.Core.DefaultUnitDescriptorTable.Instance);

    }
    private void HandleWorksheetControlDebuggerModeChange(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

    }
    // compile-order placeholder; replaced when DevComponents.WpfDock lands
    [global::Quire.Mirror.MirrorStub]
    private void SetTabHeaderBindingForNewDocument(global::Quire.DevComponents.WpfDock.DockWindow dockWin, global::Quire.Controls.Core.IEngineeringDocument newDoc, bool incrementCounter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetTabHeaderBindingForNewDocument(global::AvalonDock.Layout.LayoutAnchorable dockWin, global::Quire.Controls.Core.IEngineeringDocument newDoc, bool incrementCounter) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private string GetUntitledHeader(bool incrementIndex)

    {

        int index = UseDocumentTabs ? _nextUntitledIndex : 0;

        if (incrementIndex)

        {

            if (UseDocumentTabs || _nextUntitledIndex == 0)

            {

                _nextUntitledIndex++;

            }

            global::Quire.DebugTools.DevAssert(_nextUntitledIndex > -1, null);

        }

        if (index == 0)

        {

            return (string)TryFindResource("Application.Untitled");

        }

        return string.Format((string)TryFindResource("Application.UntitledWithIndex"), index);

    }
    private void InitializeDocument(global::Quire.Controls.EngineeringDocument document) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnDocumentLoaded(global::Quire.Controls.Core.IEngineeringDocument document)

    {

        if (global::Quire.DebugTools.AssertNotNull(document, null).Failed)

        {

            return;

        }

        if (document.IsDirty)

        {

            OnDocumentModified(document);

        }

        document.IsDirtyChanged += OnDocumentModified;

    }
    private void HandleDocumentPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)

    {

        if (!(sender is global::Quire.Controls.Core.IEngineeringDocument engineeringDocument))

        {

            return;

        }

        string propertyName = e.PropertyName;

        switch (propertyName)

        {

            case "DocumentTabIcon":

                OnDocumentTabIconChanged(engineeringDocument);

                break;

            case "Zoom":

                OnZoomChanged(engineeringDocument.Zoom);

                break;

        }

    }
    private void HandleDocumentInputsOutputsSelected(

        global::Quire.Controls.Core.IEngineeringDocument doc,

        global::System.Collections.Generic.List<string> selectedInputs,

        global::System.Collections.Generic.List<string> selectedOutputs)

    {

        DocumentInputsOutputsSelected(doc, selectedInputs, selectedOutputs);

    }
    private static void OnDocumentTabIconChanged(global::Quire.Controls.Core.IEngineeringDocument document) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UnhookFromDocument(global::Quire.Controls.EngineeringDocument document) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnDocumentFilePathChanged(global::Quire.Controls.Core.IEngineeringDocument doc)

    {

        if (Addons != null)

        {

            string filePath = doc.FilePath;

            string displayNameForDocName = Addons.GetDisplayNameForDocName(filePath);

            if (displayNameForDocName.IsNotEmpty())

            {

                doc.CustomizedDocumentName = displayNameForDocName;

            }

            string displayTitle = Addons.GetDisplayTitle(filePath);

            if (displayTitle.IsNotEmpty())

            {

                doc.CustomizedWindowTitle = displayTitle;

            }

        }

    }
    private void ExitApplication(object sender, ExecutedRoutedEventArgs e)

    {

        Close();

    }
    private void DisplayAboutDialog(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        OptionsAboutDialogOpening();

        AboutWindow aboutWindow = new AboutWindow(this);

        aboutWindow.CommandBindings.Add(HelpCommandBinding);

        aboutWindow.ShowDialog();

    }
    private void DisplayOptionsDialog(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        OptionsAboutDialogOpening();

        OptionsWindow optionsWindow = new OptionsWindow(this);

        optionsWindow.CommandBindings.Add(HelpCommandBinding);

        optionsWindow.ShowDialog(global::Quire.License.LicenseManager.Instance.HasValidLicense

            ? OptionsWindow.DialogTabIndex.OptionsTab

            : OptionsWindow.DialogTabIndex.LicenseStatusTab);

        UpdateTemplateLocationsIfNeeded();

    }
    internal void UpdateTemplateLocationsIfNeeded()

    {

        SetMyTemplatesLocation(global::Quire.Properties.Settings.Default.MyTemplatesLocation);

        SetSharedTemplatesLocation(global::Quire.Properties.Settings.Default.SharedTemplatesLocation);

    }
    private void DisplayHyperlink(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        global::System.Uri absoluteUri;

        string stringParameter = GetStringParameter(e);

        if (stringParameter != null && global::Quire.IO.UrlUtils.TryCreateAbsoluteUri(GetMathcadPrimeInstallDirectoryLocalPath(), stringParameter, out absoluteUri))

        {

            OpenUrl(absoluteUri.AbsoluteUri, useBrowser: false);

        }

        else

        {

            global::Quire.DebugTools.FailAssertion(null, global::System.Array.Empty<object>());

        }

    }
    private void OpenUrl(string url, bool useBrowser)

    {

        if (!UrlUtils.TryOpenUrl(url, useBrowser))

        {

            ShowFailedToOpenUrlMessage(url);

        }

    }
    private void ShowFailedToOpenUrlMessage(string url) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void DisplayFreemiumAboutBox(object sender, ExecutedRoutedEventArgs e)

    {

        FreemiumAboutBox.Instance.Show(e.Parameter, this);

    }
    private bool AskCancelPrintInProgressDialog() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void HandleToggleMultithreadingCalculationOption(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        global::Quire.Properties.Settings.Default.NumberOfThreads =

            BooleanToMultiThreadingValueConverter.Convert(

                !MultiThreadingValueToBooleanConverter.Convert(global::Quire.Properties.Settings.Default.NumberOfThreads));

        global::Quire.Properties.Settings.Default.Save();

    }
    private void UpdateWarningsDisplayFilterHandler(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        if (args.Parameter is global::Quire.Controls.Core.WarningsTypesToShow displayWarningsFilter)

        {

            DisplayWarningsFilter = displayWarningsFilter;

        }

        else

        {

            global::Quire.DebugTools.FailAssertion(null, global::System.Array.Empty<object>());

        }

    }
    private void ToggleShowWarningsHandler(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs args)

    {

        IsShowWarningsEnabled = !IsShowWarningsEnabled;

    }
    protected override global::System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()

    {

        return new global::Quire.Automation.SpiritMainWindowAutomationPeer(this);

    }
    protected override void OnClosing(global::System.ComponentModel.CancelEventArgs e) => base.OnClosing(e);
    private void SaveWindowState() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private bool AskToSaveDocument(global::Quire.Controls.Core.IEngineeringDocument doc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public global::Quire.FindUserProperties FindProperties => _findProperties;
    private bool HandleFindNext()

    {

        return Find(global::System.Windows.Documents.LogicalDirection.Forward);

    }
    private void HandleFindNext(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        HandleFindNext();

    }
    private void HandleFindPrev(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        Find(global::System.Windows.Documents.LogicalDirection.Backward);

    }
    private bool FindIsNotAllowedInFreemiumMode()

    {

        if (FindProperties.FindTextSubscriptPositions.IsNotEmpty())

        {

            return !FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.ToggleSubscript);

        }



        return false;

    }
    private void CanFind(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)

    {

        e.CanExecute = !FindIsNotAllowedInFreemiumMode()

            && !global::Quire.EnumerableExtensions.IsNullOrEmpty<char>(FindProperties.FindText);

    }
    private void HandleReplaceAndFindNext(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        var findReplaceQuery = new global::Quire.Controls.Core.FindReplaceQuery(

            FindProperties.FindText,

            FindProperties.ReplaceText,

            FindProperties.Options,

            FindProperties.FindTextSubscriptPositions,

            FindProperties.ReplaceTextSubscriptPositions)

        {

            Status = null

        };

        global::Quire.Controls.Core.RegionCommands.Replace.Execute(findReplaceQuery, null);

        global::Quire.DebugTools.DevAssert(

            findReplaceQuery.Status.GetValueOrDefault(out var result),

            null);

        if (!global::Quire.Controls.Core.FindReplaceStatusExtensions.IsAtDocumentBoundary(FindProperties.Status) ||

            !global::Quire.Controls.Core.FindReplaceStatusExtensions.IsEmpty(result))

        {

            FindProperties.Status = result;

        }



        bool foundNext = false;

        if (global::Quire.Controls.Core.FindReplaceStatusExtensions.IsEmpty(result) && e.Parameter as bool? != false)

        {

            foundNext = HandleFindNext();

        }



        findReplaceQuery.PostReplaceAndFindNext?.Invoke(foundNext);

    }
    private bool ReplaceIsNotAllowedInFreemiumMode()

    {

        if (FindProperties.FindTextSubscriptPositions.IsNotEmpty() || FindProperties.ReplaceTextSubscriptPositions.IsNotEmpty())

        {

            return !FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.ToggleSubscript);

        }



        return false;

    }
    private void CanReplaceAndFindNext(

        object sender,

        global::System.Windows.Input.CanExecuteRoutedEventArgs e)

    {

        var findReplaceQuery = new global::Quire.Controls.Core.FindReplaceQuery(

            FindProperties.FindText,

            FindProperties.ReplaceText,

            FindProperties.Options)

        {

            Status = null

        };

        e.CanExecute = !ReplaceIsNotAllowedInFreemiumMode() &&

                       global::Quire.Controls.Core.RegionCommands.Replace.CanExecute(findReplaceQuery, null);

        if (findReplaceQuery.Status.HasValue)

        {

            global::System.Windows.Controls.ToolTipService.SetShowOnDisabled(

                (global::System.Windows.DependencyObject)FindBar.ReplaceButton,

                !global::Quire.Controls.Core.FindReplaceStatusExtensions.IsEmpty(findReplaceQuery.Status));

            FindBar.ReplaceButtonTooltipTextBlock.Tag = findReplaceQuery.Status;

        }

    }
    private bool Find(global::System.Windows.Documents.LogicalDirection direction)

    {

        global::Quire.Controls.Core.FindReplaceStatus status = FindProperties.Status;

        FindProperties.Status = global::Quire.Controls.Core.FindReplaceStatus.Empty;

        bool found = false;

        if (!string.IsNullOrEmpty(FindProperties.FindText))

        {

            bool canContinue =

                (direction == global::System.Windows.Documents.LogicalDirection.Forward &&

                 status != global::Quire.Controls.Core.FindReplaceStatus.EndOfDocument) ||

                (direction == global::System.Windows.Documents.LogicalDirection.Backward &&

                 status != global::Quire.Controls.Core.FindReplaceStatus.BeginningOfDocument);

            found = ActiveDocument.Find(

                FindProperties.FindText,

                direction,

                canContinue,

                FindProperties.Options,

                FindProperties.FindTextSubscriptPositions);

            if (!found)

            {

                FindProperties.Status = canContinue

                    // Mathcad loads the Forward constant before direction for this comparison.

                    ? global::System.Windows.Documents.LogicalDirection.Forward == direction

                        ? global::Quire.Controls.Core.FindReplaceStatus.EndOfDocument

                        : global::Quire.Controls.Core.FindReplaceStatus.BeginningOfDocument

                    : global::Quire.Controls.Core.FindReplaceStatus.NotFound;

            }

        }



        return found;

    }
    private void HandleGoToFind(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        global::Quire.Wpf.WpfUtils.TryFindVisualAncestor<SpiritMainWindow>(

            ActiveDocument as global::System.Windows.DependencyObject)?.FindBar.FindRichBox.Focus();

    }
    private void HandleGoToPage(

        object sender,

        global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        global::Quire.Controls.Core.IEngineeringDocument activeDocument = ActiveDocument;

        var spiritMainWindow = global::Quire.Wpf.WpfUtils.TryFindVisualAncestor<SpiritMainWindow>(

            activeDocument as global::System.Windows.DependencyObject);

        new global::Quire.FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper(

                global::Quire.Controls.Worksheet.WorksheetCommands.EditCurrentGotoPage)

            .Execute(null, (global::Quire.Controls.Worksheet.WorksheetControl)spiritMainWindow.ActiveDocument.Worksheet);

    }
    [global::Quire.Mirror.MirrorStub]
    private void OnDictionaryUpdated() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void WorksheetPropertyChanged(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)

    {

        if (e.PropertyName == "IsHeaderActive" || e.PropertyName == "IsFooterActive")

        {

            FindProperties.Status = global::Quire.Controls.Core.FindReplaceStatus.Empty;

        }

    }
    private void OnActiveDocumentChanged(global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void OnActiveDocumentChanged(

        global::System.Windows.DependencyObject dependencyObject,

        global::System.Windows.DependencyPropertyChangedEventArgs e)

    {

        SpiritMainWindow window = dependencyObject as SpiritMainWindow;

        DebugTools.DevAssert(window != null, null);

        window.OnActiveDocumentChanged(e);

    }
    private static string GetStringParameter(global::System.Windows.Input.ExecutedRoutedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnIsKeyboardFocusWithinChanged(global::System.Windows.DependencyPropertyChangedEventArgs e) => base.OnIsKeyboardFocusWithinChanged(e);
    private void HandlePrintDocument(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)

    {

        ActiveEngineeringDoc.Print(AskCancelPrintInProgressDialog);

    }
    public string GetEngineVersion()

    {

        if (ActiveDocument != null)

        {

            return ActiveDocument.GetEngineVersion();

        }



        return string.Empty;

    }
    private void PrimeActivated(object sender, global::System.EventArgs e)

    {

        if (ActiveEngineeringDoc != null)

        {

            global::Quire.Controls.Core.DocumentAttachedProperties.SetDocumentIsActive(ActiveEngineeringDoc, activeState: true);

        }

    }
    private void PrimeDeactivated(object sender, global::System.EventArgs e)

    {

        if (ActiveEngineeringDoc != null)

        {

            global::Quire.Controls.Core.DocumentAttachedProperties.SetDocumentIsActive(ActiveEngineeringDoc, activeState: false);

        }

    }
    private static string GetMathcadPrimeInstallDirectoryLocalPath()

    {

        return FileUtils.GetAssemblyFolder(Assembly.GetExecutingAssembly());

    }
    private static string GetEngNbInstallDirectoryLocalPath()

    {

        try

        {

            string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            if (string.IsNullOrEmpty(version))

            {

                return string.Empty;

            }



            string keyName = "SOFTWARE\\PTC\\EngineeringNotebookHelp\\" + version;

            using RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);

            string installLocation = key?.GetValue("InstallLocation") as string;

            return string.IsNullOrEmpty(installLocation) ? string.Empty : installLocation;

        }

        catch (Exception)

        {

            return string.Empty;

        }

    }
    private void HideEngineNotification(object sender, global::System.Windows.RoutedEventArgs e)

    {

        ActiveDocument.ShowEngineChangesBanner = false;

    }
    private void SymbolicEngineChangeslinkClick(object sender, global::System.Windows.RoutedEventArgs e)

    {

        AppCommands.DisplayHelp.Execute("Ribbon.SymbolicEngineChangesBar.HyperlinkEngineChanges", this);

    }
    private void FirePropertyChanged(string propertyName)

    {

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
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
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::System.Collections.Generic.List<string>, global::System.Collections.Generic.List<string>> DocumentInputsOutputsSelected;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts> DocumentInputsOutputsStatesGeneratedWhenSave;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::Quire.Controls.Core.IORegions.IInputOutputItemsStates, global::Quire.Controls.IO.IInputOutputItemsConflicts> DocumentInputsOutputsStatesGeneratingBeforeSave;
    public event global::System.Action<global::Quire.Controls.Core.IEngineeringDocument, global::Quire.Controls.Core.IORegions.IValuesSetter> RequestToUpdateInputs;
    public event global::System.Action OptionsAboutDialogOpening;
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
}

