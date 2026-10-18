# Ptc.FrameworkElementPositionWrapper

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcFrameworkElementPositionWrapperMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(FrameworkElement)` | 0x06000239 | MemberGen: `_element = element;` | Mechanical; Release IL equals Mathcad's | Verified |
| `TranslatePoint(Point, UIElement)` | 0x0600023a | MemberGen: `_element.TranslatePoint(point, relativeTo)` | Mechanical; Release IL equals Mathcad's | Verified |
| `PointToScreen(Point)` | 0x0600023b | MemberGen: `_element.PointToScreen(point)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ActualHeight get` | 0x0600023c | MemberGen: `_element.ActualHeight` | Mechanical; Release IL equals Mathcad's | Verified |
| `ActualWidth get` | 0x0600023d | MemberGen: `_element.ActualWidth` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsVisible get` | 0x0600023e | MemberGen: `_element.IsVisible` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcFrameworkElementPositionWrapperMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
