using System.IO;
using System.Windows;
using System.Windows.Media;
using Quire.Core;

using System;
using Quire.Wpf;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Core.Ole;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.Serialization;
using Quire.PersistentData;
using Quire.Controls.ExcelComponent;
using Quire.Controls.Whiteboard;
using Quire.Calculation.Core;
using Quire.Calculation;
namespace Quire.Controls.ExcelComponent;

public partial class OleObjectControl
{
    public bool IsEmpty => false;

    public bool PreserveAspectRatio => OleObjectAutoResize;

    public FrameworkElement GetAdornedElement()
    {
        return View;
    }

    public virtual string GetHelpContext()
    {
        return "OleObjectControl";
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+SnapToPixelHelper, Ptc.Controls.ExcelComponent")]
    private partial class SnapToPixelHelper : ISnapToPixelHelper
    {
        private readonly OleObjectControl _oleObject;

        public SnapToPixelHelper(OleObjectControl oleObject)
        {
            _oleObject = oleObject;
        }

        public GeneralTransform TransformFromPoint
        {
            get
            {
                return this.CreateTransformFromChildPoint(_oleObject.View.SnappingHelper);
            }
        }

        public bool HasPointThatSnapsToPixel
        {
            get
            {
                return _oleObject.View.HasPointThatSnapsToPixel();
            }
        }

        public UIElement ItemElement
        {
            get
            {
                return _oleObject;
            }
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+CreationMethodWithDialog, Ptc.Controls.ExcelComponent")]
    public partial class CreationMethodWithDialog : OleObjectControlCreationMethod
    {
        public CreationMethodWithDialog(Window winHandle)
        {
            WinHandle = winHandle;
        }

        public Window WinHandle { get; private set; }

        public override bool Visit(OleObjectControl control, string oleContainerPath)
        {
            return control.Accept(this, oleContainerPath);
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+CreationMethodWithProgId, Ptc.Controls.ExcelComponent")]
    public partial class CreationMethodWithProgId : OleObjectControlCreationMethod
    {
        public CreationMethodWithProgId(string progId)
        {
            ProgId = progId;
        }

        public string ProgId { get; private set; }

        public override bool Visit(OleObjectControl control, string oleContainerPath)
        {
            return control.Accept(this, oleContainerPath);
        }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.OleObjectControl+CreationMethodFromFile, Ptc.Controls.ExcelComponent")]
    public partial class CreationMethodFromFile : OleObjectControlCreationMethod
    {
        private string _filePath;

        public CreationMethodFromFile(string filePath, bool link)
        {
            FilePath = filePath;
            Link = link;
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            private set
            {
                DebugTools.DevAssert(File.Exists(value), "The entered file path {0} is incorrect or doesn't exist.", value);
                _filePath = value;
            }
        }

        public bool Link { get; private set; }

        public override bool Visit(OleObjectControl control, string oleContainerPath)
        {
            return control.Accept(this, oleContainerPath);
        }
    }

    private void CanExecuteOleObjectResetSize(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanResetSize;
    }

    private void CanExecuteUpdateOleObjectLink(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanUpdateLink;
    }

    private void CanToggleOleObjectAutoResize(object sender, global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanToggleAutoResize;
    }

    private void ExecutedOleObjectResetSize(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        View.ViewModel.ExecutedOleResetSize();
    }

    private void ExecutedUpdateOleObjectLink(object sender, global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        View.ViewModel.UpdateLink();
    }

    public void CloseOle()
    {
        if (IsOleOpen)
        {
            View.StartOleCloseOperation();
        }
    }

    public global::System.Nullable<global::System.Windows.Vector> NavigateIntoItem(global::Quire.Direction direction)
    {
        Activate(isActive: true);
        return null;
    }

    public global::Quire.Controls.Core.IResizeData StoreResizeData(global::System.Windows.Size oldSize, global::System.Windows.Size newSize)
    {
        return View.ViewModel.StoreResizeData(oldSize, newSize);
    }

    public void RestoreResizeData(global::Quire.Controls.Core.IResizeData resizeData)
    {
        View.ViewModel.RestoreResizeData(resizeData);
    }

    public global::System.Windows.Size CoerceNewSize(global::System.Windows.Size newSize, global::Quire.Controls.Core.IResizeData resizeData)
    {
        return View.ViewModel.CoerceNewSize(newSize, resizeData);
    }

    private OleObject _view;

    private ISnapToPixelHelper _snappingHelper;

    public OleObject View
    {
        get
        {
            return _view;
        }
        private set
        {
            if (_view == value)
            {
                return;
            }
            if (_view != null)
            {
                _view.VisualParentChanged -= OnViewVisualParentChanged;
                _view.ViewModel.PropertyChanged -= HandleViewModelPropertyChanged;
                _view.ViewModel.RequestResizeToFit -= HandleViewModelRequestResizeToFit;
                BindingOperations.ClearBinding(_view, AttachedProperties.IsOnVisiblePageProperty);
            }
            _view = value;
            if (_view != null)
            {
                _view.VisualParentChanged += OnViewVisualParentChanged;
                _view.ViewModel.PropertyChanged += HandleViewModelPropertyChanged;
                _view.ViewModel.RequestResizeToFit += HandleViewModelRequestResizeToFit;
                BindingOperations.SetBinding(_view, AttachedProperties.IsOnVisiblePageProperty, new Binding
                {
                    Source = this,
                    Path = new PropertyPath(AttachedProperties.IsOnVisiblePageProperty)
                });
            }
        }
    }

    public double VerticalAlignmentOffset => 0.0;

    public bool IsOleOpen => View.IsOutOfPlaceActive;

    public bool OleObjectAutoResize => View.ViewModel.AutoResize;

    public bool CanResetSize => View.ViewModel.CanResetSize;

    public bool CanUpdateLink => View.ViewModel.CanUpdateLink;

    public bool CanToggleAutoResize => View.ViewModel.CanToggleAutoResize;

    // Placed before the properties that follow it: Release IL numbers the closure as <Activate>b__42_0.
    public void Activate(bool isActive)
    {
        if (isActive)
        {
            WpfUtils.ExecuteOnLoad(this, () =>
            {
                Focus();
            });
        }
    }

    public SubElementToResizeData SubElementToResizeData => new SubElementToResizeData(View, serializeDimensions: true, isFullSize: true);

    public ResizeToFitState ResizeState
    {
        get
        {
            if (!PreserveAspectRatio)
            {
                return ResizeToFitState.ResizingDisabled;
            }
            return ResizeToFitState.ResizingEnabled;
        }
    }

    public bool IsAdornedElementReady
    {
        get
        {
            UIElement view = View;
            if (view != null)
            {
                return view.GetVisualParent() != null;
            }
            return false;
        }
    }

    public ISnapToPixelHelper SnappingHelper => _snappingHelper ?? (_snappingHelper = new SnapToPixelHelper(this));

    public event PropertyChangedEventHandler PropertyChanged;

    public event Action<IWhiteboardItem, Direction> RequestRemove;

    public event Action<IWhiteboardItem> OnGotFocusEvent;

    public event Action<IResizeToFitWhiteboardItem> RequestResizeToFitOrClearRestriction;

    public event Action<object, RoutedEventArgs> OnAdornedElementReady;

    private OleObjectControl(bool autoResize)
        : this(new OleObject(autoResize))
    {
    }

    public OleObjectControl()
        : this(new OleObject())
    {
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        if (newContent is OleObject view)
        {
            View = view;
        }
    }

    private void ExecutedToggleOleObjectAutoResize(object sender, ExecutedRoutedEventArgs e)
    {
        View.ViewModel.ExecutedToggleOleAutoResize(e.Parameter);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        if (OnGotFocusEvent != null)
        {
            OnGotFocusEvent(this);
        }
    }

    public void SimulateMouseClick(Point location, int numberOfClicks)
    {
        if (numberOfClicks == 1)
        {
            Activate(isActive: true);
        }
        else if (numberOfClicks == 2)
        {
            DebugTools.FailAssertion("Not expected, should we toggle OutOfPlace activation here ?");
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        Activate(isActive: true);
        e.Handled = true;
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        return View.SetOlePersistentData(d11NProvider, (oleObjectRegionType)data);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return View.GetOlePersistentData(s11NProvider, s11NProvider.Helper.GetRegionId(this).ToString());
    }

    private void HandleViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "AutoResize")
        {
            FirePropertyChanged("OleObjectAutoResize");
        }
    }

    private void FirePropertyChanged(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    private void HandleViewModelRequestResizeToFit()
    {
        DebugTools.DevAssert(View.ViewModel.SiteCreated, null);
        if (RequestResizeToFitOrClearRestriction != null)
        {
            RequestResizeToFitOrClearRestriction(this);
        }
    }

    private void OnViewVisualParentChanged(DependencyObject oldParent, DependencyObject newParent)
    {
        if (newParent == null)
        {
            return;
        }
        if (OnAdornedElementReady != null)
        {
            OnAdornedElementReady(this, null);
        }
    }

    private bool Accept(CreationMethodWithDialog creationMethod, string oleContainerPath)
    {
        View.CreateSiteWithDialog(new WindowInteropHelper(creationMethod.WinHandle).Handle, null, oleContainerPath);
        return ValidateInstance(assertOnFailure: false);
    }

    private bool Accept(CreationMethodWithProgId creationMethod, string oleContainerPath)
    {
        View.CreateSiteWithProgId(creationMethod.ProgId, oleContainerPath);
        return ValidateInstance(assertOnFailure: true);
    }

    private bool Accept(CreationMethodFromFile creationMethod, string oleContainerPath)
    {
        View.CreateSiteFromFile(creationMethod.FilePath, creationMethod.Link, oleContainerPath);
        return ValidateInstance(assertOnFailure: true);
    }

    public static IOleObjectControl Create(OleObjectControlCreationOptions creationOptions, string oleContainerPath)
    {
        OleObjectControl oleObjectControl = new OleObjectControl(creationOptions.OleObjectAutoResize);
        if (!creationOptions.OleObjectControlCreationMethod.Visit(oleObjectControl, oleContainerPath))
        {
            return null;
        }
        return oleObjectControl;
    }

    private bool ValidateInstance(bool assertOnFailure)
    {
        if (!View.ViewModel.SiteCreated)
        {
            if (assertOnFailure)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
            return false;
        }
        return true;
    }

    public void Dispose()
    {
        View.Dispose();
    }
}
