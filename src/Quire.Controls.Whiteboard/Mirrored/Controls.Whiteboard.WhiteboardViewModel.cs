#nullable disable
#pragma warning disable

using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

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
}
