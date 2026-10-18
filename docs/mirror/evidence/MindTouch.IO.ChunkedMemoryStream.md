# MindTouch.IO.ChunkedMemoryStream

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060000f4 | MemberGen: `: this(0) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(byte[])` | 0x060000f6 | MemberGen: `: this(buffer, true) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(byte[], int, int)` | 0x060000f8 | MemberGen: `: this(buffer, index, count, true, false) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor(byte[], int, int, bool)` | 0x060000f9 | MemberGen: `: this(buffer, index, count, writable, false) { }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Flush()` | 0x06000103 | MemberGen: `EnsureOpen()` | Mechanical; Release IL equals Mathcad's | Verified |
| `ChunkSize get` | 0x060000f1 | MemberGen: `public static int ChunkSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ChunkSize set` | 0x060000f2 | MemberGen: `public static int ChunkSize { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
