# WorksheetControlContent / WorksheetModel closure order

- Owner: Worksheet ControlContent/Model lane; branch `mirror/worksheet-controlcontent-model-b2`.
- Layout: `WorksheetControlContent` members are in Mathcad IL method order in one hand-written partial. Field anchors: 2 fields before the constructor, 1 field between the constructor and `SelectItems`, 1 field between `SelectItems` and `CheckPotentialMove`, 7 after `ItemsRemoved`. Nested command types count as members of the outer type's ordinal space (7 types before the constructor).
- Anchors (Mathcad closure ordinal N = Quire count): constructor 9, `SelectItems` 77, `CheckPotentialMove` 159, `ItemsRemoved` 165. `ItemsRemoved` is exact.

## SelectItems (parked, one attempt)

- Mathcad has the static `item => item.GetCanSelect()` lambda as `<>c::<SelectItems>b__77_0` and the instance `item => !SelectedItems.Contains(item)` lambda as `<SelectItems>b__77_1`, although the decompiled source lists the instance lambda first. Roslyn numbers lambdas in source order, so the Mathcad source must have had the static lambda's expression first.
- Attempt: hoist the static predicate into a local `Func<IWhiteboardItem, bool>` declared before the `if`. The closure name then matches (`<>c::<SelectItems>b__77_0`), but the first differing instruction is 0: the delegate is created before `_selectedItems.Any()`, while Mathcad starts with the `Any` call. Reverted.
- Open: a shape that keeps the delegate creation after the `Any` call and still numbers the static lambda first. Not found.

## Parked (WorksheetModel / WorksheetControlContent)

- `WorksheetModel.ShowError` (Mathcad `<>c__DisplayClass`-free): Release IL differs at entry 20, after `GetCanActivate`; Quire's jump is one byte longer than Mathcad's. Parked.
- `WorksheetModel.SetItemWorksheetLocation`: call order differs at entry 16 and the loop branch shape differs at entry 60. Parked.
- `WorksheetModel.ContainsSymbolicEvaluation` (Mathcad closure `<get_ContainsSymbolicEvaluation>b__13_0`): Quire gives `b__115_0`. Needs the whole-type layout (generated members come before every hand-written member in compile order), not a body change. Parked.
- Calibration: the seven fields `_customSplitItemRenderSizeDelegate` … `_copyWhiteboardItems` sit after `GetItemAtLocation` in the hand partial. Moving them there moved `GetItemAtLocation` from 177 to 170 (Mathcad) with `ItemsRemoved` still exact at 165.
- `WorksheetModel.FirstItemInDirection` (Mathcad `<>c__DisplayClass178_0`, closures `b__0` = `Compare < 0` for `FirstOrDefault`, `b__1` = `Compare >= 0` for `LastOrDefault`): the display-class ordinal already matches (178). The method's shape does not. Mathcad lays out `brfalse` to the `Left` block, then `bne.un` to a throw at the end, then the `Right` block, then the `Left` block. A `switch` expression (Right, Left, `_ => throw`) gives 35 entries against Mathcad's 31. Nested `if (direction != Left)` gives the throw inline. Parked; not landed.
- `WorksheetModel.CommitItem` (Mathcad `<>c::<CommitItem>b__82_0`): the body matches the IL with the `Each` lambda, but Quire numbers the lambda `b__157_0`. The method is 82nd in Mathcad order and 157th in Quire's compile order, because generated members precede hand-written ones. Needs whole-type layout (placeholder stubs in Mathcad order or a compile-order device). Parked; not landed.
