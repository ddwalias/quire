# Ptc.Controls.IO.IOManager

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOIOManagerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `FireWaitForCalculation(DependencyObject, int)` | 0x0600026a | MemberGen: `_document.FireWaitForCalculationIdle(seconds, ShadowCalculatable.ShadowResultsProcessingPriority_Send)` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputGetRealValue(string, bool)` | 0x06000283 | MemberGen: `InputOutputGetRealValueAs(alias, isInp, null)` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputGetMatrixValue(string, bool)` | 0x06000284 | MemberGen: `InputOutputGetMatrixValueAs(alias, isInp, null)` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateScaledValueSettersCollection()` | 0x0600028e | MemberGen: `new ValuesSetter(this)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValue(bool, IUnitedValueGetter)` | 0x060002a1 | MemberGen: `GetComputedValueScaledBy(asInp, unitedValueGetter)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetAndSetOperationsCompareAliasesCaseInsensitively get` | 0x0600026f | MemberGen: `public bool GetAndSetOperationsCompareAliasesCaseInsensitively { private get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetAndSetOperationsCompareAliasesCaseInsensitively set` | 0x06000270 | MemberGen: `public bool GetAndSetOperationsCompareAliasesCaseInsensitively { private get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Inputs get` | 0x06000273 | MemberGen: `_inputs` | Mechanical; Release IL equals Mathcad's | Verified |
| `Outputs get` | 0x06000274 | MemberGen: `_outputs` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShadowCalculationsChangedNotifier get` | 0x06000276 | MemberGen: `_shadowCalculatableNotifier` | Mechanical; Release IL equals Mathcad's | Verified |
| `IORegionsSelected add` | 0x06000264 | MemberGen: `public event Action<IEngineeringDocument, List<string>, List<string>> IORegionsSelected;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IORegionsSelected remove` | 0x06000265 | MemberGen: `public event Action<IEngineeringDocument, List<string>, List<string>> IORegionsSelected;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IODataChanged add` | 0x06000266 | MemberGen: `public event Action IODataChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IODataChanged remove` | 0x06000267 | MemberGen: `public event Action IODataChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputStateGetterNotifier: Dispose()` | 0x0600033c | MemberGen: `AbortProgressTracking()` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputStateGetterNotifier: ProgressChanged add` | 0x06000332 | MemberGen: `public event ProgressChangedEventHandler ProgressChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputStateGetterNotifier: ProgressChanged remove` | 0x06000333 | MemberGen: `public event ProgressChangedEventHandler ProgressChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputStateGetterNotifier: ProgressStopped add` | 0x06000334 | MemberGen: `public event Action<bool> ProgressStopped;` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputStateGetterNotifier: ProgressStopped remove` | 0x06000335 | MemberGen: `public event Action<bool> ProgressStopped;` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarState: Value get` | 0x06000353 | MemberGen: `public double Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarState: Value set` | 0x06000354 | MemberGen: `public double Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarState: Units get` | 0x06000355 | MemberGen: `public string Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarState: Units set` | 0x06000356 | MemberGen: `public string Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarState: ErrorCode get` | 0x06000357 | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarState: ErrorCode set` | 0x06000358 | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixState: Value get` | 0x0600035a | MemberGen: `public double[][] Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixState: Value set` | 0x0600035b | MemberGen: `public double[][] Value { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixState: Units get` | 0x0600035c | MemberGen: `public string Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixState: Units set` | 0x0600035d | MemberGen: `public string Units { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixState: ErrorCode get` | 0x0600035e | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixState: ErrorCode set` | 0x0600035f | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StringState: ErrorCode get` | 0x06000363 | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StringState: ErrorCode set` | 0x06000364 | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorState: ErrorCode get` | 0x06000366 | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorState: ErrorCode set` | 0x06000367 | MemberGen: `public IOManagerErrorCode ErrorCode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueGetter: ScaledByUnits get` | 0x0600036d | MemberGen: `public string ScaledByUnits { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueGetter: ScaledByUnits set` | 0x0600036e | MemberGen: `public string ScaledByUnits { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueGetter: Alias get` | 0x0600036f | MemberGen: `public string Alias { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueGetter: Alias set` | 0x06000370 | MemberGen: `public string Alias { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueGetter: ScalingSExpression get` | 0x06000371 | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueGetter: ScalingSExpression set` | 0x06000372 | MemberGen: `public string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalarOutputGetter: ScalarState get` | 0x06000380 | MemberGen: `public IUnitedScalarState ScalarState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalarOutputGetter: ScalarState set` | 0x06000381 | MemberGen: `public IUnitedScalarState ScalarState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemAliasComparer: .ctor(bool)` | 0x06000382 | MemberGen: `_caseInsensitiveAliasComparisonEnabled = caseInsensitiveAliasComparisonEnabled;` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalarInputGetter: OriginalScalarState get` | 0x0600038c | MemberGen: `public IUnitedScalarState OriginalScalarState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalarInputGetter: OriginalScalarState set` | 0x0600038d | MemberGen: `public IUnitedScalarState OriginalScalarState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalarInputGetter: ScalarState get` | 0x0600038e | MemberGen: `public IUnitedScalarState ScalarState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ScalarInputGetter: ScalarState set` | 0x0600038f | MemberGen: `public IUnitedScalarState ScalarState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixGetter: MatrixState get` | 0x06000397 | MemberGen: `public IUnitedMatrixState MatrixState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedMatrixGetter: MatrixState set` | 0x06000398 | MemberGen: `public IUnitedMatrixState MatrixState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StringGetter: StringState get` | 0x0600039f | MemberGen: `public IStringState StringState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `StringGetter: StringState set` | 0x060003a0 | MemberGen: `public IStringState StringState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CommonValueGetter: ErrorState get` | 0x060003aa | MemberGen: `public IErrorState ErrorState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CommonValueGetter: ErrorState set` | 0x060003ab | MemberGen: `public IErrorState ErrorState { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputItemsStates: InputItems get` | 0x060003ad | MemberGen: `public IList<IScalarItemWithOriginalState> InputItems { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputItemsStates: InputItems set` | 0x060003ae | MemberGen: `public IList<IScalarItemWithOriginalState> InputItems { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputItemsStates: OutputItems get` | 0x060003af | MemberGen: `public IList<IScalarItem> OutputItems { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InputOutputItemsStates: OutputItems set` | 0x060003b0 | MemberGen: `public IList<IScalarItem> OutputItems { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetValueResults: Count get` | 0x060003b9 | MemberGen: `_operationsResults.Count` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetValueResults: AllResults get` | 0x060003bc | MemberGen: `_operationsResults` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitlessValueSetter: Alias get` | 0x060003c0 | MemberGen: `public string Alias { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitlessValueSetter: Alias set` | 0x060003c1 | MemberGen: `public string Alias { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitlessValueSetter: OperationResult get` | 0x060003c2 | MemberGen: `public IOManagerErrorCode OperationResult { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitlessValueSetter: OperationResult set` | 0x060003c3 | MemberGen: `public IOManagerErrorCode OperationResult { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueSetter: Units get` | 0x060003c8 | MemberGen: `private string Units { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueSetter: Units set` | 0x060003c9 | MemberGen: `private string Units { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueSetter: ScalingSExpression get` | 0x060003ca | MemberGen: `protected string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedValueSetter: ScalingSExpression set` | 0x060003cb | MemberGen: `protected string ScalingSExpression { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SExpressionSetter: IsValueValid()` | 0x060003d1 | MemberGen: `EnumerableExtensions.IsNotEmpty<char>(_sexprValue)` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarSetter: .ctor(string, double, string)` | 0x060003d2 | MemberGen: `_value = value;` | Mechanical; Release IL equals Mathcad's | Verified |
| `UnitedScalarSetter: IsValueValid()` | 0x060003d4 | MemberGen: `DoubleExtension.IsValid(_value)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOIOManagerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
