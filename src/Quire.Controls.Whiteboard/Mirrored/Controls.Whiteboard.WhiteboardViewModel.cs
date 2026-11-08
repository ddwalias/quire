#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardViewModel
{
    public bool IsFocused
    {
        get
        {
            return _isFocused;
        }
        set
        {
            _isFocused = value;
            OnPropertyChanged("IsFocused");
        }
    }

    public virtual bool IsCrosshairVisible
    {
        get
        {
            return _isCrosshairVisible;
        }
        set
        {
            _isCrosshairVisible = value;
            OnPropertyChanged("IsCrosshairVisible");
        }
    }

    public bool ForbidGrowingMovementRight { get; set; }

    public bool CanPasteSelection
    {
        get
        {
            return _activeItem == null;
        }
    }

    public virtual bool CanInsertVerticalSpace
    {
        get
        {
            return ActiveOrSelectedItems.Count() == 0;
        }
    }

    public WhiteboardItemsCollection WhiteboardItems
    {
        get
        {
            return _whiteboardModel.WhiteboardItems;
        }
    }

    public bool OverRightMargin { get; set; }

    public Point InsertionPoint
    {
        get
        {
            return _insertionPoint;
        }
        set
        {
            SetInsertionPoint(value);
        }
    }

    protected bool HasSelectedItems
    {
        get
        {
            return SelectedItems.Any();
        }
    }

    public IWhiteboardItem ActiveDescendant
    {
        get
        {
            if (ActiveItem == null)
            {
                return null;
            }

            IWhiteboardItemContainer container = ActiveItem as IWhiteboardItemContainer;
            if (container == null)
            {
                return ActiveItem;
            }

            IWhiteboardControl control = ActiveItem as IWhiteboardControl;
            if (control == null)
            {
                return container.ActiveItem ?? ActiveItem;
            }

            return control.ActiveDescendant ?? ActiveItem;
        }
    }

    public IAlignmentGrid Grid
    {
        get
        {
            return _alignmentGrid;
        }
    }

    public bool IsSelected { private get; set; }

    public virtual Point DefaultInsertionPoint
    {
        get
        {
            return new Point(1.0, 1.0);
        }
    }

    public Size ContentSize
    {
        get
        {
            return new Size(ContentWidth, ContentHeight);
        }
        set
        {
            ContentWidth = value.Width;
            ContentHeight = value.Height;
        }
    }

    public double ContentHeight
    {
        get
        {
            return _contentHeight;
        }
        set
        {
            SetContentHeight(value);
        }
    }

    public double ContentWidth
    {
        get
        {
            return _contentWidth;
        }
        set
        {
            SetContentWidth(value);
        }
    }

    public DragMode CurrentDragMode { get; set; }

    public Point GridPosition
    {
        get
        {
            return Grid.ConvertDipsToGridCoordinates(InsertionPoint);
        }
        set
        {
            InsertionPoint = Grid.ConvertGridCoordinatesToDips(value);
        }
    }

    public bool HandleUserInput { get; protected set; }

    public virtual INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems
    {
        get
        {
            return _selectedItems;
        }
    }

    public IEnumerable<IWhiteboardItem> ActiveOrSelectedItems
    {
        get
        {
            if (ActiveItem != null)
            {
                return new List<IWhiteboardItem> { ActiveItem };
            }

            return SelectedItems;
        }
    }

    public IList<IWhiteboardItem> SelectedDescendants
    {
        get
        {
            if (ActiveItem == null)
            {
                return SelectedItems.ToList();
            }

            IWhiteboardControl control = ActiveItem as IWhiteboardControl;
            if (control == null)
            {
                return SelectedItems.ToList();
            }

            return control.SelectedDescendants;
        }
    }

    public Point InsertionPointForResize { get; private set; }

    public Size MinimumSize
    {
        get
        {
            return new Size(MinimumWidth, MinimumHeight);
        }
        set
        {
            MinimumWidth = value.Width;
            MinimumHeight = value.Height;
        }
    }

    public double MinimumWidth
    {
        get
        {
            return _minimumWidth;
        }
        set
        {
            double minimumWidth = Math.Max(value > MaximumSize.Width ? MaximumSize.Width : value, StrictMinimumSize.Width);
            if (MinimumWidth.IsEqualTo(minimumWidth))
            {
                return;
            }

            _minimumWidth = minimumWidth;
            OnPropertyChanged("MinimumWidth");
        }
    }

    public double MinimumHeight
    {
        get
        {
            return _minimumHeight;
        }
        set
        {
            double minimumHeight = Math.Max(value, StrictMinimumSize.Height);
            if (MinimumHeight.IsEqualTo(minimumHeight))
            {
                return;
            }

            _minimumHeight = minimumHeight;
            OnPropertyChanged("MinimumHeight");
        }
    }

    protected virtual Size StrictMinimumSize
    {
        get
        {
            return new Size(100.0, 100.0);
        }
    }

    public virtual Size MaximumSize
    {
        get
        {
            return new Size(MaximumWidth, MaximumHeight);
        }
        set
        {
            MaximumWidth = value.Width;
            MaximumHeight = value.Height;
        }
    }

    public virtual double MaximumHeight
    {
        get
        {
            return _maximumHeight;
        }
        set
        {
            if (_maximumHeight.IsEqualTo(value))
            {
                return;
            }

            _maximumHeight = value;
            OnPropertyChanged("MaximumHeight");
        }
    }

    public virtual double MaximumWidth
    {
        get
        {
            return _maximumWidth;
        }
        set
        {
            if (_maximumWidth.IsEqualTo(value))
            {
                return;
            }

            _maximumWidth = value;
            OnPropertyChanged("MaximumWidth");
        }
    }
}
