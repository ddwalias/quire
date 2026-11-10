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

    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GuessValues =>
        _sbModel.GuessValueItems;

    public global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> Constraints =>
        _sbModel.ConstraintItems;

    public global::Quire.Controls.Core.IWhiteboardItem Solver => _sbModel.SolverItem;

    // Keep these members before EngineResults to preserve its Mathcad closure ordinal.
    object global::System.ICloneable.Clone() => Clone();

    public SolveBlockControl Clone() => new SolveBlockControl(this);

    public global::Quire.Controls.SolveBlockEquationTypes GetSolveBlockEquationType(
        global::Quire.Controls.Core.IWhiteboardItem item) => _sbModel.GetSolveBlockEquationType(item);

    public override int ErrorCount => _sbModel.ChildErrorCountTotal;

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

    private void HandleChildCalcErrorChanged()
    {
        FirePropertyChanged(global::Quire.Calculation.Core.ErrorChangedEventArgs.GetArgs());
    }

    public override void EndInit()
    {
        base.EndInit();
        if (_sbModel != null)
            global::Quire.Calculation.Core.ResultManager.DispatchResults(_engineResults, ResultOwners);
    }

    private void UpdateGridCellSizeIfNeeded(object sender, RoutedEventArgs e)
    {
        Size gridCellSize = WorksheetPropertiesManager.GetWorksheetProperties(this).GridCellSize;
        if (!GridCellSize.IsEqualTo(gridCellSize))
            GridCellSize = gridCellSize;
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
