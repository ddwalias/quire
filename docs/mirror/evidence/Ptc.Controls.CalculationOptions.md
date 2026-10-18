# Ptc.Controls.CalculationOptions

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCalculationOptionsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060000c6 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EngineTypeAutoCorrected get` | 0x060000bc | MemberGen: `public bool EngineTypeAutoCorrected { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EngineTypeAutoCorrected set` | 0x060000bd | MemberGen: `public bool EngineTypeAutoCorrected { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CalculationOptionChanged add` | 0x060000c0 | MemberGen: `public event Delegate2<string, object> CalculationOptionChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `CalculationOptionChanged remove` | 0x060000c1 | MemberGen: `public event Delegate2<string, object> CalculationOptionChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `BinOctHexModeOptionChanged add` | 0x060000c2 | MemberGen: `public event Delegate2<string, object> BinOctHexModeOptionChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `BinOctHexModeOptionChanged remove` | 0x060000c3 | MemberGen: `public event Delegate2<string, object> BinOctHexModeOptionChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCalculationOptionsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
