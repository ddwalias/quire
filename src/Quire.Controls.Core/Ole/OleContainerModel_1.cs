#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.FunctionalitiesLimitation;
using Quire.OLE;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Core.Ole;

public abstract partial class OleContainerModel<TPersistentData> : IPublicOleContainerModel<TPersistentData>, INotifyPropertyChanged
    where TPersistentData : IOleObjectPersistentData
{
    private readonly IOleSiteManager _oleSiteManager;
    private readonly TemporaryStorageHelper _temporaryStorageHelper = new TemporaryStorageHelper();
    protected readonly Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
    private readonly object _lock = new object();
    private string _worksheetName;
    private string _displayFilePath;
    private readonly OleSizeDataWithNotification _sizeData;
    private MessageList _messageList = new MessageList();

    protected IOleSiteManager SiteManager => _oleSiteManager;

    public string WorksheetName
    {
        get => _worksheetName;
        set
        {
            _worksheetName = value;
            SetHostNamesIfRunning(WpfUtils.ApplicationFullName, _worksheetName);
        }
    }

    public string DisplayFilePath
    {
        get => _displayFilePath;
        set
        {
            _displayFilePath = value;
            _oleSiteManager.SetOleContainerPath(value);
        }
    }

    private event Action Unlocked;

    protected internal event Action OleSavedAsync;

    protected internal event Action OleViewChangedAsync;

    protected internal event Action OleShownAsync;

    protected internal event Action OleHiddenAsync;

    protected internal event Action OleClosedAsync;

    protected internal event Action OleEditingStartedAsync;

    protected internal event Action OleEditingEndedAsync;

    protected OleContainerModel(IOleSiteManager oleSiteManager, bool autoResize)
    {
        _oleSiteManager = oleSiteManager;
        _sizeData = new OleSizeDataWithNotification(autoResize);
        SizeData.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
        {
            OnPropertyChanged(args.PropertyName);
        };
        OleSavedSync += () =>
        {
            if (OleSavedAsync != null)
            {
                Dispatcher.BeginInvoke((Delegate)OleSavedAsync, (DispatcherPriority)10, Array.Empty<object>());
            }
        };
        OleViewChangedSync += () =>
        {
            if (OleViewChangedAsync != null)
            {
                Dispatcher.BeginInvoke((Delegate)OleViewChangedAsync, (DispatcherPriority)10, Array.Empty<object>());
            }
        };
        OleShownSync += () =>
        {
            if (OleShownAsync != null)
            {
                Dispatcher.BeginInvoke((Delegate)OleShownAsync, (DispatcherPriority)10, Array.Empty<object>());
            }
        };
        OleHiddenSync += () =>
        {
            if (OleHiddenAsync != null)
            {
                Dispatcher.BeginInvoke((Delegate)OleHiddenAsync, (DispatcherPriority)10, Array.Empty<object>());
            }
        };
        OleClosedSync += OnOleClosedSync;
        OleEditingStartedSync += () =>
        {
            if (OleEditingStartedAsync != null)
            {
                Dispatcher.BeginInvoke((Delegate)OleEditingStartedAsync, (DispatcherPriority)10, Array.Empty<object>());
            }
        };
        OleEditingEndedSync += () =>
        {
            if (OleEditingEndedAsync != null)
            {
                Dispatcher.BeginInvoke((Delegate)OleEditingEndedAsync, (DispatcherPriority)10, Array.Empty<object>());
            }
        };
    }

    protected virtual void OnOleClosedSync()
    {
        if (OleClosedAsync != null)
        {
            Dispatcher.BeginInvoke((Delegate)OleClosedAsync, (DispatcherPriority)10, Array.Empty<object>());
        }
    }

    protected OleSizeDataWithNotification SizeData => _sizeData;

    public bool AutoResize
    {
        get => SizeData.AutoResize;
        set => SizeData.AutoResize = value;
    }

    public Size OleViewportSize
    {
        get => SizeData.OleViewportSize;
        // Parked: Mathcad's original compiler allocated the struct temporary in slot 0 before user variables num and num2; modern Roslyn always orders user locals before expression temporaries (slot 2 or 3) and adds a defensive copy when using assignment expressions.
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public Size AutomaticSize => SizeData.AutomaticSize;

    public void CopyFrom(IOleSizeData data, bool notify)
    {
        SizeData.CopyFrom(data, notify);
    }

    public IOleSizeData SizeDataForXamlSerialization
    {
        get => new OleSizeDataForXamlSerialization(SizeData);
        set => CopyFrom(value, false);
    }

    public bool AttemptGetVisibleSize(bool forceRun)
    {
        bool result = true;
        Size visibleSize = GetVisibleSize(forceRun);
        if (!visibleSize.IsEmpty)
        {
            OleViewportSize = visibleSize;
        }
        else
        {
            result = false;
        }
        return result;
    }

    public bool HasFakeOleViewportSize => SizeData.HasFakeOleViewportSize;

    public virtual string ProgId => _oleSiteManager.ProgId;

    protected virtual void OnStartRunning()
    {
        SetHostNamesIfRunning(WpfUtils.ApplicationFullName, WorksheetName);
    }

    protected virtual string[] ExcludeElementsNames => null;

    protected void AssertDispatcherAccess()
    {
        if (!Dispatcher.CheckAccess())
        {
            DebugTools.FailAssertion("Dispatcher Access!!!");
        }
    }

    protected internal event Action OleSavedSync
    {
        add => _oleSiteManager.Saved += value;
        remove => _oleSiteManager.Saved -= value;
    }

    protected internal event Action OleViewChangedSync
    {
        add => _oleSiteManager.ViewChanged += value;
        remove => _oleSiteManager.ViewChanged -= value;
    }

    protected internal event Action OleShownSync
    {
        add => _oleSiteManager.Shown += value;
        remove => _oleSiteManager.Shown -= value;
    }

    protected internal event Action OleHiddenSync
    {
        add => _oleSiteManager.Hidden += value;
        remove => _oleSiteManager.Hidden -= value;
    }

    protected internal event Action OleDataChangedSync
    {
        add => _oleSiteManager.DataChanged += value;
        remove => _oleSiteManager.DataChanged -= value;
    }

    protected internal event Action OleClosedSync
    {
        add => _oleSiteManager.Closed += value;
        remove => _oleSiteManager.Closed -= value;
    }

    protected internal event Action OleEditingStartedSync
    {
        add => _oleSiteManager.EditingStarted += value;
        remove => _oleSiteManager.EditingStarted -= value;
    }

    protected internal event Action OleEditingEndedSync
    {
        add => _oleSiteManager.EditingEnded += value;
        remove => _oleSiteManager.EditingEnded -= value;
    }

    public OleResult Run()
    {
        AssertDispatcherAccess();
        OleResult result = OleResult.Success;
        bool flag = IsOleSiteManagerRunning;
        if (!flag)
        {
            WpfUtils.DoWork(() =>
            {
                result = _oleSiteManager.Run();
            });
        }
        if (IsDisconnected)
        {
            Deactivate(false);
        }
        if (!IsOleSiteManagerRunning)
        {
            flag = false;
            WpfUtils.DoWork(() =>
            {
                result = _oleSiteManager.Run();
            });
        }
        if (!flag && IsOleSiteManagerRunning)
        {
            OnStartRunning();
        }
        if (DebugTools.IsDebug)
        {
            DebugTools.DevAssert(result == OleResult.Success == IsOleSiteManagerRunning && (!IsOleSiteManagerRunning || !IsDisconnected), null);
        }
        return result;
    }

    protected virtual bool IsDisconnected => false;

    protected object GetOleObject(bool forceRun, bool ignoreRunningState = false)
    {
        if (forceRun)
        {
            Run();
        }
        else
        {
            AssertDispatcherAccess();
        }
        if (!(forceRun | ignoreRunningState) && !IsOleSiteManagerRunning)
        {
            return null;
        }
        return _oleSiteManager.GetOleObject(forceRun);
    }

    private bool SetExtent(System.Drawing.Size size, bool forceRun)
    {
        if (forceRun)
        {
            Run();
        }
        bool result = false;
        if (IsOleSiteManagerRunning)
        {
            Guid classId = ClassId;
            if (classId == Guid.Empty || !OleUtils.IsMicrosoftEquationGuid(classId))
            {
                result = _oleSiteManager.SetExtent(size.Width, size.Height);
            }
        }
        return result;
    }

    private System.Drawing.Size GetExtent(bool forceRun)
    {
        DebugTools.LogData("OleContainerModel.GetExtent: Start");
        if (!SiteManager.SiteCreated)
        {
            return default;
        }
        if (forceRun)
        {
            Run();
        }
        if (!IsOleSiteManagerRunning)
        {
            return default;
        }
        SiteManager.GetExtent(out int width, out int height);
        return new System.Drawing.Size(width, height);
    }

    public virtual bool IsOleSiteManagerRunning
    {
        get
        {
            if (SiteCreated)
            {
                return _oleSiteManager.IsRunning;
            }
            return false;
        }
    }

    public bool IsOutOfPlaceActive
    {
        get
        {
            if (IsOleSiteManagerRunning)
            {
                return _oleSiteManager.IsOutOfPlaceActive;
            }
            return false;
        }
    }

    public bool IsOleAppSupportZoom
    {
        get
        {
            if (SiteCreated)
            {
                return OleUtils.IsOleAppSupportZoom(_oleSiteManager.ProgId);
            }
            return false;
        }
    }

    private void SetHostNamesIfRunning(string appName, string docName)
    {
        if (IsOleSiteManagerRunning)
        {
            _oleSiteManager.SetHostNames(appName, docName);
        }
    }

    internal nint CreateHBitmap(Int32Rect rect, nint winHandle)
    {
        if (DebugTools.DevAssert(SiteCreated, null).Failed)
        {
            return IntPtr.Zero;
        }
        return _oleSiteManager.CreateHBitmap(rect, winHandle);
    }

    public bool SiteCreated => _oleSiteManager.SiteCreated;

    public bool CreateOleFromProgId(string progId, string oleContainerPath)
    {
        bool flag = _oleSiteManager.CreateSiteFromProgID(progId, oleContainerPath);
        if (flag)
        {
            if (DebugTools.DevAssert(IsOleSiteManagerRunning, null).Passed)
            {
                OnStartRunning();
            }
            else
            {
                Run();
            }
        }
        return flag;
    }

    public bool CreateOleFromFile(string filePath, bool link, string oleContainerPath)
    {
        bool flag = _oleSiteManager.CreateSiteFromFile(filePath, link, oleContainerPath);
        if (flag)
        {
            if (IsOleSiteManagerRunning)
            {
                OnStartRunning();
            }
            else
            {
                Run();
            }
        }
        return flag;
    }

    public bool CreateOleWithDialog(nint winHandler, string dialogTitle, string oleContainerPath, out bool isNew, out bool isLink, out bool isCanceled)
    {
        bool flag = _oleSiteManager.CreateSiteWithDialog(winHandler, dialogTitle, oleContainerPath, out isNew, out isLink, out isCanceled);
        if (flag)
        {
            if (IsOleSiteManagerRunning)
            {
                OnStartRunning();
            }
            else if (!isCanceled)
            {
                DebugTools.DevAssert(!isNew, null);
                Run();
            }
        }
        return flag;
    }

    public void SetVisibleSize(bool forceRun, bool isPostCreate = false)
    {
        if (DebugTools.DevAssert(Width.IsValid() && Height.IsValid(), null).Failed)
        {
            return;
        }
        Size size = new Size(Width, Height);
        DebugTools.LogData(string.Format("OleContainerModel.SetVisibleSize: {0} => {1}: Start", OleViewportSize, size));
        int num = WpfUtils.DipsToHimetric(Width);
        int num2 = WpfUtils.DipsToHimetric(Height);
        if (!isPostCreate
            && !OleViewportSize.IsEmpty
            && num == WpfUtils.DipsToHimetric(OleViewportSize.Width)
            && num2 == WpfUtils.DipsToHimetric(OleViewportSize.Height))
        {
            return;
        }
        try
        {
            try
            {
                if (CheckServerAvailable(forceRun, (IsOleSiteManagerRunning | forceRun) ? OleContainerMessageKeysDict.ResizeMessageKeys : null))
                {
                    SetExtent(new System.Drawing.Size(num, num2), forceRun);
                }
            }
            catch (COMException ex)
            {
                DebugTools.FailAssertion(ex.Message);
            }
        }
        finally
        {
            // Release IL requires a no-op finally region.
            goto L;
            L:;
        }
    }

    public Size GetVisibleSize(bool forceRun)
    {
        System.Drawing.Size extent = GetExtent(forceRun);
        if (!extent.IsEmpty)
        {
            return new Size(WpfUtils.HimetricToDips(extent.Width), WpfUtils.HimetricToDips(extent.Height));
        }
        return Size.Empty;
    }

    public void UpdateOleSizeIfNeeded()
    {
        if (!AutoResize)
        {
            SetVisibleSize(false);
            if (AsyncUpdateVisibleSize)
            {
                AttemptGetVisibleSize(false);
            }
        }
    }

    public virtual bool AsyncUpdateVisibleSize => false;

    public void UpdateLink()
    {
        OleResult result = OleResult.UnknownFailure;
        WpfUtils.DoWork(() =>
        {
            result = _oleSiteManager.UpdateLink();
        });
        UpdateOleMessageIfNeeded(result, OleContainerMessageKeysDict.UpdateMessageKeys);
    }

    private void UpdateOleMessageIfNeeded(OleResult oleResult, Dictionary<OleResult, string> messageKeys)
    {
        if (messageKeys == null)
        {
            return;
        }
        if (oleResult == OleResult.Success)
        {
            ClearMessage(MessageCategory.OleServerError);
            return;
        }
        if (oleResult == OleResult.MissingData)
        {
            SetMessage(MessageCategory.OleServerError, NodeMessageAdornerType.Error, messageKeys[oleResult]);
            return;
        }
        if ((oleResult == OleResult.UnknownFailure || oleResult == OleResult.AppNotRegistered) && GetType().Name == "ChartContainerModel")
        {
            if (ApplicationVariant.IsEngNbPlus == true)
            {
                SetMessage(MessageCategory.OleServerError, NodeMessageAdornerType.Warning, "engineering_notebook_plus_warning");
            }
            if (ApplicationVariant.IsEngNb == true)
            {
                SetMessage(MessageCategory.OleServerError, NodeMessageAdornerType.Warning, "engineering_notebook_warning");
                return;
            }
        }
        else
        {
            SetMessage(MessageCategory.OleServerError, NodeMessageAdornerType.Warning, messageKeys[oleResult]);
        }
    }

    public virtual void Dispose()
    {
        _oleSiteManager.Dispose();
    }

    public double Width { get; set; }

    public double Height { get; set; }

    protected virtual OleResult CheckServerStatus(bool forceRun)
    {
        OleResult oleResult;
        if (forceRun)
        {
            oleResult = Run();
            DebugTools.DevAssert(oleResult != OleResult.Success || IsOleSiteManagerRunning, null);
        }
        else
        {
            oleResult = OleResult.Success;
        }
        return oleResult;
    }

    public bool CheckServerAvailable(bool forceRun, Dictionary<OleResult, string> messageKeys = null)
    {
        OleResult oleResult = CheckServerStatus(forceRun);
        UpdateOleMessageIfNeeded(oleResult, messageKeys);
        return oleResult == OleResult.Success;
    }

    protected abstract EnumFunctionalityKey ToggleActivateStateFunctionalityKey { get; }

    public virtual void Activate(bool isNew)
    {
        if (!FunctionalityService.Instance.IsFunctionalityEnabled(ToggleActivateStateFunctionalityKey))
        {
            return;
        }
        TryDoingActionWithLock(() =>
        {
            if (!BeforeActivate())
            {
                return;
            }
            DoActivate(isNew);
            AfterActivate();
        }, true);
    }

    public void Deactivate(bool mustTakeLock = true)
    {
        Unlocked -= DoDeactivate;
        if (!TryDoingActionWithLock(DoDeactivate, false, mustTakeLock))
        {
            Unlocked += DoDeactivate;
        }
    }

    protected virtual bool BeforeActivate()
    {
        if (!CheckServerAvailable(true, OleContainerMessageKeysDict.ActivateMessageKeys))
        {
            return false;
        }
        DebugTools.DevAssert(IsOleSiteManagerRunning, null);
        UpdateOleSizeIfNeeded();
        return true;
    }

    protected virtual void DoActivate(bool isNew)
    {
        if (!isNew && ProgId == "Package")
        {
            WpfUtils.DoWork(() =>
            {
                _oleSiteManager.ActivatePrimary();
            });
        }
        else
        {
            WpfUtils.DoWork(() =>
            {
                _oleSiteManager.ActivateOutOfPlace();
            });
        }
    }

    protected virtual void AfterActivate()
    {
    }

    protected virtual void DoDeactivate()
    {
        if (!SiteCreated)
        {
            return;
        }
        _oleSiteManager.Deactivate();
    }

    public void AbortDeactivate()
    {
        Unlocked -= DoDeactivate;
    }

    public string Save(Action onServerBusy)
    {
        if (!CheckServerAvailable(false))
        {
            onServerBusy?.Invoke();
            return string.Empty;
        }
        bool saveResult = true;
        _temporaryStorageHelper.SaveIfFileNameNullOrEmpty((string fileName) =>
        {
            saveResult = _oleSiteManager.Save(fileName, ExcludeElementsNames);
        });
        if (!saveResult)
        {
            onServerBusy?.Invoke();
            return string.Empty;
        }
        return StorageFileName;
    }

    public string SaveBitmap(Func<BitmapSource> getImageSource)
    {
        _temporaryStorageHelper.SaveIfFileNameNullOrEmpty(getImageSource);
        return StorageFileName;
    }

    private string StorageFileName
    {
        get => _temporaryStorageHelper.StorageFileName;
        set => _temporaryStorageHelper.StorageFileName = value;
    }

    public virtual Dictionary<Guid, Guid> GuidsConversionDict => null;

    public virtual bool Load(string storageFileName)
    {
        DebugTools.DevAssert(StorageFileName == null, null);
        StorageFileName = storageFileName;
        return _oleSiteManager.Load(storageFileName, GuidsConversionDict);
    }

    public bool IsStorageEmpty => StorageFileName == null;

    private Guid _guid = Guid.Empty;

    public Guid ClassId
    {
        get
        {
            if (_guid == Guid.Empty && SiteManager.SiteCreated)
            {
                IPersist persist = GetOleObject(false, true) as IPersist;
                if (DebugTools.AssertNotNull(persist, null).Passed)
                {
                    _guid = persist.GetClassID();
                }
            }
            return _guid;
        }
    }

    public BitmapSource LoadBitmap(string filename)
    {
        return _temporaryStorageHelper.LoadBitmap(filename);
    }

    public void CleanStorage(bool deleteFile)
    {
        _temporaryStorageHelper.CleanStorage(deleteFile);
    }

    public abstract IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, TPersistentData data);

    public abstract TPersistentData GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy);

    protected string Pack(IS11NProvider s11NProvider, string uniqueId, ExternalObjectType objectType, Action onServerBusy)
    {
        DebugTools.DevAssert(IsStorageEmpty, null);
        string sourceFileName = Save(onServerBusy);
        string result = s11NProvider.PackExternalObject(sourceFileName, uniqueId, objectType);
        CleanStorage(true);
        return result;
    }

    protected void Unpack(IDeS11NProvider d11NProvider, string itemidref)
    {
        if (!Load(d11NProvider.UnpackExternalObject(itemidref)))
        {
            throw new Exception();
        }
        CleanStorage(true);
    }

    public bool Lock(bool wait, bool checkOleActiveState)
    {
        bool lockTaken = false;
        if (wait)
        {
            Monitor.Enter(_lock, ref lockTaken);
        }
        else
        {
            Monitor.TryEnter(_lock, ref lockTaken);
        }
        if (lockTaken)
        {
            bool flag = true;
            try
            {
                flag = checkOleActiveState && IsOutOfPlaceActive;
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(_lock);
                    lockTaken = false;
                }
            }
        }
        return lockTaken;
    }

    public void Unlock()
    {
        Monitor.Exit(_lock);
        if (Dispatcher == null || Dispatcher.CheckAccess())
        {
            FireComponentUnlocked();
        }
        else
        {
            Dispatcher.BeginInvoke((DispatcherPriority)10, (Delegate)new Action(FireComponentUnlocked));
        }
    }

    private void FireComponentUnlocked()
    {
        AssertDispatcherAccess();
        try
        {
            if (Unlocked != null)
            {
                Unlocked();
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }
        finally
        {
            Unlocked = null;
        }
    }

    protected bool TryDoingActionWithLock(Action action, bool checkOleActiveState, bool mustTakeLock = true)
    {
        bool flag = false;
        try
        {
            flag = Lock(false, checkOleActiveState);
            if (flag || !mustTakeLock)
            {
                action();
                return true;
            }
        }
        finally
        {
            if (flag)
            {
                Unlock();
            }
        }
        return false;
    }

    public MessageList MessageList
    {
        get => _messageList;
        set => _messageList = value;
    }

    public void SetMessage(MessageCategory category, NodeMessageAdornerType type, string messageKey)
    {
        ClearMessage(category);
        if (DebugTools.DevAssert(!messageKey.IsNullOrEmpty(), null).Failed)
        {
            return;
        }
        _messageList.Add(new MessageItem
        {
            MessageResourceKey = messageKey,
            MessageCategory = category,
            MessageType = type
        });
    }

    public void ClearMessage(MessageCategory category)
    {
        MessageItem messageItem = _messageList.FirstOrDefault((MessageItem item) => item.MessageCategory == category);
        if (messageItem != null)
        {
            _messageList.Remove(messageItem);
        }
    }

    public void ClearMessages()
    {
        _messageList.Clear();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
