using System;
using System.Windows;
using System.Windows.Threading;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.Model;

namespace Quire.Controls.ExcelComponent.ViewModel;

public partial class ExcelComponentViewModel
{
    private global::Quire.Controls.ExcelComponent.IExcelContainer ExcelContainerItem => (global::Quire.Controls.ExcelComponent.IExcelContainer)OleContainerItem;

    private global::Quire.Controls.ExcelComponent.ViewModel.IExcelContainerViewModel ExcelContainerItemViewModel
    {
        get
        {
            if (ExcelContainerItem != null)
            {
                return ExcelContainerItem.ViewModel;
            }
            return null;
        }
    }

    public global::System.Nullable<bool> IsAsIcon
    {
        get
        {
            if (ExcelContainerItemViewModel == null)
            {
                return null;
            }
            return ExcelContainerItemViewModel.IsAsIcon;
        }
        set
        {
            if (ExcelContainerItemViewModel != null)
            {
                ExcelContainerItemViewModel.IsAsIcon = value;
            }
        }
    }

    public ExcelComponentViewModel(global::Quire.Controls.ExcelComponent.Model.ExcelComponentModel model, global::System.Nullable<global::System.Windows.Size> gridCellSize)
        : this(model, new global::Quire.Controls.Core.RestrictedAreaAlignmentGrid(gridCellSize))
    {
    }

    internal ExcelComponentViewModel(global::Quire.Controls.ExcelComponent.Model.ExcelComponentModel model, global::Quire.Controls.Core.IRestrictedAreaAlignmentGrid grid)
        : base(model, grid)
    {
    }

    private ExcelComponentViewModel(global::Quire.Controls.ExcelComponent.ViewModel.ExcelComponentViewModel original)
        : base(original)
    {
    }

    public void CollapseInputOutputAfterLoading()
    {
        SetInputSectionVisibilityState(false, false);
        SetOutputSectionVisibilityState(false, false);
        global::Quire.Delegate0 @delegate = () =>
        {
            MinimumSize = FindMinimumSize(true);
            double verticalDiff = OleContainerItemSize.Height;
            ForceOleComponentResizeDueToOleContainerItemResize(true);
            verticalDiff -= OleContainerItemSize.Height;
            OutputItems.Each((global::Quire.Controls.Core.IWhiteboardItem item) =>
            {
                global::System.Windows.Point itemOleComponentLocation = OleComponentItemMgr.GetItemOleComponentLocation(item);
                itemOleComponentLocation.Y -= verticalDiff;
                OleComponentItemMgr.SetItemOleComponentLocation(item, itemOleComponentLocation);
            });
            ContentSize = MinimumSize;
        };
        global::System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(global::System.Windows.Threading.DispatcherPriority.Background, (global::System.Delegate)@delegate);
    }

    protected override double GetSectionInitialHeight()
    {
        global::System.Windows.Size oleContainerItemInitialSize = OleContainerItemInitialSize;
        double num = Math.Round(oleContainerItemInitialSize.Height / 2.0 / _alignmentGrid.GridCellSize.Height);
        if (num < 3.0)
        {
            num = 3.0;
        }
        return num * _alignmentGrid.GridCellSize.Height;
    }

    protected override void OnLoadOleContainerItem()
    {
        if (IsAsIcon.GetValueOrDefault())
        {
            CollapseInputOutputAfterLoading();
        }
        IsAsIcon = false;
    }

    public override bool OutputSectionExists => true;

    public override object Clone()
    {
        return CloneExcelComponentViewModel();
    }

    public global::Quire.Controls.ExcelComponent.ViewModel.ExcelComponentViewModel CloneExcelComponentViewModel()
    {
        global::Quire.Controls.ExcelComponent.ViewModel.ExcelComponentViewModel excelComponentViewModel = new global::Quire.Controls.ExcelComponent.ViewModel.ExcelComponentViewModel(this);
        excelComponentViewModel.PostClone();
        return excelComponentViewModel;
    }
}
