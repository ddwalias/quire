# Ptc.DevelopmentEnvironmentState

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcDevelopmentEnvironmentStateMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ClearRtStateCache()` | 0x06000203 | MemberGen: `RtStateCache.Clear()` | Mechanical; Release IL equals Mathcad's | Verified |
| `EnableRtInCache(string)` | 0x06000204 | MemberGen: `RtStateCache[env] = true` | Mechanical; Release IL equals Mathcad's | Verified |
| `FelinesRtEnvs get` | 0x060001ff | MemberGen: `public static List<string> FelinesRtEnvs { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FelinesRtEnvs set` | 0x06000200 | MemberGen: `public static List<string> FelinesRtEnvs { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcDevelopmentEnvironmentStateMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
