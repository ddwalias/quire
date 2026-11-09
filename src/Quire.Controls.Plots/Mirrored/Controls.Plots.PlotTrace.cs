#nullable disable
#pragma warning disable

using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial class PlotTrace
{
    public void SelectTrace()
    {
        IsSelected = true;
    }

    public void DeselectTrace()
    {
        IsSelected = false;
    }

    public void fireTraceClicked()
    {
        if (TraceClicked != null)
        {
            TraceClicked(this);
        }
        IsSelected = true;
    }

    public void ResetIsTypeDefault()
    {
        if (RequestResetIsTypeDefault != null)
        {
            RequestResetIsTypeDefault();
        }
    }

    private void FireTypeChanged()
    {
        if (TypeChanged != null)
        {
            TypeChanged();
        }
        FireTraceChanged();
    }

    private void FireErrorChanged()
    {
        if (ErrorChanged != null)
        {
            ErrorChanged(this);
        }
    }

    private void FireCalculatingStatusChangedEvent()
    {
        if (CalculatingStatusChangedEvent != null)
        {
            CalculatingStatusChangedEvent();
        }
    }

    private void FireTraceChanged()
    {
        Action<IPlotTrace> handler = TraceChanged;
        if (handler != null)
        {
            handler(this);
        }
    }
}
