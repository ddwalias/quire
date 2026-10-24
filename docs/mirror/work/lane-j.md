# Lane J: mshex.dll → Quire.Native.Mshex

**Status: incomplete.** 66 of 380 exports pass strict byte validation. Export parity: 314 missing, 0 extra. The remaining work is listed under "Remaining work".

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
| Assignments and stubs of `Value`, `Environment`, `Environment::Visitor`, `Module::State`, `Tag`, `Node`, `Token`, `Interpreter`, `Library`, `DefaultNames::hash` | copy and move assignments (`= default`), `Environment::AddValue`, `SetValue`, `Visitor::Visit`, `State::GetExports`, `GetImports`, `IsConsistent` | 20/20 OK, 0 unverified | `hex_value`, `hex_environment`, `hex_module`, `hex_tag`, `hex_node`, `hex_token`, `hex_interpreter`. Assignments copy nothing where the class has no members of its own (the 4-byte `mov rax,rcx; ret` group). `Tag`'s copy and move assignments copy 24 bytes. The `xor al,al` and `mov al,1` stubs return `false` or `true`. `State`'s layout is not inferred, so only its virtual and non-virtual members are declared. |
| `Hex::imbue` constructor | `imbue(Interpreter const*)` | 1/1 OK, 0 unverified | `hex_imbue`. Stores the pointer and returns `this`. |
| Comparison bases and `Library`/`DefaultNames::hash` assignments | copy and move assignments of `equality_comparable`, `less_than_comparable` and `totally_ordered` (instantiated for `Signature`), plus `Library` and `DefaultNames::hash` | 8/8 OK, 0 unverified | `hex_comparable`, `hex_library`, `hex_default_names`. Member-less classes. |
| `Hex::Lexer` flags | `SetFlag`, `GetFlag` | 2/2 OK, 0 unverified | `hex_lexer.{h,cpp}`. Both return `false`. |
| `Hex::Interpreter` options and environment | `SetOption`, `GetOption`, `SetEnvironment` | 3/3 OK, 0 unverified | `hex_interpreter.{h,cpp}`. All return `false`. |

Consumers: `mctranslator.dll` imports `Tag::GetHandle`, `TokenOf<void>::GetTag`, `TokenOf<void>::GetToken`, `Lexer::SetFlag` and `GetFlag`, and `Interpreter::SetEnvironment`. `msmpl.dll` imports `Node::GetTag` and the `Lexer` / `Interpreter` functions listed above.

Build: MSBuild `Quire.Native.Mshex.vcxproj`, Release|x64, toolset v145, `/O2 /Gy /GS /EHsc /MD`, `stdcpp17`, `TreatWarningAsError`. Output goes to `.mirror-cache/bin/`. The original's flags were not re-derived: all 17 functions match under these flags, so no change was needed. The exact compiler build was not separately confirmed.

## Harness notes (after `b148bc06`)

- The lane branch is rebased onto `main` at `b148bc06`. The manifest route is in place, but this batch needs no internal targets, so it is not used.
- **Multi-object runs reject a weak default.** `_Avx2WmemEnabledWeakValue` is defined in `hex_node.obj` as `0x0a8664` and in `hex_interpreter.obj` as `0x0c8664`, because of the header set each object includes. `definitions_in_objects` raises on any name defined differently, even when no function references it. Each object is validated in its own run here. The maintainer should decide whether such weak data should be compared at all.

## Rebase onto main (8cfa4f8f)

- The lane is rebased onto main. Main's native harness replaced the earlier SECREL and reviewed-target commits, so the lane carries no harness changes of its own.
- Main pins MSVC build 36246 for native builds (`release_il.py`), which matches the BuildTools toolset the lane builds with. The 36 functions pass under main's harness: 36 OK, 0 unverified.
- **Still open for operator<<:** main's harness does not model SECREL (`native_bytes.py` reports it as unsupported). The TLS accessor needs it, so SECREL must be added on top of main as a harness commit. That commit is not made yet, because the accessor itself still differs from the original in one register (see below).

## Harness work for operator<< (approved decision 1)

- **SECREL (`4b9cbb03`).** `IMAGE_REL_AMD64_SECREL` is 4 bytes. The original resolves it as `.tls` base + field. The candidate's addend is read like `ADDR32NB`. Tests: 7 new cases.
- **Reviewed linker and CRT targets (`2c7cd312`).** `_tls_index`, `_Init_thread_epoch`, `_Init_thread_header` and `_Init_thread_footer` are not defined by the candidates. The toolset defines them in `libcmt.lib`, and the original has them as internal code and data, not as DLL imports. A manifest entry can now carry `"reviewed": true`. `native_targets.py --reviewed NAME` records one, and `--size NAME=BYTES` gives a data target's size. A reviewed entry verifies when its sites resolve to its RVA. Its bytes are not compared, because no candidate defines them. A stronger check would compare them against the toolset library's members; that is a possible follow-up.
- 70 harness tests pass.

### operator<< analysis (not yet validated)

- `0xa2d0` (`operator<<`) calls `0x99e0`, which calls `0xa7c0`.
- `0xa7c0` is a thread-local initialization guard: it reads the DLL's TLS index (`[0x1fdd4]`), the thread's TLS block through `gs:[58h]`, and a 4-byte per-thread value at offset 4. It compares that with `_Init_thread_epoch` (`[0x1fe50]`), calls `_Init_thread_header` (`0xc9a0`) and `_Init_thread_footer` (`0xc930`), and calls an import-like slot at `[0xf140]`.
- `0x99e0` is a function-local static: it calls `0xa7c0`, and if the result is zero it calls `[0xf030]`. It then fills a static block at `0x1fe00`/`0x1fe08` with vtable pointers, and calls `0xd786` and `0xcd90` (locale-facet lookup and static construction).
- **Resolved: `0xa7c0` is a stream-slot accessor.** The tail is `jmp [ios_base::pword]` with `index` from `[0x1fe48]`, which is `xalloc()`. A candidate `Hex::Pword(std::ios_base&)` with `static const int index = std::ios_base::xalloc(); return stream.pword(index);` gives the same instructions, the same `_Init_thread_header`/`_Init_thread_footer` guard, the same `TLS` epoch read at `[tls_index]`, `gs:[58h]`, and `_Init_thread_epoch`, and the same `jmp`. It differs from the original in one register (`eax` where the original uses `edx`) and in block layout (the original puts the slow path inline).
- **Compiler build (corrected).** The lane now builds with the BuildTools toolset (`C:\BuildTools\VS18.6-msvc`, `cl` 19.51.36246), which matches the original's Rich header. Build with `C:\BuildTools\VS18.6-msvc\MSBuild\Current\Bin\MSBuild.exe`. The Professional install's `cl` reports 19.51.36260 and is not used. The 36 functions still pass with 0 unverified targets under 36246.
- **The `0xa7c0` register difference is not the compiler build.** With 36246 the accessor still uses `eax` where the original uses `edx`. I also tried `/O1 /Os /Ox /Oi- /Ob1`, `/GL` with `/LTCG`, a helper-call variant, a local-copy variant and `noexcept`. `/Os` gives the original's branch sense (`jle` over an inline slow path), but it does not tail-jump to `pword`, which the original does. None matches the original exactly.
- **Remaining blocker: source of `0xa7c0`.** The original accessor differs from every local variant in one register and in block layout. Finding the source shape (or the flags that produce it) is the open problem.

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

## Parked (D25)

The maintainer parked these after the Rich-header finding (D25): the original's `/GL` objects are optimized by the linker, so no COFF compile reproduces them.

- **`operator<<(ostream&, Error const&)`** (`0xa2d0`). It calls `0x99e0`, which calls `0xa7c0`. `0xa7c0` is the stream-slot accessor `return stream.pword(index)`, with `index` from a function-local static `xalloc()`. A candidate with that source matches the instructions, the guard and the tail jump, except that the original reads the epoch into `edx` and the candidate into `eax`. No flag or build I tried reproduces it.
- **`Error::GetDetails`** (`0x8780`, 399 bytes, returns `std::string`). It calls standard-library internals, which are linker-optimized in the original.

- **`Error` constructors** (`0x8700`, `0x86c0`, `0x8740`). The layout is now inferred: tag at 0, code at 0x18, `std::string details_` at 0x20 (buffer 16 bytes, size at 0x30, capacity 15 at 0x38). Each constructor differs from our build by one redundant byte store, `mov byte [rcx+20h], al`, which the original's optimizer removes after the 16-byte zeroing. The constructors are not exported until that is resolved, so they are parked with this D25 evidence.

## Remaining work (after this batch)

- **Standard-library blocker (investigated):** a candidate `std::string` default construction stores one more byte than the original's (`mov byte [...], al`, after the 16-byte zeroing). The byte survives `/GL` with `/LTCG` in a separate test, so it is not explained by link-time optimization as D25 assumed. The `Error` constructors and every `std::string`-holding type (`Signature`, `Error`) hit it. `std::map<Hex::Signature, ...>` needs `Signature` complete, so the `_Tree` helpers wait on this difference, which is not understood yet.
- **Standard-library internals (166 exports):** `std::map` (`_Tree`) helpers, `_Tmap_traits`, `std::string`/`basic_*` templates, and the `DefaultNames::hash` operator. Matching them needs the same STL instantiations, and their internal calls must be recorded.
- **Vtable chains (about 57 constructors and destructors, plus virtual calls):** the `??0X` and `??1X` bodies reference vtables. Verifying a vtable verifies each slot's function, so every virtual method of `Node`, `Token`, `Value`, `Environment`, `State`, `Interpreter` and the rest must be implemented or recorded first. The `State` slots used by `Module` (slot 2 returns status, slot 4 `GetImports`, slot 5 `GetExports`) are known, but slots 1 and 3 are not identified.
- **Formatter overloads** (`Insert` for `Token`, `Value` and `Interpreter`): these dispatch through vtable slots of classes whose order is not yet inferred.
- **Parked (D25):** `operator<<`, `Error::GetDetails` and the `Error` constructors above. Parking them does not explain the `std::string` byte difference, which is still open.

**SECREL (deferred).** It is needed only by the parked TLS accessor. When it is added, its base is the TLS directory's `StartAddressOfRawData` (`0x12188`), not a `.tls` section: the original has no `.tls` section, because `.tls` is merged into `.rdata`.
