# Ptc.MouseEventGenerator

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcMouseEventGeneratorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GenerateMouseUp()` | 0x06000224 | MemberGen: `SendMouseInput(0, 0, (SendMouseInputFlags)16388)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GenerateMouseDown()` | 0x06000225 | MemberGen: `SendMouseInput(0, 0, (SendMouseInputFlags)16386)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GenerateMouseClick(Point)` | 0x06000228 | MemberGen: `GenerateMouseClick(screenPoint, 1)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x0600022d | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcMouseEventGeneratorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
