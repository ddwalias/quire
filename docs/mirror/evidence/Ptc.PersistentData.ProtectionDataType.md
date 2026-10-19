# Ptc.PersistentData.ProtectionDataType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000886 | MemberGen: `showTimestampField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Password get` | 0x06000887 | MemberGen: `passwordField; set => passwordField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Password set` | 0x06000888 | MemberGen: `passwordField; set => passwordField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShowTimestamp get` | 0x06000889 | MemberGen: `showTimestampField; set => showTimestampField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShowTimestamp set` | 0x0600088a | MemberGen: `showTimestampField; set => showTimestampField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Timestamp get` | 0x0600088b | MemberGen: `timestampField; set => timestampField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Timestamp set` | 0x0600088c | MemberGen: `timestampField; set => timestampField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TimestampSpecified get` | 0x0600088d | MemberGen: `timestampFieldSpecified; set => timestampFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TimestampSpecified set` | 0x0600088e | MemberGen: `timestampFieldSpecified; set => timestampFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProtectionLevel get` | 0x0600088f | MemberGen: `protectionLevelField; set => protectionLevelField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProtectionLevel set` | 0x06000890 | MemberGen: `protectionLevelField; set => protectionLevelField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

## Tier A timestamp wrapper

- **Artifact-bound reference:** `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.PersistentData.ProtectionDataType.cs` and `.il`, generated with ilspycmd 11.1.0.9782. Original identity is recorded above.
- **Direct test:** `PtcPersistentDataProtectionDataTypeTimestampDifferentialTests.TimestampWrapper_getter_and_setter_match_PtcCore`.

| Member | Token | Signature | Evidence/status |
|---|---:|---|---|
| `get_TimestampWrapper` | `0x06000891` | `public DateTime?()` | Direct comparisons with `TimestampSpecified` false/true and Unspecified/UTC/Local timestamp kinds. Verified for those cases. |
| `set_TimestampWrapper` | `0x06000892` | `public void(DateTime?)` | Direct comparisons for null and all three timestamp kinds; target state verifies Unspecified kind on stored values and preserves Timestamp when clearing. Verified for those cases. |

The strict Release Mathcad differential test passed: 1 passed, 0 failed, 0 skipped. `ProtectionLevelWrapper` methods remain open; they call `Ptc.EnumHelper`, which belongs to Lane 5.
