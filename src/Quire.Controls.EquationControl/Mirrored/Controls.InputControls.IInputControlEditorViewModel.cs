using System.Windows.Input;

namespace Quire.Controls.InputControls;

public partial interface IInputControlEditorViewModel
{
    string InputScript { get; set; }
    string ValidationError { get; set; }
    InputControlScriptingLanguageTypes SelectedScriptingLanguage { get; }
    bool IsApplyEnabled { get; set; }
    bool IsApplyVisibility { get; }
    bool IsInfoMsgHide { get; set; }
    bool CanRedo { get; set; }
    bool CanUndo { get; set; }
    bool IsEnabled { get; }
    bool AutoRecalculate { get; set; }
    bool IsReadOnlyScript { get; }
    void ApplyScript();
    void CancelScript();
    void OnPreviewKeyDown(object sender, InputEventArgs e);
}
