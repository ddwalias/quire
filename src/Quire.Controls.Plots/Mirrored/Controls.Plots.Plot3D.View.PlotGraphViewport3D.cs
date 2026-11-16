#nullable disable
#pragma warning disable

using System;
using System.Windows.Controls;
using Quire.Media3D;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class PlotGraphViewport3D
{
    public static bool RenderingEnabled
    {
        get
        {
            return Quire.Media3D.ViewportsListManager.RenderingEnabled;
        }
        set
        {
            Quire.Media3D.ViewportsListManager.RenderingEnabled = value;
        }
    }

    protected IViewportsListManager ViewportsListManager => Quire.Media3D.ViewportsListManager.Instance;

    public PlotGraphViewport3D()
    {
        IsHitTestVisible = false;
        ViewportsListManager.AddToList(this);
    }

    public void Dispose()
    {
        ViewportsListManager.RemoveFromList(this);
    }
}
