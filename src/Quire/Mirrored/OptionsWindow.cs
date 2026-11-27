#nullable disable
#pragma warning disable

namespace Quire;

public partial class OptionsWindow
{
    internal global::Quire.License.LicenseOptionsPage LicenseOptionsPage;

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
                ((global::System.Windows.Controls.CheckBox)target).Unchecked += RestCustomMyTemplatesLocation;
                return;
            case 7:
                CustomMyTemplatesLocationTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
            case 8:
                ((global::System.Windows.Controls.CheckBox)target).Unchecked += ResetCustomSharedTemplatesLocation;
                return;
            case 9:
                CustomSharedTemplatesLocationTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
            case 10:
                ((global::System.Windows.Controls.CheckBox)target).Unchecked += RestPathToDefaultTemplateForNewDocument;
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
                ((global::System.Windows.Controls.CheckBox)target).Unchecked += RestCustomPythonDirPath;
                return;
            case 16:
                CustomPythonDirLocationTextBox = (global::System.Windows.Controls.TextBox)target;
                return;
        }
        _contentLoaded = true;
    }
}
