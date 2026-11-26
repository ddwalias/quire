# NRtfTree compatibility fork checkpoint

- Owner/branch: luna-nrtf-fork / `feature/luna-nrtf-fork`.
- Scope: vendored NRtfTree 0.4.0 source, three Mathcad API patches, and project-reference wiring for Controls.Core and Controls.Worksheet.
- Release IL: [full-library report](../../../.mirror-cache/nrtftree-library/cfc3cd6e9ded2458/release-il-report.txt) — 357 methods, zero missing, extra, or body mismatches.
- Controls.Core/Worksheet gate: 5,232 exact; blocked by the existing generated parameterless `RtfItemModel` constructor, which has no Mathcad counterpart (Mathcad's constructor takes six parameters). The RTF implementation is owned by the parallel luna-rtf worker.
- Remaining: rerun the project gate and land once the RTF constructor/member batch is integrated; no RTF members were changed here.
