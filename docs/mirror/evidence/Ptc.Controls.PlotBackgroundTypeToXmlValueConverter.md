# Ptc.Controls.PlotBackgroundTypeToXmlValueConverter

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsPlotBackgroundTypeToXmlValueConverterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `AttributeName get` | 0x060001a5 | MemberGen: `private string AttributeName { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AttributeNameToDefaultValueDictionary get` | 0x060001a6 | MemberGen: `private Dictionary<string, object> AttributeNameToDefaultValueDictionary { get; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsPlotBackgroundTypeToXmlValueConverterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
