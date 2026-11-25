#nullable disable
#pragma warning disable

namespace Quire.Controls.Plots;

public abstract partial class PlotGraphAxis
{
    public partial class Point2ScreenHelper
    {
        protected double _firstTickPostion;

        protected double _lastTickPositoin;

        protected double _start = -10.0;

        protected double _end = 10.0;

        protected double _axisLenght;

        public Point2ScreenHelper(double size)
        {
            _firstTickPostion = 0.0;
            _lastTickPositoin = size;
            _axisLenght = _start == _end ? 1.0 : _end - _start;
        }

        public Point2ScreenHelper(PlotGraphAxis axis)
        {
            _firstTickPostion = axis.GetAxisStartPosition();
            _lastTickPositoin = axis.GetAxisEndPosition();
            _start = axis.LinearizedStart;
            _end = axis.LinearizedEnd;
            _axisLenght = _start == _end ? 1.0 : _end - _start;
        }

        public virtual double Point2Screen(double offset)
        {
            if (double.IsNegativeInfinity(offset))
            {
                return _lastTickPositoin < _firstTickPostion ? double.MaxValue : double.MinValue;
            }
            return (offset - _start) / _axisLenght * (_lastTickPositoin - _firstTickPostion) + _firstTickPostion;
        }

    }

    public int EquationCount
    {
        get
        {
            if (!HasEquationList())
            {
                return 0;
            }
            return EquationList.EquationCount;
        }
    }

    public bool ShowTicks
    {
        get => (bool)GetValue(ShowTicksProperty);
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool ShowNumbers
    {
        get => (bool)GetValue(ShowNumbersProperty);
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public double Offset
    {
        get => (double)GetValue(OffsetProperty);
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }
}
