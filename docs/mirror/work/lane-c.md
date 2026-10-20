# Lane C — Automation, Calculation.Core, and Media3D Queue

- Scope: exclusive ownership of assigned assemblies in order:
  1. `Quire.Automation` — `Ptc.MathcadPrime.Automation.dll`
  2. `Quire.EngNotebook.Automation` — `Ptc.EngNotebook.Automation.dll`
  3. `Quire.EngNotebookPlus.Automation` — `Ptc.EngNotebookPlus.Automation.dll`
  4. `Quire.Calculation.Core` — `Ptc.Calculation.Core.dll`
  5. `Quire.Media3D` — `Ptc.Media3D.dll`
- Status: in progress. Quire.Automation, Quire.EngNotebook.Automation, and Quire.EngNotebookPlus.Automation actionable members completed.
- Worktree: `C:/Projects/Quire-wt/lane-c-automation`.
- Branch: mirror/c-calc-core-20261021-c2.
- UTC start time: 2026-10-20T18:12:36Z.
- Eight-hour deadline: 2026-10-21T02:12:36Z.
- Current assembly: Quire.Calculation.Core (in progress).
- Selected members:
  - Quire.Automation:
    - Batch 1: Result interfaces and classes (IMathcadPrimeGetValueResult, IMathcadPrimeInputResult, IMathcadPrimeInputMatrixResult, IMathcadPrimeOutputResult, IMathcadPrimeOutputResultAs, IMathcadPrimeOutputMatrixResult, IMathcadPrimeOutputMatrixResultAs, IMathcadPrimeSetValueResults and implementations).
    - Batch 2: Support interfaces and classes (Matrix, Inputs, Outputs, InputsOutputsConflicts, InputsOutputsStates, ValuesSetter, Worksheets, WorksheetReadonlyOptions, IProxyComRealSubject, IReleasableRealSubject, IProxyComClassManagerAgent, ProxyComClassManagerAgent, ProxyComClassManager).
    - Batch 3a: Worksheet interfaces and proxy class (IMathcadPrimeWorksheet, IMathcadPrimeWorksheet2, IMathcadPrimeWorksheet3, Worksheet).
    - Batch 3b: Application and Events interfaces and proxy classes (IMathcadPrimeApplication, IMathcadPrimeApplication2, IMathcadPrimeApplication3, IMathcadPrimeEvents, IMathcadPrimeEvents2, Application, ApplicationObsolete, ProductInfo.Constants).
  - Quire.EngNotebook.Automation:
    - Batch 1: Complete assembly implementation across 48 source files (all Result types, Support types, Worksheet types including SaveAndPush, Application types including UpdateInputs and SetContextData, Events including Ping, and Constants).
  - Quire.EngNotebookPlus.Automation:
    - Batch 1: Complete assembly implementation across 48 source files (all Result types, Support types, Worksheet types including SaveAndPush, Application types including UpdateInputs, Events including Ping, and Constants).
  - Quire.Calculation.Core:
    - Batch 1: All interface declarations cleaned of default stub bodies (ICalculatable, ICalculatableContainer, ICalculatableContext, IResultManager, IResultNode, IEngineError, etc.).
    - Batch 2: SimpleResultNode implementations (RealSimpleResultNode, ComplexSimpleResultNode, ImaginarySimpleResultNode, UnitedValueSimpleResultNode, DisplayMatrixSimpleResultNode, LazyMatrixSimpleResultNode, EagerMatrixSimpleResultNode).
- Release IL report:
  - Quire.Automation: PASS; 275 checked; 231 exact bodies/declarations; 2 unfinished remaining; 0 failures.
  - Quire.EngNotebook.Automation: PASS; 409 checked; 215 exact bodies/declarations; 2 unfinished remaining; 0 failures.
  - Quire.EngNotebookPlus.Automation: PASS; 407 checked; 214 exact bodies/declarations; 2 unfinished remaining; 0 failures.
  - Quire.Calculation.Core: PASS; 341 checked; 0 failures; exact bodies rose from 324 to 341 (+17 exact bodies); unfinished reduced from 455 to 438 (-17 unfinished).
- Unfinished members:
  - Quire.Automation:
    1. `Automation.Matrix.g.cs`: explicit interface property `IMathcadPrimeMatrix.Matrix` blocked by shared tool `Quire.Tools.MethodAudit` type prefix bug.
    2. `Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
  - Quire.EngNotebook.Automation:
    1. `EngNotebook.Automation.Matrix.g.cs`: explicit interface property `IMathcadPrimeMatrix.Matrix` blocked by shared tool `Quire.Tools.MethodAudit` type prefix bug.
    2. `EngNotebook.Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
  - Quire.EngNotebookPlus.Automation:
    1. `EngNotebookPlus.Automation.Matrix.g.cs`: explicit interface property `IMathcadPrimeMatrix.Matrix` blocked by shared tool `Quire.Tools.MethodAudit` type prefix bug.
    2. `EngNotebookPlus.Automation.ApplicationCreatorClass.g.cs`: `.ctor` COM runtime constructor without readable IL (`0x06000002 no body`).
- Next assembly: `Quire.Calculation.Core` (`Ptc.Calculation.Core.dll`).

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only assigned assemblies and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
