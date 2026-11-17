#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class PageBodyCanvas : Canvas, IComponentConnector
{
    public readonly IObservableRangeCollection<IWhiteboardItemZombie> Zombies = new ObservableRangeCollection<IWhiteboardItemZombie>();

    private ItemsControl _itemsControl;

    private bool _contentLoaded;

    public PageBodyCanvas()
    {
        InitializeComponent();
        _itemsControl.ItemsSource = Zombies;
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
        Uri resourceLocator = new Uri("/Quire.Controls.Worksheet;component/pagebodycanvas.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            _itemsControl = (ItemsControl)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
