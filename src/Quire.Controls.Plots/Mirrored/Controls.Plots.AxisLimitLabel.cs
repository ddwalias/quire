#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace Quire.Controls.Plots;

public partial class AxisLimitLabel
{
    public static readonly DependencyProperty LabelTypeProperty = DependencyProperty.Register("LabelType", typeof(AxisLimitLabelType), typeof(AxisLimitLabel));

    internal BeginStoryboard ShowLabelBlackEnlarged;

    private bool _contentLoaded;

    public AxisLimitLabelType LabelType
    {
        get
        {
            return (AxisLimitLabelType)GetValue(LabelTypeProperty);
        }
        set
        {
            SetValue(LabelTypeProperty, value);
        }
    }

    public AxisLimitLabel()
    {
        InitializeComponent();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/axislimitlabel.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            ShowLabelBlackEnlarged = (BeginStoryboard)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
