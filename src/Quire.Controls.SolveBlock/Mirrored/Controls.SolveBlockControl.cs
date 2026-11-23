using System.Linq;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Core;

namespace Quire.Controls;

public partial class SolveBlockControl
{
    public static readonly DependencyProperty IsSolveBlockLabelVisibleProperty = DependencyProperty.Register(
        "IsSolveBlockLabelVisible", typeof(bool), typeof(SolveBlockControl), new PropertyMetadata(true));

    protected override ISerializationStrategy SerializationStrategy { get; set; }
    protected override IDeserializationStrategy DeserializationStrategy { get; set; }
    public ITextStyleChangedHandler TextStyleChangedHandler { get; set; }

    public bool IsSolveBlockLabelVisible
    {
        get => (bool)((DependencyObject)this).GetValue(IsSolveBlockLabelVisibleProperty);
        set => ((DependencyObject)this).SetValue(IsSolveBlockLabelVisibleProperty, value);
    }

    public global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> SetRegionPersistentData(
        global::Quire.Serialization.IDeS11NProvider deS11NProvider, object data)
    {
        if (data is global::Quire.PersistentDataObjects.solveBlockType solveBlockType2)
        {
            global::System.Collections.Generic.IList<global::Quire.Serialization.IRegionPersistentData> result =
                DeserializationStrategy.SetRegions(solveBlockType2.regions, deS11NProvider);
            DeserializeRegionsEpilog(deS11NProvider, solveBlockType2);
            return result;
        }

        throw new global::System.ArgumentException("should be of solveBlockType");
    }

    public object GetRegionPersistentData(global::Quire.Serialization.IS11NProvider s11NProvider)
    {
        global::Quire.PersistentDataObjects.solveBlockType result = new global::Quire.PersistentDataObjects.solveBlockType
        {
            Regions = SerializationStrategy.GetRegions(s11NProvider)
        };
        SerializeRegionsEpilog(s11NProvider, result);
        return result;
    }

    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GuessValues =>
        _sbModel.GuessValueItems;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> Constraints =>
        _sbModel.ConstraintItems;

    public global::Quire.Controls.Core.IWhiteboardItem Solver => _sbModel.SolverItem;

    public double GuessValuesIndicatorTop => AggregateViewModel.SectionIndicators[0].Top;

    public double GuessValuesIndicatorBottom => GuessValuesIndicatorTop + AggregateViewModel.SectionIndicators[0].Height;

    public double ConstraintsIndicatorTop => AggregateViewModel.SectionIndicators[1].Top;

    public double ConstraintsIndicatorBottom => ConstraintsIndicatorTop + AggregateViewModel.SectionIndicators[1].Height;

    public double SolverIndicatorTop => AggregateViewModel.SectionIndicators[2].Top;

    public double SolverIndicatorBottom => SolverIndicatorTop + AggregateViewModel.SectionIndicators[2].Height;

    // Keep these members before EngineResults to preserve its Mathcad closure ordinal.
    object global::System.ICloneable.Clone() => Clone();

    public SolveBlockControl Clone() => new SolveBlockControl(this);

    public global::Quire.Controls.SolveBlockEquationTypes GetSolveBlockEquationType(
        global::Quire.Controls.Core.IWhiteboardItem item) => _sbModel.GetSolveBlockEquationType(item);

    public override int ErrorCount => _sbModel.ChildErrorCountTotal;

    private void HandleChildCalcErrorChanged()
    {
        FirePropertyChanged(global::Quire.Calculation.Core.ErrorChangedEventArgs.GetArgs());
    }

    private void UpdateGridCellSizeIfNeeded(object sender, RoutedEventArgs e)
    {
        Size gridCellSize = WorksheetPropertiesManager.GetWorksheetProperties(this).GridCellSize;
        if (!GridCellSize.IsEqualTo(gridCellSize))
            GridCellSize = gridCellSize;
    }

    public override global::Quire.Calculation.Core.EngineError Error
    {
        get
        {
            global::Quire.Calculation.Core.ICalculatableInfo errorInfo = CalculatableChildren
                .OfType<global::Quire.Calculation.Core.ICalculatableInfo>()
                .FirstOrDefault((global::Quire.Calculation.Core.ICalculatableInfo cinfo) => ErrorCount != 0);
            if (errorInfo == null)
                return null;
            return errorInfo.Error;
        }
    }

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatableResultOwner>
        ResultOwners => _sbModel.AllCalculatables;

    public override string GetBlockEquationTypeAttributeName(
        global::Quire.Controls.Core.IWhiteboardItem item) => _sbModel.GetBlockEquationTypeAttributeName(item);

    protected override string GetInternalHelpContext() => "SolveBlockControl";

    public void ReplaceCalculatablesWithWrappersInResultManager()
    {
        foreach (global::Quire.Controls.SolveBlockCalculatable calculatable in _sbModel.AllCalculatables)
        {
            global::Quire.Calculation.Core.ResultManager.Instance.ReplaceCalculatable(
                calculatable.WrappedCalculatable, calculatable);
        }
    }

    public void ReplaceCalculatablesAndResultWithWrappersInResultManager()
    {
        foreach (global::Quire.Controls.SolveBlockCalculatable calculatable in _sbModel.AllCalculatables)
        {
            global::Quire.Calculation.Core.ResultManager.Instance.ReplaceCalculatableAndResult(
                calculatable.WrappedCalculatable, calculatable);
        }
    }

    private void HookupEvents()
    {
        if (_sbModel != null)
            _sbModel.ChildCalcErrorChanged += HandleChildCalcErrorChanged;
    }

    public override void EndInit()
    {
        base.EndInit();
        if (_sbModel != null)
            global::Quire.Calculation.Core.ResultManager.DispatchResults(_engineResults, ResultOwners);
    }

    public EngineResult[] EngineResults
    {
        get
        {
            _engineResults = _sbModel.AllCalculatables.Select((ISolveBlockCalculatable wrapper) => wrapper.EngineResult).ToArray();
            return _engineResults;
        }
        set => _engineResults = value;
    }

}
