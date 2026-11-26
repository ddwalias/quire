# WorksheetControlContent / WorksheetModel closure order

- Owner: Worksheet ControlContent/Model lane; branch `mirror/worksheet-controlcontent-model-b2`.
- Layout: `WorksheetControlContent` members are in Mathcad IL method order in one hand-written partial. Field anchors: 2 fields before the constructor, 1 field between the constructor and `SelectItems`, 1 field between `SelectItems` and `CheckPotentialMove`, 7 after `ItemsRemoved`. Nested command types count as members of the outer type's ordinal space (7 types before the constructor).
- Anchors (Mathcad closure ordinal N = Quire count): constructor 9, `SelectItems` 77, `CheckPotentialMove` 159, `ItemsRemoved` 165. `ItemsRemoved` is exact.

## SelectItems (parked, one attempt)

- Mathcad has the static `item => item.GetCanSelect()` lambda as `<>c::<SelectItems>b__77_0` and the instance `item => !SelectedItems.Contains(item)` lambda as `<SelectItems>b__77_1`, although the decompiled source lists the instance lambda first. Roslyn numbers lambdas in source order, so the Mathcad source must have had the static lambda's expression first.
- Attempt: hoist the static predicate into a local `Func<IWhiteboardItem, bool>` declared before the `if`. The closure name then matches (`<>c::<SelectItems>b__77_0`), but the first differing instruction is 0: the delegate is created before `_selectedItems.Any()`, while Mathcad starts with the `Any` call. Reverted.
- Open: a shape that keeps the delegate creation after the `Any` call and still numbers the static lambda first. Not found.
