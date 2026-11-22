# Quire.Native.Msprolog (msprolog.dll) checkpoint

- **Owner:** native-msprolog worker (Orca task `task_9789d651b18c`).
- **Branch:** `feature/haiku-msprolog-20261123`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/haiku-msprolog-20261123`.
- **Base commit:** `88eb55a8b` (`main` at start).
- **Gate:** `PYTHONUTF8=1 python tools/validation/native_gate.py --base main --project Quire.Native.Msprolog`.
- **Status:** PASS, 5 of 33 exports matched (5 checked, 0 failures).

## Members matched

- `??4Program@Prolog@@QEAAAEAV01@AEBV01@@Z` (copy assignment; `mov rax, rcx; ret`)
- `??4PObjParser@Prolog@@QEAAAEAV01@$$QEAV01@@Z` (move assignment)
- `??4PObjParser@Prolog@@QEAAAEAV01@AEBV01@@Z` (copy assignment)
- `?GetProgram@Ruleset@Prolog@@QEBAPEAVProgram@2@XZ` (returns the program at offset 8)
- `??CRuleset@Prolog@@QEBAPEAVProgram@1@XZ` (`operator->`, shares the body of `GetProgram`)

## Blocked / remaining (28 exports)

1. **Program constructors and non-deleting destructor (`??0Program` x2, `??1Program`) and `??_7Program` vftable.**
   The constructors set the vptr, so they pull the vftable into the object. The original vftable (RVA `0x24A58`,
   six slots: one deleting slot plus five `_purecall` slots) names the vector deleting destructor
   (`??_EProgram`, RVA `0x9BA0`, 137 bytes, with the array-cookie path). A per-object compile emits the scalar
   deleting destructor (`??_GProgram`, 43 bytes) as the vftable slot, and the candidate's `??_E` is a weak undefined
   symbol. The vftable is the only reference to that function, so `native_targets.py` cannot record it. Parked
   pending evidence on which source shape makes MSVC emit `??_E` in the vftable. The current batch leaves these out
   so the vftable is not emitted.
2. **`??1Ruleset` and `??_7Ruleset` vftable.** The original vftable slot 1 (RVA `0x26CE8`) is a virtual function that
   is not in the export list and has not been mirrored. The destructor is the same `delete program` shape as the
   Program case.
3. **`??0Ruleset` and `NewProgram` (`0xE7D0`).** Both call the unexported constructor `0xE800`, which builds the
   concrete program implementation (vftables `0x24F50` and `0x24FD0`, a 0x140-byte object, and std::map-like members).
   Not started.
4. **`PObjParser` parsing and matching functions (`parse` x6, `match` x4, `parse*`, `eat_white`, `format_code`,
   `get_string`, `make_variable`, `_match`).** These need the `Engine::Unmanaged` anyval/alloc types from
   `Quire.Native.Mspcons`, the `istream_wrapper` class, and `std::deque`/`std::vector` instantiations. Not started.

The Ruleset constructor (`0x9CF0`) is also unstarted; it depends on items 2 and 3.

## format_code follow-up (task_7bf814fd3ff3)

- `?format_code@PObjParser@Prolog@@CAEPEBVanyval@Unmanaged@Engine@@@Z` (RVA `0xA7D0`, 86 bytes, private static) is
  `dynamic_cast<Engine::Unmanaged::pstring const *>(value)`, then `text != nullptr && strlen(text) == 2 && text[0] == '%'`
  returning `text[1]`, else `'0'`. The import thunks `__RTDynamicCast` and `strlen` verify by name.
- With the `pstring` declaration inside `Engine::Unmanaged` (text pointer at offset 0x10; offset 0x08 not identified) and the
  null check, the candidate matches the original in all bytes except two unverified call/address targets:
  the type descriptors `??_R0?AVpstring@Unmanaged@Engine@@@8` (original RVA `0x30B10`) and
  `??_R0?AVanyval@Unmanaged@Engine@@@8` (original RVA `0x30B40`).
- These descriptors are referenced by the candidate but not defined by any Msprolog object, so `native_targets.py`
  cannot record them. Defining them would require a `pstring` vftable or key function in this project, which the original
  supplies from Mspcons. Blocked until the maintainer decides how RTTI descriptors owned by Mspcons are verified
  (or Mspcons exports them). The Mspcons header does not yet declare `pstring`; that needs a maintainer edit.
- Not landed: the gate fails on those two targets. Program/Ruleset vftables remain parked.

## format_code landing (task_6d51318dc9f3)

- `?format_code@PObjParser@Prolog@@CAEPEBVanyval@Unmanaged@Engine@@@Z` is implemented in `prolog_pobjparser.cpp` (private static) with a
  Msprolog-local `Engine::Unmanaged::pstring` declaration in `prolog.h`. The observed char pointer is at offset 0x10; offset 0x08 is
  not identified and has no semantics assigned.
- Strict native gate on this branch with the `msprolog.dll` entries from `a6b0376f8` applied (`git apply`, not committed here):
  `native_gate.py --base main --project Quire.Native.Msprolog` PASS, 9 checked, 0 failures, 0 regressions (6 exports, the RTTI
  descriptors `??_R0?AVanyval@Unmanaged@Engine@@@8` and `??_R0?AVpstring@Unmanaged@Engine@@@8`, and the `??_7type_info@@6B@` toolchain entry).
- Combined landing: the manifest entries (`a6b0376f8`, on `maintainer/msprolog-rtti-20261123`) must enter `main` with this change.
  Without them the gate fails on the two descriptors and `format_code`.
- Still parked: Program constructors, `??1Program`, `??_7Program`, `??1Ruleset`, `??_7Ruleset`, `NewProgram`, the Ruleset constructor and
  the remaining PObjParser functions.
