# Mpl.Run.deftype_keyword

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Mpl`; tests: `Quire.Differential.Tests.MplRundeftype_keywordMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060000a6 | MemberGen: `: base("deftype", 2) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `walk_declaration_as(MplEnv, SCons)` | 0x060000a8 | MemberGen: `new Cons(form, null)` | Mechanical; Release IL equals Mathcad's | Verified |
| `walk_definition_as(MplEnv, SCons)` | 0x060000a9 | MemberGen: `form` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`MplRundeftype_keywordMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
