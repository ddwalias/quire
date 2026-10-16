# Mpl.Run.ModuleTextAttribute

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRunModuleTextAttributeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ToString()` | 0x06000007 | MemberGen: `m_txt` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(string)` | 0x06000008 | MemberGen: `m_txt = s;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Text get` | 0x06000006 | MemberGen: `m_txt` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`MplRunModuleTextAttributeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
