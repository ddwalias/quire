using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class SliderControl
{
    internal SliderControl Slider;

    private bool _contentLoaded;

    public SliderControl(IInputControlDataCalculatedValue data)
    {
        DataContext = new SliderViewModel(data);
        InitializeComponent();
        ((SliderViewModel)DataContext).InvertSliderChanged += OnInvalidateVisuals;
    }

    public IInputControlViewModel ViewModel => (SliderViewModel)DataContext;

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseRightButtonDown(e);
        e.Handled = true;
    }

    private void OnInvalidateVisuals(object sender, EventArgs e)
    {
        if (Template.FindName("PART_Track", this) is Track track)
        {
            track.InvalidateVisual();
        }
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri(
            "/Quire.Controls.EquationControl;component/inputcontrols/implementations/scriptableslider/slidercontrol.xaml",
            UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            Slider = (SliderControl)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
