# Ptc.Undo.CommandBuffer

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcUndoCommandBufferMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CurrentInstance get` | 0x060006bb | MemberGen: `_currentInstance; set => _currentInstance = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CurrentInstance set` | 0x060006bc | MemberGen: `_currentInstance; set => _currentInstance = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UndoStackDepth get` | 0x060006c6 | MemberGen: `_currentInstance._undoStack.Count` | Mechanical; Release IL equals Mathcad's | Verified |
| `BeforeCommand add` | 0x060006c9 | MemberGen: `public event EventHandler<CommandEventArgs> BeforeCommand;` | Mechanical; Release IL equals Mathcad's | Verified |
| `BeforeCommand remove` | 0x060006ca | MemberGen: `public event EventHandler<CommandEventArgs> BeforeCommand;` | Mechanical; Release IL equals Mathcad's | Verified |
| `AfterCommand add` | 0x060006cb | MemberGen: `public event EventHandler<CommandEventArgs> AfterCommand;` | Mechanical; Release IL equals Mathcad's | Verified |
| `AfterCommand remove` | 0x060006cc | MemberGen: `public event EventHandler<CommandEventArgs> AfterCommand;` | Mechanical; Release IL equals Mathcad's | Verified |
| `MasterCommand: Do()` | 0x0600131f | MemberGen: `throw new InvalidOperationException("Do method should never be called on a MasterCommand.")` | Mechanical; Release IL equals Mathcad's | Verified |
| `MasterCommand: Top get` | 0x06001322 | MemberGen: `_top` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcUndoCommandBufferMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
