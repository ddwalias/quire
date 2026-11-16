using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Model;

public partial class AxisModelDefinitionCollection
{
    public partial class Constants
    {
        public const double LinearAxisStartDefault = -10.0;

        public const double LinearAxisEndDefault = 10.0;

        internal const int LinearAxisMaxTickmarks = 100;

        internal const int LinearAxisTickMarksDefault = 11;

        internal const int LinearAxisDefaultTickMagnitudeThreashold = 2;

        public const double RadialAxisStartDefault = 0.0;

        public const double RadialAxisEndDefault = 1.0;

        internal const int RadialAxisMaxTickmarks = 50;

        internal const int RadialAxisTickMarksDefault = 5;

        internal const int RadialAxisDefaultTickMagnitudeThreashold = 4;

        public const double AngularAxisStartDefault = 0.0;

        public const double AngularAxisEndDefault = 360.0;

        internal const int AngularAxisMaxTickmarks = 60;

        internal const int AngularAxisTickMarksDefault = 13;

        internal const int AxisTickMarksDefault3D = 5;

        public Constants()
        {
        }
    }

    public partial class AxisModelDefinition
    {
        private readonly double _startDefault;

        private readonly double _endDefault;

        private readonly int _maxTickmarks;

        private readonly int _tickMarksDefault;

        private readonly double _defaultTickMagnitudeThreashold;

        public double StartDefault => _startDefault;

        public double EndDefault => _endDefault;

        public int MaxTickmarks => _maxTickmarks;

        public int TickMarksDefault => _tickMarksDefault;

        public double DefaultTickMagnitudeThreashold => _defaultTickMagnitudeThreashold;

        public AxisModelDefinition(double startDefault, double endDefault, int maxTickmarks, int tickMarksDefault, double defaultTickMagnitudeThreashold)
        {
            _startDefault = startDefault;
            _endDefault = endDefault;
            _maxTickmarks = maxTickmarks;
            _tickMarksDefault = tickMarksDefault;
            _defaultTickMagnitudeThreashold = defaultTickMagnitudeThreashold;
        }
    }

    private static readonly AxisModelDefinitionCollection _instance = new AxisModelDefinitionCollection();

    private AxisModelDefinition _linearAxis;

    private AxisModelDefinition _angularAxis;

    private AxisModelDefinition _radialAxis;

    private AxisModelDefinition _linearAxisFor3DPlot;

    public static AxisModelDefinitionCollection Instance => _instance;

    public AxisModelDefinition this[AxesType axesType, PlotType plotType]
    {
        get
        {
            switch (axesType)
            {
            case AxesType.X:
            case AxesType.Y:
            case AxesType.Z:
                if (plotType == PlotType.Plot3D)
                {
                    if (_linearAxisFor3DPlot == null)
                    {
                        _linearAxisFor3DPlot = new AxisModelDefinition(-10.0, 10.0, 100, 5, 2.0);
                    }
                    return _linearAxisFor3DPlot;
                }
                if (_linearAxis == null)
                {
                    _linearAxis = new AxisModelDefinition(-10.0, 10.0, 100, 11, 2.0);
                }
                return _linearAxis;
            case AxesType.Radial:
                if (_radialAxis == null)
                {
                    _radialAxis = new AxisModelDefinition(0.0, 1.0, 50, 5, 4.0);
                }
                return _radialAxis;
            case AxesType.Angular:
                if (_angularAxis == null)
                {
                    _angularAxis = new AxisModelDefinition(0.0, 360.0, 60, 13, 2.0);
                }
                return _angularAxis;
            default:
                return null;
            }
        }
    }

    public AxisModelDefinitionCollection()
    {
    }
}
