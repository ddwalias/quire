# Lane J: mshex.dll → Quire.Native.Mshex

**Status: in progress.** 87 of 380 exports pass strict byte validation (0 unverified targets). Export parity: 293 missing, 0 extra.

- **Owner:** Lane J (native, `mshex.dll`)
- **Branch:** `mirror/j-mshex-202610241256`, rebased onto `main` (354255ea)
- **Worktree:** `C:/Projects/Quire-wt/lane-j-mshex`
- **Build:** `MSBuild Quire.Native.Mshex.vcxproj`, Release|x64, pinned toolset `C:/BuildTools/VS18.6-msvc` (cl 19.51.36246). Flags `/O2 /Gy /GS /EHa /MD`, `stdcpp17`, `_CRTIMP2=__declspec(dllimport)` (D28). Output in `.mirror-cache/`.
- **Original:** `C:/Program Files/PTC/Mathcad Prime 12.0.1.0/mshex.dll`. Disassembly and export dump are in `.mirror-cache/orig/`.

## Passing groups (strict)

| Group | Notes |
|---|---|
| `Hex::Tag` constructor, accessors, setters, `+=`, copy and move assignment | `hex_tag.{h,cpp}`. Layout: offset @0, extent @8, handle @0x10 (`uint32`), size 24. Default member initializers are offset and extent `~0`, handle 0. The getters are inline `HEX_EXPORT` so `Error` can inline them. |
| `Hex::Token` / `TokenOf<void>` | `hex_token.{h,cpp}`. Vptr @0, `uint16` token @8, `Tag` @0x10. |
| `Hex::Node::GetTag` | `hex_node`. Returns the null tag. |
| `Formatter::Insert` dispatch to `Node` and `Component` | `hex_formatter`. Tail jumps through vtable slots. |
| `Hex::Module` flags, datum, state, loadable, status and slot accessors | `hex_module`. Bytes 0x08..0x47 and 0x50..0x77 are not inferred and stay as padding. `State` slots 1 and 3 are unidentified and stay undefined. |
| `Hex::Error` constructors | `hex_error`. `Error()`, `Error(unsigned)`, `Error(unsigned, Tag const&)`, as plain source under `/EHa`. The byte store after the 16-byte zeroing is in the original too. |
| `Hex::Error` tag and code accessors, `GetDetails() → Substring<char const>` | `hex_error`, `hex_substring.h` (new, minimal `Substring<Char>` with begin and end pointers). `GetDetails` computes `data()` before `size()`, the order the original uses. |
| `Environment::Visitor` constructors (default, copy, move) and vtable `??_7Visitor` | Two slots only: `Visit` (returns false) then `Alter` (forwards to slot 0). No virtual destructor. |
| `DefaultNames::hash::bucket_size` (4) and `min_buckets` (1024) | Exported `static const` data. |
| Value, Environment, Visitor, State, Interpreter, Library, DefaultNames assignments and stubs | Member-less classes and `= default` assignments. |
| `TokenOf<void>` assignments, `Value::Insert` | `hex_token`, `hex_value`. |
| `Visitor::Alter`, `DefaultEnvironment::GetSignatures`, `Tag::operator+=` | |
| `imbue` constructor, comparison bases, `Library` and `DefaultNames::hash` assignments | |
| `Lexer::SetFlag`/`GetFlag`, `Interpreter::SetOption`/`GetOption`/`SetEnvironment` | Constant-returning stubs. |

## Parked

Each entry lists the source shapes tried under D28's settings. Parked functions are not exported and their source is not in the tree.

- **`Hex::operator+(Tag const&, Tag const&)`** (`??HHex@@YA?AVTag@0@AEBV10@0@Z`, RVA 0x2620, 26 bytes). The original copies the first tag's offset and handle as qword loads and stores, and takes the second's extent. Shapes tried: copy then `+=` (28 bytes, the copy uses `movups`/`movsd`); field-wise `Tag(first.GetOffset(), second.GetExtent(), first.GetHandle())` (24 bytes, 32-bit handle load). Neither copies the handle's padding as a qword. Parked under D25.
- **`Error::GetDetails(Substring<char const>) const → std::string`** (`0x8780`, the 399-byte overload). Not yet re-tested under D28. It calls `std::string` internals, which are not exported, so it needs reviewed internal targets first.
- **`operator<<(ostream&, Error const&)`** (`0xa2d0`). Not yet re-tested under D28. It calls `0x99e0` and `0xa7c0`, which need reviewed internal targets. The D25 evidence (the `pword` accessor `0xa7c0` differs in one register, `eax` vs `edx`) predates D28, so the re-test is open.

## Attachment (D28 amendment) and the vtable-slot blocker

`Attachment` is the virtual base of `Component`, `Value`, `Environment` and the rest. The source matches the original's
constructor (`0xC490`, 102 bytes), non-deleting destructor (`0xC500`, 58 bytes) and `Destroy` (`0xC5F0`) when its file is
compiled with `/EHsc` (D28 amendment). The toolset's `operator new` and sized `operator delete` are recorded as toolchain
entries (`0xC780`, `0xC8B0`). `Value` derives from it at offset 0. Its constructor (38 bytes, `/EHa`) and destructor
(18 bytes, implicit body, `/EHa`) match with one unverified target each, the `Attachment` vtable.

**Blocker (harness, for the maintainer):** the `Attachment` vtable (`0xF388`, 16 bytes) has two slots. Slot 1 (`Destroy`,
`0xC5F0`) verifies when it is recorded. Slot 0 (the scalar deleting destructor `??_G`, `0xC540`) is a relocation to the
COMDAT's section symbol, so `native_bytes.py` sees no name for it and `native_targets.py` cannot record it (it reports "not
referenced by any candidate function that matches the original"). The same happens for every vtable whose slot is an
implicit COMDAT, so `Value`'s vtable and everything derived from `Attachment` stay unverified. Not edited (D10/brief): the
work-in-progress is in the ignored cache, `.mirror-cache/wip/attachment/`. Landing it now would add manifest entries that
fail the gate, so it is not committed.

## Next batches

Ordered per D28 and the brief:

1. Re-test `operator<<` and `GetDetails(Substring)` with reviewed internal targets (native_targets.py, which records `0x99e0`, `0xa7c0` and the `std::string` internals from matching call sites).
2. `Signature` constructors and the `std::string`-holding types they unblock.
3. Vtable chains: each class's virtuals, then constructors, destructors and `??_7`/`??_E`/`??_G`.
4. `_Tree`/`_Tmap`/`basic_*` instantiations.

## Remaining mismatches

None among the 81 passing exports. The export-parity gap (299 missing) is the work remaining, not a mismatch.
