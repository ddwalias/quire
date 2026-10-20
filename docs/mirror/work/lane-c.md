# Lane C — Automation, Calculation.Core, and Media3D Queue

- Scope: exclusive ownership of assigned assemblies in order:
  1. `Quire.Automation` — `Ptc.MathcadPrime.Automation.dll`
  2. `Quire.EngNotebook.Automation` — `Ptc.EngNotebook.Automation.dll`
  3. `Quire.EngNotebookPlus.Automation` — `Ptc.EngNotebookPlus.Automation.dll`
  4. `Quire.Calculation.Core` — `Ptc.Calculation.Core.dll`
  5. `Quire.Media3D` — `Ptc.Media3D.dll`
- Status: in progress. Quire.Automation Batch 1, Batch 2, and Batch 3a landed.
- Worktree: `C:/Projects/Quire-wt/lane-c-automation`.
- Branch: `mirror/c-automation-20261021-c1`.
- UTC start time: 2026-10-20T18:12:36Z.
- Eight-hour deadline: 2026-10-21T02:12:36Z.
- Current assembly: `Quire.Automation`.
- Selected members:
  - Batch 1: Result interfaces and classes (IMathcadPrimeGetValueResult, IMathcadPrimeInputResult, IMathcadPrimeInputMatrixResult, IMathcadPrimeOutputResult, IMathcadPrimeOutputResultAs, IMathcadPrimeOutputMatrixResult, IMathcadPrimeOutputMatrixResultAs, IMathcadPrimeSetValueResults and implementations).
  - Batch 2: Support interfaces and classes:
    - `IMathcadPrimeMatrix`, `Matrix`
    - `IMathcadPrimeInputs`, `Inputs`
    - `IMathcadPrimeOutputs`, `Outputs`
    - `IMathcadPrimeInputsOutputsConflicts`, `InputsOutputsConflicts`
    - `IMathcadPrimeInputsOutputsStates`, `InputsOutputsStates`
    - `IMathcadPrimeValuesSetter`, `ValuesSetter`
    - `IMathcadPrimeWorksheets`, `Worksheets`
    - `IMathcadPrimeWorksheetReadonlyOptions`, `WorksheetReadonlyOptions`
    - `IProxyComRealSubject<T>`, `IReleasableRealSubject`
    - `IProxyComClassManagerAgent<T>`, `ProxyComClassManagerAgent<T>`
    - `ProxyComClassManager<T>`
  - Batch 3a: Worksheet interfaces and proxy class:
    - `IMathcadPrimeWorksheet` (exact declarations: Name, FullName, IsReadOnly, Modified, Inputs, Outputs, SetTitle, Save, SaveAs, Synchronize, PauseCalculation, ResumeCalculation, SetRealValue, InputGetRealValue, OutputGetRealValue, OutputGetRealValueAs)
    - `IMathcadPrimeWorksheet2` (exact declarations: above + CreateMatrix, SetMatrixValue, InputGetMatrixValue, OutputGetMatrixValue, OutputGetMatrixValueAs, IsOpen, Activate, Close)
    - `IMathcadPrimeWorksheet3` (exact declarations: above + WorksheetTabIcon, WorksheetTabName, WorksheetClosingPrompt, WorksheetDisplayedFilePath, WorksheetWorkingDirectory, DefaultCalculationTimeout, GetTitle, GetWorksheetReadOnlyOptionValue, CreateValuesSetter, SetStringValue, InputGetStringValue, OutputGetStringValue, SetSExprValue, InputGetSExprValue, InputGetValue, OutputGetValue)
    - `Worksheet` (exact Release IL: RealSubjectInstance, .ctor(), .ctor(IMathcadPrimeWorksheet), .ctor(IMathcadPrimeWorksheet2), .ctor(IMathcadPrimeWorksheet3), Finalize, Name, FullName, IsReadOnly, Modified, Inputs, Outputs, WorksheetTabIcon, WorksheetTabName, WorksheetClosingPrompt, WorksheetDisplayedFilePath, WorksheetWorkingDirectory, DefaultCalculationTimeout, SetTitle, Save, SaveAs, Synchronize, PauseCalculation, ResumeCalculation, SetRealValue, InputGetRealValue, OutputGetRealValue, OutputGetRealValueAs, CreateMatrix, SetMatrixValue, InputGetMatrixValue, OutputGetMatrixValue, OutputGetMatrixValueAs, IsOpen, Activate, Close, GetTitle, GetWorksheetReadOnlyOptionValue, CreateValuesSetter, SetStringValue, InputGetStringValue, OutputGetStringValue, SetSExprValue, InputGetSExprValue, InputGetValue, OutputGetValue)
- Release IL report: PASS; 277 checked; 188 exact; 89 unfinished remaining; 0 failures.
- Shared tooling blocker noted: `Quire.Tools.MethodAudit` does not map the interface type prefix for explicit interface implementations (`Quire.Automation.IMathcadPrimeMatrix.get_Matrix` vs `Ptc.MathcadPrime.Automation.IMathcadPrimeMatrix.get_Matrix`), causing false `no Mathcad counterpart` when audited. Kept as stub pending maintainer tooling update.
- COM runtime constructors without readable IL (`ApplicationCreatorClass::.ctor`) remain recorded as incomplete.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only assigned assemblies and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
