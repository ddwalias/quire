# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-persistentdata-header-footer`, based on `origin/main` `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; see [header/footer checkpoint](lane-d-header-footer.md).
- **Other pushed batches:** `mirror/d-persistentdata-areatype` (AreaType static initializer) and `mirror/d-persistentdata-oleutils` remain unmerged. Their local Release differential tests passed; each gate passed the build and focused suites, then stopped before Release differential execution at the same assembly path mismatch.
- **Current evidence:** [footer](../evidence/Ptc.PersistentData.footer.md) and [header](../evidence/Ptc.PersistentData.header.md); artifact identity, tokens and direct cases are recorded in each.
- **Current validation:** header/footer direct differential test passed, 2 passed, 0 failed, 0 skipped.
- **Blocker:** maintainer-owned `tools/validation/validate.py` looks for `Quire.Focused.Differential.Tests.dll`, but the focused project emits `Quire.Differential.Tests.dll`. Retry the pushed Lane D batches after that lookup is corrected.
