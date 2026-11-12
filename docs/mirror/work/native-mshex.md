# Quire.Native.Mshex (mshex.dll) checkpoint

- Owner: native-mshex worker (Orca task task_b01bcca0fa3b).
- Branch: `feature/native-mshex-20261113-r3`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/native-mshex-20261113-r3`, from `main`.
- Gate: `python tools/validation/native_gate.py --base main --project Quire.Native.Mshex`.

## Members landed this batch

- `Hex::Library`: default, copy and move constructors exported, and the four-slot interface vtable `??_7Library@Hex@@6B@`
  exported as data. The original's four slots are all pure-call thunks, so the class is an interface with four pure
  virtual members (names not recovered; placeholders `UnidentifiedSlot0..3`).
- Gate: PASS, 110 checked (was 106 on `main`), 0 failures, 0 regressions, 0 unfinished among checked.

## Status against the original

- Exports: 95 of 380 matched on `main` before this batch; 99 now (the three `Library` constructors and its vtable).
  The other 281 exports are absent from the candidate, so the gate does not check them.

## Remaining mismatches and blockers

- Vtables with internal slots: `Token`, `Node`, `Module`, `Environment` and the `Attachment`-derived classes reference
  virtual functions that are neither exported nor in `docs/mirror/native/internal-targets.json`, so their vtables and
  the constructors that store them stay unverified. Adding those entries is a change to a shared manifest and is left to
  the maintainer.
- `Node` and `Token` virtual destructors: the original keeps the vptr store. With `= default` MSVC drops the store
  (3 bytes against 11). Defining them out of line links only when `Node::GetError` and `Node::Insert` are defined, and
  those are not recovered.
- `DefaultNames::hash` and the `Signature`, `Error`, `Tag` operators that call `std::string` internals: internal
  targets needed (manifest).
- `Hex::operator+(Tag, Tag)`, `Error::GetDetails(Substring)`, `operator<<(ostream, Error)`, the `Signature` accessors
  and `Node::GetError`: parked per the earlier lane (see `docs/mirror/work/lane-j.md`); not re-tested here.
- Function-local statics (`Interpreter::Fetch`/`Clear`, `Formatter::Fetch`, `Tag` dynamic initializers): D29.
- `std::map<Signature, Value const*>` and `_Tree` instantiations (about 160 exports): not started.
