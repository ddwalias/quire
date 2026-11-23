# Mcfront12 checkpoint (haiku-mcfront12-20261123, batch 1)

- Owner: Haiku worker, Orca task `task_1885a2c5f700`.
- Branch: `feature/haiku-mcfront12-20261123`.
- Members: `<Module>` `Engine.ToActionResult(object, object)` and `Engine.ErrorDictFromMessage(string, uint32, uint32)`, written as global functions in `namespace Engine` in `src/Quire.Mcfront12/MixedMode/Mcfront12Port.cpp`.
- Emission: the compiler emits these only when referenced, and `OptimizeReferences` drops unreferenced managed types, so two `#pragma comment(linker, "/include:…")` lines keep the exact decorated definitions (checked with `dumpbin /symbols`).
- Release IL: `release_il.py --base main --project Quire.Mcfront12` PASS; 212 checked, 210 exact, 0 failures (`.mirror-cache/release-il-21cbef35f341/release-il.json`). Both new members are checked and exact.
- Remaining mismatches: none in checked members. 114 Mathcad members stay unpaired: 113 `<Module>` CRT/std/AST and 3 NodeMap optimizer members (`NewCENode`, `NewCENodeTopTop`, `GetLastCENode`).
- Next: `Engine.UintFromObject`/`ParseHex` (native `std::string`), `ErrorDictFromCondition`, `ErrorDictFromObject` need `mslisp.Condition` (not referenced by this project). Their only real callers are `Engine._ErrorDictFromObject` and `Engine.LispEval`/`LispBind`/`LispEvalWithState`, which are not in the port.

## Batch 2: utl.ToStrng (object) -> std::string

- Members: `<Module>` `utl.ToStrng(object)` (mcfront12 RVA 0x99a4, 163 bytes), mirrored in `Mcfront12Port.cpp` with the `EngutilsNative.h` shape, pinned with `/include` on `?ToStrng@utl@@$$FYM…`.
- Allocator rows that `std::string` pulls in: `new` (68 bytes), `delete[]` (5), `delete` (21). These bind to the D23 manifest entries on main (`docs/mirror/native/internal-targets.json`, `mcfront12.dll` managed section); the report verdict is "toolchain entry (D23): name and size verified; the candidate body is not compared" for each.
- Release IL: PASS; 243 checked; 238 exact; 0 failures; native pairs 17 of 17 exact (`.mirror-cache/release-il-a87edf32c40d/release-il.json`).
- Still open: `ParseHex`, `UintFromObject`, `ErrorDictFromCondition`, `ErrorDictFromObject`; the `sscanf` call target (`0x060000aa`, vararg native unmanaged) is not yet emitted by the candidate.
