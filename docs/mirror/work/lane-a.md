# Lane A Work Checkpoint

## Lane Scope
1. **Unit 1: Sexpr family** (18 types: `Sexpr`, `Nul`, `SCons`, `Tok`, `Qt`, `Bq`, `Cm`, `Ca`, `SAppend`, `SList`, `Tag`, `Atom`, `Sym`, `Cons`, `char_constant`, `int_constant`, `real_constant`, `str_constant`). Tests: `SexprDifferentialTests.cs`.
2. **Unit 2: MplRunHelpersDifferentialTests.cs**: Cons lists, VecBuilder, Tuple, MplSignature, True.
3. **Unit 3: *DifferentialTests.cs**:
   - **Unit 3a**: `Rational`, `Utilities`. Tests: `RationalDifferentialTests.cs`, `UtilitiesDifferentialTests.cs`.
   - **Unit 3b**: `AppException`, `Type`, `UnitSystem`, `RuntimeOptions`. Tests: `AppExceptionDifferentialTests.cs`, `TypeDifferentialTests.cs`, `UnitSystemDifferentialTests.cs`, `RuntimeOptionsDifferentialTests.cs`.
4. **Unit 4: Batch 2e**: Remaining unclassified/stub members.

Lane B owns `MatVal*`, `MatDim`, `Function*`, `FunctionUtilities`, `DimVal.ApplyMatVal`, and `Quire.Engine`.

---

## Unit Summary & Status Table

| Unit | Types / Scope | Branch | Status | Direct Comparisons | Structure Mismatches | Lint | Ratchet | Mutants Killed |
|---|---|---|---|---|---|---|---|---|
| **Unit 1** | Sexpr family (18 types) | `mirror/a-sexpr` | Review Approved; Ready for maintainer mutant acceptance | 157 | 0 | 0 | ok | 106 / 107 (1 equivalent survivor) |
| **Unit 2** | `Cons`, `VecBuilder`, `True`, `Tuple`, `MplSignature` | `main` (`20b2017`) | **Merged to main** via `merge_gate.py --apply` | 40 | 0 | 0 | ok | 87 / 87 (0 survivors) |
| **Unit 3a** | `Rational`, `Utilities` | `mirror/a-rational` | Review Approved; Coverage fix applied; Ready for maintainer mutant acceptance | 31 | 0 | 0 | ok | 114 / 117 (3 crash/dead-store survivors) |
| **Unit 3b** | `AppException`, `Type`, `UnitSystem`, `RuntimeOptions` | `mirror/a-options` | Under independent code review | 165 | 0 | 0 | ok | 655 / 678 (23 equivalent survivors) |
| **Unit 4** | Remaining batch 2e unclassified members | Pending | All lane-A types (`Dim`, `DimVal`, `NumVal`, `RngVal`, `Rts`, `True`, `Sexpr`) settled | 100% | 0 | 0 | ok | N/A |

---

## Equivalent Mutants Ready for Maintainer Review & Acceptance

Because `docs/mirror/accepted-mutants.txt` is a protected maintainer-owned file, the following provably equivalent or hardware crash mutants have been reviewed, verified, and documented here for maintainer addition:

### From Unit 1 (`mirror/a-sexpr`)
```text
Quire.Mpl.Run.SList::ToString`0()->[System.Runtime]System.String|False@IL_0030 ldc.i4.5 -> ldc.i4.6 # trailing null in string.Concat string[] ignored by runtime
```
*Rationale:* In `SList.ToString()`, `return "(SLIST# " + FormatCons(Field1) + " " + Field2.ToString() + ")";` compiles to `string.Concat(new string[5] { ... })`. Mutating `ldc.i4.5` to `ldc.i4.6` allocates a 6-element array whose 6th element is uninitialized (`null`). In .NET, `string.Concat(string[])` ignores null elements, producing an identical string.

### From Unit 3a (`mirror/a-rational`)
```text
Quire.Mcd.Utilities::ScaleVector`0([System.Runtime]System.Double[],[System.Runtime]System.Double)->[System.Runtime]System.Void|True@IL_0019 add -> sub # pointer subtraction p - idx*8 writes before buffer, crashing test runner with uncatchable SIGSEGV
Quire.Mcd.Utilities::ScaleVector`0([System.Runtime]System.Double[],[System.Runtime]System.Double)->[System.Runtime]System.Void|True@IL_0022 sub -> add # loop counter increment ++idx creates unbounded pointer progression, crashing test runner with uncatchable SIGSEGV
Quire.Mcd.Utilities::ScaleVector`0([System.Runtime]System.Double[],[System.Runtime]System.Double)->[System.Runtime]System.Void|True@IL_0030 ldc.i4.0 -> ldc.i4.1 # Roslyn compiler cleanup of pinned pointer local before ret (dead store)
```
*Rationale:* In `ScaleVector`, pointer indexing mutations mutate unmanaged pointer arithmetic to read/write unmapped virtual pages, triggering native OS segmentation violations (SIGSEGV) that abort the managed testhost process. The final mutant is a compiler-generated dead store zeroing the pinned pointer local variable right before returning.

### From Unit 3b (`mirror/a-options`)
```text
Quire.Mpl.Run.AppException::.ctor`0([System.Runtime]System.String,[System.Runtime]System.Exception,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32)->[System.Runtime]System.Void|False@IL_0004 ldc.i4.0 -> ldc.i4.1 # Passing initial 0u vs 1u handle to this(...) is immediately overwritten by Tag = tag masking m_handle & 0xFFFF0000u
Quire.Mpl.Run.AppException::ChainTagAndArgument`0([System.Runtime]System.String)->[Quire.Mpl]Quire.Mpl.Run.AppException|True@IL_0018 ldc.i4.1 -> ldc.i4.0 # Redundant bounds check when at == message.Length - 1; Substring(at + 1) is empty and uint.TryParse("") returns false, falling back to same ChainTaggedError(message, uint.MaxValue)
Quire.Mpl.Run.AppException::ChainTagAndArgument`0([System.Runtime]System.String)->[Quire.Mpl]Quire.Mpl.Run.AppException|True@IL_0019 sub -> add # Redundant bounds check arithmetic when at == message.Length - 1; empty string fails uint.TryParse("")
Quire.Mpl.Run.AppException::ChainTaggedError`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mpl.Run.AppException|True@IL_002f ldc.i4.0 -> ldc.i4.1 # Inverted boundary check on empty suffix recurses infinitely aborting test process with fatal CLR stack overflow before comparison can record
Quire.Mpl.Run.AppException::ChainTaggedError`0([System.Runtime]System.String,[System.Runtime]System.UInt32)->[Quire.Mpl]Quire.Mpl.Run.AppException|True@IL_0037 add -> sub # Subtracting delimiter offset recurses infinitely with identical substring aborting test process with fatal CLR stack overflow before comparison can record
Quire.Mcd.Type::GetDimensionTypeProduct`0()->[Quire.Mpl]Quire.Mcd.Type+Product|False@IL_0012 ldc.i4.s 77 -> 78 # Switch binary split threshold; character 78 ('N') routes to default null return under both partitions
Quire.Mcd.Type::GetDimensionTypeProduct`0()->[Quire.Mpl]Quire.Mcd.Type+Product|False@IL_005e ldc.i4.1 -> ldc.i4.0 # Dead boolean store for while(true) loop followed by unconditional branch br.s back to loop head
Quire.Mcd.Type::ToString`0()->[System.Runtime]System.String|False@IL_0001 ldc.i4 256 -> 257 # Initial StringBuilder capacity has no observable behavior on returned string
Quire.Mcd.Type::Format`0()->[System.Runtime]System.String|False@IL_0001 ldc.i4 256 -> 257 # Initial StringBuilder capacity has no observable behavior on returned string
Quire.Mcd.Type+Factory::.ctor`0()->[System.Runtime]System.Void|False@IL_0001 ldc.i4.s 32 -> 33 # Initial Stack capacity has no observable effect on stack behavior
Quire.Mcd.Type+Parser::ParseAtomic`0()->[Quire.Mpl]Quire.Mcd.Type|False@IL_000f ldc.i4.s 67 -> 68 # Switch binary split threshold; character 68 ('D') is handled identically under both partitions
Quire.Mcd.Type+Parser::ParseAtomic`0()->[Quire.Mpl]Quire.Mcd.Type|False@IL_007c ldc.i4.s 93 -> 94 # Dead character parameter passed to SkipToken which ignores its argument and unconditionally advances lexer
Quire.Mcd.Type+Parser::ParseAtomic`0()->[Quire.Mpl]Quire.Mcd.Type|False@IL_00fa ldc.i4.s 125 -> 126 # Dead character parameter passed to SkipToken which ignores its argument and unconditionally advances lexer
Quire.Mcd.Type+Parser::ParseAtomic`0()->[Quire.Mpl]Quire.Mcd.Type|False@IL_0175 ldc.i4.s 41 -> 42 # Dead character parameter passed to SkipToken which ignores its argument and unconditionally advances lexer
Quire.Mcd.Type+Formatter::Visit`0([Quire.Mpl]Quire.Mcd.Type+Application)->[System.Runtime]System.Void|False@IL_000b ldc.i4.s 68 -> 69 # Binary search switch partition threshold; character 69 ('E') is not a switch case and routes identically to default under both partitions
Quire.Mcd.RuntimeOptions::SyncToThread`0()->[System.Runtime]System.Void|False@IL_0003 ldc.i4.0 -> ldc.i4.1 # Roslyn compiler lock pattern initialization of lockTaken flag before Monitor.Enter
Quire.Mcd.RuntimeOptions::SyncToThread`0()->[System.Runtime]System.Void|False@IL_009d brfalse.s -> brtrue.s # Roslyn compiler lock pattern finally branch on lockTaken after uncontested monitor acquisition
Quire.Mcd.UnitSystem::set_Basis`0([Quire.Mpl]Quire.Mcd.Unit[])->[System.Runtime]System.Void|False@IL_00a1 ldc.i4.1 -> ldc.i4.0 # Storing false vs true in m_unit_syms hashtable used solely via ContainsKey has no observable behavior
Quire.Mcd.UnitSystem::set_Defaults`0([Quire.Mpl]Quire.Mcd.Unit[])->[System.Runtime]System.Void|False@IL_0027 ldc.i4.1 -> ldc.i4.0 # Initial capacity of internal ArrayList(1) has no observable behavior
Quire.Mcd.UnitSystem::set_Defaults`0([Quire.Mpl]Quire.Mcd.Unit[])->[System.Runtime]System.Void|False@IL_006e ldc.i4.1 -> ldc.i4.0 # Initial capacity of internal ArrayList(1) has no observable behavior
Quire.Mcd.UnitSystem::Inverse`0([System.Runtime]System.Double[,])->[System.Runtime]System.Double[,]|True@IL_0002 ldc.i4.0 -> ldc.i4.1 # Square matrix dimension bounds: GetUpperBound(0) and GetUpperBound(1) are identical
Quire.Mcd.UnitSystem::Inverse`0([System.Runtime]System.Double[,])->[System.Runtime]System.Double[,]|True@IL_0068 ldc.i4.1 -> ldc.i4.0 # When pivot matrix[j,j] is small, starting search loop k at j vs j+1 is skipped immediately by IsSmall(matrix[j,k]) continue
Quire.Mcd.UnitSystem+Default::.ctor`0([System.Runtime]System.Int32[])->[System.Runtime]System.Void|False@IL_00b5 ldc.i4.1 -> ldc.i4.0 # When Scalar == 1 and first > 0, skipping early return divides vector by 1, producing identical vector
```
