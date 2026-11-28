# Quire.Componentsmgd McxComponent checkpoint (componentsmgd-wt)

- Owner: Claude worker in `C:/Users/DuyDang/orca/workspaces/Quire/componentsmgd-wt`, coordinator `orca_session_id:fdd6235e-0c22-404c-a221-3b766724035d`.
- Branch `feature/componentsmgd-mcx-20261127`, from main.
- Landed members (`src/Quire.Componentsmgd/Mirrored/McxComponent.cs`): `CreateInputQueue`, `CreateOutputQueue`, `Initialize`, `McError`, `Eval`, `Stepped`, `GetAppState`, `Error`, `SetAppState`.
- Release IL: PASS, 99 checked, 0 failures (`--project Quire.Componentsmgd`).
- Blocked: `FixInputs` (MirrorBlocked). Its `Mathcad.IMatrixValue` reference resolves to `automationmgd` in Quire's build, while Mathcad's IL uses `interop.mathcad`. Needs the same cross-assembly mapping as the VSDataValue blocker.
- Resolved 2026-11-28 (D22): the Mathcad COM interop types are now embedded the way the original does it (`67c125450`, proofs `3c73959c2`; see lane-nopia-20261128.md). `FixInputs` and the `VSDataValue` stubs are plain `[MirrorStub]` now; `GetElement`/`SetElement` are exact.
- Still stubs (blocked, not in this lane): `VSDataValue` GetElement, SetElement, GetSubmatrix and the other IMatrixValue members.
- Batch is below the 20-member target because no other unblocked members remain in Quire.Componentsmgd.
