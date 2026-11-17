#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Quire.Controls.Worksheet;

public partial class SwitchToDraftModeButton : Button, IComponentConnector
{
    private SwitchToDraftModeButtonAdorner _adorner;

    private bool _contentLoaded;

    public SwitchToDraftModeButton()
    {
        InitializeComponent();
        Loaded += SwitchToDraftModeButtonLoaded;
        Unloaded += SwitchToDraftModeButtonUnloaded;
    }

    private void SwitchToDraftModeButtonLoaded(object sender, RoutedEventArgs e)
    {
        FrameworkElement frameworkElement = sender as FrameworkElement;
        if (frameworkElement == null)
        {
            return;
        }
        _adorner = new SwitchToDraftModeButtonAdorner(frameworkElement);
    }

    private void SwitchToDraftModeButtonUnloaded(object sender, RoutedEventArgs e)
    {
        if (_adorner != null)
        {
            _adorner.Remove();
        }
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
        Uri resourceLocator = new Uri("/Quire.Controls.Worksheet;component/switchtodraftmodebutton.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
