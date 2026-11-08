#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Undo;

namespace Quire.Controls.Mathboard;

public partial class AggregateMathboardViewModel : MathboardViewModel, IAggregateMathboardViewModel, IMathboardViewModel, ICurrentNumberFormatProperty, INotifyPropertyChanged, ICurrentMatrixFormatProperty, ICurrentTextFormatProperty, IWhiteboardViewModel, IWhiteboardItemContainerWithActiveItem, IDisposable, ICloneable
{
    protected SectionIndicatorInfo[] _sectionIndicators;
    private bool _hideFromCalculationContext;

    protected AggregateMathboardViewModel(IAggregateMathboardModel model, Size? gridCellSize = null)
        : base(model, gridCellSize)
    {
        Initialize(model);
    }

    protected AggregateMathboardViewModel(IAggregateMathboardModel model, IAlignmentGrid grid)
        : base(model, grid)
    {
        Initialize(model);
    }

    protected AggregateMathboardViewModel(AggregateMathboardViewModel original)
        : base(original)
    {
        _hideFromCalculationContext = original._hideFromCalculationContext;
        if (_mathboardModel is IVerticalWhiteboardSections verticalWhiteboardSections)
        {
            _sectionIndicators = new SectionIndicatorInfo[verticalWhiteboardSections.BlockSectionCount];
            for (uint num = 0u; num < verticalWhiteboardSections.BlockSectionCount; num++)
            {
                _sectionIndicators[num] = (original.IsInPrinting ? new SectionIndicatorInfo(original._sectionIndicators[num], isReadonly: true) : new SectionIndicatorInfo
                {
                    MinHeightRatio = verticalWhiteboardSections.GetBlockSectionMinHeightRatio(num)
                });
            }
            verticalWhiteboardSections.SectionsMaybeChanged += SetIndicatorsDimensions;
        }
    }

    public virtual object Clone()
    {
        return CloneAggregateMathboardViewModel();
    }

    public AggregateMathboardViewModel CloneAggregateMathboardViewModel()
    {
        AggregateMathboardViewModel aggregateMathboardViewModel = new AggregateMathboardViewModel(this);
        aggregateMathboardViewModel.WhiteboardItems.Each(aggregateMathboardViewModel.HandleItemAdded);
        return aggregateMathboardViewModel;
    }

    public virtual ISectionIndicatorInfo[] SectionIndicators => _sectionIndicators;

    protected void Initialize(IAggregateMathboardModel model)
    {
        MaximumSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        if (model is IVerticalWhiteboardSections verticalWhiteboardSections)
        {
            _sectionIndicators = new SectionIndicatorInfo[verticalWhiteboardSections.BlockSectionCount];
            for (uint num = 0u; num < verticalWhiteboardSections.BlockSectionCount; num++)
            {
                _sectionIndicators[num] = new SectionIndicatorInfo
                {
                    MinHeightRatio = verticalWhiteboardSections.GetBlockSectionMinHeightRatio(num)
                };
            }
            verticalWhiteboardSections.SectionsMaybeChanged += SetIndicatorsDimensions;
        }
    }

    public Size MeasureOverrideHelper(Size constraint)
    {
        SetIndicatorsDimensions();
        return constraint;
    }

    public override Size ArrangeOverride(Size size)
    {
        Size result = base.ArrangeOverride(size);
        SetIndicatorsDimensions();
        return result;
    }

    public override void HandleGridCellSizeChanged(Size oldSize, Size newSize)
    {
        base.HandleGridCellSizeChanged(oldSize, newSize);
        double num = newSize.Width / oldSize.Width;
        double num2 = newSize.Height / oldSize.Height;
        MaximumSize = new Size(MaximumSize.Width * num, MaximumSize.Height * num2);
        MinimumSize = new Size(MinimumSize.Width * num, MinimumSize.Height * num2);
        Size contentSize = new Size(ContentSize.Width * num, ContentSize.Height * num2);
        ContentSize = contentSize;
    }

    protected virtual void SetIndicatorsDimensions()
    {
        IVerticalWhiteboardSections verticalWhiteboardSections = (IVerticalWhiteboardSections)_mathboardModel;
        Pair<double, double> pair;
        double height = ContentSize.Height;
        double num = height * 0.025;
        for (uint num2 = 0u; num2 < verticalWhiteboardSections.BlockSectionCount; num2++)
        {
            if (_sectionIndicators[num2].Readonly)
            {
                continue;
            }
            IEnumerable<IWhiteboardItem> blockSectionItems = verticalWhiteboardSections.GetBlockSectionItems(num2);
            if (string.IsNullOrEmpty(_sectionIndicators[num2].Text))
            {
                _sectionIndicators[num2].Text = verticalWhiteboardSections.GetBlockSectionName(num2);
            }
            double minHeight = height * _sectionIndicators[num2].MinHeightRatio;
            if (blockSectionItems != null && blockSectionItems.Count() > 0)
            {
                pair = IndicatorRange(blockSectionItems);
            }
            else
            {
                double num3 = num;
                for (int i = 0; i < num2; i++)
                {
                    num3 += height * _sectionIndicators[i].MinHeightRatio + num;
                }
                pair = GetDefaultDimensions(num3, minHeight);
            }
            _sectionIndicators[num2].Top = pair.First;
            _sectionIndicators[num2].Height = pair.Second;
        }
    }

    public bool HideFromCalculationContext
    {
        get => _hideFromCalculationContext;
        set
        {
            if (_hideFromCalculationContext == value)
            {
                return;
            }
            Delegate1<bool> action = (bool state) =>
            {
                _hideFromCalculationContext = state;
                OnPropertyChanged("HideFromCalculationContext");
                UpdateHideFromCalculationButton();
            };
            DelegateFunction0<bool> doMethod = () =>
            {
                action(value);
                return true;
            };
            Delegate0 undoMethod = () =>
            {
                action(!value);
            };
            CommandBuffer.Execute(new UndoableCommand("Change HideFromCalculationContext on SolveBlock", doMethod, undoMethod));
        }
    }

    protected override void UpdateHideFromCalculationButton()
    {
        if (ActiveOrSelectedItems.IsNullOrEmpty())
        {
            HideFromCalculationButtonEnabled = true;
            HideFromCalculationButtonChecked = HideFromCalculationContext;
        }
        else
        {
            base.UpdateHideFromCalculationButton();
        }
    }

    public override void ToggleHideFromCalculationContext()
    {
        if (ActiveOrSelectedItems.IsNullOrEmpty())
        {
            HideFromCalculationContext = !HideFromCalculationContext;
        }
        else
        {
            base.ToggleHideFromCalculationContext();
        }
    }

    CommandBindingCollection IWhiteboardViewModel.CommandBindings => base.CommandBindings;
}
