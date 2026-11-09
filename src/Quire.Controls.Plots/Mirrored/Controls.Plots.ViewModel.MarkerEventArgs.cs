#nullable disable
#pragma warning disable

using System;

namespace Quire.Controls.Plots.ViewModel;

internal partial class MarkerEventArgs
{
    private readonly BaseMarkerViewModel _marker;

    public MarkerEventArgs(BaseMarkerViewModel marker)
    {
        _marker = marker;
    }

    public BaseMarkerViewModel Marker => _marker;
}
