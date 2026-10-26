#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Quire.Controls.Core;

public partial class PrimeToolTip : ToolTip, IComponentConnector
{
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(PrimeToolTip));

    public static readonly DependencyProperty ContentBodyProperty = DependencyProperty.Register("ContentBody", typeof(object), typeof(PrimeToolTip));

    internal PrimeToolTip _this;

    private bool _contentLoaded;

    public object Header
    {
        get
        {
            return base.GetValue(HeaderProperty);
        }
        set
        {
            base.SetValue(HeaderProperty, value);
        }
    }

    public object ContentBody
    {
        get
        {
            return base.GetValue(ContentBodyProperty);
        }
        set
        {
            base.SetValue(ContentBodyProperty, value);
        }
    }

    public PrimeToolTip()
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
        Uri resourceLocator = new Uri("/Quire.Controls.Core;component/primetooltip.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            _this = (PrimeToolTip)target;
        }
        else
        {
            _contentLoaded = true;
        }
    }
}
