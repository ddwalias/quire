using System;

namespace Quire.Controls.Plots.Presenter;

public partial interface IValueToScreen
{
    event Action<object> ValueToScreenChanged;

    double ValueToScreen(double value);

    double ScreenToValue(double value);

    double CoerceScreenValue(double value, bool toEdges);

    EdgeBelongingStatus GetEdgeBelongingStatusForCoordinate(double d);
}
