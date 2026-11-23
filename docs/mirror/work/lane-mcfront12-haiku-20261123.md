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

## Batch 3: ParseHex, UintFromObject, ErrorDictFromCondition, ErrorDictFromObject, HasErrors

- Owner: Haiku worker, Orca task `task_81af66960f90` (dispatch `ctx_3542320fba5e`).
- Members: `<Module>` `Engine.ParseHex(string)` and `utl.ToStrng(object)` in `MixedMode/Mcfront12Parse.cpp`; `Engine.UintFromObject(object)`, `Engine.ErrorDictFromCondition(mslisp.Condition)`, `Engine.ErrorDictFromObject(object)` and `Engine.HasErrors(depman.ActionResult)` in `MixedMode/Mcfront12Port.cpp`. Each is pinned with `/include` on its decorated name.
- Layout: ParseHex is in its own translation unit. Kept in the port TU, `UintFromObject` got an extra zero-initialized `uint32` temp (MSVC merges the `return 0` and `return ParseHex(...)` exits); in a separate TU both match. ToStrng moved with it, because ParseHex's leading `ldc.i4.0; stloc.s 4` appears only when ToStrng's definition is in the same TU.
- References: the csproj and vcxproj now reference `Quire.EngutilsMgd` (`StringUtils.ParseInt`) and `Quire.Mslisp` (`mslisp.Condition`). ForcedUsingFiles lists EngutilsMgd first, so the call resolves to the mixed declaration with `Int32` boxed-return modopts instead of the `Quire.Engine` stub.
- Release IL: `release_il.py --base main --project Quire.Mcfront12` FAIL on one row only: 249 checked, 243 exact (`.mirror-cache/release-il-326ecf0d7509/release-il.json`). All new members are checked and exact (ParseHex, UintFromObject, ErrorDictFromCondition, ErrorDictFromObject, HasErrors, utl.ToStrng).
- Remaining mismatch: `<Module>::sscanf` (pinvoke, original 0x060000aa), native row `+0x21: length 8 != original 13176`. The gate's leaf fallback at the nested options helper (RVA 0x9c50) is the cause; maintainer commit 0c5002c69 changes nested native comparison and is not on `main` yet. No manifest entry was added. Re-run after that fix lands on `main`.
- Unpaired Mathcad members after this batch: 80 (CRT, boost, Windows API, and `NodeMap` optimizer members `NewCENode`, `NewCENodeTopTop`, `GetLastCENode`).

## Batch 4: NodeMap.NewCENode and NodeMap.NewCENodeTopTop

- Owner: Haiku worker, Orca task `task_0074f34ad42b`. Lands separately on top of batch 3 (branch `feature/haiku-mcfront12-batch3-20261123` points at 666591028).
- Members: `Quire.Engine.NodeMap.NewCENode` and `NewCENodeTopTop` in `MixedMode/Mcfront12Port.cpp`, placed after `NewCENodeGen`. Each stores the `NewCENodeGen` result in a local before returning it, because returning the call directly makes the compiler add a `KeepAlive(this)` that the original does not have.
- Release IL: `release_il.py --base main --project Quire.Mcfront12` checks 251; both new members are exact. The only failure is the `<Module>::sscanf` boundary row, which the pending maintainer fix addresses (see batch 3).
- Not landed: `NodeMap.GetLastCENode`. The found-path `KeepAlive` matches the original once the explicit one is removed, but our compiler threads the null check past the loop test (`brfalse` to the return, original to the loop test). Several source forms were tried. The WIP stays out of the tree. It is unpaired and does not block batch 4 (unfinished members outside the changed scope do not block).
- Remaining unpaired after batch 4: 78 Mathcad members (CRT/boost/std/Win32/utl natives, anonymous-static DocumentLogging initializers, and `GetLastCENode`).