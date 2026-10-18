# Ptc.PersistentDataObjects.Plot3DType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `traces get` | 0x06000cfb | MemberGen: `tracesField; set => tracesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `traces set` | 0x06000cfc | MemberGen: `tracesField; set => tracesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `graphsize get` | 0x06000cfd | MemberGen: `graphsizeField; set => graphsizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `graphsize set` | 0x06000cfe | MemberGen: `graphsizeField; set => graphsizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes get` | 0x06000cff | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes set` | 0x06000d00 | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotEquations get` | 0x06000d01 | MemberGen: `plotEquationsField; set => plotEquationsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotEquations set` | 0x06000d02 | MemberGen: `plotEquationsField; set => plotEquationsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CameraTransformMatrix get` | 0x06000d03 | MemberGen: `cameraTransformMatrixField; set => cameraTransformMatrixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CameraTransformMatrix set` | 0x06000d04 | MemberGen: `cameraTransformMatrixField; set => cameraTransformMatrixField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CameraType get` | 0x06000d05 | MemberGen: `cameraTypeField; set => cameraTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CameraType set` | 0x06000d06 | MemberGen: `cameraTypeField; set => cameraTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equalscale get` | 0x06000d07 | MemberGen: `equalscaleField; set => equalscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equalscale set` | 0x06000d08 | MemberGen: `equalscaleField; set => equalscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsPlot3DTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
