using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Undo;

using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Mathboard;
using Quire.Wpf;
using Quire.Calculation;
using Quire;
namespace Quire.Controls.ExcelComponent.ViewModel;

public abstract partial class OleBaseComponentViewModel
{
    public WhiteboardItemsCollection InputItems { get; private set; }

    public WhiteboardItemsCollection OutputItems { get; private set; }

    internal double SectionsMinimumHeight { get; set; }

    private bool IsInternalResizing { get; set; }

    protected internal static IOleBaseComponentItemManager OleComponentItemMgr => global::Quire.Controls.ExcelComponent.Model.OleBaseComponentModel.OleComponentItemMgr;

    public bool ShouldAlignOleContainerItemWithGridLines => OutputSectionExists;

    protected override Size StrictMinimumSize => _strictMinimumSize;

    public virtual void EndInit()
    {
    }

    protected virtual void OnLoadOleContainerItem()
    {
    }

    private void OnResizeOleContainerItem()
    {
        SetRestrictedArea();
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.ViewModel.OleBaseComponentViewModel+ToggleSectionVisibilityCommand, Ptc.Controls.ExcelComponent")]
    public partial class ToggleSectionVisibilityCommand : IUndoableCommand
    {
        private readonly OleBaseComponentViewModel _oleBaseComponentViewModel;
        private readonly Action<bool> _setVisibility;
        private readonly bool _originalVisibility;
        private readonly string _name;

        public ToggleSectionVisibilityCommand(OleBaseComponentViewModel oleBaseComponentViewModel, bool isInputSection)
        {
            _oleBaseComponentViewModel = oleBaseComponentViewModel;
            _name = string.Format("Toggle {0} Section State", isInputSection ? "Input" : "Output");
            _originalVisibility = isInputSection ? _oleBaseComponentViewModel.IsInputSectionVisible : _oleBaseComponentViewModel.IsOutputSectionVisible;
            _setVisibility = isInputSection
                ? visibility => _oleBaseComponentViewModel.IsInputSectionVisible = visibility
                : visibility => _oleBaseComponentViewModel.IsOutputSectionVisible = visibility;
        }

        public string Name => _name;

        public bool Do()
        {
            _setVisibility(!_originalVisibility);
            return true;
        }

        public void Undo()
        {
            _setVisibility(_originalVisibility);
        }

        public void Redo()
        {
            Do();
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.ViewModel.OleBaseComponentViewModel+PasteOleComponentItemsCommand, Ptc.Controls.ExcelComponent")]
    public partial class PasteOleComponentItemsCommand : PasteMathboardItemsCommand
    {
        public PasteOleComponentItemsCommand(IOleBaseComponentViewModel viewModel, string commandName, IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
            : base(viewModel, commandName, pasteItemsAtLocation)
        {
        }

        private OleBaseComponentViewModel OleBaseComponentViewModel => _viewModel as OleBaseComponentViewModel;

        protected override bool DoWithResize()
        {
            double inputsHeight;
            Point insertionPoint = OleBaseComponentViewModel.InsertionPoint;
            if (insertionPoint.Y < OleBaseComponentViewModel.OleContainerItemTop)
            {
                inputsHeight = OleBaseComponentViewModel.InputSectionActualHeight;
                _itemsAtLocation.Values.Each(itemLocation =>
                {
                    inputsHeight = Math.Max(inputsHeight, itemLocation.Y + 1 + OleBaseComponentViewModel.Grid.GridCellSize.Height / 2);
                });
                if (inputsHeight > OleBaseComponentViewModel.InputSectionActualHeight)
                {
                    OleBaseComponentViewModel.OleContainerItemTop = inputsHeight;
                    inputsHeight = OleBaseComponentViewModel.OleContainerItemTop;
                    double inputsHeightDelta = inputsHeight - OleBaseComponentViewModel.InputSectionActualHeight;
                    OleBaseComponentViewModel.InputSectionActualHeight = inputsHeight;
                    Size newSize = new Size(OleBaseComponentViewModel.ContentWidth, OleBaseComponentViewModel.ContentHeight + inputsHeightDelta);
                    OleBaseComponentViewModel.FireChildCausedResize(OleBaseComponentViewModel.ContentSize, newSize);
                    CommandBuffer.ExecuteAndAttach(new WhiteboardViewModel.MoveItemsCommand(ViewModel, new Vector(0.0, inputsHeightDelta), true, OleBaseComponentViewModel.OutputItems));
                }
            }
            return base.DoWithResize();
        }
    }

    public EngineError Error => _model.Error;

    public IOleContainerItem OleContainerItem => _model.OleContainerItem;

    private IOleContainerItemViewModel OleContainerItemViewModel
    {
        get
        {
            if (!DebugTools.AssertNotNull(OleContainerItem, null).Failed)
            {
                return OleContainerItem.ViewModel;
            }
            return null;
        }
    }

    internal double OleContainerItemTop
    {
        get
        {
            if (OleContainerItem != null)
            {
                Point itemOleComponentLocation = OleComponentItemMgr.GetItemOleComponentLocation(OleContainerItem);
                return itemOleComponentLocation.Y;
            }
            return double.NaN;
        }
        set
        {
            if (OleContainerItem != null)
            {
                ClearRestrictedArea();
                Point nearestIntersectionPointDisregardingBounds = _alignmentGrid.GetNearestIntersectionPointDisregardingBounds(new Point(0.0, value), GridRestriction.Assert, Direction.Down);
                OleComponentItemMgr.SetItemOleComponentLocation(OleContainerItem, nearestIntersectionPointDisregardingBounds);
                base.SetItemLocation(OleContainerItem, default);
                SetRestrictedArea();
            }
        }
    }

    internal Size OleContainerItemSize
    {
        get
        {
            if (OleContainerItem != null)
            {
                return OleComponentItemMgr.GetSize(OleContainerItem);
            }
            return Size.Empty;
        }
    }

    protected double OleContainerItemHeight
    {
        get
        {
            Size oleContainerItemSize = OleContainerItemSize;
            return oleContainerItemSize.Height;
        }
    }

    protected double OleContainerItemWidth
    {
        get
        {
            Size oleContainerItemSize = OleContainerItemSize;
            return oleContainerItemSize.Width;
        }
    }

    public bool IsInputSectionVisible
    {
        get
        {
            return _isInputSectionVisible;
        }
        set
        {
            SetInputSectionVisibilityState(value);
        }
    }

    private double InputSectionActualTop
    {
        get
        {
            if (!IsInputSectionVisible)
            {
                return 0.0;
            }
            return InputSectionIndicator.Top;
        }
    }

    public double InputSectionActualHeight
    {
        get
        {
            if (!IsInputSectionVisible)
            {
                return 0.0;
            }
            return InputSectionIndicator.Height;
        }
        set
        {
            if (IsInputSectionVisible || InputSectionIndicator.Height == 0.0)
            {
                InputSectionIndicator.Height = value;
            }
        }
    }

    public bool IsOutputSectionVisible
    {
        get
        {
            if (OutputSectionExists)
            {
                return _isOutputSectionVisible;
            }
            return false;
        }
        set
        {
            SetOutputSectionVisibilityState(value);
        }
    }

    private double OutputSectionActualTop => InputSectionActualHeight + OleContainerItemHeight;

    public double OutputSectionActualHeight
    {
        get
        {
            if (!IsOutputSectionVisible)
            {
                return 0.0;
            }
            return OutputSectionIndicator.Height;
        }
        set
        {
            if (IsOutputSectionVisible || OutputSectionIndicator.Height == 0.0)
            {
                OutputSectionIndicator.Height = value;
            }
        }
    }

    public SectionIndicatorInfo InputSectionIndicator
    {
        get
        {
            return _sectionIndicators[0];
        }
        set
        {
            _sectionIndicators[0] = value;
        }
    }

    public SectionIndicatorInfo OutputSectionIndicator
    {
        get
        {
            return _sectionIndicators[1];
        }
        set
        {
            _sectionIndicators[1] = value;
        }
    }

    public override bool IsCrosshairVisible
    {
        get
        {
            if (base.IsCrosshairVisible)
            {
                if (!IsInputSectionVisible)
                {
                    return IsOutputSectionVisible;
                }
                return true;
            }
            return false;
        }
        set
        {
            base.IsCrosshairVisible = value && (IsInputSectionVisible || IsOutputSectionVisible);
        }
    }

    public override bool CanAddItems
    {
        get
        {
            if (base.CanAddItems)
            {
                return IsCrosshairVisible;
            }
            return false;
        }
    }

    protected double DefaultSectionsMinimumHeight
    {
        get
        {
            Size gridCellSize = Grid.GridCellSize;
            return gridCellSize.Height * 3.0;
        }
    }

    protected Size OleContainerItemInitialSize
    {
        get
        {
            Size result = OleContainerItemViewModel.EstablishInitialSize();
            result.Height = AlignHeightWithGridLines(result.Height, isOleContainerItem: true);
            return result;
        }
    }

    internal bool IsResizing
    {
        get
        {
            return OleContainerItem.IsResizing;
        }
        set
        {
            OleContainerItem.IsResizing = value;
            if (value)
            {
                DebugTools.DevAssert(_previousResizeData == null, null);
                return;
            }
            AdjustOleContainerItemSize(OleContainerItemSize);
            ReconcileOutputItemsLocations();
            MinimumSize = FindMinimumSize();
            _previousResizeData = null;
        }
    }

    public bool CanInsertOutputExpression
    {
        get
        {
            bool result = false;
            if (IsCrosshairVisible)
            {
                result = new Rect(0.0, OutputSectionActualTop, OleContainerItemWidth, OutputSectionActualHeight).ContainsAccountingForMeasurementError(InsertionPoint);
            }
            return result;
        }
    }

    public bool CanInsertInputExpression
    {
        get
        {
            bool result = false;
            if (IsCrosshairVisible)
            {
                result = new Rect(0.0, InputSectionActualTop, OleContainerItemWidth, InputSectionActualHeight).ContainsAccountingForMeasurementError(InsertionPoint);
            }
            return result;
        }
    }

    private void ClearRestrictedArea()
    {
        if (OleContainerItem != null)
        {
            ((IRestrictedAreaAlignmentGrid)_alignmentGrid).RestrictedAreas.Remove(OleContainerItem);
        }
    }

    private void SetRestrictedArea()
    {
        if (OleContainerItem != null)
        {
            Pair<double, double> value = new Pair<double, double>(InputSectionActualHeight, InputSectionActualHeight + OleContainerItemHeight);
            Dictionary<object, Pair<double, double>> restrictedAreas = ((IRestrictedAreaAlignmentGrid)_alignmentGrid).RestrictedAreas;
            if (restrictedAreas.ContainsKey(OleContainerItem))
            {
                restrictedAreas[OleContainerItem] = value;
            }
            else
            {
                restrictedAreas.Add(OleContainerItem, value);
            }
        }
    }
}
