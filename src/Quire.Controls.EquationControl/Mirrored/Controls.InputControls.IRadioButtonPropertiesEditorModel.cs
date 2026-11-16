namespace Quire.Controls.InputControls;

public partial interface IRadioButtonPropertiesEditorModel
{
    global::System.Windows.Controls.Orientation Orientation { get; set; }
    global::System.Collections.ObjectModel.Collection<string> DataSourceItems { get; set; }
    global::System.Windows.FlowDirection FlowDirection { get; set; }
    bool IsDataSourceChanged { get; set; }
    global::System.Collections.ObjectModel.ObservableCollection<global::Quire.Controls.InputControls.DataListItem> EditorDataSourceItems { get; set; }
    void HandleButtonsChanged(global::System.Collections.ObjectModel.ObservableCollection<global::Quire.Controls.InputControls.DataListItem> editorDataSourceItems);
}
