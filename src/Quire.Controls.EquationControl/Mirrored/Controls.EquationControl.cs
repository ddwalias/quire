using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Internal;
using Quire.Core;
using Quire.EquationEditor;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls;

public partial class EquationControl
{
    public EquationControl() : this((Quire.EquationEditor.IEquationModel)null) { }

    public bool IsEmpty => _model.IsEmpty;

    public IResultNode Result => null;

    static EquationControl()
    {
        RecommendedSymbolicResultsWidth = WpfUtils.InchesToDips(5.0);
        ChangedEvent = EventManager.RegisterRoutedEvent("Changed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EquationControl));
        _typeOfThis = typeof(EquationControl);
        // Release IL uses the original enum values, which differ from the legacy enum declarations.
        SynchronizationStatusProperty = DependencyProperty.Register("SynchronizationStatus", typeof(SynchronizationStatus), _typeOfThis,
            new FrameworkPropertyMetadata((SynchronizationStatus)3, FrameworkPropertyMetadataOptions.Inherits, HandleSynchronizationStatusChanged));
        CalculatingStatusProperty = DependencyProperty.Register("CalculatingStatus", typeof(CalculatingStatus), _typeOfThis,
            new FrameworkPropertyMetadata((CalculatingStatus)2, FrameworkPropertyMetadataOptions.Inherits, HandleCalculatingStatusChanged));
        ActiveElementOpacityProperty = DependencyProperty.Register("ActiveElementOpacity", typeof(double), typeof(FrameworkElement),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        ActiveNonNavigatableElementOpacityProperty = DependencyProperty.Register("ActiveNonNavigatableElementOpacity", typeof(double), typeof(FrameworkElement),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        IsEmbeddedProperty = DependencyProperty.Register("IsEmbedded", typeof(bool), _typeOfThis);
        IsResizableInputControlProperty = DependencyProperty.Register("IsResizableInputControl", typeof(bool), _typeOfThis,
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        CurrentCaretInsertionTypeProperty = DependencyProperty.Register("CurrentCaretInsertionType", typeof(CaretInsertionTypes), _typeOfThis,
            new FrameworkPropertyMetadata(CaretInsertionTypes.Normal));
        CurrentNestedResultMatrixPositionProperty = DependencyProperty.Register("CurrentNestedResultMatrixPosition", typeof(IList<TableCoordinate>), _typeOfThis,
            new FrameworkPropertyMetadata(HandleCurrentNestedResultMatrixPositionChanged));
        RootPaddingProperty = DependencyProperty.RegisterAttached("RootPadding", typeof(Thickness), _typeOfThis,
            new FrameworkPropertyMetadata(new Thickness(4.0), FrameworkPropertyMetadataOptions.Inherits));
        _unitLabel = new IdLabelToStringConverter().Convert(IdLabels.Unit, typeof(string), null, null).ToString().ToLower();
        DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        AppServiceRegistry.AddService<IFontManager>(new FontManager());
        WhiteboardDependencyProperties.IsActiveProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsActiveChanged));
        WhiteboardDependencyProperties.IsSelectedProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(IsSelectedChanged));
        EngineErrorMessages.MergeDictionary(ResourceProvider<EquationControlErrorMessages>.Instance);
        AttachedProperties.CanActivateProperty.OverrideMetadata(typeof(EquationControl), new FrameworkPropertyMetadata(HandleCanActivateChanged));
        CalcIsDisabledProperty = AttachedProperties.CalcIsDisabledProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleCalcIsDisabledChanged));
        ResultMatrixFormatProperty = AttachedProperties.ResultMatrixFormatProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleResultMatrixFormatChanged));
        DisplayNumberFormatProperty = AttachedProperties.DisplayNumberFormatProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(HandleDisplayNumberFormatChanged));
        AttachedProperties.LabeledIdFormatProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleLabeledIdFormatChanged));
        IOTag.IOTagProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(HandleIOTagPropertyChanged));
        ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(typeof(EquationControl));
    }

    internal partial class TestHelper
    {
        public static void SetEquationXmlNode(EquationControl eq, global::System.Xml.XmlNode xmlNode)
        {
            eq.EquationXmlNode = xmlNode;
        }

        public TestHelper()
        {
        }
    }

    public bool IsCommitted => !_model.IsExpressionValueModified;

    public bool IsSelected => WhiteboardDependencyProperties.GetIsSelected(this);

    public bool IsReadonly => _model.IsReadonly;

    public bool IsScoped => _model.Mode == EquationEditModeEnum.Scope;

    public bool IsTypeover => _model.Mode == EquationEditModeEnum.Typeover;

    public bool IsScriptableControl => _model.IsScriptableControl;

    public bool AtLeftEdge => _model.AtLeftEdge();

    public Direction ScopeDirection => _model.ScopeSide;

    public bool CanHideFromCalculationContext => true;

    public bool InterceptAndUpdateResult => false;

    public bool UseICalculatableToRestoreResult => false;

    public IDataExchangeModel DataExchangeModel => _model.DataExchangeModel;

    public string ErrorSExpression
    {
        get
        {
            if (_model.ErrorNode != null)
            {
                return _model.ErrorNode.SExpression;
            }

            return "";
        }
    }

    public string TypeoverSExpression => _model.TypeoverSExpression;

    public bool CurrentNodeIsNumeric => _model.CurrentModeNavigatorState.Current.DescriptorName == "NUMERIC";

    internal double ViewFullWidth => _view.ActualWidth;

    internal double ViewClippedWidth => _view.DesiredSize.Width;

    public EquationTypes EquationType => _model.EquationType;

    public string RegionType => "equation";

    public int CurrentNodeCaretPosition => _model.CurrentModeNavigatorState.CaretPosition.GetValueOrDefault(-1);

    public bool CurrentNodeIsIdentifier => _model.CurrentModeNavigatorState.Current.DescriptorName == "IDENTIFIER";

    public bool IsEmbedded
    {
        get => (bool)GetValue(IsEmbeddedProperty);
        set => SetValue(IsEmbeddedProperty, value);
    }

    public double TextBaselineY => _view.TextBaselineY;

    public bool IsAdornedElementReady
    {
        get
        {
            UIElement view = _view;
            if (view != null)
            {
                return WpfUtils.GetVisualParent(view) != null;
            }

            return false;
        }
    }

    public string ErrorCode
    {
        get
        {
            if (_model.CalculationError != null && !string.IsNullOrEmpty(_model.CalculationError.ErrorCode))
            {
                return _model.CalculationError.ErrorCode;
            }

            return "";
        }
    }

    public bool IsShrunk => IsResizable && IsExceedsWidthConstrain;

    public bool ContainsSymbolicEvaluation
    {
        get
        {
            var root = _model.EvaluationRoot;
            if (root != null)
            {
                return root.DescriptorName == "SYMBOLIC_EVALUATION" || root[0].DescriptorName == "SYMBOLIC_EVALUATION";
            }

            return false;
        }
    }

    public bool IsResizable
    {
        get
        {
            if (ContainsSymbolicEvaluation && SymbolicResultsWidth > RecommendedSymbolicResultsWidth)
            {
                return !IsEmbedded;
            }

            return false;
        }
    }

    public double SymbolicResultsWidth
    {
        get
        {
            if (!ContainsSymbolicEvaluation)
            {
                return 0.0;
            }

            var node = _model.EvaluationRoot;
            while (node[0].DescriptorName == "SYMBOLIC_EVALUATION")
            {
                node = node[0];
            }

            var symbolicNode = node[2];
            var visualNode = _map.AssociateNode(symbolicNode) as VisualMathNode;
            var origin = visualNode.TranslatePoint(new Point(0, 0), _view);
            return ViewFullWidth - origin.X;
        }
    }

    public double TextBaselineOffset
    {
        get
        {
            if (FindCommonVisualAncestor(_view) == null)
            {
                return _view.TextBaselineY;
            }

            // Release IL builds the point in a local before the call; a plain new Point(...) argument does not match.
            Point point;
            return _view.TransformToVisual(this).Transform(point = new Point(0, _view.TextBaselineY)).Y;
        }
    }

    public bool IsExceedsWidthConstrain
    {
        get
        {
            double resizeIndicatorWidth = IsEmbedded ? ResizeIndicatorWidth : 0.0;
            return ViewFullWidth > ActualWidth && ViewClippedWidth < ViewFullWidth - resizeIndicatorWidth;
        }
    }

    public double ResizeIndicatorWidth
    {
        get
        {
            var indicatorColumn = Template.FindName("PART_ResizeIndicatorColumn", this) as ColumnDefinition;
            if (indicatorColumn != null)
            {
                return indicatorColumn.ActualWidth;
            }

            return 0.0;
        }
    }

    public bool ErrorVisible
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public EngineError Error => _model.CalculationError;

    public IList<EngineRedefInfo> EngineWarnings => _model.EngineWarnings;

    public EquationEditModeEnum Mode => _model.Mode;

    internal MathNode CurrentNode => _model.NavigatorState.Current;

    public double VerticalAlignmentOffset => -TextBaselineOffset;

    public CalculatingStatus CalculatingStatus => (CalculatingStatus)GetValue(CalculatingStatusProperty);

    public SynchronizationStatus SynchronizationStatus => (SynchronizationStatus)GetValue(SynchronizationStatusProperty);

    public int ErrorCount
    {
        get
        {
            if (Error == null)
            {
                return _model.ScriptingError?.ResourceString != null ? 1 : 0;
            }

            return 1;
        }
    }

    public string ErrorText
    {
        get
        {
            if (GetErrorDisplayText != null)
            {
                return string.Concat(GetErrorDisplayText(_model.CalculationError), _model.ScriptingError?.ResourceString);
            }

            return ErrorCode;
        }
    }

    public double ActiveElementOpacity
    {
        get => (double)GetValue(ActiveElementOpacityProperty);
        set => SetValue(ActiveElementOpacityProperty, value);
    }

    public bool IsResizableInputControl
    {
        get => (bool)GetValue(IsResizableInputControlProperty);
        set => SetValue(IsResizableInputControlProperty, value);
    }

    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public string ResultScalingSExpression
    {
        get => _model.ResultScalingSExpression;
        set => _model.ResultScalingSExpression = value;
    }

    public bool ProhibitComboBox
    {
        get => _model.ProhibitComboBox;
        set => _model.ProhibitComboBox = value;
    }

    public bool ProhibitInputControl
    {
        get => _model.ProhibitInputControl;
        set => _model.ProhibitInputControl = value;
    }

    public bool AllowExpressionsOnly
    {
        get => _model.AllowExpressionsOnly;
        set => _model.AllowExpressionsOnly = value;
    }

    public bool IsMathFormatHandledLocally
    {
        get => _model.IsMathFormatHandledLocally;
        set => _model.IsMathFormatHandledLocally = value;
    }

    public bool OverrideIdLabelByResolvedForDefinitionLhs
    {
        get => _model.OverrideIdLabelByResolvedForDefinitionLhs;
        set => _model.OverrideIdLabelByResolvedForDefinitionLhs = value;
    }

    public EngineError CurrentError => Error;

    public string EquationSExpressionWithResolvedLabels => _model.EquationSExpressionWithResolvedLabels;
}
