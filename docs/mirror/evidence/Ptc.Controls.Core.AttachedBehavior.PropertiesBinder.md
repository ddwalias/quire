# Ptc.Controls.Core.AttachedBehavior.PropertiesBinder

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SourceValueConnector get` | 0x06000f45 | MemberGen: `base.GetValue(SourceValueConnectorProperty); set => base.SetValue(SourceValueConnectorProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SourceValueConnector set` | 0x06000f46 | MemberGen: `base.GetValue(SourceValueConnectorProperty); set => base.SetValue(SourceValueConnectorProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TargetValueConnector get` | 0x06000f47 | MemberGen: `base.GetValue(TargetValueConnectorProperty); set => base.SetValue(TargetValueConnectorProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TargetValueConnector set` | 0x06000f48 | MemberGen: `base.GetValue(TargetValueConnectorProperty); set => base.SetValue(TargetValueConnectorProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BlockUndoWhenBindingToTarget get` | 0x06000f4e | MemberGen: `public bool BlockUndoWhenBindingToTarget { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BlockUndoWhenBindingToTarget set` | 0x06000f4f | MemberGen: `public bool BlockUndoWhenBindingToTarget { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
