# Ptc.IO.EmbeddedResourceCopier

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** ilspycmd 11.1.0.9782; `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.IO.EmbeddedResourceCopier.cs` and `.il`
- **Quire:** `src/Quire.Core/IO/EmbeddedResourceCopier.cs`; direct comparison: `Quire.Differential.Tests.EmbeddedResourceCopierMechanicalDifferentialTests`

| Member | Token | Exact signature | Contract and evidence | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Copy(Assembly, string, string)` | `0x06000f88` | `public static void Copy(System.Reflection.Assembly assemblyContainingResource, string resourceName, string destinationFilePath)` | If destination exists, set its attributes to `FileAttributes.Normal`. Open the named manifest resource. When present, allocate byte and char arrays using stream length, make one `Read`, cast each byte to a char, write the chars with a default `StreamWriter`, flush, and dispose both streams. When absent, leave the destination contents untouched. | Decompiled C# corroborated by IL; direct comparisons cover `System.Private.CoreLib.Strings.resources` and a missing resource with a read-only existing destination. The same runtime assembly is passed to both implementations. | Verified |

## Observed cases

`Copy_resource_and_missing_resource` checks the Quire output against the expected byte-to-char string, then invokes Mathcad Prime and Quire through `MethodComparison.Compare` for `System.Private.CoreLib.Strings.resources`. It also checks preservation of existing contents and normalization of read-only attributes for a missing resource, with a direct Mathcad comparison of the output file bytes and attributes. The test checks the installed Ptc.Core SHA-256, MVID and member token before invoking the original. The test file uses the `*MechanicalDifferentialTests.cs` suffix so the portable focused profile excludes this Quire.Core-only test.

The single `Stream.Read` and conversion of resource bytes to characters intentionally match the original behavior, including its limitations for partial reads and non-ASCII data.
