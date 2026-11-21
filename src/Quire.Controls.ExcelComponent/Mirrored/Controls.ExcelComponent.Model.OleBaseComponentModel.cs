using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Mathboard;
using Quire.Controls.Whiteboard;
using Quire.Services;

namespace Quire.Controls.ExcelComponent.Model;

public abstract partial class OleBaseComponentModel : AggregateMathboardModel, IOleBaseComponentModel, IAggregateMathboardModel, IMathboardModel, IWhiteboardModel, ICloneable, IDisposable, ICalculatableContainer, ICalculatableAggregate, ICalculatable, ICanContainSymbolicCalculation, IVerticalWhiteboardSections, INotifyPropertyChanged
{
    public static partial class PropertyNames
    {
        public const string Error = "Error";
    }

    private static IOleBaseComponentItemManager _oleComponentItemMgr;

    protected readonly Dictionary<ICalculatable, IOleBaseComponentCalculatableWrapper> CalculatableWrapperMap = new Dictionary<ICalculatable, IOleBaseComponentCalculatableWrapper>();

    internal const uint InputSectionIndex = 0u;

    internal const uint OutputSectionIndex = 1u;

    private const int SectionCount = 2;

    private EngineError _error;

    public event Action<IOleBaseComponentCalculatableWrapper> OleComponentChildErrorChanged;

    protected OleBaseComponentModel()
        : base(new OleBaseItemPositionComparer())
    {
        WhiteboardItems.ParentSupportsInputsOutputs = false;
    }

    static OleBaseComponentModel()
    {
        AppServiceRegistry.AddService<IOleBaseComponentItemManager>(new OleBaseComponentItemManager());
    }

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

    public ResourceDictionary Resources { get; set; }

    public override int BlockSectionCount => 2;

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

    internal virtual IOleContainerItem OleContainerItem { get; set; }

    internal IEnumerable<IWhiteboardItem> Inputs => WhiteboardItems.Where(IsInputItem);

    internal IEnumerable<IWhiteboardItem> Outputs => WhiteboardItems.Where(IsOutputItem);

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

    public event PropertyChangedEventHandler PropertyChanged;

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

    protected void UpdateErrorAccordingToChildren()
    {
        if (Error != null && !IsErrorFromChild)
        {
            return;
        }
        IOleBaseComponentCalculatableWrapper oleBaseComponentCalculatableWrapper = CalculatableWrappers.FirstOrDefault(wrapper => wrapper.Error != null);
        EngineError engineError = oleBaseComponentCalculatableWrapper?.Error;
        if (engineError != null && DebugTools.AssertNotNull(engineError.ErrorSource, null).Failed)
        {
            engineError.ErrorSource = oleBaseComponentCalculatableWrapper;
        }
        Error = engineError;
    }

    public void SetErrorInHiddenSection(bool hasHiddenErrors)
    {
        if (Error != null && !IsErrorFromChild && Error.ErrorCode != "ErrorsInHiddenSection.Error.Key")
        {
            return;
        }
        if (Error != null && Error.ErrorCode == "ErrorsInHiddenSection.Error.Key")
        {
            if (!hasHiddenErrors && DebugTools.DevAssert(!IsErrorFromChild, null).Passed)
            {
                SetComponentError(null);
            }
            return;
        }
        if (hasHiddenErrors)
        {
            SetComponentError(new EngineError
            {
                Message = "ErrorsInHiddenSection.Error.Key",
                IsRuntime = true,
                ResourceString = (string)Resources["ErrorsInHiddenSection.Error.Key"]
            });
        }
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

    protected virtual void SetErrorFallback(EngineError error)
    {
        SetChildError(null);
        SetComponentError(error);
    }

    protected uint ComponentNodeHandle { get; set; }

    protected abstract IEnumerable<string> ComponentErrors { get; }

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

    protected IEnumerable<IOleBaseComponentCalculatableChild> CalculatableFragments
    {
        get
        {
            foreach (IOleBaseComponentCalculatableChild calculatableChild in CalculatableChildren)
            {
                Pair<bool, bool> isFragment = calculatableChild.IsFragment;
                if (isFragment.Second)
                {
                    DebugTools.DevAssert(calculatableChild.Rl.StartsWith("(fragment") == isFragment.First, calculatableChild.GetType().Name + " : " + calculatableChild.Rl);
                }
                if (isFragment.First)
                {
                    yield return calculatableChild;
                }
            }
        }
    }

    protected virtual void SetChildError(EngineError error)
    {
        CalculatableFragments.Where(fragment => fragment.Error != null && !fragment.Error.IsPlaceholderError()).Each(fragment =>
        {
            fragment.SetError(null);
        });
        if (error == null)
        {
            return;
        }
        error.ErrorSource.SetError(error);
    }

    public IEnumerable<IOleBaseComponentCalculatableWrapper> CalculatableWrappers => CalculatableWrapperMap.Values;

    public int ComponentId { get; set; }

    public void UpdateCalculationPosition(IEnumerable<IWhiteboardItem> list)
    {
        List<IWhiteboardItem> itemsToUpdatePosition = new List<IWhiteboardItem>();
        WhiteboardItems.EachWithIndex((int idx, IWhiteboardItem item) =>
        {
            if (list.IndexOf(item) != idx)
            {
                itemsToUpdatePosition.Add(item);
            }
        });
        itemsToUpdatePosition.Each(UpdatePosition);
    }

    public event Action<IOleContainerItem> OleContainerItemAdded;

    protected override void AddItems(IEnumerable<IWhiteboardItem> items)
    {
        IOleContainerItem oleContainerItem = items.OfType<IOleContainerItem>().FirstOrDefault();
        if (oleContainerItem != null && OleContainerItemAdded != null)
        {
            OleContainerItemAdded(oleContainerItem);
        }
        base.AddItems(items);
    }

    protected virtual IEnumerable<ICalculatable> CalculatablesConnectedToTheEngine =>
        WhiteboardItems.OfType<ICalculatable>().Where(item => GetWrapper(item).IsConnectedToTheEngine);

    protected override void FireCalculatableAdded(ICalculatable calculatable)
    {
        IOleBaseComponentCalculatableChild oleBaseComponentCalculatableChild = calculatable as IOleBaseComponentCalculatableChild;
        if (DebugTools.AssertNotNull(oleBaseComponentCalculatableChild, null).Failed)
        {
            return;
        }
        if (DebugTools.DevAssert(!oleBaseComponentCalculatableChild.IsConnectedToTheEngine, null).Failed)
        {
            return;
        }
        oleBaseComponentCalculatableChild.IsConnectedToTheEngine = true;
        base.FireCalculatableAdded(calculatable);
    }

    protected override void FireCalculatableRemoved(ICalculatable calculatable)
    {
        IOleBaseComponentCalculatableChild oleBaseComponentCalculatableChild = calculatable as IOleBaseComponentCalculatableChild;
        if (DebugTools.AssertNotNull(oleBaseComponentCalculatableChild, null).Failed)
        {
            return;
        }
        if (DebugTools.DevAssert(oleBaseComponentCalculatableChild.IsConnectedToTheEngine, null).Failed)
        {
            return;
        }
        base.FireCalculatableRemoved(calculatable);
        oleBaseComponentCalculatableChild.IsConnectedToTheEngine = false;
    }

    protected override void FireCalculatableMoved(ICalculatable calculatable)
    {
        IOleBaseComponentCalculatableChild oleBaseComponentCalculatableChild = calculatable as IOleBaseComponentCalculatableChild;
        if (DebugTools.AssertNotNull(oleBaseComponentCalculatableChild, null).Failed)
        {
            return;
        }
        if (oleBaseComponentCalculatableChild.IsConnectedToTheEngine)
        {
            base.FireCalculatableMoved(calculatable);
        }
    }

    protected abstract ICalculatable CreateCalculatableWrapper(ICalculatable calc, bool arg2);

    protected virtual ICalculatable LookupCalculatableWrapper(ICalculatable calc)
    {
        return GetWrapper(calc);
    }

    protected abstract void RemoveCalculatableWrapper(ICalculatable calc);

    private void HandleChildCalcHiddenStateChanged(ICalculatable calc, bool arg2)
    {
        if (calc is IOleBaseComponentCalculatableWrapper oleBaseComponentCalculatableWrapper)
        {
            FireCalculatableHiddenStatusChanged(oleBaseComponentCalculatableWrapper.WrappedCalculatable as ICalculationHideableChild);
        }
    }

    private static void HandleContainerChildrenChanged()
    {
    }

    protected override CalculatableChildHider InitializeChildManager()
    {
        return new WrappedChildManager(CalculatablesConnectedToTheEngine, FireCalculatableAdded, FireCalculatableRemoved, FireCalculatableMoved, CreateCalculatableWrapper, LookupCalculatableWrapper, RemoveCalculatableWrapper, HandleChildCalcHiddenStateChanged, HandleContainerChildrenChanged);
    }

    public override void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
        DebugTools.FailAssertion(null, Array.Empty<object>());
    }
}
