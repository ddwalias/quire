#nullable disable
#pragma warning disable

namespace Quire.Controls.Plots.ViewModel;

public partial class PlotViewModelBase<TModel>
{
    protected TModel Model { get; set; }

    private bool _showControlLevelErrorFrameForNestedError;

    private bool _areAxisExpressionsVisible;

    private bool _calcIsDisabled;

    private bool _isPendingActivation;

    private bool _isActiveRegion;

    private global::Quire.Calculation.Core.Support.CalculatableChildHider _childHider;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;

    internal PlotViewModelBase() { }

    private void OnModelDeserializationEnded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void OnModelChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void DelegateModelEvent(global::System.Action handler) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected virtual void BindCommandsAndEvents() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected override void ModelPropertyChanged(string propertyName) => base.ModelPropertyChanged(propertyName);

    public bool ShowControlLevelErrorFrameForNestedError
    {
        get
        {
            return _showControlLevelErrorFrameForNestedError;
        }
        private set
        {
            if (value != _showControlLevelErrorFrameForNestedError)
            {
                _showControlLevelErrorFrameForNestedError = value;
                OnPropertyChanged("ShowControlLevelErrorFrameForNestedError");
            }
        }
    }

    public bool AreAxisExpressionsVisible
    {
        get
        {
            return _areAxisExpressionsVisible;
        }
        private set
        {
            if (_areAxisExpressionsVisible != value)
            {
                _areAxisExpressionsVisible = value;
                OnAxisExpressionsVisibilityChanged();
                OnPropertyChanged("AreAxisExpressionsVisible");
            }
        }
    }

    protected virtual void OnAxisExpressionsVisibilityChanged() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool CalcIsDisabled
    {
        get
        {
            return _calcIsDisabled;
        }
        set
        {
            if (_calcIsDisabled != value)
            {
                _calcIsDisabled = value;
                OnPropertyChanged("CalcIsDisabled");
            }
        }
    }

    public bool HideFromCalculationContext { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool PendingActivation { private get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public virtual bool IsActiveRegion { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    protected void UpdateShowControlLevelErrorFrameForNestedError() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CollectHiddenAndVisibleCalculatables() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ComputeAreAxisExpresionsVisible() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool CalculateAreAxisExpressionsVisible { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool ShowAxisExpressions { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public bool IsCalculating { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public global::Quire.Controls.Core.BackgroundTypeE PlotBackgroundType { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public double GraphWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public double GraphHeight { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public static readonly global::System.Windows.Input.RoutedCommand ShowAxisExpressionsCommand;

    internal event global::System.Action ModelChanged;

    internal event global::System.Action DeserializationEnded;

    private void ShowAxisExpressionsCommandExecute(object arg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal void OnGraphSizeChanged(global::System.Windows.Size newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetDataObject(global::Quire.Serialization.IDeS11NProvider de11NProvider, global::Quire.PersistentDataObjects.plotType dataObject) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.PersistentDataObjects.plotType GetDataObject(global::Quire.Serialization.IS11NProvider s11NProvider) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void FireCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> CalculatableChildren { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    public global::Quire.Calculation.Core.EngineError PlotCalculatableError { get; private set; }

    private double _currentMinimalGraphWidth;

    public double CurrentMinimalGraphWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    protected void OnCalculatableAdded(global::Quire.Calculation.Core.ICalculatable calculatable) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void OnCalculatableRemoved(global::Quire.Calculation.Core.ICalculatable calculatable) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Commit() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void ModelPlotCalculatableError(object sender, global::Quire.Controls.Plots.Model.EngineErrorEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
