namespace Quire.Controls.InputControls;

public partial interface IInputControlViewModel
{
    global::Quire.Controls.InputControls.IInputControlModel Model { get; }
    bool IsDirty { get; set; }
    bool IsEnabled { get; set; }
    bool IsInitializeComponent { get; set; }
    double FontSize { get; set; }
    bool IsFunctionalityEnabled { get; }
    bool HasKeyboard { get; }
    global::Quire.Calculation.Core.CalculatableState State { get; set; }
    void InitializeComponent();
}
