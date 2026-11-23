# Mcfront12 checkpoint (haiku-mcfront12-20261123, batch 1)

- Owner: Haiku worker, Orca task `task_1885a2c5f700`.
- Branch: `feature/haiku-mcfront12-20261123`.
- Members: `<Module>` `Engine.ToActionResult(object, object)` and `Engine.ErrorDictFromMessage(string, uint32, uint32)`, written as global functions in `namespace Engine` in `src/Quire.Mcfront12/MixedMode/Mcfront12Port.cpp`.
- Emission: the compiler emits these only when referenced, and `OptimizeReferences` drops unreferenced managed types, so two `#pragma comment(linker, "/include:…")` lines keep the exact decorated definitions (checked with `dumpbin /symbols`).
- Release IL: `release_il.py --base main --project Quire.Mcfront12` PASS; 212 checked, 210 exact, 0 failures (`.mirror-cache/release-il-21cbef35f341/release-il.json`). Both new members are checked and exact.
- Remaining mismatches: none in checked members. 114 Mathcad members stay unpaired: 113 `<Module>` CRT/std/AST and 3 NodeMap optimizer members (`NewCENode`, `NewCENodeTopTop`, `GetLastCENode`).
- Next: `Engine.UintFromObject`/`ParseHex` (native `std::string`), `ErrorDictFromCondition`, `ErrorDictFromObject` need `mslisp.Condition` (not referenced by this project). Their only real callers are `Engine._ErrorDictFromObject` and `Engine.LispEval`/`LispBind`/`LispEvalWithState`, which are not in the port.
