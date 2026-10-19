# Ptc.Calculation.Core.EngineResult

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineResultMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ResultNode get` | 0x06000058 | MemberGen: `public IResultNode ResultNode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResultNode set` | 0x06000059 | MemberGen: `public IResultNode ResultNode { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Errors get` | 0x0600005a | MemberGen: `public EngineErrorCollection Errors { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Errors set` | 0x0600005b | MemberGen: `public EngineErrorCollection Errors { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Warnings get` | 0x0600005c | MemberGen: `public EngineRedefInfo[] Warnings { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Warnings set` | 0x0600005d | MemberGen: `public EngineRedefInfo[] Warnings { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SynchronizationStatus get` | 0x0600005e | MemberGen: `public SynchronizationStatus SynchronizationStatus { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SynchronizationStatus set` | 0x0600005f | MemberGen: `public SynchronizationStatus SynchronizationStatus { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializationTag get` | 0x06000062 | MemberGen: `public string SerializationTag { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializationTag set` | 0x06000063 | MemberGen: `public string SerializationTag { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(EngineResult, SynchronizationStatus)` | 0x06000051 | Default the struct; copy result node (or an empty node), errors, warnings and serialization tag; set requested status | Direct Release Mathcad comparison; Quire token 0x0600005a is exact | Verified (null node case) |
| `.ctor(EngineResult, IResultNode)` | 0x06000052 | Default the struct; use supplied node or an empty node, clear errors, copy serialization tag and status | Direct Release Mathcad comparison; Quire token 0x0600005b is exact | Verified (null node case) |
| `.ctor(EngineResult, IEnumerable<EngineRedefInfo>)` | 0x06000056 | Default the struct; copy node/errors/tag/status and materialize non-null warnings | Direct Release Mathcad comparison; Quire token 0x0600005c is exact | Verified (empty warnings case) |
| `.ctor(IResultNode, EngineErrorCollection, SynchronizationStatus)` | 0x06000054 | Reject a non-empty result node with a non-empty error collection; otherwise copy the node, errors and status | Direct Release comparisons cover null pair, empty collection and invalid pair; Quire token `0x0600005d` exactly matches Mathcad | Verified (three cases) |

## Test cases

**Observed** (`PtcCalculationCoreEngineResultMechanicalDifferentialTests`): all 14 members in this class are compared directly with Mathcad in Release. Ten accessors are invoked on default structs; setters receive null, an empty warning array, synchronization enum value 1, or a non-null serialization tag. Four constructors cover copy/fallback behavior and result/error validation. All 14 comparisons passed. The custom renderer normalizes enum values numerically and records every EngineResult property after invocation; it avoids the enum-name mismatch in Mathcad's metadata without changing shared test helpers.
