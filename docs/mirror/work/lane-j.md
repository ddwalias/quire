# Lane J: mshex.dll → Quire.Native.Mshex

**Status: in progress.** 102 candidate functions match strictly (0 unverified targets); 92 of 380 exports are present. Export parity: 288 missing, 0 extra.

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

## Attachment (D28 amendment) and the Value group

`Attachment` is the virtual base of `Component`, `Value`, `Environment` and the rest. Its file `hex_attachment.cpp` is
compiled with `/EHsc` (D28 amendment). Its constructor, non-deleting destructor and `Destroy` match, as do the toolset's
`operator new` and sized `operator delete` (toolchain entries). `Value` derives from it at offset 0 and matches: constructors
(`/EHa`), the implicit destructor, its vector deleting destructor and its vtable. The vtable slot that was unverified
resolves through the weak external `??_EValue` (the maintainer's harness fix on main, bf4b54f9).

The vtables of `Value` and `Visitor` are exported as data through `/EXPORT:…,DATA` in the project's link options. A
class-level `dllexport` would also export members the original does not export, such as `??_E`.

## Next batches

Ordered per D28 and the brief:

1. `Component`: its virtual `Insert` and `OnError` must be implemented first. `OnError` reaches the parked
   `operator<<(ostream&, Error const&)`, so this waits on that function.
2. Re-test `operator<<` and `GetDetails(Substring) → std::string` (`0x8780`) with reviewed internal targets.
3. `Signature` and the `std::string`-holding types it unblocks.
4. The other `Attachment`-derived classes (`Environment`, `Formatter`, `Lexer`, `Parser`, `Phase`, `Heap`, `Names`,
   `DefaultX`), once `Component`'s vbase layout is in place; their vtables export through `/EXPORT,DATA` the same way.
5. `_Tree`/`_Tmap`/`basic_*` instantiations.

## Remaining mismatches

None among the 81 passing exports. The export-parity gap (299 missing) is the work remaining, not a mismatch.
