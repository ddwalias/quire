#nullable disable
#pragma warning disable
using System;
using System.ComponentModel;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.Mathboard;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.ExcelComponent.ViewModel;

public partial class ChartComponentViewModel : OleBaseComponentViewModel, IChartComponentViewModel, IOleBaseComponentViewModel, IAggregateMathboardViewModel, IMathboardViewModel, ICurrentNumberFormatProperty, INotifyPropertyChanged, ICurrentMatrixFormatProperty, ICurrentTextFormatProperty, IWhiteboardViewModel, IWhiteboardItemContainerWithActiveItem, IDisposable, ICloneable, IResizableElementViewModelWithDataStore, IItemContainerWithoutTextRegion
{
    private ChartComponentModel _chartComponentModel;

    private readonly double _sectionInitialHeight = Math.Round(75.59055118110236);

    public override bool OutputSectionExists => false;

    public ChartComponentViewModel(ChartComponentModel model, Size? gridCellSize)
        : this(model, new RestrictedAreaAlignmentGrid(gridCellSize))
    {
    }

    internal ChartComponentViewModel(ChartComponentModel model, IRestrictedAreaAlignmentGrid grid)
        : base(model, grid)
    {
        _chartComponentModel = model;
        double num = WpfUtils.InchesToDips(0.2);
        _insertionPoint = new Point(num, num);
    }

    private ChartComponentViewModel(ChartComponentViewModel original)
        : base(original)
    {
        _chartComponentModel = original._chartComponentModel;
    }

    public override void Commit()
    {
        _chartComponentModel.CommitPre();
        if (ActiveItem != null)
        {
            Commit(ActiveItem);
        }
        base.Commit();
        _chartComponentModel.CommitPost();
    }

    public override void EndInit()
    {
        IsOutputSectionVisible = false;
    }

    protected override double GetSectionInitialHeight()
    {
        return _sectionInitialHeight;
    }

    public override object Clone()
    {
        return CloneChartComponentViewModel();
    }

    public ChartComponentViewModel CloneChartComponentViewModel()
    {
        ChartComponentViewModel chartComponentViewModel = new ChartComponentViewModel(this);
        chartComponentViewModel.PostClone();
        return chartComponentViewModel;
    }
}
