#nullable disable
#pragma warning disable

using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Base3D.ViewModel;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

public sealed partial class PlotViewModel3D
{
    public SynchronizationStatus SynchronizationStatus => Model.SynchronizationStatus;

    internal Base3DAxisViewModel XAxisViewModel { get; private set; }

    internal Base3DAxisViewModel YAxisViewModel { get; private set; }

    internal Base3DAxisViewModel ZAxisViewModel { get; private set; }

    public override bool IsActiveRegion
    {
        get
        {
            return base.IsActiveRegion;
        }
        set
        {
            base.IsActiveRegion = value;
            if (ActiveAxis != null)
            {
                ActiveAxis.IsActive = value;
            }
        }
    }

    public Size GridCellSize
    {
        get
        {
            return _gridCellSize;
        }
        internal set
        {
            if (_gridCellSize != value)
            {
                _gridCellSize = value;
                OnPropertyChanged("GridCellSize");
            }
        }
    }
}
