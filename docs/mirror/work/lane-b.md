# Lane B checkpoint

Updated: 2026-10-16

## Current main and merged work

Checkpoint base: Linux main `42c7a097037b2b05235ae22515beeb9cb96bc0ea`. Lane B history is retained compactly here; older worktree results are historical.

| Batch | Main status |
|---|---|
| Function / FunctionUtilities, function matrix bridge and follow-up | Merged after passing runtime gates; Function evidence refresh recorded at `26e402e`, follow-up at `8da5693`. |
| Engine interfaces | Merged; focused gate passed. |
| MatVal collection/growth and views/export | Merged after passing runtime gates at `e2532e3` and `459eb23`. Evidence records nine view/export structure differences. |
| MatVal pointer properties | Merged to main at `59df412` (`e68e5ac`). Five pointer/uniformity properties; 40 direct comparisons. |
| MatVal dimension properties | Merged to main at `e36fe93` (`f51e281`). `IsDimless`, `IsUniformDimOf`, `PhysicalDimension`, `UniformDimension`; 31 direct comparisons. |
| MatVal pointer helper remediation | Merged to main at `d1f7449` (`5674094`). Four pointer helpers; 51 mutants killed. |
| MatVal CanonicalizePtrs | Merged to main at `5026ad8` (`b3a12ce`). 18 direct comparisons. |
| MatVal ReinsertPtrs / ReinsertPtrs1 | Merged to main at `e4a2af9` (`caad393`). 23 direct comparisons. |
| MatVal contains(object) | Merged to main at `278aa9b` (`fb34932`). 15 direct comparisons. |
| DimVal.ApplyMatVal | Merged to main at `dffea4d` via `mirror/b-dimval-apply-current` (`a00b684`). 5 direct comparisons; stub removed. |
| MatVal ColDimArray / RowDimArray | Merged to main at `f447eed` (`339fcad`). 22 direct comparisons. |
| MatDim/MatVal dimension/index | Implementations are present on main; direct tests and evidence recorded across merged batches. |

The earlier report about missing `Mcd.NumVal.Compare` is not a blocker: the manifest and original member inventory have no such member. See [NumVal evidence](../evidence/Mcd.NumVal.md).

## Historical MatDim/MatVal feature branch

Historical feature branch: `mirror/b-matval-gate-feature` (`0028f2c`). The subsequent full runtime gate record at `/tmp/quire-matval-final-runtime-gate/gate.json` was interrupted at 1,180/2,112 mutants. Work from this branch was broken down into coherent, individually gated batches (pointer properties, dimension properties, pointer helpers, canonicalize/reinsert pointers, contains, and dimension arrays), which have all been merged into main.

## DimVal batch status

The DimVal proposals (47 unreachable lines and 5 dead-constant mutants) and 22 equivalent mutants were approved and merged to main at `26aceb5` and `275b377`. `DimVal.ApplyMatVal` was implemented, differentially verified (5 direct cases), and merged to main at `dffea4d` (`a00b684`).

## Merged MatVal and DimVal batches

### MatVal pointer properties (merged to main at 59df412)

Base: `6bfaea5f1646e45f81330beebaaa625d3ac220ac`; branch `mirror/b-matval-pointer-properties` (`e68e5ac`).
Original identity: mplrun Prime 12.0.1.0 SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
Exact direct member tokens recorded in `docs/mirror/evidence/Mcd.MatVal.md`; five test facts compare 40 cases (HasPtrs 9, RepHasPtrs 4, IsUniformScalar 8, IsUniformScalarPtrs 9, IsUniform 10), including return/exception, storage/table/pointer identity and mutation states.
Tests passed (8 Quire.Mpl.Tests, 5 Differential facts / 40 direct comparisons). Merged via `merge_gate.py --apply`.

### MatVal dimension properties (merged to main at e36fe93)

Branch `mirror/b-matval-dimension-properties` (`f51e281`).
Covers four dimension queries: `get_IsDimless`, `IsUniformDimOf`, `get_PhysicalDimension`, and `get_UniformDimension`.
Four direct-comparison facts cover 31 finite cases (IsDimless 7, IsUniformDimOf 10, PhysicalDimension 6, UniformDimension 8).
Tests passed (14 Quire.Mpl.Tests, 4 Differential facts / 31 direct comparisons). Merged via `merge_gate.py --apply`.

### MatVal pointer helper remediation (merged to main at d1f7449)

Branch `mirror/b-matval-pointer-helper-remediation` (`5674094`).
Exact tokens: `Canonicalize()` 0x0600043b, `TidyPtrs(int,int,Dim,bool)` 0x06000442, `IsUniformPtrs()` 0x06000458, and `IsUniformPtrsNoChange()` 0x06000459. Contracts recorded in `docs/mirror/evidence/Mcd.MatVal.md`.
Direct suite has 46 comparisons across four facts (Canonicalize 10, TidyPtrs 15, IsUniformPtrs 15, IsUniformPtrsNoChange 6). Scoped mutation killed 230/230 mutants (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal CanonicalizePtrs (merged to main at 5026ad8)

Branch `mirror/b-matval-canonicalize-ptrs` (`b3a12ce`).
Directly compares private `CanonicalizePtrs()`, token `0x06000441`. Contract recorded in `docs/mirror/evidence/Mcd.MatVal.md`.
18 direct cases cover dimension normalization/order, logical nonpointer versus pointer-only dimensions, stale/live pointers, matching/incompatible/zero dimensions, non-NumVal pointers, full/partial coverage, both reinsertion paths, shared padded storage, and exceptions.
Scoped mutation killed 179/179 mutants (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal ReinsertPtrs / ReinsertPtrs1 (merged to main at e4a2af9)

Branch `mirror/b-matval-reinsert-ptrs` (`caad393`).
Exact members: `ReinsertPtrs()` token `0x06000443` and `ReinsertPtrs1()` token `0x06000444`. Contracts in `docs/mirror/evidence/Mcd.MatVal.md`.
23 direct cases (11 for `ReinsertPtrs`, 12 for `ReinsertPtrs1`). Scoped mutation killed 179/179 mutants (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal contains(object) (merged to main at 278aa9b)

Branch `mirror/b-matval-contains` (`fb34932`).
Exact token: `Mcd.MatVal.contains(object) -> bool`, token `0x0600051a`. Contract in `docs/mirror/evidence/Mcd.MatVal.md`.
15 finite cases cover null/empty tables, null query/value, same and equal-but-distinct references, arbitrary keys, stale values, unrelated live pointer slots, nested present/absent values, and unchanged identities. Merged via `merge_gate.py --apply`.

### DimVal.ApplyMatVal (merged to main at dffea4d)

Branch `mirror/b-dimval-apply-current` (`a00b684`).
Public static `DimVal.ApplyMatVal(DimVal fv, DimVal mv)` (`0x060006b0`). Contract in `docs/mirror/evidence/Mcd.DimVal.md`.
Generated `[MirrorStub]` removed. Five direct `MethodComparison` cases in `DimValApplyMatValDifferentialTests`. Merged via `merge_gate.py --apply`.

### MatVal ColDimArray / RowDimArray (merged to main at f447eed)

Branch `mirror/b-matval-dimension-arrays` (`339fcad`).
Exact signatures and tokens: `ColDimArray(int) -> Dim[]` `0x06000438` and `RowDimArray(int) -> Dim[]` `0x06000439`. Contracts in `docs/mirror/evidence/Mcd.MatVal.md`.
22 direct comparisons across two facts (11 per method) in `MatValDimensionArraysDifferentialTests`. Merged via `merge_gate.py --apply`.

## Next Lane B queue

The following independent Lane B batches are next in line:
1. **MatDim specialized coordinate queries and operations** (`c_dim`, `r_dim`, `rc_dim`, `uniform_dim`, `crazy_dim` constructors, equality, multiplication, transforms, and dimension array accessors).
2. **MatVal flag and classifier members** (`get/set_IsRange`, `get/set_IsIntegral` tokens 0x06000464–0x06000467; `asRealVector` token 0x06000468; real classifiers).
3. **Function / FunctionUtilities** (`FunctionUtilities.take`/`but` boundary cases; `Function.closure`; `Function.appVec`; `DelegateFunction.inv`).
4. **Engine projects** (interface and factory members, subject to Linux/Windows capability).
