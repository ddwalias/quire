# Lane C — Automation, Calculation.Core, and Media3D Queue

- Scope: exclusive ownership of assigned assemblies in order:
  1. `Quire.Automation` — `Ptc.MathcadPrime.Automation.dll`
  2. `Quire.EngNotebook.Automation` — `Ptc.EngNotebook.Automation.dll`
  3. `Quire.EngNotebookPlus.Automation` — `Ptc.EngNotebookPlus.Automation.dll`
  4. `Quire.Calculation.Core` — `Ptc.Calculation.Core.dll`
  5. `Quire.Media3D` — `Ptc.Media3D.dll`
- Status: in progress. Quire.Automation Batch 1 landed.
- Worktree: `C:/Projects/Quire-wt/lane-c-automation`.
- Branch: `mirror/c-automation-20261021-c1`.
- UTC start time: 2026-10-20T18:12:36Z.
- Eight-hour deadline: 2026-10-21T02:12:36Z.
- Current assembly: `Quire.Automation`.
- Selected members:
  - `Quire.Automation.IMathcadPrimeGetValueResult` (exact declarations: ResultType, ErrorCode, Units, RealResult, StringResult, MatrixResult)
  - `Quire.Automation.IMathcadPrimeInputResult` (exact declarations: ErrorCode, RealResult, Units)
  - `Quire.Automation.IMathcadPrimeInputMatrixResult` (exact declarations: ErrorCode, MatrixResult, Units)
  - `Quire.Automation.IMathcadPrimeOutputResult` (exact declarations: ErrorCode, RealResult, Units)
  - `Quire.Automation.IMathcadPrimeOutputResultAs` (exact declarations: ErrorCode, RealResult)
  - `Quire.Automation.IMathcadPrimeOutputMatrixResult` (exact declarations: ErrorCode, MatrixResult, Units)
  - `Quire.Automation.IMathcadPrimeOutputMatrixResultAs` (exact declarations: ErrorCode, MatrixResult)
  - `Quire.Automation.IMathcadPrimeSetValueResults` (exact declarations: Count, GetResultByIndex, GetResultByAlias)
  - `Quire.Automation.GetValueResult` (exact Release IL: RealSubjectInstance, ResultType, ErrorCode, Units, RealResult, StringResult, MatrixResult, .ctor, .ctor(IMathcadPrimeGetValueResult), Finalize)
  - `Quire.Automation.InputResult` (exact Release IL: RealSubjectInstance, ErrorCode, RealResult, Units, .ctor, .ctor(IMathcadPrimeInputResult), Finalize)
  - `Quire.Automation.InputMatrixResult` (exact Release IL: RealSubjectInstance, ErrorCode, MatrixResult, Units, .ctor, .ctor(IMathcadPrimeInputMatrixResult), Finalize)
  - `Quire.Automation.OutputResult` (exact Release IL: RealSubjectInstance, ErrorCode, RealResult, Units, .ctor, .ctor(IMathcadPrimeOutputResult), Finalize)
  - `Quire.Automation.OutputResultAs` (exact Release IL: RealSubjectInstance, ErrorCode, RealResult, .ctor, .ctor(IMathcadPrimeOutputResultAs), Finalize)
  - `Quire.Automation.OutputMatrixResult` (exact Release IL: RealSubjectInstance, ErrorCode, MatrixResult, Units, .ctor, .ctor(IMathcadPrimeOutputMatrixResult), Finalize)
  - `Quire.Automation.OutputMatrixResultAs` (exact Release IL: RealSubjectInstance, ErrorCode, MatrixResult, .ctor, .ctor(IMathcadPrimeOutputMatrixResultAs), Finalize)
  - `Quire.Automation.SetValueResults` (exact Release IL: RealSubjectInstance, Count, .ctor, .ctor(IMathcadPrimeSetValueResults), Finalize, GetResultByIndex, GetResultByAlias)
- Release IL report: PASS; 95 checked; 70 exact bodies; 25 exact declarations; 327 unfinished remaining; 0 failures.
- Remaining mismatches: 327 unfinished members in Quire.Automation; COM runtime constructors without readable IL (`ApplicationCreatorClass::.ctor`) recorded as incomplete.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only assigned assemblies and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
