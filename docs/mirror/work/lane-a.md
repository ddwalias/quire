# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Original: Use the assembly map for all original assemblies consolidated into Quire.Core.
- Selected members: 346 accepted mechanical methods (+63 fields), plus 26 manually reconstructed methods in `DirectionalPair`, `NullableExtensions`, `ThicknessExtension`, three coordinate converters, `Pair<T>`, `EmbeddedResourceCopier`, two protection converters, `ProtectionDataExtension`, four Boolean/visibility converters, `NumberMatchToBooleanConverter`, `NumberToStringConverter`, `StringsMatchToBooleanConverter`, `StringFormatConverter`, and `DateTimeFormatConverter`; 12 mechanical candidates rejected by MemberGen.
- Release IL report: `.mirror-cache/release-il-1b20f0e0dfc5/release-il.json` — PASS, 2,297 checked, 0 mismatches.
- Remaining mismatches: 1,823 unfinished methods in the full Quire.Core inventory; `BooleanToLayoutTransformConverter.Convert` and `AppVariantToVisibilityConverter.Convert` remain known compiler-IL blockers; assembly completion unconfirmed.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only this assembly and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
