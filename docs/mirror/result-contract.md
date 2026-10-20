# Engine result dictionary reference

This note preserves provisional implementation details for future native-engine integration.
The original engine dictionary contents are unresolved. This research does not add an
acceptance gate to a managed [Release IL batch](PLAN.md).

### What Quire produces today (provisional)

The keys are defined in `Quire.Engine.Internal.ResultKeys`. They are written by `NodeResult.ToDictionary` and
`WorksheetEngine.ValueDictionary`, and read by `Quire.Calculation.ValueToIResultNodeTransform`.

| Key | Value | Present |
|---|---|---|
| `status` | `NodeStatus` | Always |
| `value` | `DimVal` or null | Node results and scope look-ups |
| `table` | `bool` | Node results |
| `defined` | `string[]` | Node results |
| `error`, `symbol` | `string`, `string` or null | When the node failed |
| `warning` | `string` | When there is a warning |
| `display-scale`, `display-offset` | `double` | When the region has a unit override |
| `children` | `ArrayList` of nested dictionaries with the same keys | Aggregates such as solve blocks |

The statuses Quire uses are:

- `NODE_READY`: calculated.
- `NODE_INVALID`: failed, or the name is unbound in a scope look-up.
- `NODE_NOT_COMPUTING`: never calculated, or out of date, possibly with the previous value.

Quire never produces `NODE_COMPUTING` or `NODE_TREATED_READY`. The member names come from metadata; the meanings
above are Quire's assumptions.

## Unresolved original details

Exact keys, value types, optional entries, aggregate nesting and status meanings have not
been established for the original native producer. Preserve this distinction when using
these notes. Managed consumers can still be reconstructed and compared with their original IL.
