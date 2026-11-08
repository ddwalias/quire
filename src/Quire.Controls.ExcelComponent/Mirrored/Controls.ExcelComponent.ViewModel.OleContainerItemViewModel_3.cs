#nullable disable
#pragma warning disable
using System;
using System.Windows;
using Quire.Controls.Core.Ole;
using Quire.PersistentData;

namespace Quire.Controls.ExcelComponent.ViewModel;

public abstract partial class OleContainerItemViewModel<TModel, TIPublicModel, TPersistentData>
{
    private bool _isResizing;

    public virtual bool IsResizing
    {
        get => _isResizing;
        protected internal set
        {
            if (value == _isResizing)
            {
                return;
            }
            _isResizing = value;
            if (value && !IsOleSiteRunning)
            {
                RunAndCheckAvailabilityOnResizing();
            }
        }
    }

    public event Action<bool> MinSizeChanged;

    public OleContainerItemViewModel(TModel model, Size defaultMinSize)
        : base(model, defaultMinSize)
    {
    }

    protected virtual void RunAndCheckAvailabilityOnResizing()
    {
        Model.CheckServerAvailable(forceRun: true, OleContainerMessageKeysDict.ResizeMessageKeys);
    }

    protected void FireMinimalSizeChanged()
    {
        if (MinSizeChanged != null)
        {
            MinSizeChanged(IsResizing);
        }
    }

    public override Size MinSize
    {
        get => base.MinSize;
        protected set
        {
            Size minSize = MinSize;
            base.MinSize = value;
            Size minSize2 = MinSize;
            if (minSize != minSize2)
            {
                FireMinimalSizeChanged();
            }
        }
    }
}
