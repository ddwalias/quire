# Current work

Use the [five-step Release IL workflow](PLAN.md). Exact Mathcad Release IL is the only
acceptance rule. Never inspect Debug IL.

## Active lanes

| Lane | Exclusive assembly ownership | Branch | Worktree |
|---|---|---|---|
| [A](work/lane-a.md) | `Quire.Core` | `mirror/a-core` | `C:/Projects/Quire-wt/lane-a-core` |
| [B](work/lane-b.md) | `Quire.Mpl.Emit` | `mirror/b-mpl-emit` | `C:/Projects/Quire-wt/lane-b-mpl-emit` |
| [C](work/lane-c.md) | `Quire.Utils` | `mirror/c-utils` | `C:/Projects/Quire-wt/lane-c-utils` |
| [D](work/lane-d.md) | `Quire.Engine.Utl` | `mirror/d-engine-utl` | `C:/Projects/Quire-wt/lane-d-engine-utl` |

These are the only active lanes. All former A–E and Generator assignments and file
reservations are released. Old conversations are archived; old branches and worktree
contents are preserved locally for recovery, without retaining ownership. Do not resume
their validation rules or merge their old receipts. Recovered implementations must pass
the current Release IL comparison before landing.

Each lane owns all managed members of its assembly, including members formerly deferred
by tier classification ([D18](decisions.md)). Unreadable/native bodies remain incomplete;
do not substitute behavior tests or claim an assembly is complete from stub counts.

Work only in your assigned worktree and assembly, and update only your own lane checkpoint.
Shared tools, mappings and integration belong to the maintainer; report concrete blockers
there while continuing independent members. Merge through `lane_merge.py`, which serializes
integration. Keep the lane worktree for subsequent batches; retire it when the lane finishes.

Record only owner, branch, selected members, Release IL report and remaining mismatches.
The [assembly map](assembly-map.md) shows dependencies and subsequent unlocks. Its historical
tier labels do not override the four assignments above.
