# Ptc.PersistentData.AreaType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataAreaTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetXmlSerializer(bool)` | 0x0600087f | MemberGen: `GetXmlSerializerStatic(useOverrides)` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions get` | 0x06000873 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `regions set` | 0x06000874 | MemberGen: `regionsField; set => regionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProtectionData get` | 0x06000875 | MemberGen: `protectionDataField; set => protectionDataField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ProtectionData set` | 0x06000876 | MemberGen: `protectionDataField; set => protectionDataField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsCollapsed get` | 0x06000877 | MemberGen: `isCollapsedField; set => isCollapsedField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsCollapsed set` | 0x06000878 | MemberGen: `isCollapsedField; set => isCollapsedField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBorderVisible get` | 0x06000879 | MemberGen: `isBorderVisibleField; set => isBorderVisibleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsBorderVisible set` | 0x0600087a | MemberGen: `isBorderVisibleField; set => isBorderVisibleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AreaHeight get` | 0x0600087b | MemberGen: `areaHeightField; set => areaHeightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `AreaHeight set` | 0x0600087c | MemberGen: `areaHeightField; set => areaHeightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataAreaTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
