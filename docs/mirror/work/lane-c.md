# Lane C — Automation, Calculation.Core, and Media3D Queue

- Scope: exclusive ownership of assigned assemblies in order:
  1. `Quire.Automation` — `Ptc.MathcadPrime.Automation.dll`
  2. `Quire.EngNotebook.Automation` — `Ptc.EngNotebook.Automation.dll`
  3. `Quire.EngNotebookPlus.Automation` — `Ptc.EngNotebookPlus.Automation.dll`
  4. `Quire.Calculation.Core` — `Ptc.Calculation.Core.dll`
  5. `Quire.Media3D` — `Ptc.Media3D.dll`
- Status: in progress. Quire.Automation actionable members completed.
- Worktree: `C:/Projects/Quire-wt/lane-c-automation`.
- Branch: `mirror/c-automation-20261021-c1`.
- UTC start time: 2026-10-20T18:12:36Z.
- Eight-hour deadline: 2026-10-21T02:12:36Z.
- Current assembly: `Quire.Automation` (completed, transitioning to `Quire.EngNotebook.Automation`).
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
    - `IMathcadPrimeWorksheet`, `IMathcadPrimeWorksheet2`, `IMathcadPrimeWorksheet3`, `Worksheet`
  - Batch 3b: Application and Events interfaces and proxy classes:
    - `IMathcadPrimeApplication` (exact declarations: Visible, ActiveWorksheet, Activate, Quit, Open, InitializeEvents)
    - `IMathcadPrimeApplication2` (exact declarations: above + Worksheets, CloseAll, GetVersion)
    - `IMathcadPrimeApplication3` (exact declarations: above + InitializeEvents2, SubscribeEvent, UnsubscribeEvent, CreateWorksheetReadonlyOptions, OpenEx)
    - `IMathcadPrimeEvents` (exact declarations: OnSelect, OnSave, OnExit)
    - `IMathcadPrimeEvents2` (exact declarations: OnWorksheetSaved, OnWorksheetClosed, OnWorksheetModified, OnWorksheetRenamed, OnWorksheetInputsOutputsSelected, OnExit, OnWorksheetStatesGenerated, OnWorksheetStatesGenerating, OnWorksheetRequestToUpdateInputs)
    - `Application` (exact Release IL: RealSubjectInstance, .ctor(), .ctor(IMathcadPrimeApplication3), Finalize, Initialize, Release, ReleaseRealSubject, OnRefineConnections, ReleaseAll, Visible getter/setter, ActiveWorksheet, Worksheets, Activate, Quit, Open, InitializeEvents, CloseAll, GetVersion, InitializeEvents2, SubscribeEvent, UnsubscribeEvent, CreateWorksheetReadonlyOptions, OpenEx)
    - `ApplicationObsolete` (exact Release IL: RealSubjectInstance, .ctor(), .ctor(IMathcadPrimeApplication2), Finalize, Initialize, Release, ReleaseRealSubject, OnRefineConnections, ReleaseAll, Visible getter/setter, ActiveWorksheet, Worksheets, Activate, Quit, Open, InitializeEvents, CloseAll, GetVersion)
    - `ProductInfo.Constants` (exact Release IL: .ctor())
- Release IL report: PASS; 275 checked; 231 exact bodies/declarations; 2 unfinished remaining; 0 failures.
- Unfinished members:
  1. `Automation.Matrix.g.cs`: explicit interface property `IMathcadPrimeMatrix.Matrix` blocked by shared tool `Quire.Tools.MethodAudit` type prefix bug.
  2. `Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
- Next assembly: `Quire.EngNotebook.Automation` (`Ptc.EngNotebook.Automation.dll`).

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only assigned assemblies and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
