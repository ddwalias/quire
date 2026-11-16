namespace Quire.Controls.InputControls;

public partial interface IInputControlModel
{
    global::Quire.EquationEditor.Tree.InputControlEngineTypes EngineType { get; set; }
    string EngineError { get; }
    global::Quire.Controls.InputControls.IInputControlEngine InputEngine { get; }
    global::Quire.EquationEditor.Tree.IInputControlDataCalculatedValue Data { get; }
    global::System.Windows.Size OutputFieldSize { get; }
    bool IsDirty { get; set; }
    bool IsEnabled { get; set; }
    bool IsInitializeComponent { get; set; }
    double FontSize { get; set; }
    bool IsFunctionalityEnabled { get; }
    global::Quire.Calculation.Core.CalculatableState State { get; }
    void HandlePropertyChanged(string propertyName, object value);
}
