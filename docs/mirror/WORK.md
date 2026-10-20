# Current work

Use the [five-step Release IL workflow](PLAN.md). Exact Mathcad Release IL is the only
acceptance rule. Never inspect Debug IL.

## Lane assignments — awaiting user launch

| Lane | Exclusive assembly ownership | Branch | Worktree |
|---|---|---|---|
| [A](work/lane-a.md) | `Quire.Core` | `mirror/a-core-<unique>` | `C:/Projects/Quire-wt/lane-a-core` |
| [B](work/lane-b.md) | `Quire.Mpl.Emit` | `mirror/b-mpl-emit-<unique>` | `C:/Projects/Quire-wt/lane-b-mpl-emit` |
| [C](work/lane-c.md) | `Quire.Automation`, `Quire.EngNotebook.Automation`, `Quire.EngNotebookPlus.Automation`, `Quire.Calculation.Core`, `Quire.Media3D` | `mirror/c-media3d-20261021-m2` | `C:/Projects/Quire-wt/lane-c-automation` |
| [D](work/lane-d.md) | `Quire.Engine.Utl` | `mirror/d-engine-utl-<unique>` | `C:/Projects/Quire-wt/lane-d-engine-utl` |

The user launches these agents in separate Codex/AGY/Claude threads using the
[copyable prompts](work/agent-prompts.md). No worker is currently running. Worktree paths
above are reserved locations to create on launch, not pre-existing checkouts. Create a
uniquely named branch from current `main`; use `C:/Projects/Quire` as the main checkout.
Do not spawn additional agents or threads.

All former A–E and Generator assignments and file reservations are released. Old
conversations are archived. Retained branches and local backups grant no ownership.
Do not resume their validation rules or merge their old receipts. Recovered implementations
must pass the current Release IL comparison before landing.

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
