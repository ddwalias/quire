#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class IOManager
{
    private int _defaultCalculationTimeout;

    public int DefaultCalculationTimeout
    {
        get => _defaultCalculationTimeout;
        set
        {
            if (value < -1)
            {
                return;
            }
            _defaultCalculationTimeout = value;
        }
    }

    public bool IsValidAlias(string alias, bool isIn)
    {
        if (string.IsNullOrEmpty(alias))
        {
            return false;
        }
        if (isIn)
        {
            return Inputs.GetItem(alias, false) == null;
        }
        return Outputs.GetItem(alias, false) == null;
    }

    private void FireIODataChanged()
    {
        if (IODataChanged != null)
        {
            IODataChanged.Invoke();
        }
    }

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
        internal UnitedScalarState(double value, string units, IOManagerErrorCode errorCode)
        {
            Value = value;
            Units = units;
            ErrorCode = errorCode;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public double Value { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Units { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class UnitedMatrixState
    {
        internal UnitedMatrixState(double[][] value, string units, IOManagerErrorCode errorCode)
        {
            Value = value;
            Units = units;
            ErrorCode = errorCode;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public double[][] Value { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Units { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class StringState
    {
        private string _value;

        internal StringState(string value, IOManagerErrorCode errorCode)
        {
            Value = value;
            ErrorCode = errorCode;
        }

        public string Value
        {
            get
            {
                DebugTools.DevAssert(_value == null || ErrorCode == IOManagerErrorCode.NoError, null);
                return _value;
            }
            private set => _value = value;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class ErrorState
    {
        public ErrorState(IOManagerErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode ErrorCode { get; private set; }
    }

    partial class UnitedValueGetter
    {
        public virtual bool IsScalingValid()
        {
            return EnumerableExtensions.IsNullOrEmpty<char>(ScaledByUnits) || EnumerableExtensions.IsNotEmpty<char>(ScalingSExpression);
        }

        protected UnitedValueGetter(string alias, string scaledByUnits)
        {
            Alias = alias;
            ScaledByUnits = scaledByUnits;
            ScalingSExpression = EnumerableExtensions.IsNotEmpty<char>(scaledByUnits) ? InfixMonomialToSExpressionTransform.GetInstance(true).Transform(ScaledByUnits) : string.Empty;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string ScaledByUnits { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Alias { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string ScalingSExpression { get; private set; }
    }

    partial class ScalarOutputGetter
    {
        string IItemAlias.Alias => Alias;

        private readonly GetOutputValueTypes _valueTypes;

        public ScalarOutputGetter(string alias, GetOutputValueTypes valueTypes, string scaledByUnits = default)
            : base(alias, scaledByUnits)
        {
            _valueTypes = valueTypes;
            DebugTools.DevAssert((uint)valueTypes > 0, null);
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedScalarState ScalarState { get; private set; }
    }

    partial class ItemAliasComparer
    {
        public override bool Equals(IItemAlias x, IItemAlias y)
        {
            return x.Alias.Equals(y.Alias, _caseInsensitiveAliasComparisonEnabled ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        public override int GetHashCode(IItemAlias obj)
        {
            return obj.Alias.ToUpperInvariant().GetHashCode();
        }

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
        string IItemAlias.Alias => Alias;

        private readonly GetInputValueTypes _valueTypes;

        public ScalarInputGetter(string alias, GetInputValueTypes valueTypes, string scaledByUnits = default)
            : base(alias, scaledByUnits)
        {
            _valueTypes = valueTypes;
            DebugTools.DevAssert((uint)valueTypes > 0, null);
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedScalarState OriginalScalarState { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedScalarState ScalarState { get; private set; }
    }

    partial class UnitedMatrixGetter
    {
        string IItemAlias.Alias => Alias;

        private readonly CalculationOverridesTypes _getterType;

        public UnitedMatrixGetter(string alias, bool isInput, string scaledByUnits = default)
            : base(alias, scaledByUnits)
        {
            _getterType = isInput ? CalculationOverridesTypes.InputOverriddenValueGetter : CalculationOverridesTypes.OutputOverriddenValueGetter;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IUnitedMatrixState MatrixState { get; private set; }
    }

    partial class StringGetter
    {
        private readonly CalculationOverridesTypes _getterType;

        public StringGetter(string alias, bool isInput)
            : base(alias, null)
        {
            _getterType = isInput ? CalculationOverridesTypes.InputOverriddenValueGetter : CalculationOverridesTypes.OutputOverriddenValueGetter;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IStringState StringState { get; private set; }
    }

    partial class CommonValueGetter
    {
        private readonly CalculationOverridesTypes _getterType;

        public CommonValueGetter(string alias, bool isInput)
            : base(alias, null)
        {
            _getterType = isInput ? CalculationOverridesTypes.InputOverriddenValueGetter : CalculationOverridesTypes.OutputOverriddenValueGetter;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IErrorState ErrorState { get; private set; }
    }

    partial class InputOutputItemsStates
    {
        public InputOutputItemsStates(IList<IScalarItemWithOriginalState> inputs, IList<IScalarItem> outputs)
        {
            InputItems = inputs;
            OutputItems = outputs;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IList<IScalarItemWithOriginalState> InputItems { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IList<IScalarItem> OutputItems { get; private set; }
    }

    partial class SetValueResults
    {
        public SetValueResults(IEnumerable<ISingleValueSetter> setters)
        {
            _operationsResults = setters.Select(setter => setter.OperationResult).ToList();
            _aliasToOperationResultsMap = setters.ToDictionary(setter => setter.Alias, setter => setter.OperationResult);
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private readonly IList<IOManagerErrorCode> _operationsResults;

        private readonly IDictionary<string, IOManagerErrorCode> _aliasToOperationResultsMap;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public int Count => _operationsResults.Count;

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IEnumerable<IOManagerErrorCode> AllResults => _operationsResults;
    }

    partial class UnitlessValueSetter
    {
        protected UnitlessValueSetter(string alias)
        {
            Alias = alias;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public string Alias { get; private set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        public IOManagerErrorCode OperationResult { get; set; }
    }

    partial class UnitedValueSetter
    {
        protected UnitedValueSetter(string alias, string units)
            : base(alias)
        {
            Units = units;
            ScalingSExpression = EnumerableExtensions.IsNotEmpty<char>(Units) ? InfixMonomialToSExpressionTransform.GetInstance(true).Transform(Units) : string.Empty;
        }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        private string Units { get; set; }

        [MirrorVerified("PtcControlsIOIOManagerMechanicalDifferentialTests")]
        protected string ScalingSExpression { get; private set; }
    }

    partial class SExpressionSetter
    {
        private string _sexprScaling;

        public SExpressionSetter(string alias, string sexpr)
            : base(alias, null)
        {
            _sexprValue = sexpr;
            _sexprScaling = null;
            SExpressionHelperUtilites.SplitScalingAndValue(ref _sexprValue, out _sexprScaling);
        }

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

    partial class StringValueSetter
    {
        private readonly string _stringValueSExpr;

        public StringValueSetter(string alias, string stringValue)
            : base(alias)
        {
            _stringValueSExpr = stringValue == null ? null : string.Format("\"{0}\"", SExpressionUtility.EscapeStringValue(stringValue));
        }
    }

    partial class ValuesSetter
    {
        private readonly List<ISingleValueSetter> _setters;

        private readonly WeakReference _managerRef;

        public ValuesSetter(IOManager manager)
        {
            _managerRef = new WeakReference(manager);
            _setters = new List<ISingleValueSetter>();
        }
    }

    partial class UnitedMatrixSetter
    {
        private readonly double[,] _value;

        public UnitedMatrixSetter(string alias, object[,] value, string units)
            : base(alias, units)
        {
            if (value == null)
            {
                return;
            }
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);
            bool valid = true;
            _value = new double[rows, cols];
            for (int i = 0; i < rows & valid; i++)
            {
                for (int j = 0; j < cols & valid; j++)
                {
                    if (value[i, j] is double)
                    {
                        _value[i, j] = (double)value[i, j];
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }
            if (!valid)
            {
                _value = null;
            }
        }
    }
}
