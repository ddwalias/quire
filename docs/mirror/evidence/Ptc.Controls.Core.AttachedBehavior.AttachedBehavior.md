# Ptc.Controls.Core.AttachedBehavior.AttachedBehavior

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SetParentPropertyValueKeepers(FrameworkElement, ParentPropertyValueKeepers)` | 0x06000eef | MemberGen: `elem.SetValue(ParentPropertyValueKeepersProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetChildPropertyValueFinders(FrameworkElement, ChildPropertyValueFinders)` | 0x06000ef1 | MemberGen: `elem.SetValue(ChildPropertyValueFindersProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetPropertyBinders(FrameworkElement, object)` | 0x06000ef3 | MemberGen: `elem.SetValue(PropertyBindersProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetRoutedEventBindings(FrameworkElement, RoutedEventBindingCollection)` | 0x06000ef4 | MemberGen: `element.SetValue(RoutedEventBindingsProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetMessagesDecorator(FrameworkElement, MessagesDecorator)` | 0x06000ef7 | MemberGen: `elem.SetValue(MessagesDecoratorProperty, decorator)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreAttachedBehaviorAttachedBehaviorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
