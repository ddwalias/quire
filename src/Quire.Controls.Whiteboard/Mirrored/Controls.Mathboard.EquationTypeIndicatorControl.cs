#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Quire.Controls.Mathboard;

public partial class EquationTypeIndicatorControl : UserControl, IComponentConnector
{
    public static readonly DependencyProperty IndicatorTextProperty;
    public static readonly DependencyProperty TopProperty;
    internal Grid MainGrid;
    private bool _contentLoaded;

    public string IndicatorText
    {
        get => (string)GetValue(IndicatorTextProperty);
        set => SetValue(IndicatorTextProperty, value);
    }

    public double Top
    {
        get => (double)GetValue(TopProperty);
        set => SetValue(TopProperty, value);
    }

    public Color IndicatorColor { get; set; }

    public EquationTypeIndicatorControl()
    {
        InitializeComponent();
    }

    static EquationTypeIndicatorControl()
    {
        IndicatorTextProperty = DependencyProperty.Register("IndicatorText", typeof(string), typeof(EquationTypeIndicatorControl));
        TopProperty = Canvas.TopProperty.AddOwner(typeof(EquationTypeIndicatorControl));
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
        Uri resourceLocator = new Uri("/Quire.Controls.Whiteboard;component/mathboard/equationtypeindicatorcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            MainGrid = (Grid)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
