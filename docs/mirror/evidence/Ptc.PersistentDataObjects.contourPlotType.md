# Ptc.PersistentDataObjects.contourPlotType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000c55 | MemberGen: `equalscaleField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes get` | 0x06000c56 | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes set` | 0x06000c57 | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `contour get` | 0x06000c58 | MemberGen: `contourField; set => contourField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `contour set` | 0x06000c59 | MemberGen: `contourField; set => contourField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `graphsize get` | 0x06000c5a | MemberGen: `graphsizeField; set => graphsizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `graphsize set` | 0x06000c5b | MemberGen: `graphsizeField; set => graphsizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotEquation get` | 0x06000c5c | MemberGen: `plotEquationField; set => plotEquationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotEquation set` | 0x06000c5d | MemberGen: `plotEquationField; set => plotEquationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equalscale get` | 0x06000c5e | MemberGen: `equalscaleField; set => equalscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equalscale set` | 0x06000c5f | MemberGen: `equalscaleField; set => equalscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectscontourPlotTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
