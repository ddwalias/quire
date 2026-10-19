# Ptc.ArrayExtension

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** ilspycmd 11.1.0.9782; `.mirror-cache/Ptc.Core/d60141dbcf757ae6/Ptc.ArrayExtension.cs` and `.il`
- **Quire:** `src/Quire.Core/ArrayExtension.cs`; direct comparison: `Quire.Differential.Tests.PtcArrayExtensionTierAMechanicalDifferentialTests`

| Member | Token | Exact signature | Contract and evidence | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Fill<T>(T[], T, int, int)` | `0x06000284` | `public static void Fill<T>(T[] array, T value, int startIndex = 0, int endIndex = -1)` | Replace a negative or over-length end with `array.Length`; replace a negative or over-length start with zero; assign `value` to each index from start inclusive to end exclusive. Release IL opcode sequence matches Mathcad. | Decompiled C# corroborated by IL; direct cases include each range clamp, empty and reversed ranges, and reference-type elements. | Verified |

`ToStringFormatted(Array)` is Tier C in the inventory and remains outside this Tier A batch.
