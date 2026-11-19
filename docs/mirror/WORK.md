# Current work

Use the [assembly map](assembly-map.md) to delegate work by assembly and understand
dependencies. Check active assignments before claiming members to avoid overlap.
Historical lane names, reservations and fixed worktree paths are obsolete. Historical
checkpoints remain available under [work](work/) as records of prior work and blockers.

Each agent works in a separate worktree and branch from current `main`, keeps the shared
checkout on `main`, and edits only its delegated assembly and checkpoint. Assignments
cover all managed members regardless of historical tier (D18). Shared tools, mappings
and integration belong to the maintainer. Report concrete blockers and continue with
independent members.

Use the [Release IL workflow](PLAN.md), continue through passing batches, and land via
`lane_merge.py`. Keep a short checkpoint with owner, branch, members, Release IL report
and remaining mismatches. Missing or unreadable counterparts remain incomplete.
