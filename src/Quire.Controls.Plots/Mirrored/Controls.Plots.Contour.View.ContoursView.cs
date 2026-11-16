#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Quire.Wpf;

namespace Quire.Controls.Plots.Contour.View;

internal partial class ContoursView
{
    private readonly PropertyBindingArray _itemsSourceBindings;
    private bool _contentLoaded;

    public ContoursView()
    {
        _itemsSourceBindings = new PropertyBindingArray(new PropertyBinding
        {
            TargetProperty = VisualDataCollection.HeightProperty,
            Binding = new Binding
            {
                Source = this,
                Path = new PropertyPath("ActualHeight")
            }
        }, new PropertyBinding
        {
            TargetProperty = VisualDataCollection.WidthProperty,
            Binding = new Binding
            {
                Source = this,
                Path = new PropertyPath("ActualWidth")
            }
        });
        InitializeComponent();
    }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        if (oldValue is DependencyObject)
        {
            _itemsSourceBindings.Clear(oldValue as DependencyObject);
        }
        if (newValue is DependencyObject)
        {
            _itemsSourceBindings.Set(newValue as DependencyObject);
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
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/contour/view/contoursview.xaml", UriKind.Relative);
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
