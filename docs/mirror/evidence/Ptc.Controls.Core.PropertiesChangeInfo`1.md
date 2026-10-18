# Ptc.Controls.Core.PropertiesChangeInfo`1

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCorePropertiesChangeInfo1MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `PropertyNames get` | 0x060008b9 | MemberGen: `public string[] PropertyNames { get; protected set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyNames set` | 0x060008ba | MemberGen: `public string[] PropertyNames { get; protected set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertiesSource get` | 0x060008bb | MemberGen: `public TSource PropertiesSource { get; protected set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertiesSource set` | 0x060008bc | MemberGen: `public TSource PropertiesSource { get; protected set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCorePropertiesChangeInfo1MechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
