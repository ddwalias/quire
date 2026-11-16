#nullable disable
#pragma warning disable

using System.ComponentModel;
using Quire.Controls.Core;

namespace Quire.Controls.Plots.ViewModel;

public partial class PlotTitleViewModel
{
    public IPlotTitleModel Model { get; }

    public PlotTitleType TitleType => Model.TitleType;

    public LegendPositions.PlotTitlePosition Position
    {
        get
        {
            return Model.Position;
        }
        set
        {
            if (Model.Position == value)
            {
                return;
            }
            Model.Position = value;
        }
    }

    public bool ShowTitle
    {
        get
        {
            return Model.ShowTitle;
        }
        set
        {
            if (Model.ShowTitle == value)
            {
                return;
            }
            Model.ShowTitle = value;
        }
    }

    public bool ShowBorder
    {
        get
        {
            return Model.ShowBorder;
        }
        set
        {
            if (Model.ShowBorder == value)
            {
                return;
            }
            Model.ShowBorder = value;
        }
    }

    public bool IsVertical
    {
        get
        {
            return Model.IsVertical;
        }
        set
        {
            if (Model.IsVertical == value)
            {
                return;
            }
            Model.IsVertical = value;
        }
    }

    public PlotTitleViewModel(IPlotTitleModel model)
    {
        Model = model;
        Model.PropertyChanged += HandleModelPropertyChanged;
    }

    protected void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
        case "ShowTitle":
        case "Position":
        case "ShowBorder":
        case "IsVertical":
            Model.IsDirty = true;
            OnPropertyChanged(e.PropertyName);
            break;
        }
    }
}
