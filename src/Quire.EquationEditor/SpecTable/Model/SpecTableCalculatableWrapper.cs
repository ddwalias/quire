#nullable disable
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableCalculatableWrapper
{
    [MirrorVerified("Release IL")]
    public void RestoreResult() => throw new NotImplementedException();

    [MirrorVerified("Release IL")]
    public void SetResult(IResultNode result) { }

    [MirrorVerified("Release IL")]
    public void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap) { }

    [MirrorVerified("Release IL")]
    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap) { }

    [MirrorVerified("Release IL")]
    public bool CanHideFromCalculationContext => true;

    [MirrorVerified("Release IL")]
    private SynchronizationStatus f;

    [MirrorVerified("Release IL")]
    public SynchronizationStatus SynchronizationStatus => f;

    [MirrorVerified("Release IL")]
    private CalculatingStatus g;

    [MirrorVerified("Release IL")]
    public CalculatingStatus CalculatingStatus => g;

    [MirrorVerified("Release IL")]
    private EngineResult i;

    [MirrorVerified("Release IL")]
    public EngineResult EngineResult { get => i; set => i = value; }

    [MirrorVerified("Release IL")]
    public bool InterceptAndUpdateResult => true;

    [MirrorVerified("Release IL")]
    public bool UseICalculatableToRestoreResult => true;

    [MirrorVerified("Release IL")]
    public string ErrorText => throw new NotImplementedException();

    [MirrorVerified("Release IL")]
    public EngineError CurrentError => Error;

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }

        [MirrorVerified("Release IL")]
        internal void a(object valueArg, RequestShowErrorEventArgs requestShowErrorEventArgs) { }

        [MirrorVerified("Release IL")]
        internal EngineRedefInfo a(ValueTuple<RedefinitionWarning, MathNode> valueTuple) => valueTuple.Item1.Original;
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> RequestEvaluation;
    public event global::System.Func<global::Quire.Calculation.Core.ICalculatable, string, global::Quire.Calculation.Core.IResultNode> GetValueOfIdentifierBefore;
    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> TreatAsUpToDate;
    public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    public event global::System.EventHandler<global::Quire.Calculation.Core.RequestShowErrorEventArgs> RequestShowError;
}
