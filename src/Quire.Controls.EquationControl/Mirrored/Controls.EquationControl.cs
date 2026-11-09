using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
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
}
