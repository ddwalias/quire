# Ptc.PersistentData.footer

- **Original:** `Ptc.Core` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`.
- **Quire:** `src/Quire.Core`; direct test: `PtcPersistentDataHeaderFooterMechanicalDifferentialTests.Footer_Regions_and_conversion_flag_match_PtcCore`.
- **Reference:** `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.PersistentData.footer.cs` and `.il`, generated with ilspycmd 11.1.0.9782. Cache artifacts are local and ignored.

| Member | Token | Signature | Evidence/status |
|---|---:|---|---|
| `get_Regions` | `0x0600097d` | `public IEnumerable<IRegionType>()` | Directly compared for null and populated backing arrays. Verified for those cases. |
| `set_Regions` | `0x0600097e` | `public void(IEnumerable<IRegionType>)` | Directly compared for null and matching arrays, including values and resulting base fields. Other `IRegionType` input directly throws `InvalidCastException` on both sides; exception messages contain the mapped type names (`Quire`/`Ptc`), so that case compares exception type and unchanged state. |
| `get_FooterConvertedFromMC14` | `0x0600097f` | `public bool()` | Directly compared with `convertedfromSpecified` false and true, using `Mathcad14`. Verified for both states. |

Validation ran against Mathcad with Release Quire: 2 tests passed, 0 failed, 0 skipped. No COM or installed-application assumptions apply.
