namespace Quire.Controls.InputControls;

public partial interface IInputControlBase
{
    global::System.Windows.Media.Brush Background { get; set; }
    bool IsVisible { get; }
    bool IsHitTestVisible { get; set; }
    bool IsFocused { get; }
    new bool IsEnabled { get; set; }
    global::System.Windows.Size RenderSize { get; }
    event global::System.Windows.DependencyPropertyChangedEventHandler IsVisibleChanged;
}
