# Lane B checkpoint

Updated: 2026-10-16

## Current main and merged work

Checkpoint base: Linux main `d2f8f67fc4ca0427845ba0bb51e185856b3e944b`. Lane B history is retained compactly here; older worktree results are historical.

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
| MatVal IsRange / IsIntegral accessors | Merged to main at `3dda994` (`78d636f`). Four accessors; exact IL; 18 direct comparisons. |
| MatVal asRealVector(object) | Merged to main at `dc92792` (`f0ae439`). Exact IL; 9 direct comparisons. |
| MatDim RowDimensionArray / ColDimensionArray | Merged to main at `2930f1a` (`5bd227c`). Exact IL; 8 direct comparisons. |
| FunctionUtilities but / take | Merged to main at `27c06a0` (`6e27dc2`). Exact IL; 17 direct comparisons. |
| MatDim base getters | Merged to main at `e38ab20` (`3138ad2`). Exact IL; 8 direct comparisons. |
| MatDim c_dim constructor / factory | Merged to main at `c4529f9` (`93d9ee2`). Exact IL; 8 direct comparisons. |
| MatDim r_dim constructor / factory | Merged to main at `7ee2d00` (`3c83bbf`). Exact IL; 8 direct comparisons. |
| MatDim rc_dim constructor / factory | Merged to main at `67615ea` (`a63d30e`). Exact IL; 12 direct comparisons. |
| MatDim crazy_dim constructor / factory | Merged to main at `d2f8f67` (`cb23481`). Exact IL; 12 direct comparisons. |
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

### MatVal IsRange / IsIntegral accessors (merged to main at 3dda994)

Branch `mirror/b-matval-range-integral` (`78d636f`), based on clean main `9e982ce09a19`. Verified status annotations and contracts for `get_IsRange` (0x06000464), `set_IsRange` (0x06000465), `get_IsIntegral` (0x06000466), and `set_IsIntegral` (0x06000467). Setters use compound assignments (`|=` / `&=`) matching Mathcad IL exactly; all four accessors are exact opcode matches in Release IL. 18 direct MethodComparison cases in `MatValRangeIntegralDifferentialTests`. Scoped mutation on `Quire.Mcd.MatVal` passed with 286/286 mutants killed (0 survivors). Merged via `merge_gate.py --apply`.

### MatVal asRealVector(object) (merged to main at dc92792)

Branch `mirror/b-matval-asrealvector` (`f0ae439`), based on clean main `3dda994`. Added direct differential tests, verified status annotation, and contract in [Mcd.MatVal evidence](../evidence/Mcd.MatVal.md). Exact original signature, token 0x06000468, and baseline hash `ef8374270a85abf2d28331ffad9e8c7ee7292fea727d03ddc032d37d525d19e4` (exact IL match in baseline). 9 direct MethodComparison cases in `MatValAsRealVectorDifferentialTests`. Merged via `merge_gate.py --apply`.

### MatDim RowDimensionArray / ColDimensionArray (merged to main at 2930f1a)

Branch `mirror/b-matdim-dimension-arrays` (`5bd227c`), based on clean main `dc92792`. Added direct differential tests in `MatDimDimensionArraysDifferentialTests.cs`, verified status annotations on `RowDimensionArray(int)` (token 0x06000613) and `ColDimensionArray(int)` (token 0x06000614) in `src/Quire.Mpl/Mcd/MatDim.cs`, and contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures and hashes are in that table. Both methods compile to exact opcode matches in Release IL. 8 direct MethodComparison cases (4 per method) in `MatDimDimensionArraysDifferentialTests`. Scoped mutation on `Quire.Mcd.MatDim` passed with 10/10 mutants killed (0 survivors). Merged via `merge_gate.py --apply`.

### FunctionUtilities take / but boundary cases (merged to main at 27c06a0)

Branch `mirror/b-functionutilities-take-negative` (`6e27dc2`), based on clean main `2930f1a`. Added direct differential tests in `FunctionUtilitiesButDifferentialTests.cs` and `FunctionUtilitiesTakeNegativeDifferentialTests.cs`, verified status annotations on `but<T>` (token 0x0600027a) and `take<T>` (token 0x0600027b) in `src/Quire.Mpl/Run/FunctionUtilities.cs`, and updated contracts in [Mpl.Run.FunctionUtilities evidence](../evidence/Mpl.Run.FunctionUtilities.md). Exact original signatures and tokens are in that table; both methods are exact IL matches in baseline (`exact: True`). 17 direct MethodComparison cases against the pinned artifact. Merged via `merge_gate.py --apply`.

### MatDim base getters (merged to main at e38ab20)

Branch `mirror/b-matdim-base-getters` (`3138ad2`), based on clean main `27c06a0`. Added direct differential tests in `MatDimBaseGettersDifferentialTests.cs`, verified status annotations on `get_UniformDim` (token 0x0600060e), `get_RowDimensions` (token 0x06000611), `get_ColDimensions` (token 0x06000612), and `get_IsLinear` (token 0x06000619) in `src/Quire.Mpl/Mcd/MatDim.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`ce2965e0f5bbb04ab5d1eb4e0cc84a9790f9d24a539a14fd7c2dbc5ba7a2867f`) are in that table. All four base getters return `null` (`ldnull; ret`), matching Mathcad IL exactly in Release builds. 8 direct MethodComparison cases against the pinned artifact. Merged via `merge_gate.py --apply`.

### MatDim c_dim constructor and factory (merged to main at c4529f9)

Branch `mirror/b-matdim-c-dim-constructor` (`93d9ee2`), based on clean main `e38ab20`. Added direct differential tests in `MatDimCDimConstructorDifferentialTests.cs`, verified status annotations on `c_dim..ctor(Dim[], int)` (token 0x060007d5) and `c_dim.make(Dim[], int)` (token 0x060007d6) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`3ed8e46824c37c277efd830868b9b37158da6f3f688e0bc88d50f509a4c5c4ee` and `936602215b8315df780417f85d815de32230a6876e5ad336e53ff7c72c277f1d`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. 8 direct MethodComparison cases against the pinned artifact. Merged via `merge_gate.py --apply`.

### MatDim r_dim constructor and factory (merged to main at 7ee2d00)

Branch `mirror/b-matdim-r-dim-constructor` (`3c83bbf`), based on clean main `c4529f9`. The batch adds direct differential tests in `MatDimRDimConstructorDifferentialTests.cs`, verified status annotations on `r_dim..ctor(Dim[], int)` (token 0x060007c3) and `r_dim.make(Dim[], int)` (token 0x060007c4) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`5d60a56b9ab2c338259a2f61867e4e387da38cb6ec2bd577d9177372b8720549` and `b8375eed03f58bc3c1a15dcc3dad1dd507a2224acdb8fd7523413eb1ad3faf80`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim rc_dim constructor and factory (merged to main at 67615ea)

Branch `mirror/b-matdim-rc-dim-constructor` (`a63d30e`), based on clean main `7ee2d00`. The batch adds direct differential tests in `MatDimRcDimConstructionDifferentialTests.cs`, verified status annotations on `rc_dim..ctor(Dim[], Dim[])` (token 0x060007b3) and `rc_dim.make_rc(Dim[], Dim[])` (token 0x060007b4) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`36f0aab7306cee9c64d72148666c18aa9084f1bfa0a74b4233e0a739d7294d9b` and `137790307c7b431b8b12f55af9aecd00aea49189f013cf73acb27b81411dbf0d`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

### MatDim crazy_dim constructor and factory (merged to main at d2f8f67)

Branch `mirror/b-matdim-crazy-dim-constructor` (`cb23481`), based on clean main `67615ea`. The batch adds direct differential tests in `MatDimCrazyDimConstructionDifferentialTests.cs`, verified status annotations on `crazy_dim..ctor(MatVal)` (token 0x060007ad) and `crazy_dim.make(MatVal)` (token 0x060007b2) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`7ff0599165db240939134df5fcff1eaede0e162d44cb06e96bde6851d83fb242` and `de5613a5a2ce61aa5c74eb1ca33d5ee947b3cb2f30c50bfeb8932bd417d837aa`) are in that table. Both constructor and factory match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`. Merged via `merge_gate.py --apply`.

## MatDim c_dim equality — active checkpoint

Branch `mirror/b-matdim-c-dim-equality`, based on current clean main `d2f8f67`. The batch adds direct differential tests in `MatDimCDimEqualityDifferentialTests.cs`, verified status annotations on `c_dim.eq_b(MatDim)` (token 0x060007dc) and `c_dim.eq2_b(c_dim)` (token 0x060007dd) in `src/Quire.Mpl/Mcd/MatDim.Operations.cs`, and updated contracts in [Mcd.MatDim evidence](../evidence/Mcd.MatDim.md). Exact original signatures, tokens, and hashes (`219987b242c90329d2a289fbc1c8b7278516009ad009ecbdde0f3845519961ff` and `1a882f6aa8d0535aacbb4952b2fffab6cee55f5b9d6507780ad42b3a3e9c0e80`) are in that table. Both methods match Mathcad IL exactly in Release builds. Artifact: Prime 12.0.1.0 `mplrun.dll`, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.

Direct differential tests compare equality dispatch to rhs (7 direct cases) and column factor array comparison (6 direct cases), verifying row count indifference, distinct dimension reference equality, dimension mismatch rejection, and null exception handling.

## Next Lane B queue

The following independent Lane B batches are next in line:
1. **MatDim specialized coordinate queries and operations** (`rc_dim`, `uniform_dim`, `crazy_dim` constructors, equality, multiplication, transforms).
2. **MatVal flag and classifier members** (`asRealMatrix`, `assertRealVector`, `assertRealMatrix`, real classifiers).
3. **Function / DelegateFunction** (`Function.closure`; `Function.appVec`; `DelegateFunction.inv`).
4. **Engine projects** (interface and factory members, subject to Linux/Windows capability).
