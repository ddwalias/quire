# Mpl.Run.Primitive

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** ilspycmd 11.1.0.9782 (tools/reference)
- **Quire:** `src/Quire.Mpl/Run/Primitive.cs`, `src/Quire.Mpl/Generated/Mirror/Mpl.Run.Primitive.g.cs` (stubs);
  tests: `Quire.Differential.Tests.PrimitiveDifferentialTests`

Only the twelve comparisons are implemented; every other member is still a stub. Each comparison is a single
`ceq`/`cgt`/`clt` (negated with `ceq 0` for `!=`, `<=` and `>=`, using `cgt.un`/`clt.un` for doubles so NaN gives 0)
returning 1 or 0, which the Release build of `x OP y ? 1 : 0` reproduces exactly.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ceq_i4(int, int)` | 0x0600036f | 1 when `x == y`, else 0. | High | Verified |
| `cne_i4(int, int)` | 0x06000370 | 1 when `x != y`, else 0. | High | Verified |
| `clt_i4(int, int)` | 0x06000371 | 1 when `x < y` (signed), else 0. | High | Verified |
| `cle_i4(int, int)` | 0x06000372 | 1 when `x <= y` (signed), else 0. | High | Verified |
| `cgt_i4(int, int)` | 0x06000373 | 1 when `x > y` (signed), else 0. | High | Verified |
| `cge_i4(int, int)` | 0x06000374 | 1 when `x >= y` (signed), else 0. | High | Verified |
| `ceq_f8(double, double)` | 0x0600037a | 1 when `x == y` (IEEE: -0 equals 0, NaN equals nothing), else 0. | High | Verified |
| `cne_f8(double, double)` | 0x0600037b | 1 when `x != y` (true whenever either is NaN), else 0. | High | Verified |
| `clt_f8(double, double)` | 0x0600037c | 1 when `x < y`; 0 when either is NaN. | High | Verified |
| `cle_f8(double, double)` | 0x0600037d | 1 when `x <= y`; 0 when either is NaN. | High | Verified |
| `cgt_f8(double, double)` | 0x0600037e | 1 when `x > y`; 0 when either is NaN. | High | Verified |
| `cge_f8(double, double)` | 0x0600037f | 1 when `x >= y`; 0 when either is NaN. | High | Verified |

## Test cases

**Observed** (`PrimitiveDifferentialTests`, against the installed `mplrun.dll`; results compared as values):
- each `_i4` comparison on every ordered pair of `int.MinValue`, -1, 0, 1 and `int.MaxValue`;
- each `_f8` comparison on every ordered pair of -∞, -1, -0, 0, 1, +∞ and NaN.
