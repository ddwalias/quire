# Lane Gen checkpoint

Updated: 2026-10-18

## Current main and merged work

Checkpoint base: Linux main `7a81f31` (pushed to origin).

## Step 1: Baseline Measurement of Tier B Stubs

Tier B inventory: **9445** total members across **17** assemblies.
- **Already implemented:** 738
- **Remaining stubs:** 8707
  - **Exact match:** 6435 (73.9% of stubs)
  - **IL mismatch:** 2009 (23.1% of stubs)
  - **Not emitted:** 263 (3.0% of stubs)

### Summary per assembly

| Assembly | Total Tier B | Implemented | Remaining Stubs | Exact Match | IL Mismatch | Not Emitted |
|---|--:|--:|--:|--:|--:|--:|
| Quire.Automation.Native | 66 | 0 | 66 | 46 | 19 | 1 |
| Quire.Calculation | 46 | 0 | 46 | 35 | 10 | 1 |
| Quire.Calculation.Core | 241 | 0 | 241 | 189 | 47 | 5 |
| Quire.Controls | 281 | 0 | 281 | 234 | 42 | 5 |
| Quire.Controls.CollapsibleArea | 78 | 0 | 78 | 53 | 21 | 4 |
| Quire.Controls.Core | 812 | 0 | 812 | 695 | 110 | 7 |
| Quire.Controls.EquationControl | 1618 | 181 | 1437 | 940 | 468 | 29 |
| Quire.Controls.Include | 213 | 0 | 213 | 156 | 57 | 0 |
| Quire.Controls.Plots | 1142 | 6 | 1136 | 852 | 275 | 9 |
| Quire.Controls.SolveBlock | 80 | 0 | 80 | 55 | 23 | 2 |
| Quire.Controls.Text | 158 | 6 | 152 | 111 | 40 | 1 |
| Quire.Controls.Worksheet | 832 | 4 | 828 | 454 | 356 | 18 |
| Quire.Core | 1889 | 2 | 1887 | 1709 | 168 | 10 |
| Quire.Efi | 148 | 0 | 148 | 109 | 38 | 1 |
| Quire.Engine | 17 | 0 | 17 | 16 | 1 | 0 |
| Quire.EquationEditor | 1262 | 0 | 1262 | 760 | 332 | 170 |
| Quire.Mpl | 562 | 539 | 23 | 21 | 2 | 0 |

### Breakdown per outcome and cause

| Outcome / Cause | Total | Assemblies affected | Examples |
|---|--:|---|---|
| **exact match** | 6435 | 17 assemblies | `Quire.Automation.FunctionValue::WriteValueRL`0([System.Runtime]System.IO.TextWriter,[Quire.Automation.Native]Quire.Automation.UnitAction)->[System.Runtime]System.Void|False`<br/>`Quire.Automation.FunctionValue::get_Type`0()->[System.Runtime]System.String|False`<br/>`Quire.Automation.FunctionValueProxy::get_AsFunction`0()->[Quire.Automation.Native]Quire.Automation.IFunctionValueProxy|False` |
| **IL mismatch: a callee not yet settled** | 1932 | 17 assemblies | `Quire.Automation.FunctionValue::.ctor`0([Quire.Mpl]Quire.Mpl.Run.Function,[Quire.Mpl]Quire.Mcd.RuntimeOptions)->[System.Runtime]System.Void|False`<br/>`Quire.Automation.FunctionValueProxy::.ctor`0([Quire.Mpl]Quire.Mpl.Run.Function,[System.Runtime]System.Boolean,[Quire.Mpl]Quire.Mcd.RuntimeOptions)->[System.Runtime]System.Void|False`<br/>`Quire.Automation.MatrixValueProxy+MatrixValueData::GetColumnsPlainDataValues`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[System.Collections]System.Collections.Generic.List`1<[System.Runtime]System.Collections.IList>|False` |
| **not emitted: render failure: unresolved member** | 136 | Quire.Controls.Plots, Quire.Controls.Worksheet, Quire.EquationEditor | `Quire.Controls.Plots.PlotEquationWOUnit::get_SynchronizationStatus`0()->[Quire.Calculation.Core]Quire.Calculation.Core.SynchronizationStatus|False`<br/>`Quire.Controls.Worksheet.RtfItemModel::get_Rtf`0()->[System.Runtime]System.String|False`<br/>`Quire.EquationEditor.Equation.EquationModel+b_::a3`0()->[Quire.EquationEditor]Quire.Native.PtcEquationEditor.r|False` |
| **not emitted: no Mathcad counterpart** | 83 | 12 assemblies | `Quire.Calculation.DataExchangeIValueHandler::GetValueDispatch`0([System.Runtime]System.String,[System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[Quire.Automation.Native]Quire.IValue|False`<br/>`Quire.Calculation.Core.DisplayMatrixSimpleResultNode::global::Quire.Calculation.Core.IMatrixResultNode.get_Item`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[Quire.Calculation.Core]Quire.Calculation.Core.IResultNode|False`<br/>`Quire.Calculation.Core.EagerMatrixSimpleResultNode::global::Quire.Calculation.Core.IMatrixResultNode.get_Item`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)->[Quire.Calculation.Core]Quire.Calculation.Core.IResultNode|False` |
| **IL mismatch: generic shape** | 77 | 6 assemblies | `Quire.Calculation.Core.Trace2DResults.ListResultNode`1::get_Count`0()->[System.Runtime]System.Int32|False`<br/>`Quire.Calculation.Core.Trace2DResults.ListResultNode`1::get_Item`0([System.Runtime]System.Int32)->!0|False`<br/>`Quire.Calculation.Core.Trace2DResults.ListResultNode`1::get_List`0()->[System.Runtime]System.Collections.Generic.IList`1<!0>|False` |
| **not emitted: field initializers** | 17 | 8 assemblies | `Quire.Automation.MatrixValue::.ctor`0([Quire.Mpl]Quire.Mcd.MatVal,[Quire.Mpl]Quire.Mcd.RuntimeOptions)->[System.Runtime]System.Void|False`<br/>`Quire.Controls.CollapsibleArea.CollapsibleAreaModel::.ctor`0()->[System.Runtime]System.Void|False`<br/>`Quire.Controls.InputControls.InputButtonEngineVBScript::.ctor`0()->[System.Runtime]System.Void|False` |
| **not emitted: addresses (value types)** | 9 | Quire.Core, Quire.EquationEditor | `Quire.MsiHelper::GuidToString`0([System.Runtime]System.Guid)->[System.Runtime]System.String|True`<br/>`Quire.EquationEditor.Equation.EquationModel+f_::a`0([System.Runtime]System.Nullable`1<[Quire.Controls.Core]Quire.Controls.Core.IdLabels>)->[System.Runtime]System.Boolean|False`<br/>`Quire.EquationEditor.Tree.Transform.MathNodeToRlTransformer+b_::a`0([System.Runtime]System.Collections.Generic.KeyValuePair`2<[System.Runtime]System.String,[Quire.EquationEditor]Quire.EquationEditor.Tree.Transform.MathNodeToRlTransformer+PositionsToLabel>)->[Quire.EquationEditor]Quire.EquationEditor.Tree.Transform.MathNodeToRlTransformer+PositionsToLabel|False` |
| **not emitted: ref, out or pointer parameter** | 8 | 5 assemblies | `Quire.Controls.InputControls.ListBoxModel::HandlePostInputFieldResultChanged`0([System.Runtime]System.Object&,[System.Runtime]System.Object,[System.Runtime]System.String,[System.Runtime]System.Boolean)->[System.Runtime]System.Void|False`<br/>`Quire.Controls.Worksheet.ZombieCreator::ConvertPageLocationAndClipBoundsFromItemToZombie`0([Quire.Controls.Worksheet]Quire.Controls.Worksheet.IWhiteboardItemZombie,[WindowsBase]System.Windows.Point&,[System.Runtime]System.Nullable`1<[WindowsBase]System.Windows.Rect>&)->[System.Runtime]System.Void|True`<br/>`Quire.Controls.Worksheet.ZombieCreator::ConvertPageLocationAndClipBoundsFromZombieToItem`0([Quire.Controls.Worksheet]Quire.Controls.Worksheet.IWhiteboardItemZombie,[WindowsBase]System.Windows.Point&,[System.Runtime]System.Nullable`1<[WindowsBase]System.Windows.Rect>&)->[System.Runtime]System.Void|True` |
| **not emitted: dup (compound assignment or initializer)** | 6 | Quire.Controls.Core, Quire.Controls.EquationControl, Quire.EquationEditor | `Quire.Controls.Core.WorksheetPropertiesManager+WorksheetPropertiesChangeInfo::.ctor`0([System.Runtime]System.String,[Quire.Controls.Core]Quire.Controls.Core.IWorksheetPropertiesSource,[PresentationFramework]System.Windows.FrameworkElement)->[System.Runtime]System.Void|False`<br/>`Quire.Controls.Internal.HorizontalFence::.ctor`0([System.Runtime]System.Char)->[System.Runtime]System.Void|False`<br/>`Quire.Controls.Internal.HorizontalFence::.ctor`0([System.Runtime]System.Char,[System.Runtime]System.Boolean,[System.Runtime]System.Boolean)->[System.Runtime]System.Void|False` |
| **not emitted: non-virtual call on another object** | 4 | Quire.EquationEditor | `Quire.EquationEditor.Equation.EquationModel+m_::a`0()->[System.Runtime]System.Boolean|False`<br/>`Quire.EquationEditor.Equation.EquationModel+m_::b`0()->[System.Runtime]System.Void|False`<br/>`Quire.EquationEditor.Tree.Transform.MathNodeToRlTransformer+i_::a`0()->[System.Runtime]System.String|False` |

## Step 2: Widen MemberGen (Branch `maint-gen-1`, HEAD `ede0216`)

### Deficiencies Identified and Fixed
1. **Generic type arguments omitted when emitting new partial files:** `Emitter.Files.cs` emitted `partial class Foo` without `<T1, T2>`. Fixed by appending generic type parameters to type declarations.
2. **Generic partial declaration regex:** `Declaration(top)` regex in `Emitter.Files.cs` did not match generic signatures (`(?:<[^>]+>)?\b`). Fixed.
3. **Invalid implicit global using assumptions:** `Names.cs` assumed `System.IO` and `System.Net.Http` were implicit global usings; they are not available in WPF projects. Removed from `Names.Implicit` so necessary `using` directives are emitted.
4. **Invalid namespace imports for nested types:** `Names.cs` emitted outer class names as namespaces (`using System.ComponentModel.TypeConverter;`). Fixed by stripping known outer types from `typeNs`.
5. **Generic parameter substitution:** In `Decompiler.cs`, added `Decompiler.Substitute` and updated `FieldRef.FieldType`, `MethodRef.ReturnType`, `MethodRef.ParameterType` to substitute generic parameters (`!0`, `!1`, `!!0`, `!!1`) from declaring type and method type arguments. Resolves enum constants, bool arguments in delegate invocations, and cast targets.
6. **Indexer setter assignment:** In `Renderer.cs`, added support for `Do { Value: Call { M.Name: "set_Item" } c } when c.Args.Length >= 2` to render idiomatic indexer assignments `${Indexed(c)}[{indexArgs}] = {value};` instead of invalid method calls.
7. **Enum resolution:** In `World.cs`, updated `Enum(TypeSig)` to check loaded `QuireTypes` in addition to Mathcad metadata.
8. **Open generic syntax in differential tests:** In `Emitter.Files.cs`, open generic test types emitted `typeof(global::Foo)` which violates `CS0305`. Fixed to emit `typeof(global::Foo<...>)` with generic arity commas.
9. **Windows dotnet MSBuild path translation:** In `accept.py`, translated `/mnt/c/` paths to Windows `C:/` format to avoid MSBuild `MSB1001` switch parsing errors.
10. **Windows error path normalization:** In `accept.py`, normalized backslash compiler paths to repository-relative forward-slash paths for accurate member failure attribution.
11. **Differential test project references:** Added missing Windows project references to `Quire.Differential.Tests.csproj` and scoped `Quire.Focused.Differential.Tests.csproj` to exclude non-Mpl mechanical tests.

### Verification and Impact
- **`Quire.Harness.Tests`:** 96/96 passed (100%).
- **`tools/Quire.Tools.MemberGen` Python tests:** 5/5 passed (100%).
- **`Quire.Core` emission compilation errors:**
  - **Before:** 26 compilation errors blocking all candidate emissions.
  - **After:** **0 compilation errors**; all 1,197 members emitted and compiled cleanly.

## Step 3: Quire.Core Accepted Batch (Branch `mirror/gen-core`, Commit `fe20b82`)

Accepted **1,196 Tier B members (+618 fields moved out of stubs)** with exact Release-IL match against Mathcad Prime 12.0.1.0:
- Working tree: `/mnt/c/Projects/Quire-wt/gen-core`
- Branch: `mirror/gen-core` (pushed to origin)
- Execution log and summary: `.mirror-cache/membergen/Quire.Core-20261018-214838/summary.json`
- Outcome: 1,196 kept, 1 rejected for IL mismatch (`FunctionalityServiceWrapper::FunctionalityServiceLicenseTypeChanged` uses `WeakMultiCastDelegate`).

## Step 4: Quire.Mpl Analysis and Hand-offs

An experimental run of `accept.py` over `Quire.Mpl` confirmed that all 23 remaining Tier B stubs achieve exact Release-IL matches (`exact=True`). However, differential testing fails because they depend on uninitialized singletons or unsettled callees owned by Lane A:
- **11 Keyword singletons / methods** (`case_keyword`, `declare_keyword`, `defdata_keyword`, `define_keyword`, `deftype_keyword`, `defun_keyword`, `do_keyword`, `if_keyword`, `lambda_keyword`, `let_keyword`, `try_keyword`): in Mathcad, `.cctor` initializes `singleton = new <keyword>()`. In Quire, these singletons remain `null` because `.cctor` is not yet implemented. Handed off to Lane A.
- **5 Instruction code emitters** (`Instruction.EmitCode`, `InstructionArgType.EmitCode`, `InstructionArgInt.EmitCode`, `InstructionArgField.EmitCode`, `InstructionArgCtor.EmitCode`): depend on `ILGenerator.Emit` which is currently being implemented by Lane A in `mirror/a-instruction-emission`. Handed off to Lane A.
- **2 DynamicType methods** (`DefineRecord`, `TupleNameDemangle`): depend on dynamic type demangling/records being implemented in `mirror/a-dynamic-type-demangle`. Handed off to Lane A.
- **1 Rts helper** (`Rts.GetTypeInitializer`): depends on dynamic type builder support in `mirror/a-rts-*`. Handed off to Lane A.
- **1 Destructuring bind** (`MplDestructuringBind..ctor`): depends on pattern macros in `mirror/a-scons-pattern-macro`. Handed off to Lane A.
- **3 other methods**: depend on Lane A's MPL runtime infrastructure.

## Hand-offs to Maintainer

1. **Tool Branch `maint-gen-1` (HEAD `ede0216`):**
   - Protected paths: `tools/Quire.Tools.MemberGen/*`, `tests/Quire.Differential.Tests/*.csproj`, `tests/Quire.Focused.Differential.Tests/*.csproj`.
   - Gate with `--allow-protected`.
   - All 96/96 Harness tests pass; all 5/5 MemberGen unit tests pass.
2. **Accepted Batch `mirror/gen-core` (Commit `fe20b82`):**
   - Contains 1,196 accepted Tier B members (+618 fields) in `Quire.Core` with exact Release-IL matches and differential tests.
   - Requires maintainer merge with `--allow-protected` (due to deleting stubs in `src/Quire.Core/Generated/Mirror/`).
