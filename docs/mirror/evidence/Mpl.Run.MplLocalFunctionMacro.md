# Mpl.Run.MplLocalFunctionMacro

- **Original:** `mplrun` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** `ilspycmd` 11.1.0.9782 via `tools/reference/reference.py`; member tokens and IL read from the installed artifact.
- **Quire:** `src/Quire.Mpl/Run/MplLocalFunctionMacro.cs`; tests: `MplLocalFunctionMacroDifferentialTests`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(MplEnv, Sym, uint, Sexpr)` | `0x06000146` | Calls `MplMacro(Sym)`, then stores `env`, `handle`, and `lambda` to `m_env`, `m_handle`, and `m_lambda` in that order. | Direct comparison checks all declared and inherited instance fields for a representative non-default handle and lambda. | Verified for constructor state. |
| `ExpandCall(MplEnv, Sexpr)` | `0x06000147` | Calls `MplMacro.CanNotExpand(form.handle)` and has a trailing null return. | Direct comparison checks the exception type/message/handle through `MethodComparison`; the return is unreachable because `CanNotExpand` always throws. | Verified for the exception case. |
| `ExpandRef(MplEnv, Tok)` | `0x06000148` | Calls `MplMacro.CanNotExpand(s.handle)` and has a trailing null return. | Direct comparison checks the exception type/message/handle; the return is unreachable because `CanNotExpand` always throws. | Verified for the exception case. |

## Test cases

`MplLocalFunctionMacroDifferentialTests` compares constructor state and both expander exception paths using distinct nonzero handles. The original `MplMacro.CanNotExpand(uint)` body directly throws `AppException`, and its independent comparison is recorded in [MplMacro evidence](Mpl.Run.MplMacro.md). No successful expansion case exists for this local macro.

## Open

The four trailing source lines (the `return null;` statements and closing sequence points) are recorded as pending unreachable-line proposals. Original tokens `0x06000147` and `0x06000148` both call `MplMacro.CanNotExpand(uint)` (`0x0600012c`), whose 13-byte body constructs `AppException` and throws unconditionally before Mathcad's `ldnull; ret` tails. No source restructuring is appropriate.
