namespace Quire.Controls.InputControls;

public partial interface ISliderPropertiesEditorModel
{
    global::System.Windows.Controls.Orientation Orientation { get; set; }
    global::System.Windows.Controls.Primitives.TickPlacement TickPlacement { get; set; }
    double TickFrequency { get; set; }
    bool Border { get; set; }
    double Minimum { get; set; }
    double Maximum { get; set; }
    bool ShowMinMax { get; set; }
    bool Invert { get; set; }
    double SmallChange { get; set; }
    double LargeChange { get; set; }
}
