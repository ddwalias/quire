namespace Quire.Controls.InputControls;

public partial class RadioButtonCheckedChangedEventArgs
{
    public RadioButtonViewModel SourceViewModel { get; }

    public bool IsChecked { get; }

    public RadioButtonCheckedChangedEventArgs(RadioButtonViewModel sourceViewModel, bool isChecked)
    {
        SourceViewModel = sourceViewModel;
        IsChecked = isChecked;
    }
}
