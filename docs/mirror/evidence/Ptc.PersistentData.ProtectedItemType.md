# Ptc.PersistentData.ProtectedItemType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600090f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemtype get` | 0x06000905 | MemberGen: `itemtypeField; set => itemtypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemtype set` | 0x06000906 | MemberGen: `itemtypeField; set => itemtypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `S11NProvider get` | 0x06000909 | MemberGen: `public IS11NProvider S11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `S11NProvider set` | 0x0600090a | MemberGen: `public IS11NProvider S11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeS11NProvider get` | 0x0600090b | MemberGen: `public IDeS11NProvider DeS11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeS11NProvider set` | 0x0600090c | MemberGen: `public IDeS11NProvider DeS11NProvider { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataProtectedItemTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
