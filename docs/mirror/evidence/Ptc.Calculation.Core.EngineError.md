# Ptc.Calculation.Core.EngineError

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineErrorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600003f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorCode get` | 0x06000028 | MemberGen: `_code` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorMessageArguments get` | 0x06000029 | MemberGen: `_arguments` | Mechanical; Release IL equals Mathcad's | Verified |
| `NodeHandle get` | 0x0600002a | MemberGen: `public uint NodeHandle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NodeHandle set` | 0x0600002b | MemberGen: `public uint NodeHandle { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument get` | 0x0600002c | MemberGen: `public uint Argument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument set` | 0x0600002d | MemberGen: `public uint Argument { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResourceString get` | 0x0600002e | MemberGen: `public string ResourceString { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResourceString set` | 0x0600002f | MemberGen: `public string ResourceString { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OriginalError get` | 0x06000030 | MemberGen: `public EngineError OriginalError { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OriginalError set` | 0x06000031 | MemberGen: `public EngineError OriginalError { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsRuntime get` | 0x06000036 | MemberGen: `public bool IsRuntime { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsRuntime set` | 0x06000037 | MemberGen: `public bool IsRuntime { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorSource get` | 0x06000038 | MemberGen: `public ICalculatable ErrorSource { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ErrorSource set` | 0x06000039 | MemberGen: `public ICalculatable ErrorSource { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Message get` | 0x06000032 | Return `_message` | Mathcad getter directly loads the backing field; three direct cases | Verified |
| `IsEquivalentTo(EngineError, bool)` | 0x0600003a | Compare error code, optionally node handle, argument, `ErrorSource.Equals`, and recursively equivalent original errors | Direct cases cover equal nested errors, node-handle checking on/off, differing argument/code/nested error, same and distinct source references, and null input | Verified |
| `FindNextError(EngineError)` | 0x0600003b | Walk `OriginalError` until its next error is equivalent to the requested error, then return that current node | Direct cases cover immediate/deep match, absent match, and null request | Verified |
| `IsAncestorOf(EngineError)` | 0x0600003c | Walk this error and its `OriginalError` chain, checking equivalence at each node | Direct cases cover self, nested member, nonmember, and null input | Verified |
| `Clone()` | 0x0600003d | Calls the non-Tier-A `set_Message` member | Blocked until its dependency is independently in scope | Blocked |
| `MessagesForXamlSerialization get/set` | 0x06000034 / 0x06000035 | Getter creates a one-item collection from `Message`; setter uses `Ptc.EnumerableExtensions.IsNotEmpty` before selecting the first value | Setter depends on out-of-scope `Ptc.Core`; property remains a whole generated stub pair | Blocked |

## Test cases

**Observed** (`PtcCalculationCoreEngineErrorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`PtcCalculationCoreEngineErrorRelationsMechanicalDifferentialTests`, Release with Mathcad required): direct paired `MethodComparison.Compare`
cases for the four Verified members above. `get_Message` covers empty, ordinary, and tab-containing values. Relation tests cover matching and
different relation fields and chains, including same/different error-source references. These are targeted cases, not exhaustive state-space
coverage. The generated Quire-only `EngineError(string, string?)` constructor is not used to initialize Mathcad targets.

**Blocked:** `Clone` cannot be mirrored as Tier A while its body calls the Tier-A-excluded `set_Message`; both
`MessagesForXamlSerialization` accessors remain a generated stub pair because the setter calls `Ptc.EnumerableExtensions.IsNotEmpty` from `Ptc.Core`.
