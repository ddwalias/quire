# Lane J: mshex.dll → Quire.Native.Mshex

**Status: incomplete.** 17 of 380 exports pass strict byte validation. Export parity: 363 missing, 0 extra.

- **Owner:** Lane J (native, `mshex.dll`)
- **Branch:** `mirror/j-mshex-202610241256`
- **Worktree:** `C:/Projects/Quire-wt/lane-j-mshex`
- **Base commit:** `c11fb045` (current `main`)
- **Harness:** `tools/validation/native_bytes.py` on `main` (used unmodified).

## Completed groups

| Group | Functions | Strict result | Notes |
|---|---|---|---|
| `Hex::Tag` accessors and constructor | `Tag(u64, u64, u32)`, `GetOffset`, `GetExtent`, `GetHandle`, `SetOffset`, `SetExtent`, `SetHandle` | 7/7 OK, 0 unverified | `hex_tag.{h,cpp}`. Layout from machine code: offset @0, extent @8, handle @0x10 (`sizeof` 24). The constructor is inline in the header (the original inlines it into `Node::GetTag`). |
| `Hex::Token` / `TokenOf<void>` tag and token access | `Token::GetTag`, `TokenOf<void>::GetToken`, `GetTag` (const, by value), `GetTag` (non-const, reference) | 4/4 OK, 0 unverified | `hex_token.{h,cpp}`. `TokenOf<void>` layout: vptr @0, `uint16` token @8, `Tag` @0x10. Explicit instantiation of `TokenOf<void>`. |
| `Hex::Node::GetTag` | `GetTag` (const, by value) | 1/1 OK, 0 unverified | `hex_node.{h,cpp}`. Returns the null tag (`-1`, `-1`, `0`). Imported by `msmpl.dll`. |
| `Hex::Lexer` flags | `SetFlag`, `GetFlag` | 2/2 OK, 0 unverified | `hex_lexer.{h,cpp}`. Both return `false`. |
| `Hex::Interpreter` options and environment | `SetOption`, `GetOption`, `SetEnvironment` | 3/3 OK, 0 unverified | `hex_interpreter.{h,cpp}`. All return `false`. |

Consumers: `mctranslator.dll` imports `Tag::GetHandle`, `TokenOf<void>::GetTag`, `TokenOf<void>::GetToken`, `Lexer::SetFlag` and `GetFlag`, and `Interpreter::SetEnvironment`. `msmpl.dll` imports `Node::GetTag` and the `Lexer` / `Interpreter` functions listed above.

Build: MSBuild `Quire.Native.Mshex.vcxproj`, Release|x64, toolset v145, `/O2 /Gy /GS /EHsc /MD`, `stdcpp17`, `TreatWarningAsError`. Output goes to `.mirror-cache/bin/`. The original's flags were not re-derived: all 17 functions match under these flags, so no change was needed. The exact compiler build was not separately confirmed.

## Imports (D10)

`mshex.dll` imports only `KERNEL32.dll`, `MSVCP140.dll`, `VCRUNTIME140.dll`, `VCRUNTIME140_1.dll` and the API CRT sets. None is a Mathcad-owned DLL, so no `.def` or `lib.exe` import library is needed for this project.

## Remaining work

- 363 exports remain, from 4-byte accessors up to large STL-using bodies such as `Error::GetDetails` (399 bytes).
- Engutils imports two functions: `Error::GetDetails` (returns `std::string`, `Substring`) and `operator<<(ostream&, Error const&)`. Neither is a leaf: both call non-exported code, so neither passes strict yet.
- Next leaves that need only known layouts: `Module` accessors (`GetState` reads `[rcx+88h]`; `GetFlags`, `GetDatum`, `Is*` need the flag and datum offsets), `Error` accessors (`GetCode` reads `[rcx+18h]`; the rest of `Error` is not yet inferred), `State`, `Visitor`. The `Error` getters that read `Tag` fields need to be defined in `Error`'s own translation unit, because a call to `Tag::Get*` from another translation unit is unverified under strict.
- The `Insert(Formatter, Node|Token|Value|...)` family (10 bytes each) dispatches through vtable slots. Those slot indices depend on the full virtual method order of each class, which is not yet inferred.

## Harness limitations (reported, not worked around)

- **Internal references are always unverified.** Under `--strict`, a call to a non-exported function, or any reference to vtables, string literals or globals, counts as unverified and fails, even when the bytes match. By a heuristic scan of the original's disassembly, about 210 of the 344 non-data exports reference such internals, so strict cannot pass them until the harness can resolve internal symbols. I did not change the harness (D10).
- **No `.pdata` or relocation problems seen.** All 17 extents were exact, from `.pdata` or from the next function start with trailing `int3` removed, and no relocation gaps came up.
- **Product ID:** not checked against the original's Rich header for this group. The compiled groups match byte-for-byte, so no effect was observed.

## Landing

Not landed. Commit only owned files on this branch. Ask the maintainer to land it. Do not use `lane_merge.py`, since its Release IL gate does not cover native code (D10).

## Harness proposal for the maintainer (not applied)

The brief keeps `tools/validation/native_bytes.py` under the maintainer. This is the change Lane J needs, with `--strict` unchanged:

- **Internal calls:** a relocation to a non-exported symbol is verified when the original's call site resolves to a function
  whose bytes equal the candidate callee's bytes. The harness needs a reviewed manifest, for example
  `docs/mirror/native/internal-targets.json`, mapping each decorated internal name to its original RVA. The harness checks
  the manifest entries against the original's bytes and still reports anything missing as unverified.
- **Data references:** a relocation to vtables or `.rdata` is verified by comparing the referenced data slot by slot (vtable
  entries compared through the function bytes above) and by comparing constants byte for byte.
- **Scope:** the change affects only how unverified targets are classified. Byte comparison, the export check and `--strict`
  stay as they are.

Until the maintainer lands this, the next Lane J batches are the Module and Error leaves. Those need no internal calls.
