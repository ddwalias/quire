# Ptc.Wpf.Printing.PrintDialog

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcWpfPrintingPrintDialogMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `PrintToFile get` | 0x0600064e | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `FileName get` | 0x0600064f | MemberGen: `string.Empty` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcWpfPrintingPrintDialogMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
