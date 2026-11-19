#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire;
using Quire.Calculation.Core.Trace2DResults;
using Quire.Wpf;

namespace Quire.Controls.Plots.Model;

internal partial class PlottingPointsFilter : ISupportInitialize
{
    private IList<Point> _plottingPoints;
    private readonly Func<Point, bool> _containes;
    private bool _filteringAllowed;
    private PointCollection _filteredPointCollection;
    private Action _propertyChanged;
    private bool _propertyWasChangedDuringInitialization;
    private int _notNullPointsCount;
    internal Action FilteredPlottingPointsChanged;

    internal IList<Point> PlottingPoints
    {
        get => _plottingPoints;
        set
        {
            if (_plottingPoints != value)
            {
                _plottingPoints = value;
                _propertyChanged();
            }
        }
    }

    public int NotNullPointsCount
    {
        get
        {
            if (!PlottingPoints.IsNullOrEmpty())
            {
                return _notNullPointsCount;
            }
            return 0;
        }
    }

    private int PlottingPointsCount
    {
        get
        {
            if (PlottingPoints != null)
            {
                return PlottingPoints.Count;
            }
            return 0;
        }
    }

    internal bool FilteringAllowed
    {
        get => _filteringAllowed;
        set
        {
            if (_filteringAllowed != value)
            {
                _filteringAllowed = value;
                _propertyChanged();
            }
        }
    }

    private IList<TraceTypeRange> GetIndexesOfPointsInRange()
    {
        _notNullPointsCount = 0;
        List<TraceTypeRange> list = new List<TraceTypeRange>();
        if (FilteringAllowed)
        {
            for (int i = 0; i < PlottingPointsCount; i++)
            {
                int counNotNaNs = 0;
                i = ScanPoints(PlottingPoints, i, PlottingPointsCount, _containes, false, ref counNotNaNs);
                DebugTools.DevAssert(counNotNaNs == 0, null);
                if (i < PlottingPointsCount)
                {
                    TraceTypeRange item = default;
                    item.Start = (i > 0) ? (i - 1) : i;
                    i = ScanPoints(PlottingPoints, i, PlottingPointsCount, _containes, true, ref counNotNaNs);
                    _notNullPointsCount += counNotNaNs;
                    item.End = (i < PlottingPointsCount) ? i : (PlottingPointsCount - 1);
                    list.Add(item);
                }
            }
        }
        else
        {
            _notNullPointsCount = PlottingPoints.Sum(point => !point.IsNaN() ? 1 : 0);
            list.Add(new TraceTypeRange
            {
                Start = 0,
                End = PlottingPointsCount - 1
            });
        }
        return list;
    }

    private static int ScanPoints(IList<Point> plottingPoints, int currentIndex, int plottingPointsCount, Func<Point, bool> containes, bool inRange, ref int counNotNaNs)
    {
        while (currentIndex < plottingPointsCount && inRange == InRangeOrNone(containes, plottingPoints[currentIndex]))
        {
            if (inRange && !plottingPoints[currentIndex].IsNaN())
            {
                counNotNaNs++;
            }
            currentIndex++;
        }
        return currentIndex;
    }

    private static bool InRangeOrNone(Func<Point, bool> containes, Point plottingPoint)
    {
        if (!plottingPoint.IsNaN())
        {
            return containes(plottingPoint);
        }
        return true;
    }

    private IEnumerable<Point> FilteredPointsEnumeration
    {
        get
        {
            IList<TraceTypeRange> ranges = GetIndexesOfPointsInRange();
            int count = ranges.Count;
            if (count > 0)
            {
                TraceTypeRange indexRange = ranges[0];
                for (int pointIndex = indexRange.Start; pointIndex <= indexRange.End; pointIndex++)
                {
                    yield return PlottingPoints[pointIndex];
                }

                for (int i = 1; i < count; i++)
                {
                    yield return PointExtension.NaN;
                    indexRange = ranges[i];
                    for (int pointIndex = indexRange.Start; pointIndex <= indexRange.End; pointIndex++)
                    {
                        yield return PlottingPoints[pointIndex];
                    }
                }
            }
        }
    }

    internal PointCollection FilteredPointsCollection => _filteredPointCollection;

    internal PlottingPointsFilter(IList<Point> plottingPoints, Func<Point, bool> containes, bool filteringAllowed)
    {
        _plottingPoints = plottingPoints;
        _containes = containes;
        _filteringAllowed = filteringAllowed;
        UpdateFilteredPointCollection();
        _propertyChanged = UpdateFilteredPointCollection;
    }

    private void UpdateFilteredPointCollection()
    {
        PointCollection pointCollection = (_plottingPoints == null) ? null : new PointCollection(FilteredPointsEnumeration);
        if (_filteredPointCollection != pointCollection)
        {
            _filteredPointCollection = pointCollection;
            FilteredPlottingPointsChanged();
        }
    }

    public void BeginInit()
    {
        _propertyChanged = () =>
        {
            _propertyWasChangedDuringInitialization = true;
        };
    }

    public void EndInit()
    {
        if (_propertyWasChangedDuringInitialization)
        {
            UpdateFilteredPointCollection();
            _propertyWasChangedDuringInitialization = false;
        }
        _propertyChanged = UpdateFilteredPointCollection;
    }
}
