#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Quire.Core;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardCanvas : Canvas, IComponentConnector
{
    private bool _contentLoaded;

    public WhiteboardCanvas(IWhiteboardViewModel dataContext)
    {
        if (DebugTools.AssertNotNull(dataContext, null).Passed)
        {
            DataContext = dataContext;
        }
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
        Uri resourceLocator = new Uri("/Quire.Controls.Whiteboard;component/whiteboard/whiteboardcanvas.xaml", UriKind.Relative);
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
