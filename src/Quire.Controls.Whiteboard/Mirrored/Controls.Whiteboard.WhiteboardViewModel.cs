#nullable disable
#pragma warning disable

using System.Linq;

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
}
