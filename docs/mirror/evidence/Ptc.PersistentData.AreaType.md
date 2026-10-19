# Ptc.PersistentData.AreaType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataAreaTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetXmlSerializer(bool)` | 0x0600087f | MemberGen: `GetXmlSerializerStatic(useOverrides)` | Serializer dependency graph still differs; see blocked scope below | Blocked |
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

## Tier A static initializer

- **Artifact-bound reference:** `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.PersistentData.AreaType.cs` and `.il`, generated with ilspycmd 11.1.0.9782. Original `Ptc.Core` identity above.
- **Direct test:** `PtcPersistentDataAreaTypeCctorDifferentialTests.TypeOfThis_initializer_matches_PtcCore`.

| Member | Token | Signature | Evidence/status |
|---|---:|---|---|
| `.cctor` | `0x06000885` | `private static void()` | Direct comparison of the initializer and its `TypeOfThis` field value. Verified for the initialized state. |
| `get_Protect` | `0x0600087d` | `public bool()` | Directly compared for null ProtectionData; returns false. Non-null state depends on the Lane 5 ProtectionData wrapper. |
| `get_Regions` | `0x06000881` | `public IEnumerable<IRegionType>()` | Directly compared for null and populated `worksheetRegionType[]` backing data. Verified for those cases. |
| `set_Regions` | `0x06000882` | `public void(IEnumerable<IRegionType>)` | Directly compared for null and populated `worksheetRegionType[]` input, including resulting backing state. Verified for those cases. |
| `get_ProtectionDataWrapper` | `0x06000883` | `public ProtectionData()` | Directly compared for null persistent data. Non-null state depends on the Lane 5 ProtectionData wrapper. |
| `set_ProtectionDataWrapper` | `0x06000884` | `public void(ProtectionData)` | Directly compared with null, which clears persistent data. Non-null state depends on Lane 5's `GetProtectionPersistentData`. |

The cctor and accessor comparisons ran with `QUIRE_REQUIRE_MATHCAD=1` against Release Quire: 2 cctor tests passed, then the expanded accessor run passed 11 tests with the serializer case skipped. The initializer supplies the static type dependency used by `ProtectableTypesRegistry`.

`GetXmlSerializer(bool)` remains blocked. A direct call shows Mathcad returns a serializer, while Quire's type graph fails reflection because mirrored `worksheetRegionType` reaches `XmlNamespaceManager` without the Mathcad XML attributes. The `worksheetRegionType.CreateDefaultSerializer` helper is now implemented and directly compared for string and integer roots; this does not yet settle the AreaType graph. The test is explicitly skipped pending the remaining serializer metadata and dependent types.
