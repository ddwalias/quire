#nullable disable
#pragma warning disable

using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Plot3D.Tools;
using Quire.Controls.Plots.Plot3D.ViewModel;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots;
using Quire.PersistentDataObjects;
using Quire.Undo;
using Quire;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotControl3D
{
    public CameraType3D PlotCameraType
    {
        get
        {
            return (CameraType3D)GetValue(PlotCameraTypeProperty);
        }
        set
        {
            SetValue(PlotCameraTypeProperty, value);
        }
    }

    public string TraceSymbol
    {
        get
        {
            return (string)GetValue(TraceSymbolProperty);
        }
        set
        {
            SetValue(TraceSymbolProperty, value);
        }
    }

    public string TraceLineStyle
    {
        get
        {
            return (string)GetValue(TraceLineStyleProperty);
        }
        set
        {
            SetValue(TraceLineStyleProperty, value);
        }
    }

    public SolidColorBrush TraceColor
    {
        get
        {
            return (SolidColorBrush)GetValue(TraceColorProperty);
        }
        set
        {
            SetValue(TraceColorProperty, value);
        }
    }

    public double? TraceLineWeight
    {
        get
        {
            return (double?)GetValue(TraceLineWeightProperty);
        }
        set
        {
            SetValue(TraceLineWeightProperty, value);
        }
    }

    public SolidColorBrush TraceSurfaceFillColor
    {
        get
        {
            return (SolidColorBrush)GetValue(TraceSurfaceFillColorProperty);
        }
        set
        {
            SetValue(TraceSurfaceFillColorProperty, value);
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
        Uri resourceLocator = new Uri("/Quire.Controls.Plots;component/plot3d/view/plotcontrol3d.xaml", UriKind.Relative);
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
                _graphContainerFullSize = (Grid)target;
                break;
            case 2:
                _graphContainer = (Grid)target;
                break;
            case 3:
                _background = (Rectangle)target;
                break;
            case 4:
                _graph = (PlotGraph3D)target;
                break;
            case 5:
                _axesWidgetHost = (ContentControl)target;
                break;
            case 6:
                _graphContainerFullSizeComplementary = (Rectangle)target;
                _graphContainerFullSizeComplementary.SizeChanged += OnSizeChangedOfElementAffectingGraphWidth;
                break;
            case 7:
                _zEquationsList = (EquationList)target;
                break;
            case 8:
                _zEquationListComplementary = (Rectangle)target;
                _zEquationListComplementary.SizeChanged += OnSizeChangedOfElementAffectingGraphWidth;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }

    internal IBaseTrackball Trackball
    {
        get
        {
            return _trackball;
        }
        set
        {
            if (_trackball != null)
            {
                _trackball.Detach();
                _trackball.MouseUp -= ViewModel.TrackballMouseUp;
                _trackball.MouseWheel -= ViewModel.TrackballMouseWheel;
            }
            _trackball = value;
            if (_trackball != null)
            {
                _trackball.Attach(_graphContainer, ViewModel, _graph.Viewport);
                _trackball.MouseUp += ViewModel.TrackballMouseUp;
                _trackball.MouseWheel += ViewModel.TrackballMouseWheel;
            }
        }
    }

    internal PlotAxesWidget3D AxesWidget
    {
        get
        {
            return AxesWidgetHost.Content as PlotAxesWidget3D;
        }
        private set
        {
            using (new CommandScope(new CompoundCommand()))
            {
                AxesWidgetHost.Content = value;
            }
        }
    }

    public string GraphImageFileNameForPrintXamlSerialization
    {
        get
        {
            string result = null;
            if (_graph.IsInPrinting && _graph.CanPrint)
            {
                bool isActiveRegion = IsActiveRegion;
                try
                {
                    if (isActiveRegion)
                    {
                        IsActiveRegionChanged(false);
                    }
                    result = _graph.ImageFileNameForPrintXamlSerialization;
                }
                catch (Exception ex)
                {
                    DebugTools.FailAssertion(ex.Message);
                }
                finally
                {
                    if (isActiveRegion)
                    {
                        IsActiveRegionChanged(true);
                    }
                }
            }
            return result;
        }
        set
        {
            if (value != null)
            {
                _graph.ImageFileNameForPrintXamlSerialization = value;
            }
        }
    }

    public RegionBorder RegionBorder { get; private set; }

    public CameraControlState CameraControlState => ViewModel.CameraControlState;

    private ContentControl AxesWidgetHost => _axesWidgetHost;

    public PlotViewModel3D ViewModel { get; private set; }

    public override bool HideFromCalculationContext
    {
        get
        {
            return ViewModel.HideFromCalculationContext;
        }
        set
        {
            ViewModel.HideFromCalculationContext = value;
        }
    }

    protected override string ErrorMessage
    {
        get
        {
            string result = null;
            IPlotEquation activePlotEquation = ActivePlotEquation;
            if (activePlotEquation != null)
            {
                result = GetErrorMessage(activePlotEquation.PlotCalculatableError);
            }
            return result;
        }
    }

    private PlotGraphAxis AxesWidgetZAxis => AxesWidget.ZAxis;

    private PlotGraphAxis AxesWidgetYAxis => AxesWidget.YAxis;

    private PlotGraphAxis AxesWidgetXAxis => AxesWidget.XAxis;

    public override Size PlotGraphSize
    {
        get
        {
            return SubElementToResizeData.Element.RenderSize;
        }
    }

    internal Size PlotGraphViewportSize
    {
        get
        {
            return Graph.Viewport.RenderSize;
        }
    }

    public override PlotType PlotType
    {
        get
        {
            return PlotType.Plot3D;
        }
        protected set
        {
            DebugTools.DevAssert(value == PlotType.Plot3D, null);
        }
    }

    internal bool IsCameraControlWidgetVisible => _cameraControlWidget != null;

    public bool BoxVisibility
    {
        get
        {
            return _graph.BoxVisibility;
        }
        private set
        {
            _graph.BoxVisibility = value;
        }
    }

    internal EquationList ZEquationsList => _zEquationsList;

    protected override IWhiteboardItem FirstChildItem => ZEquationsList[0].ExpressionItem;

    protected override IWhiteboardItem LastChildItem => ZEquationsList[ZEquationsList.EquationCount - 1].ScalingItem;

    public SubElementToResizeData SubElementToResizeData => new SubElementToResizeData(_graphContainerFullSize, serializeDimensions: false, isFullSize: false);

    public bool PreserveAspectRatio => false;

    public plotType XamlDataObject
    {
        get
        {
            return ViewModel.XamlDataObject;
        }
        set
        {
            ViewModel.XamlDataObject = value;
        }
    }

    internal bool IsAxisSelectorVisible => AxesWidgetHost.IsVisible;

    public IEnumerable<ICalculatableResultOwner> ResultOwners => ViewModel.CollectHiddenAndVisibleCalculatables().OfType<ICalculatableResultOwner>();

    internal PlotGraph3D Graph => _graph;

    public int TraceCount => _graph.TraceViewsCount;

    protected override bool ErrorFrameShouldBeVisible
    {
        get
        {
            if (!base.ErrorFrameShouldBeVisible)
            {
                return ViewModel.ShowControlLevelErrorFrameForNestedError;
            }
            return true;
        }
    }

    public override int NumberOfPointsMin => 11;

    public override int NumberOfPointsMax => 101;
}
