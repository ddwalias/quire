# Agent workflow

The goal is exact **Release IL** parity with Mathcad Prime 12.0.1.0. Read
[the plan](docs/mirror/PLAN.md) and use [WORK](docs/mirror/WORK.md) for ownership.
The [assembly map](docs/mirror/assembly-map.md) shows dependency order and what each completed assembly unlocks.

Only four lane assignments exist: A owns `Quire.Core`, B `Quire.Mpl.Emit`, C `Quire.Utils`,
and D `Quire.Engine.Utl`. All former lane reservations are released. These assignments
authorize every managed member in those assemblies regardless of historical tier (D18).
Use the assigned worktrees and edit only your own assembly and checkpoint; the maintainer
handles shared tools and integration. Continue through passing batches autonomously.

The user starts each agent in a separate Codex/AGY/Claude thread. Do not spawn agents
or create agent threads. Each user-started agent creates its own worktree from current
`main`; `C:/Projects/Quire` is the shared main checkout and must stay on `main`.

1. Claim unowned members in a separate branch/worktree.
2. Read their original Mathcad IL (`python tools/reference/reference.py <type> --il`).
3. Implement the same members using the project's name mappings.
4. Build Release and compare IL (`python tools/validation/release_il.py --base main --project <project>`).
5. Commit and land using `python tools/validation/lane_merge.py <branch>`.

Release is the default for every IL operation. Never inspect or compare Debug IL.
Changed methods must match; previously exact methods in affected projects must remain exact.
Missing/unreadable counterparts and structure-only matches are incomplete.

Never edit `src/*/Generated/`: MirrorGen rewrites those files. Implement members in
hand-written partials (`src/<Project>/Mirrored/<Type>.cs` or the type's own file); a
hand-written member without `[MirrorStub]` replaces the generated stub. Remove the stub from
the `.g.cs` in the same change, exactly as regeneration would. When a closure or iterator
needs Mathcad's member ordinal while other members are unfinished, declare those members as
unmarked placeholder stubs (`=> throw global::Quire.Mirror.MirrorStub.NotImplemented();`) in
Mathcad order in the hand-written file ([validation](docs/mirror/validation.md)).

Write idiomatic C#: the source the original author wrote, not a transcription of the IL or
ILSpy output. Roslyn emits the `as`/null-test/`goto` shapes of a type-pattern `switch` and the
bottom-tested branches of `while` loops, so write those instead of labels. Local names are not
in Release IL; never keep `value2`/`field3`-style names. Use the repo's 4-space formatting.
Try the idiomatic form first and keep an unusual shape only when the Release IL comparison
shows the idiomatic form differs, with a one-line comment saying what the IL requires.
Compile-order devices (file-name prefixes that fix closure ordinals) need the same comment.

Tests, coverage, mutation, status ladders, evidence essays, approval handoffs and phase
audits are not batch requirements. Keep only a short checkpoint: owner, branch, members,
Release IL report, remaining mismatches. This is the complete workflow (decision D17).

Keep original binaries/decompiled code/IL in the ignored local cache. Do not commit them, and
never copy original IL, signatures, metadata or bytes into a build output; builds must not
read the Mathcad install ([D19](docs/mirror/decisions.md)).
