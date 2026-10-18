# Ptc.Controls.IO.InputOutputItemsConflicts

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GeneralWarnings get` | 0x06000240 | MemberGen: `_generalWarnings` | Mechanical; Release IL equals Mathcad's | Verified |
| `GeneralErrors get` | 0x06000241 | MemberGen: `_generalErrors` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemWarnings get` | 0x06000242 | MemberGen: `_itemWarnings` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemErrors get` | 0x06000243 | MemberGen: `_itemErrors` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsIOInputOutputItemsConflictsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
