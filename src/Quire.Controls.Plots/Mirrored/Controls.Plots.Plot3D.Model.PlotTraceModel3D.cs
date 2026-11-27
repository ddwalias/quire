#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Media3D;
using Quire.Controls.Plots.Presenter;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class PlotTraceModel3D
{
    public PlotTraceVisualData3D VisualData
    {
        get
        {
            return _visualData;
        }
        private set
        {
            if (_visualData != value)
            {
                _visualData = value;
                FirePropertyChanged("VisualData");
            }
        }
    }

    public TraceType3D VisualDataTypeOrDefault
    {
        get
        {
            if (VisualData == null)
            {
                return TraceType3D.Undefined;
            }
            return VisualData.TraceType;
        }
    }

    public Rect3D CommonRange
    {
        get
        {
            return _commonRange;
        }
        set
        {
            bool flag = VisualDataTypeOrDefault == TraceType3D.Undefined;
            if (_commonRange != value)
            {
                _commonRange = value;
                FirePropertyChanged("CommonRange");
                flag = true;
            }
            if (flag)
            {
                UpdateVisualData();
            }
        }
    }

    public Rect3D TraceBounds
    {
        get
        {
            return _traceBounds;
        }
        private set
        {
            if (value != _traceBounds)
            {
                _traceBounds = value;
                FirePropertyChanged("TraceBounds");
            }
        }
    }

    public IPlotEquationModel3D TraceEquation => _traceEquation;

    public double LineWeight
    {
        get
        {
            return _lineWeight;
        }
        set
        {
            _lineWeight = value;
            FirePropertyChanged("LineWeight");
        }
    }

    public bool IsStyleAutomatic
    {
        get
        {
            return _isStyleAutomatic;
        }
        private set
        {
            _isStyleAutomatic = value;
            FirePropertyChanged("IsStyleAutomatic");
        }
    }

    public LineStyle LineStyle
    {
        get
        {
            return _lineStyle;
        }
        set
        {
            if (_lineStyle == value)
            {
                return;
            }
            _lineStyle = value;
            FirePropertyChanged("LineStyle");
        }
    }

    public TraceSymbol Symbol
    {
        get
        {
            return _symbol;
        }
        set
        {
            if (_symbol == value)
            {
                return;
            }
            _symbol = value;
            FirePropertyChanged("Symbol");
        }
    }

    public Color? SurfaceFill
    {
        get
        {
            return _surfaceFill;
        }
        set
        {
            if (_surfaceFill == value)
            {
                return;
            }
            _surfaceFill = value;
            FirePropertyChanged("SurfaceFill");
        }
    }

    public bool AxesHaveMinMaxErrors
    {
        private get
        {
            return _axesHaveMinMaxErrors;
        }
        set
        {
            if (_axesHaveMinMaxErrors != value)
            {
                _axesHaveMinMaxErrors = value;
                if (_axesHaveMinMaxErrors)
                {
                    UpdateVisualData();
                }
            }
        }
    }

    public UserDefinedLimits UserDefinedLimits { get; private set; }

    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            if (_color != value)
            {
                _color = value;
                FirePropertyChanged("Color");
            }
        }
    }

    public Color RenderColor
    {
        get
        {
            return _renderColor;
        }
        private set
        {
            if (_renderColor != value)
            {
                _renderColor = value;
                FirePropertyChanged("RenderColor");
            }
        }
    }

    public bool IsInPrinting { get; set; }

    internal TraceType3D TraceType
    {
        get
        {
            if (EngineResultParser == null)
            {
                return TraceType3D.Undefined;
            }
            return EngineResultParser.TraceType;
        }
    }

    private string ScalingRl => GetReplacedRl(_traceEquation.EquationProxy.ScalingEquation, _traceEquation.EquationProxy.ScalingRl);

    protected string NumberOfNodesRl => VectorFormatHelper.VectorRl(NumberOfNodesX, NumberOfNodesY);

    internal double NumberOfNodesY => NumberOfPoints;

    internal double NumberOfNodesX => NumberOfPoints;

    internal Func<double, double, double, bool> Contains
    {
        get
        {
            return EngineResultParser.Contains;
        }
        set
        {
            EngineResultParser.Contains = value;
        }
    }

    public override bool IsCommitted => _traceEquation.IsCommitted;

    public double SymbolWeight { get; set; }

    public EngineResult EngineResultForSerialization
    {
        get
        {
            if (!IsInPrinting)
            {
                return base.EngineResult;
            }
            return GetPrintResults(base.EngineResult);
        }
    }

    private void AttachCleanErrorsToExpressionValueChanged()
    {
        if (_traceEquation.EquationProxy != null)
        {
            AttachOnTraceEquationSExpressionChanged(_traceEquation.EquationProxy);
        }
    }

    private void DetachCleanErrorsFromExpressionValueChanged()
    {
        if (_traceEquation.EquationProxy != null)
        {
            DetachOnTraceEquationSExpressionChanged(_traceEquation.EquationProxy);
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case "Color":
            case "CalculatingStatus":
            case "SynchronizationStatus":
                UpdateRenderColor();
                break;
        }
    }

    public void ResetOnRemoval()
    {
        SetSynchronizationStatus(SynchronizationStatus.Synchronized);
        SetCalculatingStatus(CalculatingStatus.Idle);
        EngineResultParser = EngineTrace3DResultParser.CreateUnknownParser();
    }

    public bool ChangeSymbol(TraceSymbol value)
    {
        Symbol = value;
        return Symbol == value;
    }

    public bool ChangeLineStyle(LineStyle value)
    {
        LineStyle = value;
        return LineStyle == value;
    }

    public TraceStyleState? DoStyleChangeCommand()
    {
        return null;
    }

    public void UndoStyleChangeCommand(TraceStyleState? doData)
    {
        DebugTools.DevAssert(!doData.HasValue, null);
    }

    private void UpdateRenderColor()
    {
        RenderColor = ((CalculatingStatus == CalculatingStatus.Calculating || SynchronizationStatus == SynchronizationStatus.Pending || !IsCommitted) ? _disabledColor : Color);
    }

    public override void Commit()
    {
        _traceEquation.Commit();
    }

    public override void SetResult(IResultNode result)
    {
        EngineResultParser = CreateParser(result);
    }

    internal virtual IEngineTrace3DResultParser CreateParser(IResultNode result)
    {
        if (!(result is IUnitedValueResultNode))
        {
            return EngineTrace3DResultParser.CreateParser(result);
        }
        return EngineTrace3DResultParser.CreatePrintParser(result);
    }

    public void UpdateVisualData()
    {
        if (AxesHaveMinMaxErrors)
        {
            if (VisualData == null || VisualData.TraceType != TraceType3D.Undefined)
            {
                VisualData = PlotTraceVisualData3D.Undefined;
                return;
            }
        }
        else
        {
            VisualData = EngineResultParser.CreateVisualData(TransformForVisual(CommonRange));
            EngineError engineError;
            if (VisualData.HasNoData)
            {
                engineError = new EngineError
                {
                    Message = NoDataFound
                };
            }
            else if (EngineResultParser.Error != null)
            {
                engineError = EngineResultParser.Error;
            }
            else
            {
                engineError = Error;
                if (engineError != null && FindChildWithMatchingId(engineError.NodeHandle) != null)
                {
                    engineError = null;
                }
            }
            OnPlotCalculatableError(new EngineErrorEventArgs(engineError));
        }
    }

    private void UpdateStyle()
    {
        if (IsStyleAutomatic)
        {
            if (EngineResultParser.EngineTraceType == EngineTraceType3D.ScatteredData)
            {
                LineStyle = LineStyle.None;
                Symbol = TraceSymbol.point;
            }
            else
            {
                LineStyle = LineStyle.Solid;
                Symbol = TraceSymbol.none;
            }
        }
    }

    public void SetUserDefinedLimits(double xStart, double xEnd, double yStart, double yEnd, double zStart, double zEnd, out bool needsCommit)
    {
        UserDefinedLimits.Set(xStart, xEnd, yStart, yEnd, zStart, zEnd, out var xLimitsChanged, out var yLimitsChanged, out var zLimitsChanged);
        EngineTraceType3D engineTraceType = EngineResultParser.EngineTraceType;
        needsCommit = (xLimitsChanged | yLimitsChanged) && (engineTraceType == EngineTraceType3D.NonParametricSurface || engineTraceType == EngineTraceType3D.Unknown);
    }

    private static Transform3D TransformForVisual(Rect3D bounds)
    {
        TranslateTransform3D value = new TranslateTransform3D(-Tools3D.BoxCenter(bounds));
        ScaleTransform3D scaleTransform = new ScaleTransform3D(Tools3D.InverseVector((Vector3D)bounds.Size));
        Transform3DGroup transform3DGroup = new Transform3DGroup
        {
            Children =
            {
                (Transform3D)value,
                (Transform3D)scaleTransform,
                (Transform3D)PlotCommonConstants.Plot3DLogicalToVisualTransform
            }
        };
        ((Freezable)transform3DGroup).Freeze();
        return transform3DGroup;
    }

    public void UserChangedStyle()
    {
        IsStyleAutomatic = EngineResultParser.EngineTraceType == EngineTraceType3D.ScatteredData ? LineStyle == LineStyle.None && Symbol == TraceSymbol.point : LineStyle == LineStyle.Solid && Symbol == TraceSymbol.none;
    }
}
