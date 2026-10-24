# Lane J: mshex.dll → Quire.Native.Mshex

**Status: incomplete.** 36 of 380 exports pass strict byte validation. Export parity: 344 missing, 0 extra.

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
| `Formatter::Insert` dispatch to `Node` and `Component` | `Formatter::Insert(ostream&, Node const&)`, `Formatter::Insert(ostream&, Component const&)` | 2/2 OK, 0 unverified | `hex_formatter.{h,cpp}`. Each is a tail jump through the argument's vtable (`Node::Insert` at slot 3, `Component::Insert` at slot 0). `Node` and `Component` declare only the methods needed to reach those slots, in the order the slots imply (`Node`: dtor, `GetError`, `GetTag`, `Insert`; `Component`: `Insert`, `OnError`, dtor). The other `Insert` overloads need slots of classes not yet inferred. |
| `Hex::Module` flag, datum, state and loadable accessors | `GetFlags`, `GetDatum`, `GetState`, `IsPersistent`, `IsTransient`, `IsLoadable`, `HasFlags`, `SetLoadable`, `SetDatum`, `SetFlags`, `AddFlags`, `ClearFlags` | 12/12 OK, 0 unverified | `hex_module.{h,cpp}`. Layout from the accessors: vptr at 0, `persistent_` at 0x48, flags at 0x78, datum at 0x80, state at 0x88, loadable at 0x90. The flag mutators return the previous flags. Bytes 0x08..0x47 and 0x50..0x77 are not yet inferred, so they stay as padding. |
| `Hex::Error` tag and code | `Error::GetTag`, `Error::GetCode` | 2/2 OK, 0 unverified | `hex_error.{h,cpp}`. Layout: tag at 0, code at 0x18. `Error`'s other members are not yet inferred. `Error::GetOffset`, `GetExtent` and `GetHandle` read the `Tag` getters, which are inline and exported from `hex_tag.h` (approved decision). |
| `Hex::Tag` getters, inline | `GetOffset`, `GetExtent`, `GetHandle` | 3/3 OK, 0 unverified | `hex_tag.h`. Inline with `HEX_EXPORT`, so `Error`'s translation unit can inline them. The setters stay out of line. |
| `Hex::Lexer` flags | `SetFlag`, `GetFlag` | 2/2 OK, 0 unverified | `hex_lexer.{h,cpp}`. Both return `false`. |
| `Hex::Interpreter` options and environment | `SetOption`, `GetOption`, `SetEnvironment` | 3/3 OK, 0 unverified | `hex_interpreter.{h,cpp}`. All return `false`. |

Consumers: `mctranslator.dll` imports `Tag::GetHandle`, `TokenOf<void>::GetTag`, `TokenOf<void>::GetToken`, `Lexer::SetFlag` and `GetFlag`, and `Interpreter::SetEnvironment`. `msmpl.dll` imports `Node::GetTag` and the `Lexer` / `Interpreter` functions listed above.

Build: MSBuild `Quire.Native.Mshex.vcxproj`, Release|x64, toolset v145, `/O2 /Gy /GS /EHsc /MD`, `stdcpp17`, `TreatWarningAsError`. Output goes to `.mirror-cache/bin/`. The original's flags were not re-derived: all 17 functions match under these flags, so no change was needed. The exact compiler build was not separately confirmed.

## Harness notes (after `b148bc06`)

- The lane branch is rebased onto `main` at `b148bc06`. The manifest route is in place, but this batch needs no internal targets, so it is not used.
- **Multi-object runs reject a weak default.** `_Avx2WmemEnabledWeakValue` is defined in `hex_node.obj` as `0x0a8664` and in `hex_interpreter.obj` as `0x0c8664`, because of the header set each object includes. `definitions_in_objects` raises on any name defined differently, even when no function references it. Each object is validated in its own run here. The maintainer should decide whether such weak data should be compared at all.

## Imports (D10)

`mshex.dll` imports only `KERNEL32.dll`, `MSVCP140.dll`, `VCRUNTIME140.dll`, `VCRUNTIME140_1.dll` and the API CRT sets. None is a Mathcad-owned DLL, so no `.def` or `lib.exe` import library is needed for this project.

## Remaining work

- 363 exports remain, from 4-byte accessors up to large STL-using bodies such as `Error::GetDetails` (399 bytes).
- Engutils imports two functions. `operator<<(ostream&, Error const&)` (`0xa2d0`, 61 bytes) reads the `ostream` vbase offset, calls internal `0x99e0` (a `use_facet`-style helper that reads a thread-local through `gs:[58h]` and a static guard in `0xa7c0`), and dispatches through `[vtbl+38h]`. Its internal callees must be compiled, recorded with `native_targets.py`, and passed with every object. That is the next step for this function. `GetDetails` is in the same position.
- Earlier note, still accurate: `Error::GetDetails` (returns `std::string`, `Substring`) and `operator<<(ostream&, Error const&)`. Neither is a leaf: both call non-exported code, so neither passes strict yet.
- Next leaves that need only known layouts (the Module and Error leaves above are done): `Module` accessors (`GetState` reads `[rcx+88h]`; `GetFlags`, `GetDatum`, `Is*` need the flag and datum offsets), `Error` accessors (`GetCode` reads `[rcx+18h]`; the rest of `Error` is not yet inferred), `State`, `Visitor`. The `Error` getters that read `Tag` fields need to be defined in `Error`'s own translation unit, because a call to `Tag::Get*` from another translation unit is unverified under strict.
- The `Insert(Formatter, Node|Token|Value|...)` family (10 bytes each) dispatches through vtable slots. Those slot indices depend on the full virtual method order of each class, which is not yet inferred.

## Harness limitations (reported, not worked around)

- **Internal references are always unverified.** Under `--strict`, a call to a non-exported function, or any reference to vtables, string literals or globals, counts as unverified and fails, even when the bytes match. By a heuristic scan of the original's disassembly, about 210 of the 344 non-data exports reference such internals, so strict cannot pass them until the harness can resolve internal symbols. I did not change the harness (D10).
- **No `.pdata` or relocation problems seen.** All 17 extents were exact, from `.pdata` or from the next function start with trailing `int3` removed, and no relocation gaps came up.
- **Product ID:** not checked against the original's Rich header for this group. The compiled groups match byte-for-byte, so no effect was observed.

## Landing

Not landed. Commit only owned files on this branch. Ask the maintainer to land it. Do not use `lane_merge.py`, since its Release IL gate does not cover native code (D10).

## Harness proposal (superseded by the maintainer's manifest, `b148bc06`)

The brief keeps `tools/validation/native_bytes.py` under the maintainer. This is the change Lane J needs, with `--strict` unchanged:

- **Internal calls:** a relocation to a non-exported symbol is verified when the original's call site resolves to a function
  whose bytes equal the candidate callee's bytes. The harness needs a reviewed manifest, for example
  `docs/mirror/native/internal-targets.json`, mapping each decorated internal name to its original RVA. The harness checks
  the manifest entries against the original's bytes and still reports anything missing as unverified.
- **Data references:** a relocation to vtables or `.rdata` is verified by comparing the referenced data slot by slot (vtable
  entries compared through the function bytes above) and by comparing constants byte for byte.
- **Scope:** the change affects only how unverified targets are classified. Byte comparison, the export check and `--strict`
  stay as they are.

The next Lane J batches are the Module and Error leaves, which need no internal calls.
