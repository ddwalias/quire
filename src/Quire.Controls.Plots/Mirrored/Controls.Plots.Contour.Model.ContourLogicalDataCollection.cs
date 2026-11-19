using System;
using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Plots.Contour.Model;

public partial class ContourLogicalDataCollection
{
    private ContourLogicalData[] _contours;
    private Rect _boundingRectangleXy;
    private SortedList<double, object> _levels;

    public ContourLogicalData[] Contours
    {
        internal get => _contours;
        set => _contours = value;
    }

    private Interval ZInterval => new Interval
    {
        Lower = Levels[0],
        Upper = Levels[LevelsCount - 1]
    };

    public Rect BoundingRectangleXy
    {
        get => _boundingRectangleXy;
        set
        {
            _boundingRectangleXy = value;
            BoundingContour = CreateBoundingCountour();
        }
    }

    public int Length
    {
        get
        {
            if (Contours != null)
            {
                return Contours.Length;
            }
            return 0;
        }
    }

    internal IList<double> Levels
    {
        get
        {
            if (_levels != null)
            {
                return _levels.Keys;
            }
            return null;
        }
    }

    internal int LevelsCount
    {
        get
        {
            if (Levels != null)
            {
                return Levels.Count;
            }
            return 0;
        }
    }

    internal ContourLogicalData BoundingContour { get; private set; }

    public ContourLogicalDataCollection()
    {
        _levels = new SortedList<double, object>();
    }

    private void AddLevel(double level)
    {
        if (!_levels.ContainsKey(level))
        {
            _levels.Add(level, null);
        }
    }

    internal void MergeRequestLevels(double[] requestLevels)
    {
        DebugTools.DevAssert(requestLevels != null && requestLevels.Length > 1, null);
        _levels = new SortedList<double, object>(new DoubleComparer(Math.Abs((requestLevels[1] - requestLevels[0]) / 2.0)));
        foreach (double level in requestLevels)
        {
            AddLevel(level);
        }
    }

    public void Compute(ContourLogicalData contourLogicalData, int contourIndex)
    {
        Contours[contourIndex] = contourLogicalData;
        AddLevel(contourLogicalData.ZLevel);
    }

    public int GetExtendedIntervalIndex(int contourIndex)
    {
        ContourLogicalData contour = (contourIndex > 0) ? Contours[contourIndex - 1] : BoundingContour;
        return GetIntervalIndex(contour);
    }

    private int GetIntervalIndex(ContourLogicalData contour)
    {
        double gradientSign = contour.GradientSign;
        int num;
        if (Math.Abs(gradientSign) > 1.5)
        {
            num = -1;
        }
        else
        {
            num = Levels.IndexOf(contour.ZLevel);
            if (gradientSign > 0.0 && num > -1)
            {
                num++;
            }
        }
        return num;
    }

    private ContourLogicalData CreateBoundingCountour()
    {
        ContourLogicalData result;
        ContourLogicalData contourLogicalData = default;
        contourLogicalData.Points = new Point[4]
        {
            BoundingRectangleXy.BottomRight,
            BoundingRectangleXy.BottomLeft,
            BoundingRectangleXy.TopLeft,
            BoundingRectangleXy.TopRight
        };
        result = contourLogicalData;
        if (Contours.Length != 0)
        {
            ContourLogicalData contourLogicalData2 = Contours[0];
            result.ZLevel = contourLogicalData2.ZLevel;
            result.GradientSign = -contourLogicalData2.GradientSign;
        }
        return result;
    }

    internal double NormalizedCenter(int intervalIndex)
    {
        return ZInterval.Normalize((Levels[intervalIndex] + Levels[intervalIndex - 1]) / 2.0);
    }
}
