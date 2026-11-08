#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Mathboard;

public abstract partial class MathboardModel : WhiteboardModel, IMathboardModel, IWhiteboardModel, ICloneable, IDisposable, ICalculatableContainer
{
    protected CalculatableChildHider _childManager;
    private IIORegionManager _ioRegionManager;
    private IBookmarksManager _bookmarksManager;

    public event Action<ICalculationHideableChild> CalculatableHiddenStatusChanged;

    protected MathboardModel(IWhiteboardItemPositionComparer itemPositionComparer)
        : base(itemPositionComparer)
    {
        WhiteboardItems.RangeChanged += HandleItemsListChanged;
        _childManager = InitializeChildManager();
    }

    protected abstract CalculatableChildHider InitializeChildManager();

    private void HandleItemsListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // IL requires local variables declared before switch to match local slots
        IEnumerable<IWhiteboardItem> items1;
        IEnumerable<IWhiteboardItem> items2;
        IEnumerable<IWhiteboardItem> items3;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                items1 = e.NewItems.Cast<IWhiteboardItem>();
                AddItems(items1);
                break;
            case NotifyCollectionChangedAction.Remove:
                items2 = e.OldItems.Cast<IWhiteboardItem>();
                RemoveItems(items2);
                break;
            case NotifyCollectionChangedAction.Move:
                items3 = e.NewItems.Cast<IWhiteboardItem>();
                MoveItems(items3);
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
    }

    protected virtual void AddItems(IEnumerable<IWhiteboardItem> items)
    {
        items.Each(item =>
        {
            _childManager.ChildAdded(item);
        });
        if (IoRegionManager != null)
        {
            items.OfType<IIORegion>().Each(item =>
            {
                IoRegionManager.HandleElementAddedToParentContainer(item);
            });
        }
    }

    protected virtual void RemoveItems(IEnumerable<IWhiteboardItem> items)
    {
        items.Each(item =>
        {
            _childManager.ChildRemoved(item);
        });
        if (IoRegionManager != null)
        {
            items.OfType<IIORegion>().Each(item =>
            {
                IoRegionManager.HandleElementRemovedFromParentContainer(item);
            });
        }
    }

    protected virtual void MoveItems(IEnumerable<IWhiteboardItem> items)
    {
        items.Each(item =>
        {
            _childManager.ChildMoved(item);
        });
        if (IoRegionManager != null)
        {
            items.OfType<IIORegion>().Each(ioRegion =>
            {
                IoRegionManager.HandleElementRemovedFromParentContainer(ioRegion);
                IoRegionManager.HandleElementAddedToParentContainer(ioRegion);
            });
        }
    }

    public bool IsEmpty => WhiteboardItems.Count == 0;

    public void CommitItem(IWhiteboardItem item)
    {
        if (item is ICalculatable calculatable)
        {
            calculatable.Commit();
        }
        if (item is ICalculatableContainer calculatableContainer && !(calculatableContainer is ICalculatableAggregate))
        {
            calculatableContainer.CalculatableChildren.Each(child =>
            {
                child.Commit();
            });
        }
    }

    public IIORegionManager IoRegionManager
    {
        get => _ioRegionManager;
        set
        {
            DebugTools.DevAssert(_ioRegionManager == null || value == null, "IORegionManager of Mathboard cannot be set more than once");
            if (WhiteboardItems.IsNotEmpty())
            {
                WhiteboardItems.OfType<IIORegionContainer>().Each(mathBoardCtrl =>
                {
                    mathBoardCtrl.InputOutputRegionManager = value;
                });
            }
            if (_ioRegionManager != null)
            {
                if (WhiteboardItems.Count != 0)
                {
                    WhiteboardItems.OfType<IIORegion>().Each(IoRegionManager.HandleElementRemovedFromParentContainer);
                }
                _ioRegionManager.ShadowCalculationsChangedNotifier.CollectionChanged -= HandleShadowCalculationsChanged;
            }
            _ioRegionManager = value;
            if (_ioRegionManager != null)
            {
                if (WhiteboardItems.Count != 0)
                {
                    WhiteboardItems.OfType<IIORegion>().Each(IoRegionManager.HandleElementAddedToParentContainer);
                }
                _ioRegionManager.ShadowCalculationsChangedNotifier.CollectionChanged += HandleShadowCalculationsChanged;
            }
        }
    }

    public IBookmarksManager BookmarksManager
    {
        get => _bookmarksManager;
        set
        {
            if (value != null && WhiteboardItems.IsNotEmpty())
            {
                foreach (IWhiteboardItem whiteboardItem in WhiteboardItems)
                {
                    switch (whiteboardItem)
                    {
                        case IMathboardControl mathboardControl when mathboardControl.BookmarksManager == null:
                            mathboardControl.BookmarksManager = value;
                            break;
                        case IRichTextWithEmbeddedItem richText when richText.BookmarksManager == null:
                            richText.BookmarksManager = value;
                            break;
                    }
                }
            }
            _bookmarksManager = value;
        }
    }

    public event Action<ICalculatableContainer, ICalculatable> CalculatableAdded;
    public event Action<ICalculatable> CalculatableRemoved;
    public event Action<ICalculatable> CalculatableMoved;

    public virtual IEnumerable<ICalculatable> CalculatableChildren
    {
        get
        {
            foreach (ICalculatable child in _childManager.CalculatableChildren)
            {
                yield return child;
                foreach (ICalculatable calculatableOverride in GetCalculatableOverrides(child))
                {
                    yield return calculatableOverride;
                }
            }
        }
    }

    protected virtual void FireCalculatableAdded(ICalculatable calc)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(this, calc);
        }
    }

    protected virtual void FireCalculatableRemoved(ICalculatable calc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calc);
        }
    }

    protected virtual void FireCalculatableMoved(ICalculatable calc)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(calc);
        }
    }

    protected virtual void FireCalculatableHiddenStatusChanged(ICalculationHideableChild hideable)
    {
        if (CalculatableHiddenStatusChanged != null)
        {
            CalculatableHiddenStatusChanged(hideable);
        }
    }

    private void HandleShadowCalculationsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                GetChildShadowCalculatables(e.NewItems.Cast<Pair<IIORegion, ICalculatable>>()).Each(calc =>
                {
                    _childManager.ChildAdded(calc);
                });
                break;
            case NotifyCollectionChangedAction.Remove:
                GetChildShadowCalculatables(e.OldItems.Cast<Pair<IIORegion, ICalculatable>>()).Each(calc =>
                {
                    _childManager.ChildRemoved(calc);
                });
                break;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                break;
        }
    }

    private IEnumerable<ICalculatable> GetChildShadowCalculatables(IEnumerable<Pair<IIORegion, ICalculatable>> calculationPairs)
    {
        IList<ICalculatable> blockShadowCalculatables = new List<ICalculatable>();
        calculationPairs.Each(pair =>
        {
            if (WhiteboardItems.Contains(pair.First as IWhiteboardItem))
            {
                blockShadowCalculatables.Add(pair.Second);
            }
        });
        return blockShadowCalculatables;
    }

    protected IEnumerable<ICalculatable> GetCalculatableOverrides(ICalculatable calculatable)
    {
        if (IoRegionManager != null)
        {
            if (GetWrappedCalculatable(calculatable) is IIORegion region)
            {
                IList<ICalculatable> calculationOverrides = IoRegionManager.GetCalculationOverrides(region);
                if (calculationOverrides != null)
                {
                    foreach (ICalculatable item in calculationOverrides)
                    {
                        yield return item;
                    }
                }
            }
        }
    }

    protected virtual ICalculatable GetWrappedCalculatable(ICalculatable calculatable)
    {
        if (!(calculatable is ICalculatableWrapper))
        {
            return calculatable;
        }
        return (calculatable as ICalculatableWrapper).WrappedCalculatable;
    }

    public virtual bool IsInPrinting { get; set; }
}
