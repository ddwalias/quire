#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
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

    internal void SetContextDataEngNote(string[] contextData)
    {
        global::Quire.Controls.Help.EngNoteContextData.Parse(contextData);
        _helpManager.EngNbLocalHelpEnvironmentVariableName = global::Quire.Controls.Help.EngNoteContextData.HelpEnvVarName;
    }

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

    private void DisplayHelp(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs args)
    {
        string stringParameter = GetStringParameter(args);
        global::System.Windows.UIElement sourceElement = args.Source as global::System.Windows.UIElement;
        global::Quire.Controls.Core.IWhiteboardItem activeItem = ActiveEngineeringDoc.Worksheet.ActiveItem;
        string url = _helpManager.ConstructTopicPath(stringParameter, activeItem, sourceElement);
        OpenUrl(url, useBrowser: true);
    }

    private void HandleWindowUnloaded()
    {
        UnhookWndProc();
    }

    private void OnFunctionalityServiceLicenseTypeChanged()
    {
        OnFunctionalityServiceLicenseTypeChanged(null, global::System.EventArgs.Empty);
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

    private void HandleDocumentInputsOutputsSelected(
        global::Quire.Controls.Core.IEngineeringDocument doc,
        global::System.Collections.Generic.List<string> selectedInputs,
        global::System.Collections.Generic.List<string> selectedOutputs)
    {
        DocumentInputsOutputsSelected(doc, selectedInputs, selectedOutputs);
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

    private void HideEngineNotification(object sender, global::System.Windows.RoutedEventArgs e)
    {
        ActiveDocument.ShowEngineChangesBanner = false;
    }

    private void SymbolicEngineChangeslinkClick(object sender, global::System.Windows.RoutedEventArgs e)
    {
        AppCommands.DisplayHelp.Execute("Ribbon.SymbolicEngineChangesBar.HyperlinkEngineChanges", this);
    }

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
}
