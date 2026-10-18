# Ptc.Controls.Core.Excel.ExcelSheet

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreExcelExcelSheetMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000ea9 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x06000ea3 | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x06000ea4 | MemberGen: `public string Name { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows get` | 0x06000ea5 | MemberGen: `public IEnumerable<ExcelRow> Rows { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Rows set` | 0x06000ea6 | MemberGen: `public IEnumerable<ExcelRow> Rows { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns get` | 0x06000ea7 | MemberGen: `public IEnumerable<string> Columns { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns set` | 0x06000ea8 | MemberGen: `public IEnumerable<string> Columns { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreExcelExcelSheetMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
