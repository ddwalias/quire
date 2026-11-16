using Quire.Calculation.Core;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Mathboard;
using Quire.Controls.Whiteboard;
using Quire.Services;
namespace Quire.Controls.ExcelComponent.Model;

public abstract partial class OleBaseComponentModel
{
    public global::System.Windows.ResourceDictionary Resources { get; set; }

    public override int BlockSectionCount => 2;

    internal virtual global::Quire.Controls.ExcelComponent.View.IOleContainerItem OleContainerItem { get; set; }

    public EngineError Error
    {
        get
        {
            return _error;
        }
        protected set
        {
            if (value == _error)
            {
                return;
            }
            _error = value;
            OnPropertyChanged("Error");
        }
    }

    protected uint ComponentNodeHandle { get; set; }

    public int ComponentId { get; set; }

    private static void HandleContainerChildrenChanged()
    {
    }

    protected virtual ICalculatable LookupCalculatableWrapper(ICalculatable calc)
    {
        return GetWrapper(calc);
    }

    protected virtual void SetErrorFallback(EngineError error)
    {
        SetChildError(null);
        SetComponentError(error);
    }

    private static IOleBaseComponentItemManager _oleComponentItemMgr;

    protected readonly Dictionary<ICalculatable, IOleBaseComponentCalculatableWrapper> CalculatableWrapperMap = new Dictionary<ICalculatable, IOleBaseComponentCalculatableWrapper>();

    internal const uint InputSectionIndex = 0u;

    internal const uint OutputSectionIndex = 1u;

    private const int SectionCount = 2;

    private EngineError _error;

    public event Action<IOleBaseComponentCalculatableWrapper> OleComponentChildErrorChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public event Action<IOleContainerItem> OleContainerItemAdded;

    protected internal static IOleBaseComponentItemManager OleComponentItemMgr
    {
        get
        {
            if (!(_oleComponentItemMgr?.IsConnected ?? false))
            {
                return _oleComponentItemMgr = AppServiceRegistry.GetService<IOleBaseComponentItemManager>();
            }
            return _oleComponentItemMgr;
        }
    }

    internal IEnumerable<IWhiteboardItem> Inputs => WhiteboardItems.Where(IsInputItem);

    internal IEnumerable<IWhiteboardItem> Outputs => WhiteboardItems.Where(IsOutputItem);

    public bool IsErrorFromChild
    {
        get
        {
            if (Error != null)
            {
                return Error.ErrorSource is IOleBaseComponentCalculatableWrapper;
            }
            return false;
        }
    }

    protected abstract IEnumerable<string> ComponentErrors { get; }

    public IEnumerable<IOleBaseComponentCalculatableWrapper> CalculatableWrappers => CalculatableWrapperMap.Values;

    protected OleBaseComponentModel()
        : base(new OleBaseItemPositionComparer())
    {
        WhiteboardItems.ParentSupportsInputsOutputs = false;
    }

    public override string GetBlockSectionName(uint sectionIdx)
    {
        try
        {
            OleBaseComponentHeaderKeys key = (OleBaseComponentHeaderKeys)sectionIdx;
            return (string)Resources[key];
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
            return string.Empty;
        }
    }

    public override IEnumerable<IWhiteboardItem> GetBlockSectionItems(uint sectionIdx)
    {
        switch (sectionIdx)
        {
            case 0u:
                return Inputs;
            case 1u:
                return Outputs;
            default:
                throw new IndexOutOfRangeException("OLE component contains 2 sections");
        }
    }

    public override double GetBlockSectionMinHeightRatio(uint sectionIdx)
    {
        if (sectionIdx <= 1)
        {
            return 0.25;
        }
        throw new IndexOutOfRangeException("OLE component contains 2 sections");
    }

    protected bool IsInputItem(IWhiteboardItem item)
    {
        IOleBaseComponentCalculatableWrapper wrapper = GetWrapper(item as ICalculatable);
        if (wrapper != null)
        {
            return wrapper.IsInputEquation;
        }
        return false;
    }

    protected bool IsOutputItem(IWhiteboardItem item)
    {
        IOleBaseComponentCalculatableWrapper wrapper = GetWrapper(item as ICalculatable);
        if (wrapper != null)
        {
            return wrapper.IsOutputEquation;
        }
        return false;
    }

    public IOleBaseComponentCalculatableWrapper GetWrapper(ICalculatable calc)
    {
        if (calc == null)
        {
            return null;
        }
        if (DebugTools.DevAssert(!(calc is IOleBaseComponentCalculatableWrapper), null).Failed)
        {
            return calc as IOleBaseComponentCalculatableWrapper;
        }
        if (CalculatableWrapperMap.TryGetValue(calc, out var value))
        {
            return value;
        }
        return null;
    }

    protected void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public override void AddItemAtLocation(IWhiteboardItem item, Point location)
    {
        OleComponentItemMgr.SetItemOleComponentLocation(item, location);
        InsertItemAtAppropriatePosition(item);
    }

    protected override Point GetItemLocation(IWhiteboardItem item)
    {
        return OleComponentItemMgr.GetItemOleComponentLocation(item);
    }

    public override void SetItemLocation(IWhiteboardItem item, Point location)
    {
        if (WhiteboardModel.WhiteboardItemMgr.GetItemLocation(item) == location)
        {
            return;
        }
        WhiteboardModel.WhiteboardItemMgr.SetItemLocation(item, location);
    }

    protected abstract void UpdateCustomErrorBasedOnPlacement(IWhiteboardItem item, bool placedAfterOleContainer);

    internal void UpdateCustomErrorBasedOnPlacement(IWhiteboardItem item)
    {
        int num = WhiteboardItems.IndexOf(item);
        int num2 = WhiteboardItems.IndexOf(OleContainerItem);
        UpdateCustomErrorBasedOnPlacement(item, num2 != -1 && num > num2);
    }

    internal void UpdateAllErrors()
    {
        bool placedAfterOleContainer = false;
        foreach (IWhiteboardItem whiteboardItem in WhiteboardItems)
        {
            if (whiteboardItem is IOleContainerItem)
            {
                placedAfterOleContainer = true;
            }
            else
            {
                UpdateCustomErrorBasedOnPlacement(whiteboardItem, placedAfterOleContainer);
            }
        }
    }

    protected void SetComponentError(EngineError error)
    {
        if (error != null)
        {
            error.ErrorSource = null;
            error.NodeHandle = ComponentNodeHandle;
            Error = error;
            if (Error == error)
            {
                DebugTools.DevAssert(!IsErrorFromChild, null);
            }
            return;
        }
        Error = null;
        UpdateErrorAccordingToChildren();
    }

    protected void HandleChildWrapperErrorChanged(IOleBaseComponentCalculatableWrapper wrapper, EngineError newError)
    {
        UpdateErrorAccordingToChildren();
        if (DebugTools.AssertNotNull(OleComponentChildErrorChanged, null).Passed)
        {
            OleComponentChildErrorChanged(wrapper);
        }
    }

    protected abstract void SetErrorByNodeHandle(EngineError error);

    public override void SetError(EngineError error)
    {
        OleContainerItem.OnEngineError(error);
        if (error == null)
        {
            SetChildError(null);
            SetComponentError(null);
            return;
        }
        IEnumerable<IOleBaseComponentCalculatableChild> calculatableFragments = CalculatableFragments;
        IOleBaseComponentCalculatableChild oleBaseComponentCalculatableChild = error.ErrorSource as IOleBaseComponentCalculatableChild;
        bool flag = error.ErrorSource is ICalculatableAggregateWithComponent;
        if (ComponentErrors.Contains(error.ErrorCode))
        {
            SetChildError(null);
            SetComponentError(error);
        }
        else if (calculatableFragments.Contains(error.ErrorSource))
        {
            SetChildError(error);
            SetComponentError(null);
        }
        else if (flag && error.NodeHandle != 0 && error.NodeHandle != ComponentNodeHandle)
        {
            SetErrorByNodeHandle(error);
        }
        else if (error.OriginalError != null && calculatableFragments.Contains(error.OriginalError.ErrorSource))
        {
            DebugTools.DevAssert(flag, null);
            SetChildError(error.OriginalError);
            SetComponentError(null);
        }
        else if (oleBaseComponentCalculatableChild == null || oleBaseComponentCalculatableChild.IsConnectedToTheEngine)
        {
            DebugTools.DevAssert(flag, null);
            SetErrorFallback(error);
        }
    }

    protected override void AddItems(IEnumerable<IWhiteboardItem> items)
    {
        base.AddItems(items);
    }

    protected override void FireCalculatableAdded(ICalculatable calculatable)
    {
        base.FireCalculatableAdded(calculatable);
    }

    protected override void FireCalculatableRemoved(ICalculatable calculatable)
    {
        base.FireCalculatableRemoved(calculatable);
    }

    protected override void FireCalculatableMoved(ICalculatable calculatable)
    {
        base.FireCalculatableMoved(calculatable);
    }
}
