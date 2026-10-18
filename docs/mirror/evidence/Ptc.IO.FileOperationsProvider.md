# Ptc.IO.FileOperationsProvider

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcIOFileOperationsProviderMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `FileExists(string)` | 0x06000f89 | MemberGen: `File.Exists(fileName)` | Mechanical; Release IL equals Mathcad's | Verified |
| `DirectoryExists(string)` | 0x06000f8a | MemberGen: `Directory.Exists(path)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFileReader(string)` | 0x06000f8b | MemberGen: `new StreamReader(fileName)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFileWriter(string)` | 0x06000f8c | MemberGen: `new StreamWriter(fileName)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFileWriter(string, bool)` | 0x06000f8d | MemberGen: `new StreamWriter(path, appendFlag)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFiles(string)` | 0x06000f8e | MemberGen: `Directory.GetFiles(path)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFiles(string, string)` | 0x06000f8f | MemberGen: `Directory.GetFiles(path, filter)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetDirectories(string)` | 0x06000f92 | MemberGen: `Directory.GetDirectories(path)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetDirectories(string, string)` | 0x06000f93 | MemberGen: `Directory.GetDirectories(path, filter)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetFullPath(string)` | 0x06000f95 | MemberGen: `Path.GetFullPath(relativePath)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetExtension(string)` | 0x06000f96 | MemberGen: `Path.GetExtension(fileName)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000f99 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentDirectory get` | 0x06000f90 | MemberGen: `Environment.CurrentDirectory; set => Environment.CurrentDirectory = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentDirectory set` | 0x06000f91 | MemberGen: `Environment.CurrentDirectory; set => Environment.CurrentDirectory = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConnected get` | 0x06000f97 | MemberGen: `public bool IsConnected { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsConnected set` | 0x06000f98 | MemberGen: `public bool IsConnected { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcIOFileOperationsProviderMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
