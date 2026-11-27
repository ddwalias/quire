#nullable disable
#pragma warning disable

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using Quire.Properties;

namespace Quire;

public partial class OptionsWindow
{
    public static readonly RoutedUICommand BrowseForTemplatesFolder = new RoutedUICommand();
    public static readonly RoutedUICommand ChooseNewDefaultTemplate = new RoutedUICommand();
    public static readonly RoutedUICommand InsertPythonDirPath = new RoutedUICommand();

    internal global::Quire.License.LicenseOptionsPage LicenseOptionsPage;

    public OptionsWindow(SpiritMainWindow spiritMainWindow)
    {
        _spiritMainWindow = spiritMainWindow;
        CommandBindings.Add(new CommandBinding(BrowseForTemplatesFolder, HandleBrowseForTemplatesFolder));
        CommandBindings.Add(new CommandBinding(ChooseNewDefaultTemplate, HandleChooseNewDefaultTemplate));
        CommandBindings.Add(new CommandBinding(InsertPythonDirPath, HandleBrowseForPythonDirPath));
        InitializeComponent();
        Owner = LicenseOptionsPage.ParentOwner = _spiritMainWindow;
    }

    private void HandleChooseNewDefaultTemplate(object sender, ExecutedRoutedEventArgs e)
    {
        string text;
        string title = TryFindResource("Application.DefaultTemplateFileOpenTitle") as string;
        string pathToDefaultTemplateForNewDocument = _spiritMainWindow.GetPathToDefaultTemplateForNewDocument();
        string templatesFolderLocation;
        try
        {
            templatesFolderLocation = Path.GetDirectoryName(pathToDefaultTemplateForNewDocument);
        }
        catch
        {
            templatesFolderLocation = Directory.Exists(Settings.Default.CWD) ? Settings.Default.CWD : Directory.GetCurrentDirectory();
        }
        text = _spiritMainWindow.LaunchOpenFileDialogAndGetFileNameForTemplates(templatesFolderLocation, overrideDialogTitle: false, title);
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        _spiritMainWindow.SetPathToDefaultTemplateForNewDocument(text);
    }

    private void HandleBrowseForTemplatesFolder(object sender, ExecutedRoutedEventArgs e)
    {
        string text = e.Parameter as string;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        using global::System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new global::System.Windows.Forms.FolderBrowserDialog();
        folderBrowserDialog.SelectedPath = _spiritMainWindow.IsGivenKeyMyTemplatesKey(text) ? _spiritMainWindow.GetMyTemplatesLocation() : _spiritMainWindow.GetSharedTemplatesLocation();
        folderBrowserDialog.ShowNewFolderButton = true;
        if (folderBrowserDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
        {
            if (_spiritMainWindow.IsGivenKeyMyTemplatesKey(text))
            {
                _spiritMainWindow.SetMyTemplatesLocation(folderBrowserDialog.SelectedPath);
            }
            else
            {
                _spiritMainWindow.SetSharedTemplatesLocation(folderBrowserDialog.SelectedPath);
            }
        }
    }

    private void HandleBrowseForPythonDirPath(object sender, ExecutedRoutedEventArgs e)
    {
        using global::System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new global::System.Windows.Forms.FolderBrowserDialog();
        if (folderBrowserDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
        {
            _spiritMainWindow.SetPythonDirectoryPath(folderBrowserDialog.SelectedPath);
        }
    }

    private void RestCustomPythonDirPath(object sender, RoutedEventArgs e)
    {
        _spiritMainWindow.SetPythonDirectoryPath(string.Empty);
        e.Handled = true;
    }

    public bool? ShowDialog(DialogTabIndex tab)
    {
        dialogTabControl.SelectedIndex = (int)tab;
        return ShowDialog();
    }

    protected override void OnClosed(EventArgs e)
    {
        Settings.Default.Save();
        base.OnClosed(e);
    }

    private void RestPathToDefaultTemplateForNewDocument(object sender, RoutedEventArgs e)
    {
        _spiritMainWindow.SetPathToDefaultTemplateForNewDocument(_spiritMainWindow.DefaultPathToDefaultTemplateForNewDocument);
        e.Handled = true;
    }

    private void ResetCustomSharedTemplatesLocation(object sender, RoutedEventArgs e)
    {
        _spiritMainWindow.SetSharedTemplatesLocation(_spiritMainWindow.DefaultSharedTemplatesLocation);
        e.Handled = true;
    }

    private void RestCustomMyTemplatesLocation(object sender, RoutedEventArgs e)
    {
        _spiritMainWindow.SetMyTemplatesLocation(_spiritMainWindow.DefaultMyTemplatesLocation);
        e.Handled = true;
    }

    public string GetHelpContext()
    {
        return (dialogTabControl.Items[dialogTabControl.SelectedIndex] as UIElement)?.Uid;
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/optionswindow.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                options = (global::Quire.OptionsWindow)target;
                return;
            case 2:
                dialogTabControl = (global::System.Windows.Controls.TabControl)target;
                return;
            case 3:
                LicenseOptionsPage = (global::Quire.License.LicenseOptionsPage)target;
                return;
            case 4:
                RemoteHelpCheckBox = (global::System.Windows.Controls.CheckBox)target;
                return;
            case 5:
                OptOutQualityAgentCheckBox = (global::System.Windows.Controls.CheckBox)target;
                return;
            case 6:
                CustomMyTemplatesLocationCheckBox = (global::System.Windows.Controls.CheckBox)target;
                CustomMyTemplatesLocationCheckBox.Unchecked += RestCustomMyTemplatesLocation;
                return;
            case 7:
                CustomMyTemplatesLocationTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
            case 8:
                CustomSharedTemplatesLocationCheckBox = (global::System.Windows.Controls.CheckBox)target;
                CustomSharedTemplatesLocationCheckBox.Unchecked += ResetCustomSharedTemplatesLocation;
                return;
            case 9:
                CustomSharedTemplatesLocationTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
            case 10:
                CustomPathToDefaultTemplateForNewDocumentCheckBox = (global::System.Windows.Controls.CheckBox)target;
                CustomPathToDefaultTemplateForNewDocumentCheckBox.Unchecked += RestPathToDefaultTemplateForNewDocument;
                return;
            case 11:
                CustomPathToDefaultTemplateForNewDocumentTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
            case 12:
                DisableScriptableControls = (global::System.Windows.Controls.CheckBox)target;
                return;
            case 13:
                ScriptingRunWorksheetOpenCheckBox = (global::System.Windows.Controls.CheckBox)target;
                return;
            case 14:
                ScriptingRunWorksheetRecalculateCheckBox = (global::System.Windows.Controls.CheckBox)target;
                return;
            case 15:
                CustomPathToPythonDirCheckBox = (global::System.Windows.Controls.CheckBox)target;
                CustomPathToPythonDirCheckBox.Unchecked += RestCustomPythonDirPath;
                return;
            case 16:
                CustomPythonDirLocationTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
        }
        _contentLoaded = true;
    }
}
