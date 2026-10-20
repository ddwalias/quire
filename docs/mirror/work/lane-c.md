# Lane C — Automation, Calculation.Core, and Media3D Queue

- Scope: exclusive ownership of assigned assemblies in order:
  1. `Quire.Automation` — `Ptc.MathcadPrime.Automation.dll`
  2. `Quire.EngNotebook.Automation` — `Ptc.EngNotebook.Automation.dll`
  3. `Quire.EngNotebookPlus.Automation` — `Ptc.EngNotebookPlus.Automation.dll`
  4. `Quire.Calculation.Core` — `Ptc.Calculation.Core.dll`
  5. `Quire.Media3D` — `Ptc.Media3D.dll`
- Status: in progress. Quire.Automation Batch 1 and Batch 2 landed.
- Worktree: `C:/Projects/Quire-wt/lane-c-automation`.
- Branch: `mirror/c-automation-20261021-c1`.
- UTC start time: 2026-10-20T18:12:36Z.
- Eight-hour deadline: 2026-10-21T02:12:36Z.
- Current assembly: `Quire.Automation`.
- Selected members:
  - Batch 1: Result interfaces and classes (IMathcadPrimeGetValueResult, IMathcadPrimeInputResult, IMathcadPrimeInputMatrixResult, IMathcadPrimeOutputResult, IMathcadPrimeOutputResultAs, IMathcadPrimeOutputMatrixResult, IMathcadPrimeOutputMatrixResultAs, IMathcadPrimeSetValueResults and implementations).
  - Batch 2: Support interfaces and classes:
    - `IMathcadPrimeMatrix` (exact declarations: Matrix, Rows, Columns, SetMatrixElement, GetMatrixElement)
    - `Matrix` (exact Release IL: RealSubjectInstance, Rows, Columns, .ctor(), .ctor(IMathcadPrimeMatrix), Finalize, SetMatrixElement, GetMatrixElement)
    - `IMathcadPrimeInputs` (exact declarations: Count, GetAliasByIndex)
    - `Inputs` (exact Release IL: RealSubjectInstance, Count, .ctor(), .ctor(IMathcadPrimeInputs), Finalize, GetAliasByIndex)
    - `IMathcadPrimeOutputs` (exact declarations: Count, GetAliasByIndex)
    - `Outputs` (exact Release IL: RealSubjectInstance, Count, .ctor(), .ctor(IMathcadPrimeOutputs), Finalize, GetAliasByIndex)
    - `IMathcadPrimeInputsOutputsConflicts` (exact declarations: AddGeneralWarning, AddGeneralError, AddItemWarning, AddItemError)
    - `InputsOutputsConflicts` (exact Release IL: RealSubjectInstance, .ctor(), .ctor(IMathcadPrimeInputsOutputsConflicts), Finalize, AddGeneralWarning, AddGeneralError, AddItemWarning, AddItemError)
    - `IMathcadPrimeInputsOutputsStates` (exact declarations: ScalarInputsCount, ScalarOutputsCount, GetInputScalarStateByIndex, GetOutputScalarStateByIndex, GetInputScalarStateByAlias, GetOutputScalarStateByAlias)
    - `InputsOutputsStates` (exact Release IL: RealSubjectInstance, ScalarInputsCount, ScalarOutputsCount, .ctor(), .ctor(IMathcadPrimeInputsOutputsStates), Finalize, GetInputScalarStateByIndex, GetOutputScalarStateByIndex, GetInputScalarStateByAlias, GetOutputScalarStateByAlias)
    - `IMathcadPrimeValuesSetter` (exact declarations: AddScalarValue, AddMatrixValue, AddStringValue, AddSExprValue, SetValues)
    - `ValuesSetter` (exact Release IL: RealSubjectInstance, .ctor(), .ctor(IMathcadPrimeValuesSetter), Finalize, AddScalarValue, AddMatrixValue, AddStringValue, AddSExprValue, SetValues)
    - `IMathcadPrimeWorksheets` (exact declarations: Count, Item)
    - `Worksheets` (exact Release IL: RealSubjectInstance, Count, .ctor(), .ctor(IMathcadPrimeWorksheets), Finalize, Item)
    - `IMathcadPrimeWorksheetReadonlyOptions` (exact declarations: SetOptionValue)
    - `WorksheetReadonlyOptions` (exact Release IL: RealSubjectInstance, .ctor(), .ctor(IMathcadPrimeWorksheetReadonlyOptions), Finalize, SetOptionValue)
    - `IProxyComRealSubject<T>` (exact declarations: RealSubjectInstance)
    - `IReleasableRealSubject` (exact declarations: ReleaseComEvents)
    - `IProxyComClassManagerAgent<T>` (exact declarations: IsRegistered, IsReleased, Release)
    - `ProxyComClassManagerAgent<T>` (exact Release IL: .ctor, IsRegistered getter/setter, IsReleased getter/setter, Release)
    - `ProxyComClassManager<T>` (exact Release IL: .ctor, .cctor, get_Instance, get_HasConnections, add_RefineConnections, remove_RefineConnections, Register, Unregister, Lock, Unlock, InvokeRefineConnections, IsConnectionAlive, CreateInstanceByProgId)
- Release IL report: PASS; 168 checked; 135 exact bodies; 33 exact declarations; 231 unfinished remaining; 0 failures.
- Shared tooling blocker noted: `Quire.Tools.MethodAudit` does not map the interface type prefix for explicit interface implementations (`Quire.Automation.IMathcadPrimeMatrix.get_Matrix` vs `Ptc.MathcadPrime.Automation.IMathcadPrimeMatrix.get_Matrix`), causing false `no Mathcad counterpart` when audited. Kept as stub pending maintainer tooling update.
- COM runtime constructors without readable IL (`ApplicationCreatorClass::.ctor`) remain recorded as incomplete.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only assigned assemblies and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
