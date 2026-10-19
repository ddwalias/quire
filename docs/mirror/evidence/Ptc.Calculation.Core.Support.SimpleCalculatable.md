# Ptc.Calculation.Core.Support.SimpleCalculatable

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600031d | Initializes `_calculatingStatus` to underlying value 2 (`NotComputing`), `_evaluationResult` to a new `EmptySimpleResultNode`, and `_systemOrigin` to a new `RealSimpleResultNode` whose `Value` is 0.0; then calls `object::.ctor()` | Direct Mathcad constructor comparison; Release IL follows the field-initializer sequence | Verified |
| `Commit()` | 0x06000310 | Clears `_error`, replaces `_evaluationResult` with a new `EmptySimpleResultNode`, and invokes `RequestEvaluation(this)` when subscribed | Direct Mathcad comparisons cover null and callback paths | Verified |
| `SetStatusSynchronized()` | 0x06000311 | Invokes `TreatAsUpToDate(this)` when subscribed | Direct Mathcad comparisons cover null and callback paths | Verified |
| `SetSynchronizationStatus(SynchronizationStatus)` | 0x06000312 | Stores the status; invokes `Synchronized(this)` only for underlying value 3 when subscribed | Direct Mathcad comparisons cover a nonmatching value and callback value 3 | Verified |
| `SetCalculatingStatus(CalculatingStatus)` | 0x06000313 | MemberGen: `_calculatingStatus = computingStatus` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetResult(IResultNode)` | 0x06000314 | MemberGen: `_evaluationResult = result` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetError(EngineError)` | 0x06000315 | MemberGen: `_error = error` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetAutoSelectAlgorithm(IDictionary)` | 0x06000316 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetRedefinitionWarnings(IList)` | 0x06000317 | MemberGen: `_warnings = warnings` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetIdentifierLabels(IDictionary)` | 0x06000318 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Result get` | 0x06000301 | MemberGen: `_evaluationResult` | Mechanical; Release IL equals Mathcad's | Verified |
| `EngineWarnings get` | 0x06000302 | MemberGen: `_warnings` | Mechanical; Release IL equals Mathcad's | Verified |
| `SynchronizationStatus get` | 0x06000303 | MemberGen: `_syncStatus` | Mechanical; Release IL equals Mathcad's | Verified |
| `CalculatingStatus get` | 0x06000304 | MemberGen: `_calculatingStatus` | Mechanical; Release IL equals Mathcad's | Verified |
| `Error get` | 0x06000305 | MemberGen: `_error` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rl get` | 0x0600030e | MemberGen: `public virtual string Rl { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rl set` | 0x0600030f | MemberGen: `public virtual string Rl { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SystemOrigin get` | 0x0600031a | MemberGen: `_systemOrigin` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsEngineWarningsEquals(IList<EngineRedefInfo>, IList<EngineRedefInfo>)` | 0x0600031b | Returns true for two null lists; false for exactly one null or different counts; otherwise compares each warning's `Name`, `OldLocation`, and `OldType.Equals` in order | Direct Mathcad cases cover nulls, empty lists, match, count, and each field mismatch | Verified |
| `FireGetValueOfIdentifierBefore(string)` | 0x0600031c | Invokes `GetValueOfIdentifierBefore(this, idRl)` when subscribed; otherwise returns null | Direct Mathcad comparisons cover the null fallback and a subscribed callback | Verified |
| `Synchronized add` | 0x06000306 | MemberGen: `public event Action<ICalcWithSyncStatus> Synchronized;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Synchronized remove` | 0x06000307 | MemberGen: `public event Action<ICalcWithSyncStatus> Synchronized;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestEvaluation add` | 0x06000308 | MemberGen: `public event Action<ICalculatable> RequestEvaluation;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestEvaluation remove` | 0x06000309 | MemberGen: `public event Action<ICalculatable> RequestEvaluation;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValueOfIdentifierBefore add` | 0x0600030a | MemberGen: `public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetValueOfIdentifierBefore remove` | 0x0600030b | MemberGen: `public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TreatAsUpToDate add` | 0x0600030c | MemberGen: `public event Action<ICalculatable> TreatAsUpToDate;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TreatAsUpToDate remove` | 0x0600030d | MemberGen: `public event Action<ICalculatable> TreatAsUpToDate;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreSupportSimpleCalculatableMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`PtcCalculationCoreSupportSimpleCalculatableLifecycleDifferentialTests`): direct Mathcad calls to the constructor, both synchronization callbacks, `Commit` with its request callback, `FireGetValueOfIdentifierBefore` with its delegate, and `IsEngineWarningsEquals` for nulls, empty lists, a match, unequal counts, and each compared field. Evidence source: `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.Support.SimpleCalculatable --il`; original identity above. The `SimpleCalculatable` token tests and lifecycle tests pass 28/28 against Release Quire.

The direct token tests use numeric enum rendering to compare the original enum values faithfully. IL confirms `_calculatingStatus` initializes with `ldc.i4.2`, and `SetSynchronizationStatus` checks `ldc.i4.3`; Quire's local enum labels/values differ, so the implementation uses the original integer constants without changing the Quire enum definition.
