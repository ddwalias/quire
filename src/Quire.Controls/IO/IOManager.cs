#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class IOManager
{
    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly IEngineeringDocument _document;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private void FireWaitForCalculation(DependencyObject region, int seconds) => _document.FireWaitForCalculationIdle(seconds, ShadowCalculatable.ShadowResultsProcessingPriority_Send);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IScalarItem InputOutputGetRealValue(string alias, bool isInp) => InputOutputGetRealValueAs(alias, isInp, null);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IMatrixItem InputOutputGetMatrixValue(string alias, bool isInp) => InputOutputGetMatrixValueAs(alias, isInp, null);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IValuesSetter CreateScaledValueSettersCollection() => new ValuesSetter(this);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private void GetValue(bool asInp, IUnitedValueGetter unitedValueGetter) => GetComputedValueScaledBy(asInp, unitedValueGetter);

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public bool GetAndSetOperationsCompareAliasesCaseInsensitively { private get; set; }

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly IOItemCollection _inputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IIOItemCollection Inputs => _inputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly IOItemCollection _outputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IIOItemCollection Outputs => _outputs;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    private readonly ShadowCalculationNotifier _shadowCalculatableNotifier;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public IShadowCalculationNotifier ShadowCalculationsChangedNotifier => _shadowCalculatableNotifier;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public event Action<IEngineeringDocument, List<string>, List<string>> IORegionsSelected;

    [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
    public event Action IODataChanged;

    partial class InputOutputStateGetterNotifier
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public void Dispose() => AbortProgressTracking();

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public event ProgressChangedEventHandler ProgressChanged;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public event Action<bool> ProgressStopped;
    }

    partial class UnitedScalarState
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public double Value { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Units { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class UnitedMatrixState
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public double[][] Value { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Units { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class StringState
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class ErrorState
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class UnitedValueGetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string ScaledByUnits { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Alias { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string ScalingSExpression { get; private set; }
    }

    partial class ScalarOutputGetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedScalarState ScalarState { get; private set; }
    }

    partial class ItemAliasComparer
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly bool _caseInsensitiveAliasComparisonEnabled;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public ItemAliasComparer(bool caseInsensitiveAliasComparisonEnabled)
        {
            _caseInsensitiveAliasComparisonEnabled = caseInsensitiveAliasComparisonEnabled;
        }
    }

    partial class ScalarInputGetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedScalarState OriginalScalarState { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedScalarState ScalarState { get; private set; }
    }

    partial class UnitedMatrixGetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedMatrixState MatrixState { get; private set; }
    }

    partial class StringGetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IStringState StringState { get; private set; }
    }

    partial class CommonValueGetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IErrorState ErrorState { get; private set; }
    }

    partial class InputOutputItemsStates
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IList<IScalarItemWithOriginalState> InputItems { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IList<IScalarItem> OutputItems { get; private set; }
    }

    partial class SetValueResults
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly IList<IOManagerErrorCode> _operationsResults;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public int Count => _operationsResults.Count;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IEnumerable<IOManagerErrorCode> AllResults => _operationsResults;
    }

    partial class UnitlessValueSetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Alias { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode OperationResult { get; set; }
    }

    partial class UnitedValueSetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private string Units { get; set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected string ScalingSExpression { get; private set; }
    }

    partial class SExpressionSetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private string _sexprValue;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected override bool IsValueValid() => EnumerableExtensions.IsNotEmpty<char>(_sexprValue);
    }

    partial class UnitedScalarSetter
    {
        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly double _value;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public UnitedScalarSetter(string alias, double value, string units) : base(alias, units)
        {
            _value = value;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected override bool IsValueValid() => DoubleExtension.IsValid(_value);
    }
}
