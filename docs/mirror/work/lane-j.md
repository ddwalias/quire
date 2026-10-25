# Lane J: mshex.dll → Quire.Native.Mshex

**Status: in progress.** 106 candidate functions match strictly (0 unverified targets); 95 of 380 exports are present. Export parity: 285 missing, 0 extra.

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

## Parked with evidence (this round)

- **`Node::GetError(unsigned) const`** (`0xaf10`, 109 bytes). Its body is `Error(code, GetTag())` with the constructor inlined,
  which matches. It is parked because the destructor it pulls in, `Error::~Error` (`??1Error`, 107 bytes, exported), does
  not. The original keeps the `std::string` reset stores (`size = 0`, capacity 15, first byte 0). Shapes tried: implicit
  destructor (22 bytes, the stores are eliminated), out-of-line empty body (22 bytes), explicit `details_ = std::string()`
  (matches the shape, but two objects then define `??1basic_string` differently, which the harness refuses).
- **`Signature` accessors** (`GetName`, `GetType`, `GetModule`, const and non-const; `0x2430`, `0x2460`, `0x2490`).
  The layout is `uint16` name end at `+0`, `uint16` module start at `+2` and `std::string` text at `+8` (size at `+18h`,
  capacity at `+20h`). The default constructor (`0xB400`) matches. The accessors do not: `GetName` is 34 bytes against 35,
  `GetType` 41 against 46 and `GetModule` 39 against 41. The original keeps the data pointer in `rax` and decrements it
  (`dec rax` on the pointer), loads the header as a qword before splitting it, and loads `module_start` before the cap
  check. Shapes tried: integer decrement, pointer decrement with a named `text`, the length loaded into a local first,
  the expression without locals (58 bytes), and `/EHsc` for the file (breaks the constructor, 43 against 48, and leaves the
  accessors unchanged). Parked under D25; they are not exported, so their rows stay absent.
- **`Signature` destructor** (`??1Signature`, `0x1C20`, 107 bytes). The same `std::string` reset stores as `~Error`. The
  implicit destructor is not defined, for the same reason.
- **D29 static layout** (`Formatter::Fetch` `0x99E0`, the stream-slot accessor `0xA7C0`). Retested under D28: with
  `_CRTIMP2=dllimport`, `0xA7C0` tail-jumps to `pword` through the import, so that difference is gone. What remains is the
  function-local static: the original keeps its first-time initialization inline after a `jle`. Per-object compilation
  puts it out of line under `/EHsc`, `/EHa`, `/O1`, `/Os`, `/Ox`, `/Og /Ot`, `/Oi-`, `/Ob1` and three source shapes. `Fetch`
  and `operator<<(ostream&, Error const&)` (`0xA2D0`, an export, no manifest entry needed) reach it.
  Component's `Insert` (`0x2E50`) and `OnError` (`0x2F20`, through `0x89A0`) contain no such static. They reach
  `operator<<` through the exported `0xA2D0`, so they are not parked themselves. The per-object tooling cannot define the
  export `operator<<` as matching, so the candidate cannot link `OnError` until a D29 decision covers that export.
- **`Hex::operator+(Tag, Tag)`**: still parked (see the Tag section above).

## Remaining work

Export parity: 286 missing. The largest groups are the `_Tree`/`map` instantiations (about 120), the `Attachment`-derived
constructors and vtables (about 40, waiting on `Component`'s virtual base and its `Insert`/`OnError`), `Formatter::Fetch` and
the locale-based helpers (which need `Component`), and the `std::string`-based `Signature` and `Error` members.

## Remaining mismatches

None among the 81 passing exports. The export-parity gap (299 missing) is the work remaining, not a mismatch.
