using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Core;

[Serializable]
public partial class WhiteboardItemsCollection : SerializableCollection<IWhiteboardItem>, IDisposable
{
    private bool? _parentIsOnVisiblePage;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ParentIsOnVisiblePage
    {
        get
        {
            return _parentIsOnVisiblePage.GetValueOrDefault();
        }
        set
        {
            _parentIsOnVisiblePage = value;
            Items.Each(item =>
            {
                WhiteboardItemContainerExtensions.ItemManager.SetItemIsOnVisiblePage(item, value);
            });
        }
    }

    private bool? _parentSupportsInputsOutputs;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ParentSupportsInputsOutputs
    {
        get
        {
            return _parentSupportsInputsOutputs.GetValueOrDefault();
        }
        set
        {
            _parentSupportsInputsOutputs = value;
            Items.Each(item =>
            {
                WhiteboardItemContainerExtensions.ItemManager.SetItemSupportsInputsOutputs(item, value);
            });
        }
    }

    private bool? _containedInCollapsedWhiteboard;

    public bool ContainedInCollapsedWhiteboard
    {
        get
        {
            return _containedInCollapsedWhiteboard.GetValueOrDefault();
        }
        set
        {
            _containedInCollapsedWhiteboard = value;
            Items.Each(item =>
            {
                WhiteboardItemContainerExtensions.ItemManager.SetContainedInCollapsedWhiteboard(item, value);
            });
        }
    }

    public WhiteboardItemsCollection()
    {
    }

    public WhiteboardItemsCollection(IEnumerable<IWhiteboardItem> items)
        : base(items)
    {
    }

    protected override void OnRangeChanged(NotifyCollectionChangedEventArgs args)
    {
        IList list;
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
            {
                list = new object[0];
                foreach (IWhiteboardItem item in (args.OldItems ?? list).OfType<IWhiteboardItem>())
                {
                    if (_parentIsOnVisiblePage.HasValue)
                    {
                        WhiteboardItemContainerExtensions.ItemManager.ClearItemIsOnVisiblePage(item);
                    }
                    if (_parentSupportsInputsOutputs.HasValue)
                    {
                        WhiteboardItemContainerExtensions.ItemManager.ClearItemSupportsInputsOutputs(item);
                    }
                    if (_containedInCollapsedWhiteboard.HasValue)
                    {
                        WhiteboardItemContainerExtensions.ItemManager.ClearContainedInCollapsedWhiteboard(item);
                    }
                }
                foreach (IWhiteboardItem item2 in (args.NewItems ?? list).OfType<IWhiteboardItem>())
                {
                    if (_parentIsOnVisiblePage.HasValue)
                    {
                        WhiteboardItemContainerExtensions.ItemManager.SetItemIsOnVisiblePage(item2, ParentIsOnVisiblePage);
                    }
                    if (_parentSupportsInputsOutputs.HasValue)
                    {
                        WhiteboardItemContainerExtensions.ItemManager.SetItemSupportsInputsOutputs(item2, ParentSupportsInputsOutputs);
                    }
                    if (_containedInCollapsedWhiteboard.HasValue)
                    {
                        WhiteboardItemContainerExtensions.ItemManager.SetContainedInCollapsedWhiteboard(item2, ContainedInCollapsedWhiteboard);
                    }
                }
                break;
            }
            default:
                DebugTools.FailAssertion(null, Array.Empty<object>());
                break;
            case NotifyCollectionChangedAction.Move:
                break;
        }
        base.OnRangeChanged(args);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        int count = Count;
        info.AddValue("count", count);
        for (int i = 0; i < count; i++)
        {
            info.AddValue(i.ToString(), base[i]);
        }
    }

    protected WhiteboardItemsCollection(SerializationInfo info, StreamingContext context)
    {
        int num = (int)info.GetValue("count", typeof(int));
        for (int i = 0; i < num; i++)
        {
            Add(info.GetValue(i.ToString(), info.GetType()) as IWhiteboardItem);
        }
    }

    public void Dispose()
    {
        this.OfType<IDisposable>().Each(item =>
        {
            item.Dispose();
        });
    }
}
