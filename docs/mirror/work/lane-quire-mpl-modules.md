# Lane Quire.Mpl.Modules Checkpoint

- Owner: Lane Quire.Mpl.Modules (task `task_42d7b4d955fb`). Branch `mirror/mpl-modules-park-diff-20261116` from `main` `1d9add7e7`. Worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-quire-mpl-modules`.
- Target: `src/Quire.Mpl.Modules` (D2 host project hosting 15 Mpl assemblies: `Hashtable`, `McdRun`, `McdStaticUnitSystem`, `McdStaticUnitSystemCGS`, `McdStaticUnitSystemMKS`, `McdStaticUnitSystemNone`, `McdStaticUnitSystemSI`, `McdStaticUnitSystemUS`, `Mpl`, `MplConstants`, `MplMacro`, `MplPrelude`, `MplPrimitives`, `MplSexpr`, `MplUnitSystemSI`).

## Release IL Report

- `release_il.py --base main --project Quire.Mpl.Modules`: PASS; 0 checked; 0 failures; 530 stubs skipped (not implemented), 119 Mathcad members without Quire counterpart.
- `release_il.py --base main --project Quire.Mpl.Modules --all`: 530 checked; FAIL (all checked members differ from Mathcad Release IL due to Mpl compiler conventions: `.maxstack 48` fat method headers, `tail.` calls, and Mpl module attributes).
- `[MirrorStub]` line count in `src/Quire.Mpl.Modules/Generated`: 1,693 lines across 82 generated files.

## Opcode-Level Release IL Diff

To demonstrate the fundamental difference between Mathcad's Mpl compiler output and Roslyn C# output, `Hashtable.values::mlookupInv` was implemented in a hand-written partial:

```csharp
namespace Quire.Hashtable;

public partial class values
{
    public static object mlookupInv(object arg0, object arg1)
    {
        return hlookup(((Map)arg0).Field2, arg1);
    }
}
```

Disassembly comparison at opcode level:

### Mathcad Prime 12.0.1.0 (`hashtable.dll`) Release IL:
```il
.method public static object mlookupInv (
    object '',
    object ''
) cil managed 
{
    // Header size: 12 (fat header)
    // Code size: 19 (0x13)
    .maxstack 48
    .locals init (
        [0] object,
        [1] class Hashtable.Map
    )

    IL_0000: ldarg.0
    IL_0001: stloc.0
    IL_0002: ldloc.0
    IL_0003: stloc.1
    IL_0004: ldloc.1
    IL_0005: ldfld object Hashtable.Map::Field2
    IL_000a: ldarg.1
    IL_000b: tail.
    IL_000d: call object Hashtable.values::hlookup(object, object)
    IL_0012: ret
}
```

### Quire Candidate (`Quire.Mpl.Modules.dll`, Roslyn C# Release):
```il
.method public hidebysig static object mlookupInv (
    object arg0,
    object arg1
) cil managed 
{
    // Header size: 1 (tiny header)
    // Code size: 18 (0x12)
    .maxstack 8

    IL_0000: ldarg.0
    IL_0001: castclass Quire.Hashtable.Map
    IL_0006: ldfld object Quire.Hashtable.Map::Field2
    IL_000b: ldarg.1
    IL_000c: call object Quire.Hashtable.values::hlookup(object, object)
    IL_0011: ret
}
```

### Opcode-Level Differences:
1. **Evaluation Stack (`.maxstack`):** Mathcad's Mpl compiler emits `.maxstack 48` with a fat method header (size 12); Roslyn C# computes minimal maxstack and emits `.maxstack 8` with a tiny header (size 1). Even empty methods like `Update()` emit `.maxstack 48` in Mathcad vs `.maxstack 8` in Roslyn.
2. **Tail Call Prefix (`tail.`):** Mathcad emits opcode `tail.` (`0xFE 0x14`) at `IL_000b` before the `call` instruction. Roslyn C# never emits `tail.` call prefixes.
3. **Pattern-Matching Dispatch vs Castclass:** Mathcad allocates `.locals init ([0] object, [1] class Hashtable.Map)` and binds through local variable slots without a CLI `castclass` opcode. Roslyn C# emits `castclass Quire.Hashtable.Map` (`0x74`).

## Status and Blockers

All members of `src/Quire.Mpl.Modules` remain parked with an exact architectural blocker:
- **Blocker:** Missing Mpl compiler toolchain (`msmpl`/`mshex` native ports, `msgen`/`msclr` C++/CLI, `mplasb`/`mplrun` C#).
- **Governing Decision (Decision D20):** In `docs/mirror/decisions.md` (Decision D20, Mpl language section), Mpl modules are emitted by Mathcad's Mpl compiler: every method has `.maxstack 48` and many calls carry `tail.`, which Roslyn never emits. C# stand-ins for Mpl modules cannot pass the Release IL gate.
- Per `docs/mirror/assembly-map.md`, these 15 assemblies hosted by `Quire.Mpl.Modules` remain deferred / out of scope until the Quire Mpl toolchain exists.

## Remaining Mismatches

- All 1,693 `MirrorStub` lines across 82 generated files remain parked on the Mpl toolchain blocker.
