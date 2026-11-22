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
