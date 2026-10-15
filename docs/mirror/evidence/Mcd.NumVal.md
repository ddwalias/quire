# Mcd.NumVal

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/NumVal.cs`; tests: `Quire.Differential.Tests.NumValDifferentialTests`

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `.cctor`0()->[System.Runtime]System.Void|True` | 0x06000594 | IL | .cctor`0()->[System.Runtime]System.Void|True contract | High | Verified |
| `.ctor`0()->[System.Runtime]System.Void|False` | 0x0600055a | IL | .ctor`0()->[System.Runtime]System.Void|False contract | High | Verified |
| `.ctor`0([System.Runtime]System.Int32)->[System.Runtime]System.Void|False` | 0x0600055b | IL | .ctor`0([System.Runtime]System.Int32)->[System.Runtime]System.Void|False contract | High | Verified |
| `.ctor`0([Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Void|False` | 0x0600055c | IL | .ctor`0([Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Void|False contract | High | Verified |
| `.ctor`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.Dim)->[System.Runtime]System.Void|False` | 0x0600055d | IL | .ctor`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.Dim)->[System.Runtime]System.Void|False contract | High | Verified |
| `.ctor`0([System.Runtime]System.Double)->[System.Runtime]System.Void|False` | 0x0600055e | IL | .ctor`0([System.Runtime]System.Double)->[System.Runtime]System.Void|False contract | High | Verified |
| `.ctor`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Void|False` | 0x0600055f | IL | .ctor`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Void|False contract | High | Verified |
| `.ctor`0([System.Runtime]System.Double,[System.Runtime]System.Double,[Quire.Mpl]Quire.Mcd.Dim)->[System.Runtime]System.Void|False` | 0x06000560 | IL | .ctor`0([System.Runtime]System.Double,[System.Runtime]System.Double,[Quire.Mpl]Quire.Mcd.Dim)->[System.Runtime]System.Void|False contract | High | Verified |
| `get_PhysicalDimension`0()->[Quire.Mpl]Quire.Mcd.Dim|False` | 0x06000564 | IL | get_PhysicalDimension`0()->[Quire.Mpl]Quire.Mcd.Dim|False contract | High | Verified |
| `get_IsPureReal`0()->[System.Runtime]System.Object|False` | 0x06000565 | IL | get_IsPureReal`0()->[System.Runtime]System.Object|False contract | High | Verified |
| `get_IsPureImag`0()->[System.Runtime]System.Object|False` | 0x06000566 | IL | get_IsPureImag`0()->[System.Runtime]System.Object|False contract | High | Verified |
| `get_Real`0()->[System.Runtime]System.Double|False` | 0x06000567 | IL | get_Real`0()->[System.Runtime]System.Double|False contract | High | Verified |
| `set_Real`0([System.Runtime]System.Double)->[System.Runtime]System.Void|False` | 0x06000568 | IL | set_Real`0([System.Runtime]System.Double)->[System.Runtime]System.Void|False contract | High | Verified |
| `get_Imag`0()->[System.Runtime]System.Double|False` | 0x06000569 | IL | get_Imag`0()->[System.Runtime]System.Double|False contract | High | Verified |
| `set_Imag`0([System.Runtime]System.Double)->[System.Runtime]System.Void|False` | 0x0600056a | IL | set_Imag`0([System.Runtime]System.Double)->[System.Runtime]System.Void|False contract | High | Verified |
| `get_IsInt`0()->[System.Runtime]System.Int32|False` | 0x0600057f | IL | get_IsInt`0()->[System.Runtime]System.Int32|False contract | High | Verified |
| `get_IsReal`0()->[System.Runtime]System.Int32|False` | 0x06000583 | IL | get_IsReal`0()->[System.Runtime]System.Int32|False contract | High | Verified |
| `NumDv`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|True` | 0x06000561 | IL | NumDv`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|True contract | High | Verified |
| `IntDv`0([System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.NumVal|True` | 0x06000562 | IL | IntDv`0([System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.NumVal|True contract | High | Verified |
| `RealDv`0([System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|True` | 0x06000563 | IL | RealDv`0([System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|True contract | High | Verified |
| `Set`0([System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|False` | 0x0600056b | IL | Set`0([System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|False contract | High | Verified |
| `Set`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|False` | 0x0600056c | IL | Set`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|False contract | High | Verified |
| `Set`0([System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.NumVal|False` | 0x0600056d | IL | Set`0([System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.NumVal|False contract | High | Verified |
| `ExactEquals`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True` | 0x0600056e | IL | ExactEquals`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `FuzzyEquals`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x0600056f | IL | FuzzyEquals`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `FuzzyEquals`0([System.Runtime]System.Double,[System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000570 | IL | FuzzyEquals`0([System.Runtime]System.Double,[System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `FuzzyEquals`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True` | 0x06000571 | IL | FuzzyEquals`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `FuzzyEquals`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000572 | IL | FuzzyEquals`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `Equals`0([System.Runtime]System.Object)->[System.Runtime]System.Boolean|False` | 0x06000573 | IL | Equals`0([System.Runtime]System.Object)->[System.Runtime]System.Boolean|False contract | High | Verified |
| `Equals1`0([System.Runtime]System.Object)->[System.Runtime]System.Boolean|False` | 0x06000574 | IL | Equals1`0([System.Runtime]System.Object)->[System.Runtime]System.Boolean|False contract | High | Verified |
| `nearly_equal`0([Quire.Mpl]Quire.Mcd.DimVal,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|False` | 0x06000575 | IL | nearly_equal`0([Quire.Mpl]Quire.Mcd.DimVal,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|False contract | High | Verified |
| `LT`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000576 | IL | LT`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `GT`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000577 | IL | GT`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `LE`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000578 | IL | LE`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `GE`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000579 | IL | GE`0([System.Runtime]System.Double,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `LessThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True` | 0x0600057a | IL | LessThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `GreaterThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True` | 0x0600057b | IL | GreaterThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `LessEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True` | 0x0600057c | IL | LessEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `GreaterEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True` | 0x0600057d | IL | GreaterEqThan`0([Quire.Mpl]Quire.Mcd.NumVal,[Quire.Mpl]Quire.Mcd.NumVal)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `GetHashCode`0()->[System.Runtime]System.Int32|False` | 0x0600057e | IL | GetHashCode`0()->[System.Runtime]System.Int32|False contract | High | Verified |
| `SetDim`0([Quire.Mpl]Quire.Mcd.Dim)->[Quire.Mpl]Quire.Mcd.DimVal|False` | 0x06000580 | IL | SetDim`0([Quire.Mpl]Quire.Mcd.Dim)->[Quire.Mpl]Quire.Mcd.DimVal|False contract | High | Verified |
| `isNearInt`0([System.Runtime]System.Int32*,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True` | 0x06000581 | IL | isNearInt`0([System.Runtime]System.Int32*,[System.Runtime]System.Double)->[System.Runtime]System.Boolean|True contract | High | Verified |
| `isInt`0([System.Runtime]System.Int32*)->[System.Runtime]System.Boolean|False` | 0x06000582 | IL | isInt`0([System.Runtime]System.Int32*)->[System.Runtime]System.Boolean|False contract | High | Verified |
| `asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.Double|False` | 0x06000584 | IL | asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[System.Runtime]System.Double|False contract | High | Verified |
| `McdBool`0([System.Runtime]System.Object)->[Quire.Mpl]Quire.Mcd.NumVal|True` | 0x06000585 | IL | McdBool`0([System.Runtime]System.Object)->[Quire.Mpl]Quire.Mcd.NumVal|True contract | High | Verified |
| `ibv`0([System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.NumVal|True` | 0x06000586 | IL | ibv`0([System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.NumVal|True contract | High | Verified |
| `Format`0([System.Runtime]System.String,[System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False` | 0x06000587 | IL | Format`0([System.Runtime]System.String,[System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False contract | High | Verified |
| `Format`0([System.Runtime]System.IFormatProvider,[System.Runtime]System.String,[System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False` | 0x06000588 | IL | Format`0([System.Runtime]System.IFormatProvider,[System.Runtime]System.String,[System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False contract | High | Verified |
| `Format`0([System.Runtime]System.Boolean)->[System.Runtime]System.String|False` | 0x06000589 | IL | Format`0([System.Runtime]System.Boolean)->[System.Runtime]System.String|False contract | High | Verified |
| `Format`0([System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False` | 0x0600058a | IL | Format`0([System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False contract | High | Verified |
| `Format`0([System.Runtime]System.String,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False` | 0x0600058b | IL | Format`0([System.Runtime]System.String,[System.Runtime]System.Boolean)->[System.Runtime]System.String|False contract | High | Verified |
| `ToString`0([System.Runtime]System.IFormatProvider,[System.Runtime]System.String)->[System.Runtime]System.String|False` | 0x0600058c | IL | ToString`0([System.Runtime]System.IFormatProvider,[System.Runtime]System.String)->[System.Runtime]System.String|False contract | High | Verified |
| `ToString`0([System.Runtime]System.String)->[System.Runtime]System.String|False` | 0x0600058d | IL | ToString`0([System.Runtime]System.String)->[System.Runtime]System.String|False contract | High | Verified |
| `ToString`0([System.Runtime]System.IFormatProvider)->[System.Runtime]System.String|False` | 0x0600058e | IL | ToString`0([System.Runtime]System.IFormatProvider)->[System.Runtime]System.String|False contract | High | Verified |
| `ToString`0()->[System.Runtime]System.String|False` | 0x0600058f | IL | ToString`0()->[System.Runtime]System.String|False contract | High | Verified |
| `asResult`0()->[Quire.Mpl]Quire.Mcd.DimVal|False` | 0x06000590 | IL | asResult`0()->[Quire.Mpl]Quire.Mcd.DimVal|False contract | High | Verified |
| `LoadNaN`0([System.Runtime]System.Int32)->[System.Runtime]System.Double|True` | 0x06000591 | IL | LoadNaN`0([System.Runtime]System.Int32)->[System.Runtime]System.Double|True contract | High | Verified |
| `NotNoValue`0([System.Runtime]System.Object)->[System.Runtime]System.Object|True` | 0x06000592 | IL | NotNoValue`0([System.Runtime]System.Object)->[System.Runtime]System.Object|True contract | High | Verified |
| `op_Division`0([Quire.Mpl]Quire.Mcd.NumVal,[System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|True` | 0x06000593 | IL | op_Division`0([Quire.Mpl]Quire.Mcd.NumVal,[System.Runtime]System.Double)->[Quire.Mpl]Quire.Mcd.NumVal|True contract | High | Verified |

## Test cases

**Observed** (`NumValDifferentialTests`, against the installed `mplrun.dll`):
- Statics and constructors
- Setter and conversion operations
- Exact and fuzzy comparisons (nearly_equal, NearlyEqual)
- Strict comparisons
- Formatting in invariant and custom cultures

## Unresolved structure

- `Quire.Mcd.NumVal.get_IsReal`: conditional branches: Quire 1, Mathcad 0. Original IL in Mathcad Prime 12 (`mplrun.dll` token `0x06000583`) is `ldarg.0; ldflda Data; ldfld ip; ldc.r8 0.0; ceq; ret`. C# cannot return comparison bool as int without branching in debug build; original IL emitted direct `ceq; ret`.

## Scope reconciliation (2026-10-15)

The Phase 1 checkpoint in `docs/mirror/work/lane-b.md` reported a manifest-shape failure for
`Mcd.NumVal.Compare`. Rechecked on `local/windows-main` (`08946db27e3950d51001192cfa98898d812c40b7`):
`docs/mirror/manifest/Quire.Mpl.txt` has no `Mcd.NumVal.Compare` row, and the hand-written source and generated
`NumVal` declaration have no such member. The installed `mplrun.dll` reference above was read again with
`tools/reference/reference.py Mcd.NumVal --il`; its complete member index has no `Compare` method or token.
This reported blocker is not present in the current manifest or original artifact, so it is out of scope for
implementation and has no direct-comparison test. The previous checkpoint note is corrected in
`docs/mirror/work/lane-b.md`. Checkpoint and validation scope: [Lane A](../work/lane-a.md#numvalcompare-scope-correction).

