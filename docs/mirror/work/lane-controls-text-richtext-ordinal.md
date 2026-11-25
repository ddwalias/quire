# Quire.Controls.Text RichText closure order

- Owner: Lane Controls.Text; branch `feature/controls-text-richtext-ordinal`, based on `c067d216c`.
- Scope: `RichTextBase` first slice, all members before and including `GetInsertionPointAtPointer` (IL method index 135); the `RichTextWithEmbeddedItem` reorder remains.
- Method: for each generated closure `<Member>b__N_M`, local function, or iterator, derive the non-method offset as `N - IL method index`; place fields, properties, events, and nested types around the method stream to meet the anchors. The generated `SyncTextChangedCommand` shell is the first parent member; hand-bound `PropertyNames` and `BlockedSelectionDirection` are positioned in the hand-written partial.

## Derived anchor table

Outer `RichTextBase` anchors (29 positions, ordered by IL method index):

| IL method index | Member | Closure ordinal N | Offset N − index |
|---:|---|---:|---:|
| 0 | `get_CustomScriptsSizesForXamlSerialization` | 2 | 2 |
| 1 | `set_CustomScriptsSizesForXamlSerialization` | 3 | 2 |
| 31 | `.cctor` | 81 | 50 |
| 39 | `.ctor` | 93 | 54 |
| 41 | `InitializeFeatureFlagCommandBindings` | 95 | 54 |
| 50 | `SetStyleFromTextStyles` | 105 | 55 |
| 54 | `HandleApplyTextStyle` | 109 | 55 |
| 55 | `DoApplyTextStyle_internal` | 110 | 55 |
| 56 | `HandleClearTextFormat` | 111 | 55 |
| 57 | `ClearFormatPropertiesInRange` | 112 | 55 |
| 58 | `ClearSelectedRegionFormatProperties` | 113 | 55 |
| 61 | `HandleToggleSubscriptCommand` | 118 | 57 |
| 62 | `HandleToggleSuperscriptCommand` | 119 | 57 |
| 65 | `ToggleRunElementAccordingToScriptState` | 122 | 57 |
| 66 | `RecalculateTextScriptState` | 123 | 57 |
| 70 | `HandleSetCurrentProofingLanguage` | 127 | 57 |
| 98 | `PostPasting` | 159 | 61 |
| 103 | `WasScriptElementDeletedOnChangeEvent` | 165 | 62 |
| 104 | `UpdateCurrentRuns` | 167 | 63 |
| 106 | `ComputeProofingLanguage` | 169 | 63 |
| 113 | `ExecuteTextChangedAndReactivateCommand` | 177 | 64 |
| 128 | `GetTextFormatForWholeDocument` | 196 | 68 |
| 130 | `set_CaretOffset` | 199 | 69 |
| 135 | `GetInsertionPointAtPointer` | 207 | 72 |
| 142 | `EnumerateEmbeddedObjectsInRange` iterator | 217 | 75 |
| 160 | `IsInvalidOperation` | 235 | 75 |
| 177 | `Activate` | 261 | 84 |
| 192 | `SetTextFormatPropertyValue` | 284 | 92 |
| 203 | `IsFullySubscript` | 297 | 94 |

The two anchors inside nested `RichTextBase.SyncTextChangedCommand` are `.ctor` (nested method index 0, ordinal 9, offset 9) and `Undo` (index 1, ordinal 10, offset 9). This makes 31 anchors total; nested-class offsets use that nested type's method stream.

## Slice 1

- Reordered the direct declarations through `GetInsertionPointAtPointer` in Mathcad method order, with non-method declarations interleaved to hit the anchor offsets. Implemented `GetInsertionPointAtPointer` from the reference shape; its closure emits `<GetInsertionPointAtPointer>b__207_0`.
- Release IL report: `.mirror-cache/release-il-f742d506ade8/release-il.json` — PASS, 695 checked, zero failures; 76 generated stubs, 44 no-pair members, 3 unchanged/unmatched.
- Remaining: continue the later `RichTextBase` slice and reorder `RichTextWithEmbeddedItem`; request a landing slot before each merge.
