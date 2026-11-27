#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
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

    internal void UpdateInputsEngNote(object sender)
    {
        UpdateInputs(sender, null);
    }

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

    internal bool HandleCloseEngineeringDocument(global::Quire.Controls.Core.IEngineeringDocument doc)
    {
        return CloseDocument(doc);
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

    private bool IsDirectoryOneOfDefaultTemplateLocations(string location)
    {
        if (!string.Equals(location, DefaultMyTemplatesLocation, global::System.StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(location, DefaultSharedTemplatesLocation, global::System.StringComparison.OrdinalIgnoreCase);
        }
        return true;
    }

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
}
