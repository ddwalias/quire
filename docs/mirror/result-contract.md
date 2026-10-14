# Engine result-dictionary work

## 3b. Unfinished: engine result-dictionary compatibility

**Status: open. Scheduled at the start of Phase 2**, before any of the result-node hierarchy, `ResultManager` or the
Calculation handlers are built on top of it. Until it is done, nothing in Quire may claim that its engine results are
Mathcad-compatible.

A matching return signature (`IDictionary GetValueSync(int, int)`, `GetValueAsync`, `GetValueAfter`,
`GetValueBeforeAsync`) says nothing about what the dictionary holds. The keys, the type of each value, which entries
are optional, how aggregate results nest, and what each `NodeStatus` means are all part of the contract. A
Mathcad-shaped consumer would depend on all of them. Quire's current answers were chosen for Quire, not verified.

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

### Execution

Apply [PLAN.md §3c](PLAN.md#3c-method-by-method-fidelity) and the [evidence workflow](evidence/README.md).
Analyze original producers, consumers, helpers, and status transitions before changing runtime keys. Record
assembly/version, exact signatures, findings, confidence/limits, and observed versus inferred cases. Update
engine producers and calculation consumers together, or use a compatibility adapter. Preserve existing
calculation behavior. Test exact key sets, value types, optional/error entries, ready/failed/stale/in-progress
states, aggregate nesting, and scope look-ups; compare with Mathcad where feasible. Claim compatibility only
for contract parts supported by evidence and tests.

### Findings

| Item | Verified fact | Evidence source | Unresolved |
|---|---|---|---|
| (none yet) | | | Keys, value types, optional entries, nesting, status meanings |
