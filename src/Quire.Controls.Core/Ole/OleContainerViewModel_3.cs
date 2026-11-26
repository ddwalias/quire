#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Undo;

namespace Quire.Controls.Core.Ole;

public abstract partial class OleContainerViewModel<TModel, TIPublicModel, TPersistentData> : OleContainerViewModelBase, IOleContainerViewModel
    where TModel : OleContainerModel<TPersistentData>, TIPublicModel
    where TIPublicModel : IPublicOleContainerModel<TPersistentData>
    where TPersistentData : IOleObjectPersistentData
{
    public static partial class PropertyNames
    {
        public const string AutoResize = "AutoResize";
        public const string AutomaticSize = "AutomaticSize";
        public const string GrayVisibility = "GrayVisibility";
        public const string Height = "Height";
        public const string Width = "Width";
        public const string MinHeight = "MinHeight";
        public const string MinWidth = "MinWidth";
        public const string MaxHeight = "MaxHeight";
        public const string MaxWidth = "MaxWidth";
    }

    protected readonly TModel Model;

    public TIPublicModel PublicModel => (TIPublicModel)Model;

    public event Action ViewChangedSync;
    public event Action ViewChangedAsync;
    public event Action SavedSync;
    public event Action SavedAsync;
    public event Action RequestResizeToFit;
    public event Action BeforeActiveStateToggled;

    protected OleContainerViewModel(TModel model, Size defaultMinSize)
    {
        Model = model;
        Model.OleViewChangedAsync += OnOleViewChangedAsync;
        Model.OleSavedAsync += OnOleSavedAsync;
        Model.OleEditingStartedAsync += OnOleEditingStartedAsync;
        Model.OleEditingEndedAsync += OnOleEditingEndedAsync;
        Model.OleViewChangedSync += OnOleViewChangedSync;
        Model.OleSavedSync += OnOleSavedSync;
        Model.PropertyChanged += HandleModelPropertyChanged;
        DefaultMinSize = defaultMinSize;
        MinSizeValue = Size.Empty;
        Width = MinWidth;
        Height = MinHeight;
        CreateCommandBinding(OleContainerViewModelBase.ToggleActiveState, (object sender) =>
        {
            ToggleOutOfPlaceActiveState();
        });
    }

    private void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "AutomaticSize")
        {
            UpdateContainerSizeIfNeeded();
        }
        OnPropertyChanged(args.PropertyName);
    }

    public void UpdateOleSizeIfNeeded()
    {
        Model.UpdateOleSizeIfNeeded();
    }

    private void UpdateContainerSizeIfNeeded()
    {
        Size automaticSize = AutomaticSize;
        if (automaticSize.IsEmpty || DebugTools.DevAssert(automaticSize.Width != 0.0 && automaticSize.Height != 0.0, null).Failed)
        {
            MinSize = new Size(4.800000000000001, 4.800000000000001);
        }
        else
        {
            MaxWidth = double.PositiveInfinity;
            MaxHeight = double.PositiveInfinity;
            double num2;
            double num3;
            double num = automaticSize.Width / automaticSize.Height;
            if (automaticSize.Width.IsGreaterThan(automaticSize.Height))
            {
                num2 = 9.600000000000001;
                num3 = Math.Max(4.800000000000001, num2 / num);
            }
            else
            {
                num3 = 9.600000000000001;
                num2 = Math.Max(4.800000000000001, num3 * num);
            }
            MinSize = new Size(num2, num3);
            CoerceAndUpdateSize(Size.Empty, CalculateDiagonalLength(new Size(Width, Height)));
        }
        RequestResizeToFit?.Invoke();
    }

    public bool IsOleSiteRunning => Model.IsOleSiteManagerRunning;

    protected override void OnDispose()
    {
        Model.Dispose();
    }

    protected virtual bool MustTakeLockOnUserDeactivation => true;

    protected void ToggleOutOfPlaceActiveState()
    {
        BeforeActiveStateToggled?.Invoke();
        if (!IsOutOfPlaceActive)
        {
            Model.Activate(false);
        }
        else
        {
            Model.Deactivate(MustTakeLockOnUserDeactivation);
        }
    }

    public void AbortDeactivate()
    {
        Model.AbortDeactivate();
    }

    public bool IsOutOfPlaceActive => Model.IsOutOfPlaceActive;

    public bool IsCreated => SiteCreated;

    internal IntPtr CreateHBitmap(Int32Rect rect, IntPtr winHandle)
    {
        return Model.CreateHBitmap(rect, winHandle);
    }

    public MessageList MessageList
    {
        get => Model.MessageList;
        set => Model.MessageList = value;
    }

    public bool AutoResize
    {
        get => Model.AutoResize;
        protected set => Model.AutoResize = value;
    }

    public Size OleViewportSize
    {
        get => Model.OleViewportSize;
        protected set => Model.OleViewportSize = value;
    }

    public Size AutomaticSize => Model.AutomaticSize;

    public bool SiteCreated => Model.SiteCreated;

    public string WorksheetName
    {
        get => Model.WorksheetName;
        private set => Model.WorksheetName = value;
    }

    private string DisplayFilePath
    {
        get => Model.DisplayFilePath;
        set => Model.DisplayFilePath = value;
    }

    public void ClearMessage(MessageCategory category)
    {
        Model.ClearMessage(category);
    }

    public void SetMessage(MessageCategory category, NodeMessageAdornerType type, string messageKey)
    {
        Model.SetMessage(category, type, messageKey);
    }

    protected virtual void OnOleViewChangedAsync()
    {
        DebugTools.LogData(string.Format("OleContainerViewModel.OnOleViewChangedAsync: Start"));
        if (Model.AsyncUpdateVisibleSize)
        {
            AttemptUpdatingSizeOnViewChanged(false);
        }
        if (ViewChangedAsync != null)
        {
            ViewChangedAsync();
        }
    }

    protected virtual void OnOleSavedAsync()
    {
        Guid classId = Model.ClassId;
        if (classId != Guid.Empty && OleUtils.IsSupportedExcelGuid(classId) && Model.IsOutOfPlaceActive)
        {
            UpdateOleSizeIfNeeded();
        }
        if (SavedAsync != null)
        {
            SavedAsync();
        }
    }

    protected virtual void OnOleEditingStartedAsync()
    {
        FireGrayVisibilityChanged();
    }

    protected virtual void OnOleEditingEndedAsync()
    {
        FireGrayVisibilityChanged();
    }

    protected virtual void OnOleViewChangedSync()
    {
        DebugTools.LogData(string.Format("OleContainerViewModel.OnOleViewChangedSync: Start"));
        if (!Model.AsyncUpdateVisibleSize)
        {
            AttemptUpdatingSizeOnViewChanged(true);
        }
        if (ViewChangedSync != null)
        {
            ViewChangedSync();
        }
    }

    protected void AttemptUpdatingSizeOnViewChanged(bool assert)
    {
        Size size = new Size(Width, Height);
        Size oleViewportSize = OleViewportSize;
        if (Model.AttemptGetVisibleSize(false))
        {
            if (AutoResize && !oleViewportSize.IsEmpty)
            {
                Size size2 = new Size(Width, Height);
                double diagonalForUndo = CalculateDiagonalLength(size);
                double diagonalForRedo = CalculateDiagonalLength(size2);
                if (!diagonalForUndo.IsEqualTo(diagonalForRedo))
                {
                    CommandBuffer.Execute("OnOleViewChangedSync on OleObject", () => true, () =>
                    {
                        CoerceAndUpdateSize(Size.Empty, diagonalForUndo);
                    }, () =>
                    {
                        CoerceAndUpdateSize(Size.Empty, diagonalForRedo);
                    });
                    return;
                }
            }
        }
        else if (assert)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
    }

    protected virtual void OnOleSavedSync()
    {
        if (SavedSync != null)
        {
            SavedSync();
        }
    }

    internal void OleForceClose()
    {
        Model.Deactivate();
    }

    public virtual IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, TPersistentData data)
    {
        Model.CopyFrom(data, false);
        return Model.SetRegionPersistentData(d11NProvider, data);
    }

    public virtual TPersistentData GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy)
    {
        return Model.GetRegionPersistentData(s11NProvider, uniqueId, onServerBusy);
    }

    public void ExecutedToggleOleAutoResize(object parameter)
    {
        bool oldValue = AutoResize;
        bool newValue = ((parameter is bool) ? ((bool)parameter) : (!oldValue));
        if (oldValue == newValue)
        {
            return;
        }
        Size oldSize = new Size(Width, Height);
        Size newSize = Size.Empty;
        double diagonalForUndo = double.NaN;
        double diagonalForRedo = double.NaN;
        DelegateFunction0<bool> doMethod = () =>
        {
            Size oleViewportSize = OleViewportSize;
            if (DebugTools.DevAssert(!oleViewportSize.IsEmpty, null).Failed && !Model.AttemptGetVisibleSize(true))
            {
                return false;
            }
            diagonalForUndo = CalculateDiagonalLength(oldSize);
            AutoResize = newValue;
            UpdateOleSizeIfNeeded();
            newSize = new Size(Width, Height);
            diagonalForRedo = CalculateDiagonalLength(newSize);
            return true;
        };
        Delegate0 undoMethod = () =>
        {
            AutoResize = oldValue;
            CoerceAndUpdateSize(oldSize, diagonalForUndo);
            UpdateOleSizeIfNeeded();
        };
        Delegate0 redoMethod = () =>
        {
            AutoResize = newValue;
            CoerceAndUpdateSize(newSize, diagonalForRedo);
            UpdateOleSizeIfNeeded();
        };
        CommandBuffer.Execute("ToggleOleObjectAutoResize on OleObject", doMethod, undoMethod, redoMethod);
    }

    private bool _forceGrayVisibility;

    internal void ForceGrayVisibility()
    {
        _forceGrayVisibility = true;
        FireGrayVisibilityChanged();
    }

    public Visibility GrayVisibility
    {
        get
        {
            if (!_forceGrayVisibility && (!IsCreated || !IsOutOfPlaceActive))
            {
                return Visibility.Hidden;
            }
            return Visibility.Visible;
        }
    }

    private void FireGrayVisibilityChanged()
    {
        OnPropertyChanged("GrayVisibility");
    }

    public IOleSizeData SizeDataForXamlSerialization
    {
        get => Model.SizeDataForXamlSerialization;
        set => Model.SizeDataForXamlSerialization = value;
    }

    public virtual bool CreateOleWithDialog(IntPtr winHandler, string dialogTitle, string oleContainerPath, out bool isNew, out bool isLink, out bool isCanceled)
    {
        bool flag = Model.CreateOleWithDialog(winHandler, dialogTitle, oleContainerPath, out isNew, out isLink, out isCanceled);
        if (flag)
        {
            PostCreation(isNew, isLink, isNew);
        }
        return flag;
    }

    public virtual bool CreateOleFromProgId(string progId, bool activate, string oleContainerPath)
    {
        bool flag = Model.CreateOleFromProgId(progId, oleContainerPath);
        if (flag)
        {
            PostCreation(activate, false, true);
        }
        return flag;
    }

    public virtual bool CreateOleFromFile(string filePath, bool link, string oleContainerPath)
    {
        bool flag = Model.CreateOleFromFile(filePath, link, oleContainerPath);
        if (flag)
        {
            PostCreation(false, link, false);
        }
        return flag;
    }

    public bool CreateWindowlessOle()
    {
        bool flag = CreateOleFromProgId(Model.ProgId, false, null);
        if (flag)
        {
            PostCreationWindowless();
        }
        return flag;
    }

    protected virtual void PostCreationWindowless()
    {
        EstablishInitialSize();
    }

    private void PostCreation(bool activate, bool isLink, bool isNew)
    {
        PostCreation(isNew);
        if (isLink)
        {
            DebugTools.DevAssert(Model.IsOleSiteManagerRunning, null);
            DebugTools.DevAssert(!IsOutOfPlaceActive, null);
            DebugTools.DevAssert(!activate, null);
            Model.Deactivate();
        }
        if (activate && !IsOutOfPlaceActive)
        {
            Model.Activate(isNew);
        }
    }

    protected virtual void PostCreation(bool isNew)
    {
        DebugTools.DevAssert(!IsOutOfPlaceActive, "Object shouldn't be active");
        if (!Model.AttemptGetVisibleSize(false))
        {
            DebugTools.FailAssertionDoNotThrow("VisibleSize shouldn't be empty");
        }
    }

    public void UpdateLink()
    {
        Model.UpdateLink();
    }

    public double Height
    {
        get => Model.Height;
        set
        {
            if (Model.Height.IsEqualTo(value))
            {
                return;
            }
            if (IsStrictMinSize)
            {
                DebugTools.DevAssert(value.IsGreaterThanOrEqual(MinHeight), null);
            }
            Model.Height = value;
            OnPropertyChanged("Height");
        }
    }

    public double Width
    {
        get => Model.Width;
        set
        {
            if (Model.Width.IsEqualTo(value))
            {
                return;
            }
            if (IsStrictMinSize)
            {
                DebugTools.DevAssert(value.IsGreaterThanOrEqual(MinWidth), null);
            }
            Model.Width = value;
            OnPropertyChanged("Width");
        }
    }

    public Size DefaultMinSize { get; private set; }

    internal Size MinSizeValue { get; private set; }

    protected virtual bool IsStrictMinSize => true;

    public bool IsOleAppSupportZoom => Model.IsOleAppSupportZoom;

    public virtual Size MinSize
    {
        get
        {
            Size minSizeValue = MinSizeValue;
            if (!minSizeValue.IsEmpty)
            {
                return MinSizeValue;
            }
            return DefaultMinSize;
        }
        protected internal set
        {
            if (value.IsEqualTo(MinSizeValue))
            {
                return;
            }
            double minWidth = MinWidth;
            double minHeight = MinHeight;
            MinSizeValue = value;
            double minWidth2 = MinWidth;
            double minHeight2 = MinHeight;
            if (minWidth != minWidth2)
            {
                OnPropertyChanged("MinWidth");
            }
            if (minHeight != minHeight2)
            {
                OnPropertyChanged("MinHeight");
            }
        }
    }

    public double MinHeight
    {
        get
        {
            Size minSize = MinSize;
            return minSize.Height;
        }
    }

    public double MinWidth
    {
        get
        {
            Size minSize = MinSize;
            return minSize.Width;
        }
    }

    private double _maxHeight = double.PositiveInfinity;

    public double MaxHeight
    {
        get => _maxHeight;
        set
        {
            if (_maxHeight.IsEqualTo(value))
            {
                return;
            }
            _maxHeight = value;
            OnPropertyChanged("MaxHeight");
        }
    }

    private double _maxWidth = double.PositiveInfinity;

    public double MaxWidth
    {
        get => _maxWidth;
        set
        {
            if (_maxWidth.IsEqualTo(value))
            {
                return;
            }
            _maxWidth = value;
            OnPropertyChanged("MaxWidth");
        }
    }

    public abstract Size EstablishInitialSize();

    public bool CanToggleAutoResize => !HasFakeOleViewportSize;

    public bool CanResetSize
    {
        get
        {
            Size oleViewportSize = OleViewportSize;
            return !oleViewportSize.IsEmpty;
        }
    }

    public bool CanUpdateLink => true;

    public void ExecutedOleResetSize()
    {
        DelegateFunction0<bool> doMethod;
        Delegate0 undoMethod;
        Delegate0 redoMethod;
        Size oleViewportSize = OleViewportSize;
        if (DebugTools.DevAssert(!oleViewportSize.IsEmpty, null).Failed)
        {
            return;
        }
        Size oldSize = new Size(Width, Height);
        Size newSize = Size.Empty;
        double diagonalForUndo = CalculateDiagonalLength(oldSize);
        double diagonalForRedo = double.NaN;
        doMethod = () =>
        {
            bool flag;
            Size oleViewportSize2;
            if (!HasFakeOleViewportSize)
            {
                oleViewportSize2 = OleViewportSize;
                if (!DebugTools.DevAssert(!oleViewportSize2.IsEmpty, null).Failed)
                {
                    // Release IL requires the shared continuation branch.
                    goto IL_0063;
                }
            }
            Model.AttemptGetVisibleSize(true);
            oleViewportSize2 = OleViewportSize;
            if (oleViewportSize2.IsEmpty)
            {
                return false;
            }
            IL_0063:
            CoerceAndUpdateSize(OleViewportSize, CalculateDiagonalLength(OleViewportSize));
            newSize = new Size(Width, Height);
            diagonalForRedo = CalculateDiagonalLength(newSize);
            if (AutoResize)
            {
                flag = !diagonalForUndo.IsEqualTo(diagonalForRedo);
            }
            else
            {
                flag = !oldSize.IsEqualTo(new Size(Width, Height));
            }
            return flag;
        };
        undoMethod = () =>
        {
            CoerceAndUpdateSize(oldSize, diagonalForUndo);
        };
        redoMethod = () =>
        {
            CoerceAndUpdateSize(newSize, diagonalForRedo);
        };
        CommandBuffer.Execute("OleResetSize on OleObject", doMethod, undoMethod, redoMethod);
    }

    public virtual void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        IList list = propertyNames;
        if (list.Contains("WorksheetName"))
        {
            WorksheetName = newProperties.WorksheetName;
        }
        if (list.Contains("DisplayFilePath"))
        {
            DisplayFilePath = newProperties.DisplayFilePath;
        }
    }

    protected Size ApplySizeRestrictions(Size size)
    {
        return new Size(Math.Min(MaxWidth, Math.Max(MinWidth, size.Width)), Math.Min(MaxHeight, Math.Max(MinHeight, size.Height)));
    }

    protected bool HasFakeOleViewportSize => Model.HasFakeOleViewportSize;

    protected Size CoerceSize(Size size, double diagonal)
    {
        Size val = Size.Empty;
        if (!AutoResize)
        {
            if (DebugTools.DevAssert(!size.IsEmpty, null).Passed)
            {
                val = size;
            }
        }
        else
        {
            if (DebugTools.DevAssert(diagonal.IsValid() && !OleViewportSize.IsEmpty, null).Passed)
            {
                val = CalculateSize(OleViewportSize, diagonal);
            }
        }
        if (!val.IsEmpty)
        {
            return ApplySizeRestrictions(val);
        }
        return val;
    }

    private void CoerceAndUpdateSize(Size size, double diagonal)
    {
        Size val = CoerceSize(size, diagonal);
        if (val.IsEmpty)
        {
            return;
        }
        Width = val.Width;
        Height = val.Height;
    }

    protected double CalculateDiagonalLength(Size size)
    {
        return Math.Sqrt(Math.Pow(size.Width, 2.0) + Math.Pow(size.Height, 2.0));
    }

    protected Size CalculateSize(Size size, double diagonal)
    {
        double num = size.Width / size.Height;
        double num2 = Math.Sqrt(Math.Pow(diagonal, 2.0) / (Math.Pow(num, 2.0) + 1.0));
        return new Size(num * num2, num2);
    }
}
