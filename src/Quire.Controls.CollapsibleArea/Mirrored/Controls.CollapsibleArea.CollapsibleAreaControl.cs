#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Protection;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Mathboard;
using Quire.Controls.Whiteboard;
using Quire.FunctionalitiesLimitation;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.CollapsibleArea;

public partial class CollapsibleAreaControl
{
    internal const string ToggleButtonCanvasName = "PART_AreaSection";

    internal const string DragBorderName = "PART_DragBorder";

    public static readonly RoutedUICommand AreaControlToggleCollapseState = new RoutedUICommand("AreaControlToggleCollapseState", "Toggle Collapse State", typeof(CollapsibleAreaControl));

    public static readonly RoutedUICommand BorderVisibilityCommand = new RoutedUICommandWithFunctionalityKey("AreaControlBorderState", "Border State", typeof(CollapsibleAreaControl), EnumFunctionalityKey.ToggleBorderStateCommand);

    public static readonly DependencyProperty ControlWidthProperty = DependencyProperty.Register("ControlWidth", typeof(double), typeof(WhiteboardControl), new PropertyMetadata((PropertyChangedCallback)ControlWidthChangedHandler));

    public static readonly DependencyProperty ControlMinHeightProperty = DependencyProperty.Register("ControlMinHeight", typeof(double), typeof(WhiteboardControl), new PropertyMetadata((PropertyChangedCallback)ControlMinHeightChangedHandler));

    private static void ControlWidthChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Release IL returns early when the sender is not a CollapsibleAreaControl.
        CollapsibleAreaControl collapsibleAreaControl = d as CollapsibleAreaControl;
        if (collapsibleAreaControl == null)
        {
            return;
        }
        double num = (double)e.NewValue;
        if (num > 1.0)
        {
            (collapsibleAreaControl.DataContext as ICollapsibleAreaViewModel).AreaWidth = num;
            (collapsibleAreaControl.DataContext as WhiteboardViewModel).MinimumWidth = num;
            (collapsibleAreaControl.DataContext as WhiteboardViewModel).ContentWidth = num;
        }
    }

    public double ControlWidth
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(ControlWidthProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(ControlWidthProperty, (object)value);
        }
    }

    protected override ISerializationStrategy SerializationStrategy { get; set; }

    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    private static void ControlMinHeightChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Release IL returns early when the sender is not a CollapsibleAreaControl.
        CollapsibleAreaControl collapsibleAreaControl = d as CollapsibleAreaControl;
        if (collapsibleAreaControl == null)
        {
            return;
        }
        (collapsibleAreaControl.DataContext as WhiteboardViewModel).MinimumHeight = (double)e.NewValue;
    }

    public double ControlMinHeight
    {
        get
        {
            return (double)((DependencyObject)this).GetValue(ControlMinHeightProperty);
        }
        set
        {
            ((DependencyObject)this).SetValue(ControlMinHeightProperty, (object)value);
        }
    }

    private ICollapsibleAreaViewModel AreaViewModel => ViewModel as ICollapsibleAreaViewModel;

    public CollapsibleAreaControl()
        : this((Size?)null)
    {
    }

    public CollapsibleAreaControl(Size? gridCellSize)
        : base(new CollapsibleAreaViewModel(new CollapsibleAreaModel(), gridCellSize), null)
    {
        InitializeComponent();
        SerializationStrategy = new CollapsibleAreaSerializationStrategy(this, () => new worksheetRegionType());
        DeserializationStrategy = new CollapsibleAreaDeserializationStrategy(this);
        AreaViewModel.ShowProtectionDialogRequest = (ProtectionDialogState state) => ProtectDialog.ShowProtectionDialog(state, new AreaProtectionDictionary(), Application.Current.MainWindow);
        AreaViewModel.ShowUnProtectDialogRequest = (Func<string, bool> pwdCheck) => UnprotectDialog.ShowUnProtectDialog(pwdCheck, new AreaProtectionDictionary(), Application.Current.MainWindow);
        AreaViewModel.ShowWarningDialogRequest = (string messageKey) =>
        {
            ShowWarningDialog(messageKey);
        };
        BindCommands();
    }

    private static IMessageBoxManager _messageBoxMngr;

    private static IMessageBoxManager MessageBoxManager
    {
        get
        {
            if (_messageBoxMngr == null || !_messageBoxMngr.IsConnected)
            {
                return _messageBoxMngr = AppServiceRegistry.GetService<IMessageBoxManager>();
            }
            return _messageBoxMngr;
        }
    }

    public ITextStyleChangedHandler TextStyleChangedHandler { get; set; }

    private static void ShowWarningDialog(string messageKey)
    {
        MessageBoxManager.ShowOkDialog(Application.Current.MainWindow, (string)new AreaProtectionDictionary()[messageKey], WpfUtils.ApplicationFullName, MessageBoxImage.Exclamation);
    }

    private CollapsibleAreaControl(CollapsibleAreaControl original)
        : base(original.AreaViewModel.Clone(), null)
    {
        InitializeComponent();
        if (Resources.MergedDictionaries.Count != original.Resources.MergedDictionaries.Count)
        {
            foreach (ResourceDictionary mergedDictionary in original.Resources.MergedDictionaries)
            {
                if (!Resources.MergedDictionaries.Contains(mergedDictionary))
                {
                    Resources.MergedDictionaries.Add(mergedDictionary);
                }
            }
        }
        RegionFactory = original.RegionFactory;
        SerializationStrategy = new CollapsibleAreaSerializationStrategy(this, () => new worksheetRegionType());
        DeserializationStrategy = new CollapsibleAreaDeserializationStrategy(this);
        BindCommands();
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(WhiteboardCommands.InsertVerticalSpace, InsertVerticalSpace, CanInsertVerticalSpace));
    }

    public override void CanInsertVerticalSpace(object sender, CanExecuteRoutedEventArgs e)
    {
        if (IsCollapsed)
        {
            e.CanExecute = false;
        }
        else
        {
            base.CanInsertVerticalSpace(sender, e);
        }
    }

    protected override string GetInternalHelpContext()
    {
        return "CollapsibleAreaControl";
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public CollapsibleAreaControl Clone()
    {
        return new CollapsibleAreaControl(this);
    }

    public Rect? PrintViewport
    {
        get
        {
            return AreaViewModel.PrintViewport;
        }
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    internal static void HandlePrintViewportChanged(ICollapsibleAreaControl area, Size renderSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public override string GetBlockEquationTypeAttributeName(IWhiteboardItem item)
    {
        throw new NotImplementedException();
    }

    protected override void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        base.ViewModelPropertyChangedHandler(sender, e);
        if (e.PropertyName == "IsCollapsed")
        {
            FireCollapsedStateChanged();
        }
        if (e.PropertyName == "ProtectionData")
        {
            HandleProtectedStateChanged();
        }
        if (e.PropertyName == "IsBorderVisible")
        {
            FireBorderVisibilityStateChanged();
        }
    }

    private void FireBorderVisibilityStateChanged()
    {
        BorderStateChanged?.Invoke(this);
    }

    private void HandleProtectedStateChanged()
    {
        bool value = !ProtectionLevel.IsActivationProtected();
        AttachedProperties.SetCanActivate(this, value);
        FireProtectedStateChanged();
    }

    private void FireProtectedStateChanged()
    {
        if (ProtectedStateChanged != null)
        {
            ProtectedStateChanged(this);
        }
    }

    private void FireCollapsedStateChanged()
    {
        if (CollapsedStateChanged != null)
        {
            CollapsedStateChanged(this);
        }
    }

    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        if (!AttachedProperties.GetCanActivate(this))
        {
            e.Handled = true;
        }
        else
        {
            base.OnPreviewGotKeyboardFocus(e);
        }
    }

    public override Vector? NavigateIntoItem(Direction direction)
    {
        DebugTools.DevAssert(direction == Direction.Down || direction == Direction.Up, null);
        if (!AttachedProperties.GetCanActivate(this))
        {
            DebugTools.FailAssertion((string)null);
            return NavigateInto(direction, new Point(0.0, (direction == Direction.Down) ? 0.0 : ViewModel.ContentHeight));
        }
        return base.NavigateIntoItem(direction);
    }

    public bool NotHookupSizeChangedHandlerUntilLoaded { get; set; }

    public event Action HookupSizeChangedHandler = () =>
    {
    };

    public void FireHookupSizeChangedHandler()
    {
        HookupSizeChangedHandler();
    }

    public event Action<ICollapsibleAreaControl> CollapsedStateChanged;

    public event Action<IProtectableControl> ProtectedStateChanged;

    public event Action<ICollapsibleAreaControl> BorderStateChanged;

    public void OnBorderVisibilityCommand()
    {
        AreaViewModel.OnBorderVisibilityCommand();
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider deS11NProvider, object data)
    {
        if (!(data is AreaType areaType))
        {
            return null;
        }
        IList<IRegionPersistentData> result = DeserializationStrategy.SetRegions(areaType.Regions, deS11NProvider);
        AreaHeight = areaType.AreaHeight;
        AreaViewModel.SetPersistentData(areaType, deS11NProvider.Helper.FormatVersion);
        return result;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        AreaType areaType = new AreaType
        {
            Regions = SerializationStrategy.GetRegions(s11NProvider),
            AreaHeight = AreaHeight
        };
        AreaViewModel.GetPersistentData(areaType);
        return areaType;
    }

    public override bool CanHideFromCalculationContext => WhiteboardItems.OfType<ICalculationHideableChild>().Any((ICalculationHideableChild hideable) => hideable.CanHideFromCalculationContext);

    public override bool HideFromCalculationContext
    {
        get
        {
            return !WhiteboardItems.OfType<ICalculationHideableChild>().Any((ICalculationHideableChild hideable) => !hideable.HideFromCalculationContext);
        }
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public Action GridCellSizeChangedAction { private get; set; }

    public override void GridCellSizeChangedHandler(Size oldSize, Size newSize)
    {
        base.GridCellSizeChangedHandler(oldSize, newSize);
        if (GridCellSizeChangedAction != null)
        {
            GridCellSizeChangedAction();
        }
        GridCellSizeChangedAction = null;
    }

    public IAreaType AreaDataForXamlSerialization
    {
        get
        {
            AreaTypeXamlCopyDataObject areaTypeXamlCopyDataObject = new AreaTypeXamlCopyDataObject();
            AreaViewModel.GetPersistentData(areaTypeXamlCopyDataObject);
            return areaTypeXamlCopyDataObject;
        }
        set
        {
            AreaViewModel.SetPersistentData(value);
        }
    }

    public bool IsCollapsed => AreaViewModel.IsCollapsed;

    public bool IsBorderVisible => AreaViewModel.IsBorderVisible;

    public double AreaHeight
    {
        get
        {
            return AreaViewModel.AreaHeight;
        }
        set
        {
            AreaViewModel.AreaHeight = value;
        }
    }

    public bool AreaHasErrors
    {
        get
        {
            return AreaViewModel.AreaHasErrors;
        }
        set
        {
            AreaViewModel.AreaHasErrors = value;
        }
    }

    public Rect CollapsedAreaBounds
    {
        get
        {
            Rect descendantBounds = WhiteboardViewModel.WhiteboardItemMgr.GetDescendantBounds(this);
            descendantBounds.Y = 0.0;
            descendantBounds.Height = CollapsibleAreaViewModel.CollapsedAreaHeight;
            return descendantBounds;
        }
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        base.OnContextMenuOpening(e);
        WpfUtils.SetMostRecentInputDeviceToKeyboardDevice();
    }

    public ProtectionLevel ProtectionLevel => AreaViewModel.ProtectionData.ProtectionLevel();

    public ProtectionData ProtectionData => AreaViewModel.ProtectionData;

    public void Accept(ISpecialCopyVisitor visitor)
    {
        visitor.Visit(this);
    }

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        // Release IL returns early once the component is loaded.
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.CollapsibleArea;component/collapsibleareacontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
