#nullable disable
#pragma warning disable

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows;
using System;

namespace Quire.Controls.Plots.Contour.View;

public partial class ContourPlotControl
{

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/contour/view/contourplotcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _content = (ContourPlotCanvas)target;
                break;
            case 2:
                _background = (Rectangle)target;
                break;
            case 3:
                plotGrid = (Grid)target;
                break;
            case 4:
                _contourLines = (ContoursView)target;
                break;
            case 5:
                PART_ySplitter = (PlotGridSplitter)target;
                break;
            case 6:
                _yAxis = (PlotGraphLinearAxis)target;
                break;
            case 7:
                PART_xSplitter = (PlotGridSplitter)target;
                break;
            case 8:
                _xAxis = (PlotGraphLinearAxis)target;
                break;
            case 9:
                _zAxisStackPanel = (StackPanel)target;
                break;
            case 10:
                _zAxisColorBlocks = (ItemsControl)target;
                break;
            case 11:
                _zAxis = (PlotGraphLinearAxis)target;
                break;
            case 12:
                _zEquation = (PlotEquation)target;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
