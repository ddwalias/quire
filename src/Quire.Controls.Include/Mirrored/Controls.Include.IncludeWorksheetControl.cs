using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.FunctionalitiesLimitation;
using Quire.Serialization;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Include;

[TemplatePart(Name = "PART_ContentHost", Type = typeof(UIElement))]
public partial class IncludeWorksheetControl : TextBox, ICalculatableContainer, ICalculationHideableChild, INotifyPropertyChanged, ICalculationErrorOwner, IHasPreCalculationAction, IIncludeWorksheetControl, IWhiteboardItem, IHelpContext, IDitaRegion, IPersistentDataProvider, IPageWideSizable, IRegionFactoryOwner, IDisposable, IWorksheetPropertiesTarget, IMultiLevelInclusionProvider, IErrorWarningOwner, ISearchableRegion, INodeMessageAdornerReferenceHolder, IComponentConnector, IStyleConnector
{



    public static readonly DependencyProperty CalcIsDisabledProperty;



    public static readonly DependencyProperty IsCalculatingProperty;



    public static readonly DependencyProperty IsPendingProperty;



    public static readonly DependencyProperty SynchronizationStatusProperty;



    private readonly IncludeWorksheetModel _model;



    private ErrorWarningHandler _errorwarningViewHandler;



    private static readonly Type _typeOfThis;



    public static readonly DependencyProperty IsIBeamCursorProperty;



    private bool _isTopLevel;



    public static readonly RoutedCommand IncludeReferenceCommand;



    public static readonly RoutedCommand RefreshReferenceCommand;



    public static readonly RoutedUICommand OpenFileCommand;



    public static readonly DependencyProperty IncludedFilePathProperty;



    private static DispatcherOperation _lastDispatcherOperation;



    private UIElement _contentElement;



    private const string DataPropertyName = "Data";




    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        private set => SetValue(CalcIsDisabledProperty, value);
    }




    public bool IsCalculating
    {
        get => (bool)GetValue(IsCalculatingProperty);
        set => SetValue(IsCalculatingProperty, value);
    }




    public bool IsPending
    {
        get => (bool)GetValue(IsPendingProperty);
        set => SetValue(IsPendingProperty, value);
    }




    public SynchronizationStatus SynchronizationStatus
    {
        get => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);
        set => SetValue(SynchronizationStatusProperty, value);
    }




    public IEnumerable<ICalculatable> CalculatableChildren => _model.CalculatableChildren;




    public bool CanHideFromCalculationContext => true;




    public bool HideFromCalculationContext
    {
        get => CalcIsDisabled;
        set
        {
            if (!(CalcIsDisabled == value ? false : true))
            {
                return;
            }
            DelegateFunction0<bool> doMethod = () =>
            {
                HandleHideFromCalculationContext(value);
                return true;
            };
            Delegate0 undoMethod = () =>
            {
                HandleHideFromCalculationContext(!value);
            };
            CommandBuffer.Execute(new UndoableCommand("Change HideFromCalculationContext", doMethod, undoMethod));
        }
    }




    private static void PreRecalculateAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IncludeWorksheetControl includeWorksheetControl = d as IncludeWorksheetControl;
        if (DebugTools.AssertNotNull(includeWorksheetControl, null).Passed && (bool)e.NewValue)
        {
            includeWorksheetControl.Reload();
        }
    }




    private void HandleSynchronizationStatusChanged()
    {
        IsPending = _model.SynchronizationStatus == SynchronizationStatus.Pending;
    }




    private void HandleCalculatingStatusChanged()
    {
        IsCalculating = _model.CalculatingStatus == CalculatingStatus.Calculating;
    }




    public event Action<ICalculatableContainer, ICalculatable> CalculatableAdded = delegate { };



    public event Action<ICalculatable> CalculatableRemoved = delegate { };



    public event Action<ICalculatable> CalculatableMoved;




    private void HandleHideFromCalculationContext(bool hideFromCalculationContext)
    {
        CalcIsDisabled = hideFromCalculationContext;
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("HideFromCalculationContext"));
        }
        _model.HideFromCalculationContext = hideFromCalculationContext;
        if (!hideFromCalculationContext)
        {
            _model.Commit();
        }
    }




    public ICalculatableInfo CurrentErrorInfo => _model.CurrentErrorInfo;




    public bool ContainsError(EngineError error)
    {
        return _model.ContainsError(error);
    }




    public void ShowError(EngineError error)
    {
        _model.FireErrorOnChild("IncludeControlCalculationErrors", new Pair<string, bool>(FullFilePath, Path.IsPathRooted(IncludedFilePath)));
    }




    public void PreCalculationAction()
    {
        Reload();
    }




    public string FilePath => IncludedFilePath;




    public string FileName
    {
        get
        {
            try
            {
                return Path.GetFileName(FullFilePath);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }




    public string FullFilePath => _model.IncludedFullFilePath;




    public int RegionCountCalculatable => _model.CountEnabledRegions;




    public string Error
    {
        get
        {
            if (ErrorsList.Count <= 0)
            {
                return null;
            }
            return ErrorsList[0].Text;
        }
    }




    public int ErrorCount => ErrorsList.Count;




    public string ErrorCode
    {
        get
        {
            if (ErrorsList.Count <= 0)
            {
                return null;
            }
            return ErrorsList[0].Code;
        }
    }




    public int WarningCount
    {
        get
        {
            if (WarningsList.Count <= 0)
            {
                return 0;
            }
            return WarningsList[0].WarningCount;
        }
    }




    public string WarningText
    {
        get
        {
            if (WarningsList.Count <= 0)
            {
                return null;
            }
            return WarningsList[0].Text;
        }
    }




    public string WarningCode
    {
        get
        {
            if (WarningsList.Count <= 0)
            {
                return null;
            }
            return GetCodeFromLongWarningCode(WarningsList[0].Code);
        }
    }




    public string[] RegionErrorTextList
    {
        get
        {
            ArrayList arrayList = new ArrayList();
            foreach (ErrorOnIncludeWorksheet errors in ErrorsList)
            {
                arrayList.Add(errors.Text);
            }
            return arrayList.Cast<string>().ToArray();
        }
    }




    public string[] RegionErrorCodeList
    {
        get
        {
            ArrayList arrayList = new ArrayList();
            foreach (ErrorOnIncludeWorksheet errors in ErrorsList)
            {
                arrayList.Add(errors.Code);
            }
            return arrayList.Cast<string>().ToArray();
        }
    }




    public int RegionWarningCountTotal => WarningsList.Count;




    public string[] RegionWarningCountList => GetWarningParameterList(WarningOnIncludeWorksheet.FieldName.WarningCount);




    public string[] RegionWarningTextList => GetWarningParameterList(WarningOnIncludeWorksheet.FieldName.WarningText);




    public string[] RegionWarningCodeList => GetWarningParameterList(WarningOnIncludeWorksheet.FieldName.WarningCode);




    public CalculationOptions IncludedFileCalculationOptions => _model.IncludedFileCalculationOptions;




    public CalculationVariables IncludedFileCalculationVariables => _model.IncludedFileCalculationVariables;




    static IncludeWorksheetControl()
    {
        IsCalculatingProperty = DependencyProperty.Register("IsCalculating", typeof(bool), typeof(IncludeWorksheetControl), new PropertyMetadata(false));
        IsPendingProperty = DependencyProperty.Register("IsPending", typeof(bool), typeof(IncludeWorksheetControl), new PropertyMetadata(false));
        SynchronizationStatusProperty = DependencyProperty.Register("SynchronizationStatus", typeof(SynchronizationStatus), typeof(IncludeWorksheetControl), new PropertyMetadata((SynchronizationStatus)3));
        _typeOfThis = typeof(IncludeWorksheetControl);
        IsIBeamCursorProperty = DependencyProperty.Register("IsIBeamCursor", typeof(bool), _typeOfThis, new PropertyMetadata(false));
        IncludeReferenceCommand = new RoutedCommand("IncludeReference", _typeOfThis);
        RefreshReferenceCommand = new RoutedCommand("RefreshReference", _typeOfThis);
        OpenFileCommand = new RoutedUICommandWithFunctionalityKey("Open File", "OpenFile", _typeOfThis, EnumFunctionalityKey.OpenMcdxDocumentCommand);
        IncludedFilePathProperty = DependencyProperty.Register("IncludedFilePath", typeof(string), _typeOfThis, new FrameworkPropertyMetadata(null, IncludedFilePathChanged));
        WhiteboardDependencyProperties.IsActiveProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsActiveRegionChanged));
        WorksheetPropertiesManager.RegisterToWorksheetPropertyNotifications(_typeOfThis);
        CalcIsDisabledProperty = EquationControl.CalcIsDisabledProperty.AddOwner(typeof(IncludeWorksheetControl));
        DocumentAttachedProperties.PreRecalculateAllProperty.OverrideMetadata(typeof(IncludeWorksheetControl), new FrameworkPropertyMetadata(PreRecalculateAllChanged));
        DocumentAttachedProperties.IsInCopyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnIsInCopyPropertyChanged));
        Canvas.LeftProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(CanvasLeftChanged, CoerceCanvasLeft));
    }




    private static void OnIsInCopyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IncludeWorksheetControl includeWorksheetControl = d as IncludeWorksheetControl;
        if (includeWorksheetControl == null)
        {
            return;
        }
        if (DocumentAttachedProperties.GetIsInCopyNonSerializable(includeWorksheetControl))
        {
            includeWorksheetControl._model.Regions.Each((UIElement item) =>
            {
                DocumentAttachedProperties.SetIsInCopy(item, value: true);
            });
        }
        else
        {
            includeWorksheetControl._model.Regions.Each(DocumentAttachedProperties.ClearIsInCopy);
        }
    }




    public IncludeWorksheetControl()
    {
        InitializeComponent();
        DataContext = this;
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            {
                "premium%_only",
                EngineErrorMessages.GetStringByKey("premium%_only")
            }
        });
        _model = new IncludeWorksheetModel(Resources);
        _model.CalculatableAdded += (ICalculatable calc) =>
        {
            CalculatableAdded(this, calc);
        };
        _model.CalculatableRemoved += (ICalculatable calc) =>
        {
            CalculatableRemoved(calc);
        };
        _model.SynchronizationStatusChanged += HandleSynchronizationStatusChanged;
        _model.CalculatingStatusChanged += HandleCalculatingStatusChanged;
        _model.ContentChanged += HandleContentChanged;
        CommandBindings.Add(new CommandBinding(EquationCommands.Commit, CommitExecuted));
        CommandBindings.Add(new CommandBinding(EquationCommands.ToggleHideFromCalculationContext, OnToggleHideFromCalculationContext));
        CommandBindings.Add(new CommandBinding(IncludeReferenceCommand, InsertFilePath));
        CommandBindings.Add(new CommandBinding(RefreshReferenceCommand, RefreshWorksheet));
        SetBinding(TextBox.TextProperty, new Binding
        {
            Source = this,
            Path = new PropertyPath(IncludedFilePathProperty),
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        GetInformationFromDocument(this);
    }




    private ErrorsList ErrorsList => _errorwarningViewHandler.GetMergedErrorsList();




    private WarningsList WarningsList => _errorwarningViewHandler.GetMergedWarningsList();




    private string[] GetWarningParameterList(WarningOnIncludeWorksheet.FieldName parameter)
    {
        ArrayList arrayList = new ArrayList();
        foreach (WarningOnIncludeWorksheet warnings in WarningsList)
        {
            if (parameter == WarningOnIncludeWorksheet.FieldName.WarningText)
            {
                arrayList.Add(warnings.Text);
            }
            if (parameter == WarningOnIncludeWorksheet.FieldName.WarningCode)
            {
                arrayList.Add(GetCodeFromLongWarningCode(warnings.Code));
            }
            if (parameter == WarningOnIncludeWorksheet.FieldName.WarningCount)
            {
                arrayList.Add(warnings.WarningCount.ToString(CultureInfo.InvariantCulture));
            }
        }
        return arrayList.Cast<string>().ToArray();
    }




    private static string GetCodeFromLongWarningCode(string longCode)
    {
        int num = longCode.IndexOf("#");
        if (num <= 0)
        {
            return longCode;
        }
        return longCode.Substring(0, num);
    }




    private static void GetInformationFromDocument(IncludeWorksheetControl element)
    {
        if (element.IsLoaded)
        {
            ElementIsLoadedGetInformationFromDocument(element, null);
        }
        else
        {
            element.Loaded += ElementIsLoadedGetInformationFromDocument;
        }
    }




    public bool IsIBeamCursor
    {
        get => (bool)GetValue(IsIBeamCursorProperty);
        set => SetValue(IsIBeamCursorProperty, value);
    }




    private static void ElementIsLoadedGetInformationFromDocument(object sender, RoutedEventArgs e)
    {
        IncludeWorksheetControl includeWorksheetControl = sender as IncludeWorksheetControl;
        if (includeWorksheetControl != null)
        {
            IWorksheetProperties worksheetProperties = WorksheetPropertiesManager.GetWorksheetProperties(includeWorksheetControl);
            if (worksheetProperties != null)
            {
                includeWorksheetControl.IsTopLevel = true;
                if (includeWorksheetControl._model.CacheValid && !worksheetProperties.CacheWorksheet)
                {
                    includeWorksheetControl._model.CacheValid = false;
                }
                includeWorksheetControl.HandleDocumentFilePathChanged(worksheetProperties);
                includeWorksheetControl._model.UpdateEngineeringDocCalculationOptions(worksheetProperties.DocCalculationOptions);
                includeWorksheetControl.IsParentDataUpdated = true;
                includeWorksheetControl.Loaded -= ElementIsLoadedGetInformationFromDocument;
            }
        }
    }




    public bool IsTopLevel
    {
        get => _isTopLevel;
        set
        {
            if (value == _isTopLevel)
            {
                return;
            }

            if (DebugTools.DevAssert(value, null).Passed)
            {
                _isTopLevel = true;
                _model.IsTopLevel = true;
                _errorwarningViewHandler = new ErrorWarningHandler(this);
                _model.ErrorsUpdated += _errorwarningViewHandler.ShowErrors;
                _model.WarningsUpdated += _errorwarningViewHandler.ShowWarnings;
                IncludeWorksheetModel model = _model;
                model.IsInFreemiumMode = (Func<bool>)Delegate.Combine(model.IsInFreemiumMode, (Func<bool>)IsInFreemiumMode);
                _model.PathIndexes = string.Empty;
                _model.HookupSystemVariables();
            }
        }
    }




    [DefaultValue(null)]
    [DependsOn("IsTopLevel")]
    public ErrorsList ErrorsForXamlSerialization
    {
        get
        {
            if (!DocumentAttachedProperties.GetIsInPrinting(this))
            {
                return null;
            }
            return _errorwarningViewHandler.GetMergedErrorsList();
        }
        set
        {
            if (value == null)
            {
                return;
            }
            _errorwarningViewHandler.ShowErrors(null, value);
        }
    }




    [DefaultValue(null)]
    [DependsOn("IsTopLevel")]
    public WarningsList WarningsForXamlSerialization
    {
        get
        {
            if (!DocumentAttachedProperties.GetIsInPrinting(this))
            {
                return null;
            }
            return _errorwarningViewHandler.GetMergedWarningsList();
        }
        set
        {
            if (value == null)
            {
                return;
            }
            _errorwarningViewHandler.ShowWarnings(null, value);
        }
    }




    private void HandleContentChanged()
    {
        RaiseEvent(new RoutedEventArgs(CommonRoutedEvents.ContentChangedEvent));
    }




    private void HandleDocumentFilePathChanged(IWorksheetProperties properties)
    {
        string documentFilePath = GetDocumentFilePath(properties);
        DebugTools.DevAssert(documentFilePath != null, null);
        string text = (!string.IsNullOrEmpty(documentFilePath)) ? Path.GetFullPath(documentFilePath) : documentFilePath;
        if (ParentFileNames != null)
        {
            ParentFileNames[0] = text;
            return;
        }
        List<string> parentFileNames = new List<string> { text };
        ParentFileNames = parentFileNames;
    }




    private static string GetDocumentFilePath(IWorksheetProperties properties)
    {
        if (!string.IsNullOrEmpty(properties.FilePath))
        {
            return properties.FilePath;
        }
        return properties.DocCalculationVariables.CurrentWorkingDirectory;
    }




    private void InsertFilePath(object sender, ExecutedRoutedEventArgs e)
    {
        IncludeWorksheetControl includeWorksheetControl = sender as IncludeWorksheetControl;
        if (includeWorksheetControl != null)
        {
            string text = OpenIncludeDialog();
            if (!string.IsNullOrEmpty(text))
            {
                includeWorksheetControl.IncludedFilePath = text;
            }
            includeWorksheetControl.Focus();
            includeWorksheetControl.CaretIndex = includeWorksheetControl.Text.Length;
        }
    }




    private void RefreshWorksheet(object sender, ExecutedRoutedEventArgs e)
    {
        CommitExecuted(sender, e);
    }




    private string OpenIncludeDialog()
    {
        string result = null;
        string filter = string.Format(Resources["IncludeControlOpenFileDialogFilter"] as string, ".mcdx");
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = filter
        };
        if (openFileDialog.ShowDialog() == true)
        {
            result = openFileDialog.FileName;
        }
        return result;
    }




    private static void CommitExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        IncludeWorksheetControl includeWorksheetControl = sender as IncludeWorksheetControl;
        if (includeWorksheetControl != null)
        {
            includeWorksheetControl._model.Commit();
        }
    }




    private void OnToggleHideFromCalculationContext(object sender, ExecutedRoutedEventArgs e)
    {
        HideFromCalculationContext = !HideFromCalculationContext;
    }




    private static void IncludedFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        string text = args.OldValue as string;
        string text2 = args.NewValue as string;
        if (text2 != null && text2.Equals(text))
        {
            return;
        }
        IncludeWorksheetControl includeWorksheetControl = d as IncludeWorksheetControl;
        if (includeWorksheetControl != null)
        {
            if (IsInFreemiumMode() && text != null)
            {
                includeWorksheetControl.ReloadedInFreemium = true;
                if (_lastDispatcherOperation == null || (int)_lastDispatcherOperation.Status == 2)
                {
                    _lastDispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke((DispatcherPriority)8, new Action<IncludeWorksheetControl>(ResetOldIncludedFilePathFromModel), includeWorksheetControl);
                }
                return;
            }
            if (includeWorksheetControl.IsTopLevel && !EnvironmentScope.IsCurrentlyInsideUndoOrRedo)
            {
                CommandBuffer.Execute(new IncludeWorksheetFilePathChange(includeWorksheetControl, text2));
            }
            includeWorksheetControl._model.IncludedFilePath = text2;
        }
    }




    private static void ResetOldIncludedFilePathFromModel(IncludeWorksheetControl control)
    {
        control.IncludedFilePath = control._model.IncludedFilePath;
    }




    public string IncludedFilePath
    {
        get => (string)GetValue(IncludedFilePathProperty);
        set => SetValue(IncludedFilePathProperty, value);
    }




    internal bool RegionsLoaded
    {
        get => _model.RegionsLoaded;
        set => _model.RegionsLoaded = value;
    }




    [DependsOn("Data")]
    public bool ReloadedInFreemium
    {
        get => _model.ReloadedInFreemium;
        set => _model.ReloadedInFreemium = value;
    }




    private static void IsActiveRegionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IncludeWorksheetControl includeWorksheetControl = d as IncludeWorksheetControl;
        if (includeWorksheetControl != null && includeWorksheetControl.IsLoaded && !(bool)e.NewValue && !includeWorksheetControl._model.RegionsLoaded)
        {
            includeWorksheetControl._model.Commit();
        }
    }



    public event PropertyChangedEventHandler PropertyChanged;



    public event Action<IWhiteboardItem> OnGotFocusEvent = delegate { };



    public event Action<IWhiteboardItem, Direction> RequestRemove = delegate { };




    public virtual double VerticalAlignmentOffset
    {
        get
        {
            GeneralTransform obj = ((_contentElement == null || FindCommonVisualAncestor(_contentElement) == null) ? Transform.Identity : _contentElement.TransformToAncestor(this));
            double top;
            double num2;
            Thickness padding;
            Point val;
            double num = FontFamily.Baseline * FontSize;
            padding = Padding;
            top = padding.Top;
            num2 = num + top;
            val = obj.Transform(new Point(0.0, num2));
            return -val.Y;
        }
    }




    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _contentElement = (UIElement)GetTemplateChild("PART_ContentHost");
    }




    public bool IsEmpty => string.IsNullOrEmpty(IncludedFilePath);




    public void Activate(bool isActive)
    {
        if (isActive)
        {
            AppServiceRegistry.GetService<IFrameworkElementFacade>().ExecuteOnLoad(this, () =>
            {
                Focus();
            });
        }
    }




    public Vector? NavigateIntoItem(Direction direction)
    {
        Activate(isActive: true);
        return null;
    }




    public virtual void SimulateMouseClick(Point location, int numberOfClicks)
    {
        Activate(isActive: true);
    }




    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }




    public bool FindOncePrivate(string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        if ((options & FindOptions.FindInText) == 0)
        {
            return false;
        }
        int start = 0;
        int end = Text.Length;
        int selectionStart = 0;
        int num = 0;
        if (startFromCaret)
        {
            if (LogicalDirection.Forward == direction)
            {
                if (!string.IsNullOrEmpty(SelectedText))
                {
                    selectionStart = SelectionStart;
                    num = SelectionLength;
                    CaretIndex += SelectionLength;
                }
                start = CaretIndex;
            }
            else
            {
                end = CaretIndex;
            }
        }
        bool flag = FindInRange(start, end, text, direction, options);
        if (!flag && num > 0)
        {
            SelectionStart = selectionStart;
            SelectionLength = num;
        }
        return flag;
    }




    private bool FindInRange(int start, int end, string text, LogicalDirection direction, FindOptions options)
    {
        DebugTools.DevAssert((options & FindOptions.FindInText) != 0, null);
        DebugTools.DevAssert(start.CompareTo(end) <= 0, null);
        int? indexInText = FindUtilities.GetIndexInText(Text.Substring(start, end - start), text, direction, options);
        bool hasValue = indexInText.HasValue;
        if (hasValue)
        {
            Focus();
            Select(start + indexInText.Value, text.Length);
        }
        return hasValue;
    }




    private static object CoerceCanvasLeft(DependencyObject d, object basevalue)
    {
        DebugTools.DevAssert(basevalue is double, null);
        double num = (double)basevalue;
        if (!num.Equals(0.0))
        {
            num = 0.0;
        }
        return num;
    }




    private static void CanvasLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }




    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (FrameworkElement.StyleProperty.Name != dp.Name && Control.TemplateProperty.Name != dp.Name)
        {
            return base.ShouldSerializeProperty(dp);
        }
        return false;
    }




    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        OnGotFocusEvent(this);
    }




    public EngineError CurrentError
    {
        get
        {
            if (CurrentErrorInfo != null)
            {
                return CurrentErrorInfo.Error;
            }
            return null;
        }
    }




    public virtual void Dispose()
    {
        _model.CalculatableAdded -= (ICalculatable calc) =>
        {
            CalculatableAdded(this, calc);
        };
        _model.CalculatableRemoved -= CalculatableRemoved;
        _model.SynchronizationStatusChanged -= HandleSynchronizationStatusChanged;
        _model.CalculatingStatusChanged -= HandleCalculatingStatusChanged;
        if (IsTopLevel)
        {
            _model.UpdateEngineeringDocCalculationOptions(null);
            _model.UnHookSystemVariables();
            IncludeWorksheetModel model = _model;
            model.IsInFreemiumMode = (Func<bool>)Delegate.Remove(model.IsInFreemiumMode, (Func<bool>)IsInFreemiumMode);
            _model.ErrorsUpdated -= _errorwarningViewHandler.ShowErrors;
            _model.WarningsUpdated -= _errorwarningViewHandler.ShowWarnings;
        }
        _model.Dispose();
    }




    public void WaitForLoad(int seconds)
    {
        _model.WaitForLoad(seconds);
    }




    public IRegionFactory RegionFactory
    {
        get => _model.RegionFactory;
        set => _model.RegionFactory = value;
    }




    public void Reset()
    {
        _model.ResetDataRecursivly();
    }




    public void ReadCachedPart(PackagePart part)
    {
        _model.ReadCachedPart(part);
    }



    public event Func<string, PackagePart> GetPackagePartByRefId;




    public bool IsLastLoadedPath(string path)
    {
        return _model.IsLastLoadedPath(path);
    }




    public void Reload()
    {
        _model.ReloadIfNeeded();
    }




    public static bool IsInFreemiumMode()
    {
        if (FunctionalityService.Instance != null)
        {
            return !FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.IncludeWorksheetWithinWorksheetCommand);
        }
        return false;
    }




    internal List<string> ParentFileNames
    {
        private get => _model.ParentFileNames;
        set => _model.ParentFileNames = value;
    }




    internal bool IsParentDataUpdated
    {
        set => _model.IsParentDataUpdated = value;
    }




    public BackupData Data
    {
        get => _model.GetData(DocumentAttachedProperties.GetIsInPrinting(this));
        set => _model.SetData(value);
    }




    public bool CacheValid
    {
        get => _model.CacheValid;
        set => _model.CacheValid = value;
    }




    public ErrorWarningHandler ErrorWarningHandler => _errorwarningViewHandler;




    public IMultiLevelInclusionItem MultiLevelInclusionItem => _model;




    public void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        foreach (string text in propertyNames)
        {
            if (text == "FilePath")
            {
                HandleDocumentFilePathChanged(newProperties);
            }
        }
    }




    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        IList<IRegionPersistentData> result = _model.SetRegionPersistentData(data, GetPackagePartByRefId);
        IncludedFilePath = _model.IncludedFilePath;
        return result;
    }




    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        bool useCache = (bool)GetValue(DocumentAttachedProperties.CacheWorksheetProperty);
        if (DebugTools.DevAssert(s11NProvider != null && s11NProvider.Helper != null, null).Passed)
        {
            return _model.GetRegionPersistentData(useCache, s11NProvider, s11NProvider.Helper.GetRegionId(this).ToString());
        }
        return null;
    }




    public void DeserializeEpilog()
    {
        _model.DeserializeEpilog(SerializationHelperUtils.IsConvertedFromMathcad14(this));
    }




    public bool NotHookupSizeChangedHandlerUntilLoaded { get; set; }



    public event Action HookupSizeChangedHandler;




    public void FireHookupSizeChangedHandler()
    {
    }




    public IEnumerable<NodeMessageAdorner> NodeMessageAdorners => _errorwarningViewHandler.AdornersList;




    public string GetHelpContext()
    {
        return "IncludeWorksheetControl";
    }




    public Button GetIncludeButton => (Button)Template.FindName("_includeButton", this);




    private void MouseDownOpenFile(object sender, MouseButtonEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) <= 0)
        {
            return;
        }
        if (!IsInFreemiumMode())
        {
            AttachedProperties.SetIsBringToView(this, value: true);
            OpenFileCommand.Execute(FullFilePath, null);
        }
        else
        {
            DisplayFreemiumAboutBoxCommandParameter displayFreemiumAboutBoxCommandParameter = new DisplayFreemiumAboutBoxCommandParameter(GetIncludeButton, EnumFunctionalityKey.IncludeWorksheetWithinWorksheetCommand);
            FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox.Execute(displayFreemiumAboutBoxCommandParameter, null);
        }
        e.Handled = true;
    }




    private void ScrollViewerGotFocus(object sender, RoutedEventArgs e)
    {
        IsIBeamCursor = true;
    }




    private void ScrollViewerLostFocus(object sender, RoutedEventArgs e)
    {
        IsIBeamCursor = false;
    }




    private void CtrlButtonDown(object sender, KeyEventArgs e)
    {
        if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
        {
            IsIBeamCursor = false;
        }
    }




    private void CtrlButtonUp(object sender, KeyEventArgs e)
    {
        IsIBeamCursor = true;
    }




    private bool _contentLoaded;




    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Include;component/view/includeworksheetcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }




    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }




    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                ((ContentControl)target).MouseLeftButtonDown += MouseDownOpenFile;
                ((ContentControl)target).KeyDown += CtrlButtonDown;
                ((ContentControl)target).KeyUp += CtrlButtonUp;
                break;
            case 2:
                ((ScrollViewer)target).GotFocus += ScrollViewerGotFocus;
                ((ScrollViewer)target).LostFocus += ScrollViewerLostFocus;
                break;
        }
    }
}
