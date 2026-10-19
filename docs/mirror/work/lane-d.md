# Lane D — Ptc.PersistentData* in Ptc.Core

## Active handoff

- **Current batch:** `mirror/d-persistentdata-header-footer`, created from `f863245e87b35d0e03a9fcf26f6a18949532ccaf`; its latest gate rebased from `97694a2d`. See [header/footer checkpoint](lane-d-header-footer.md).
- **Other pushed batches:** worksheet/worksheet serializer and header/footer pass focused and Release direct comparisons, but their phase gates hit unrelated Differential.Tests failures and test-generated `bin` files. AreaType, OleUtils, and ProtectionData timestamp have local Release evidence and await gate retry.
- **Current evidence:** [footer](../evidence/Ptc.PersistentData.footer.md) and [header](../evidence/Ptc.PersistentData.header.md); artifact identity, tokens and direct cases are recorded in each.
- **Current validation:** corrected header/footer direct Release differential test passed, 2 passed, 0 failed, 0 skipped.
- **Blocker:** the phase profile reports unrelated Differential.Tests failures and test-generated `bin` files after the focused and Release gates pass.
