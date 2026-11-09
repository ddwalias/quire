using System;

namespace Quire.Controls.InputControls;

public partial class RadioButtonEventAggregator
{
    public event EventHandler<RadioButtonCheckedChangedEventArgs> IsCheckedChanged;

    public void PublishIsCheckedChanged(RadioButtonViewModel sourceViewModel, bool isChecked)
    {
        IsCheckedChanged?.Invoke(this, new RadioButtonCheckedChangedEventArgs(sourceViewModel, isChecked));
    }

    public RadioButtonEventAggregator()
    {
    }
}
