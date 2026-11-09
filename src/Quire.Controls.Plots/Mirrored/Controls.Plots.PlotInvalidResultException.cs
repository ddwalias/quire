#nullable disable
#pragma warning disable

using System;

namespace Quire.Controls.Plots;

internal partial class PlotInvalidResultException
{
    public PlotInvalidResultException(string message)
        : base(message)
    {
    }

    public PlotInvalidResultException()
    {
    }

    public PlotInvalidResultException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
