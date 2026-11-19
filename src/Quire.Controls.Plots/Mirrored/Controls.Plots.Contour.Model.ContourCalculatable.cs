using System;
using System.Globalization;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Contour.Model;

internal sealed partial class ContourCalculatable : PlotCalculatable
{
    public new sealed partial class PropertyNames : PlotCalculatable.PropertyNames
    {
        public const string Result = "Result";

        private PropertyNames()
        {
        }
    }

    internal const string RlStringFormat = "(eval (numeric (contour_plot  {0} {1} {2} {3} {4}))  )";
    private const string RlLimitsFormat = "(op_matrix (unboxed 4) (unboxed 1) (list (number {0}) (number {1}) (number {2}) (number {3})))";
    private const string RlGridSizeFormat = "(op_matrix (unboxed 2) (unboxed 1) (list (number {0}) (number {1})))";
    private const double RangeMinValueDefault = double.NaN;
    private const double RangeMaxValueDefault = double.NaN;
    private const int ComputationalGridRowsDefault = 40;
    private const int ComputationalGridColumnsDefault = 40;

    private IPlotEquation _plotEquation;
    private ContourCalculationResult _result;
    private Levels _levels;

    public ContourCalculatable()
    {
        XMin = double.NaN;
        XMax = double.NaN;
        YMin = double.NaN;
        YMax = double.NaN;
    }

    internal IPlotEquation PlotEquation
    {
        get => _plotEquation;
        set
        {
            DebugTools.DevAssert(_plotEquation == null, "Set of ContourCalculatable.PlotEquation is not supposed to be called more then once.");
            _plotEquation = value;
            _plotEquation.RequestEvaluation += FireRequestEvaluation;
            AttachOnTraceEquationSExpressionChanged(_plotEquation);
        }
    }

    public override string Rl
    {
        get
        {
            ClearNodeHandleMapper();
            return string.Format(RlStringFormat, GetReplacedRl(PlotEquation.Expression), ScalingRl, GridRL, LimitsRL, Levels.RL);
        }
    }

    private string GridRL => string.Format(CultureInfo.InvariantCulture, RlGridSizeFormat, ComputationalGridRowsDefault, ComputationalGridColumnsDefault);

    private string LimitsRL => string.Format(CultureInfo.InvariantCulture, RlLimitsFormat, XMin, XMax, YMin, YMax);

    private string ScalingRl => GetReplacedRl(PlotEquation.ScalingEquation, PlotEquation.ScalingRl);

    private double XMin { get; set; }

    private double XMax { get; set; }

    private double YMin { get; set; }

    private double YMax { get; set; }

    public new ContourCalculationResult Result
    {
        get => _result;
        private set
        {
            _result = value;
            FirePropertyChanged("Result");
        }
    }

    public override bool IsCommitted
    {
        get
        {
            if (PlotEquation != null)
            {
                return PlotEquation.IsCommitted;
            }
            return false;
        }
    }

    internal Levels Levels
    {
        get => _levels;
        set
        {
            _levels = value;
            Commit();
        }
    }

    public void SetHorizontalRange(double min, double max)
    {
        if (!XMax.AreSame(max) || !XMin.AreSame(min))
        {
            InitializeHorizontalRange(min, max);
            Commit();
        }
    }

    private void InitializeHorizontalRange(double min, double max)
    {
        XMax = max;
        XMin = min;
    }

    public void SetVerticalRange(double min, double max)
    {
        if (!YMax.AreSame(max) || !YMin.AreSame(min))
        {
            InitializeVerticalRange(min, max);
            Commit();
        }
    }

    private void InitializeVerticalRange(double min, double max)
    {
        YMax = max;
        YMin = min;
    }

    public override void Commit()
    {
        if (PlotEquation != null)
        {
            PlotEquation.Commit();
        }
    }

    public override void SetResult(IResultNode result)
    {
        if (!(result is EmptySimpleResultNode))
        {
            try
            {
                Result = new ContourCalculationResult(result as IMatrixResultNode);
                PlotEquation.Units = Result.Units;
            }
            catch (PlotInvalidResultException ex)
            {
                OnPlotCalculatableError(new EngineErrorEventArgs(new EngineError
                {
                    Message = ex.Message
                }));
            }
        }
    }
}
