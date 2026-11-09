#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Mathboard;

public abstract partial class MathboardControl : WhiteboardControl, IMathboardControl, IWhiteboardControl, IWhiteboardItemContainer, IHasRegionsWithConverterAnnotations, IMathFormatDeserializationProvider, IMaybeHasPointThatSnapsToPixel, IHasChildArrangeInfoProvider, IDisposable, ICalculatableContainer, ICalculationHideableChild, INotifyPropertyChanged, IWhiteboardItem, IHelpContext, ICalculationErrorOwner, IIORegionContainer, IIORegionBase, IHideFromCalculationButtonStatus, ICurrentNumberFormatProperty, ICurrentMatrixFormatProperty, ICurrentTextFormatProperty, IRegionFactoryOwner, IMathFormatForDeserialization
{
    protected readonly IMathboardModel _model;

    protected IMathboardViewModel MathboardViewModel => ViewModel as IMathboardViewModel;

    protected abstract ISerializationStrategy SerializationStrategy { get; set; }

    protected abstract IDeserializationStrategy DeserializationStrategy { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public NumberFormat CurrentNumberFormat
    {
        get => MathboardViewModel.CurrentNumberFormat;
        set => MathboardViewModel.CurrentNumberFormat = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MatrixFormat CurrentMatrixFormat
    {
        get => MathboardViewModel.CurrentMatrixFormat;
        set => MathboardViewModel.CurrentMatrixFormat = value;
    }

    public TextFormat CurrentTextFormat
    {
        get => MathboardViewModel.CurrentTextFormat;
        set => MathboardViewModel.CurrentTextFormat = value;
    }

    public bool HideFromCalculationButtonEnabled => MathboardViewModel.HideFromCalculationButtonEnabled;

    public bool HideFromCalculationButtonChecked => MathboardViewModel.HideFromCalculationButtonChecked;

    static MathboardControl()
    {
        DocumentAttachedProperties.IsInPrintingProperty.OverrideMetadata(typeof(MathboardControl), new FrameworkPropertyMetadata(OnIsInPrintingPropertyChanged));
    }

    protected MathboardControl(IMathboardViewModel viewModel, ResourceDictionary dict)
        : base(viewModel)
    {
        _model = viewModel.Model;
        if (dict != null)
        {
            Resources.MergedDictionaries.Add(dict);
        }
        BindProperties();
        HookupEvents();
        BindCommands();
        SetupInputGestures();
    }

    private void SetupInputGestures()
    {
        InputBindings.Add(new KeyBinding(EditingCommands.MoveDownByPage, Gestures.PageDown));
        InputBindings.Add(new KeyBinding(EditingCommands.MoveUpByPage, Gestures.PageUp));
    }

    private void BindProperties()
    {
        BindProperty("ContentHeight", FrameworkElement.HeightProperty);
        BindProperty("ContentWidth", FrameworkElement.WidthProperty);
        BindProperty("MinimumHeight", FrameworkElement.MinHeightProperty);
        BindProperty("MinimumWidth", FrameworkElement.MinWidthProperty);
        BindProperty("MaximumHeight", FrameworkElement.MaxHeightProperty);
        BindProperty("MaximumWidth", FrameworkElement.MaxWidthProperty);
        Content = new WhiteboardCanvas(DataContext as IWhiteboardViewModel)
        {
            Background = Brushes.Transparent
        };
    }

    protected void BindProperty(string sourceProperty, DependencyProperty targetProperty, BindingMode mode = BindingMode.TwoWay)
    {
        Binding binding = new Binding
        {
            Mode = mode,
            Path = new PropertyPath(sourceProperty),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
        BindingOperations.SetBinding(this, targetProperty, binding);
    }

    private void HookupEvents()
    {
        _model.CalculatableAdded += HandleCalculatableAdded;
        _model.CalculatableRemoved += HandleCalculatableRemoved;
        _model.CalculatableMoved += HandleCalculatableMoved;
        MathboardViewModel.Activated += FireOnGotFocus;
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(EquationCommands.Commit, (object sender, ExecutedRoutedEventArgs args) =>
        {
            MathboardViewModel.RecalculateCmdHandler();
        }));
        CommandBindings.Add(new CommandBinding(EquationCommands.SetNumberFormatFieldCommand, OnSetNumberFormatField));
        CommandBindings.Add(new CommandBinding(EquationCommands.ToggleHideFromCalculationContext, OnToggleHideFromCalculationContext));
        CommandBindings.Add(new CommandBinding(EquationCommands.SetMatrixFormatFieldCommand, OnSetMatrixFormatField));
        CommandBindings.Add(new CommandBinding(EditingCommands.ToggleBullets, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.ToggleNumbering, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.AlignLeft, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.AlignRight, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.AlignCenter, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.AlignJustify, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.IncreaseIndentation, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(EditingCommands.DecreaseIndentation, OnSetTextFormatField, CanChangeTextStyle));
        CommandBindings.Add(new CommandBinding(WhiteboardCommands.InsertVerticalSpace, InsertVerticalSpace, CanInsertVerticalSpace));
        CommandBindings.Add(new CommandBinding(WhiteboardCommands.DeleteVerticalSpaceAbove, DeleteVerticalSpaceAbove, CanDeleteVerticalSpaceAbove));
        CommandBindings.Add(new CommandBinding(WhiteboardCommands.DeleteVerticalSpaceBelow, DeleteVerticalSpaceBelow, CanDeleteVerticalSpaceBelow));
    }

    protected override void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        base.ViewModelPropertyChangedHandler(sender, e);
        if (e.PropertyName == "CurrentNumberFormat" || e.PropertyName == "CurrentMatrixFormat" || e.PropertyName == "CurrentTextFormat" || e.PropertyName == "ContentWidth" || e.PropertyName == "ContentHeight" || e.PropertyName == "HideFromCalculationButtonEnabled" || e.PropertyName == "HideFromCalculationButtonChecked" || e.PropertyName == "ActiveDescendant" || e.PropertyName == "SelectedDescendants")
        {
            FirePropertyChanged(e);
        }
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        FireOnGotFocus();
    }

    private void OnSetNumberFormatField(object sender, ExecutedRoutedEventArgs e)
    {
        if (this != e.Source)
        {
            return;
        }
        RoutedUICommand routedUICommand = e.Command as RoutedUICommand;
        if (routedUICommand == null)
        {
            return;
        }
        using (new CommandScope(routedUICommand.ToString()))
        {
            if (ViewModel.IsCrosshairVisible)
            {
                ((NumberFormat)GetValue(EquationControl.DisplayNumberFormatProperty)).SetPropertyValueWithCommand((SetPropertyValueCommandParameter)e.Parameter);
            }
            else
            {
                DispatchCommandWithinSolveBlock(routedUICommand, e.Parameter, PropertiesForUpdate.CurrentNumberFormat);
            }
        }
    }

    private void OnSetMatrixFormatField(object sender, ExecutedRoutedEventArgs e)
    {
        if (this != e.Source)
        {
            return;
        }
        RoutedUICommand routedUICommand = e.Command as RoutedUICommand;
        if (routedUICommand == null)
        {
            return;
        }
        using (new CommandScope(routedUICommand.ToString()))
        {
            if (ViewModel.IsCrosshairVisible)
            {
                ((MatrixFormat)GetValue(EquationControl.ResultMatrixFormatProperty)).SetPropertyValueWithCommand((SetPropertyValueCommandParameter)e.Parameter);
            }
            else
            {
                DispatchCommandWithinSolveBlock(routedUICommand, e.Parameter, PropertiesForUpdate.CurrentMatrixFormat);
            }
        }
    }

    private void OnSetTextFormatField(object sender, ExecutedRoutedEventArgs e)
    {
        if (this != e.Source)
        {
            return;
        }
        RoutedUICommand routedUICommand = e.Command as RoutedUICommand;
        if (routedUICommand == null)
        {
            return;
        }
        using (new CommandScope(routedUICommand.ToString()))
        {
            if (ViewModel.IsCrosshairVisible)
            {
                return;
            }
            DispatchCommandWithinSolveBlock(routedUICommand, e.Parameter, PropertiesForUpdate.TextFormat);
        }
    }

    private void CanChangeTextStyle(object sender, CanExecuteRoutedEventArgs args)
    {
        if (SelectedDescendants.Any())
        {
            IRegionCollectionWithProperty regionCollectionWithProperty = AppServiceRegistry.GetService<IRegionCollectionWithPropertyCreator>().CreateRegionCollection(SelectedDescendants);
            args.CanExecute = regionCollectionWithProperty.AreAllTextRegions;
        }
        else
        {
            args.CanExecute = false;
        }
    }

    private void DispatchCommandWithinSolveBlock(RoutedCommand cmd, object parameter, PropertiesForUpdate propertyForUpdate)
    {
        if (SelectedDescendants.Any())
        {
            AppServiceRegistry.GetService<IRegionCollectionWithPropertyCreator>().CreateRegionCollection(SelectedDescendants).DispatchCommand(cmd, parameter, propertyForUpdate);
        }
        else if (ActiveDescendant != null)
        {
            cmd.Execute(parameter, ActiveItem as IInputElement);
        }
        else if (!ViewModel.IsCrosshairVisible)
        {
            AppServiceRegistry.GetService<IRegionCollectionWithPropertyCreator>().CreateRegionCollection(RecursivelyEnumerateDescendantItemssBypassWhiteboard()).DispatchCommand(cmd, parameter, propertyForUpdate);
        }
    }

    protected virtual void HandleCalculatableAdded(ICalculatableContainer container, ICalculatable calculatable)
    {
        CalculatableAdded(this, calculatable);
    }

    protected virtual void HandleCalculatableRemoved(ICalculatable calculatable)
    {
        CalculatableRemoved(calculatable);
    }

    protected virtual void HandleCalculatableMoved(ICalculatable calculatable)
    {
        CalculatableMoved(calculatable);
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public virtual bool CanHideFromCalculationContext => true;

    public abstract bool HideFromCalculationContext { get; set; }

    public void OnToggleHideFromCalculationContext(object sender, RoutedEventArgs e)
    {
        MathboardViewModel.ToggleHideFromCalculationContext();
    }

    protected void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        if (ViewModel.IsCrosshairVisible)
        {
            if (e.PropertyName == "CurrentNumberFormat" && CurrentNumberFormat == null && GetValue(EquationControl.DisplayNumberFormatProperty) != null)
            {
                CurrentNumberFormat = (NumberFormat)GetValue(EquationControl.DisplayNumberFormatProperty);
            }
            if (e.PropertyName == "CurrentMatrixFormat" && CurrentMatrixFormat == null && GetValue(EquationControl.ResultMatrixFormatProperty) != null)
            {
                CurrentMatrixFormat = (MatrixFormat)GetValue(EquationControl.ResultMatrixFormatProperty);
            }
            if (e.PropertyName == "CurrentTextFormat" && CurrentTextFormat == null && GetValue(TextFormat.TextFormatProperty) != null)
            {
                CurrentTextFormat = (TextFormat)GetValue(TextFormat.TextFormatProperty);
            }
        }
        if (e.PropertyName == "ContentHeight" || e.PropertyName == "ContentWidth")
        {
            InvalidateMeasure();
        }
        PropertyChanged(this, e);
    }

    public event Action<ICalculatableContainer, ICalculatable> CalculatableAdded = delegate { };

    public event Action<ICalculatable> CalculatableRemoved = delegate { };

    public event Action<ICalculatable> CalculatableMoved = delegate { };

    public IEnumerable<ICalculatable> CalculatableChildren => _model.CalculatableChildren;

    public bool ContainsError(EngineError error)
    {
        foreach (ICalculatable calculatableChild in CalculatableChildren)
        {
            if (calculatableChild is ICalculationErrorOwner calculationErrorOwner && calculationErrorOwner.ContainsError(error))
            {
                return true;
            }
        }
        return false;
    }

    public void ShowError(EngineError error)
    {
        foreach (ICalculatable calculatableChild in CalculatableChildren)
        {
            if (calculatableChild is ICalculationErrorOwner calculationErrorOwner && calculationErrorOwner.ContainsError(error))
            {
                calculationErrorOwner.ShowError(error);
                break;
            }
        }
    }

    public EngineError CurrentError
    {
        get
        {
            if (ActiveItem is ICalculationErrorOwner calculationErrorOwner)
            {
                return calculationErrorOwner.CurrentError;
            }
            return null;
        }
    }

    public abstract string GetBlockEquationTypeAttributeName(IWhiteboardItem item);

    public event Action<IWhiteboardItem> OnGotFocusEvent = delegate { };

    public event Action<IWhiteboardItem, Direction> RequestRemove = delegate { };

    public double VerticalAlignmentOffset => 0.0;

    public bool IsEmpty => _model.IsEmpty;

    public void Activate(bool isActive)
    {
        ActivateWhiteboard(isActive);
    }

    public virtual Vector? NavigateIntoItem(Direction direction)
    {
        Activate(isActive: true);
        return null;
    }

    public void SimulateMouseClick(Point location, int numberOfClicks)
    {
        InsertionPoint = location;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IIORegionManager InputOutputRegionManager
    {
        get => _model.IoRegionManager;
        set => _model.IoRegionManager = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBookmarksManager BookmarksManager
    {
        get => _model.BookmarksManager;
        set => _model.BookmarksManager = value;
    }

    public bool CanToggleInputRegion()
    {
        return MathboardViewModel.CanToggleRegionsAsInput();
    }

    public bool CanToggleOutputRegion()
    {
        return MathboardViewModel.CanToggleRegionsAsOutput();
    }

    public void ToggleInputRegion()
    {
        MathboardViewModel.ToggleInputRegions();
    }

    public void ToggleOutputRegion()
    {
        MathboardViewModel.ToggleOutputRegions();
    }

    public IEnumerable<IIORegionBase> IORegions => WhiteboardItems.OfType<IIORegionBase>();

    public override void CanInsertVerticalSpace(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.CanInsertVerticalSpace;
    }

    public override void CanDeleteVerticalSpaceAbove(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.CanDeleteVerticalSpaceAbove;
    }

    public override void CanDeleteVerticalSpaceBelow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ViewModel.CanDeleteVerticalSpaceBelow;
    }

    public override void InsertVerticalSpace(object sender, ExecutedRoutedEventArgs e)
    {
        int numberOfLines = (e.Parameter as int?) ?? 1;
        ViewModel.InsertVerticalSpace(numberOfLines, pushInsertionPoint: false, ignoreItemsOnCurrentLine: false);
        e.Handled = true;
    }

    public override void DeleteVerticalSpaceAbove(object sender, ExecutedRoutedEventArgs e)
    {
        int numberOfLines = (e.Parameter as int?) ?? 1;
        ViewModel.DeleteVerticalSpaceAbove(numberOfLines);
        e.Handled = true;
    }

    public override void DeleteVerticalSpaceBelow(object sender, ExecutedRoutedEventArgs e)
    {
        int numberOfLines = (e.Parameter as int?) ?? WhiteboardControl.DeleteEmptySpaceDefaultLinesCount;
        ViewModel.DeleteVerticalSpaceBelow(numberOfLines);
        e.Handled = true;
    }

    private void FireOnGotFocus()
    {
        OnGotFocusEvent(this);
    }

    public IRegionFactory RegionFactory { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IMathFormat DeserializeMathFormat
    {
        set => DeserializationStrategy.DeserializeMathFormat = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat
    {
        set => DeserializationStrategy.DeserializeLabeledIdFormat = value;
    }

    public override bool ShouldSerializeContent()
    {
        return false;
    }

    protected abstract string GetInternalHelpContext();

    public virtual string GetHelpContext()
    {
        if (ActiveItem == null)
        {
            return GetInternalHelpContext();
        }
        return ActiveItem.GetHelpContext();
    }

    private static void OnIsInPrintingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MathboardControl mathboardControl)
        {
            return;
        }
        mathboardControl.MathboardViewModel.IsInPrinting = (bool)e.NewValue;
    }

    public bool HasItemInViewPort(Rect initialViewPortRect)
    {
        bool flag = false;
        Queue<(IWhiteboardItem item, Rect viewPortRect)> nodesToProcess = new Queue<(IWhiteboardItem, Rect)>();
        WhiteboardItems.Each((IWhiteboardItem item3) =>
        {
            nodesToProcess.Enqueue((item3, initialViewPortRect));
        });
        while (!flag && nodesToProcess.IsNotEmpty())
        {
            (IWhiteboardItem item, Rect item2) = nodesToProcess.Dequeue();
            Rect subViewPort;
            flag = IsItemInViewPort(WhiteboardItemManager, item, item2, out subViewPort);
            if (!flag && item is IMathboardControl mathboardControl)
            {
                mathboardControl.WhiteboardItems.Each((IWhiteboardItem subItem) =>
                {
                    nodesToProcess.Enqueue((subItem, subViewPort));
                });
            }
        }
        return flag;
    }

    protected static bool IsItemInViewPort(IWhiteboardItemManager parentItemsManager, IWhiteboardItem item, Rect viewPortRect, out Rect subViewPort)
    {
        Rect itemRect = parentItemsManager.GetItemRect(item);
        bool result = viewPortRect.IntersectsWith(itemRect);
        subViewPort = Rect.Offset(viewPortRect, -itemRect.X, -itemRect.Y);
        return result;
    }
}
