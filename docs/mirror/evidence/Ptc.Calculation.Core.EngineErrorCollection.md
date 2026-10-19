# Ptc.Calculation.Core.EngineErrorCollection

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreEngineErrorCollectionMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600004f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(IEnumerable<EngineError>, EngineError)` | 0x06000050 | Calls List<EngineError>(errors), then List<EngineError>.Add(error) | Direct Release Mathcad cases; Release IL exact | Verified; normal gate pending |

## Test cases

The existing no-argument constructor is checked by its mechanical comparison. The two-argument constructor is compared directly against Mathcad for empty and populated sequences, null list elements, a null appended error, and a null sequence (matching the thrown exception). The constructor test compares resulting enumeration order and exception type.

## Two-argument constructor

The Tier A constructor at token 0x06000050 initializes List<EngineError> from the input sequence, then invokes List<EngineError>.Add for the appended error. This follows Mathcad IL inspected with ilspycmd through the reference.py --il workflow. Release MethodAudit reports exact_match=true and structure_matches=true for both constructors in this type. Direct Release Mathcad comparisons include empty and populated sequences, null elements, a null appended error, and a null sequence.
