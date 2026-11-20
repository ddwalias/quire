# Quire.Native.Mshex (mshex.dll) checkpoint

- Owner: AGY, Orca task `task_b966c70bda3b`, dispatch `ctx_8f50f58f4210`.
- Branch: `feature/mshex-agy`, from `main`.
- Landed Commit: `0a87c74c4` (main merge receipt: `gate-il-c9103dc990ad.json`).
- Gate: `python tools/validation/native_gate.py --base main --project Quire.Native.Mshex`.

## Members landed this batch

- `Hex::Tag`: default constructor closure (`??_FTag@Hex@@QEAAXXZ`) exported and matched exactly by providing default argument values on the constructor.
- `Hex::operator==(const Signature&, const Signature&)` (`??8Hex@@YA_NAEBVSignature@0@0@Z`): exported and matched byte-for-byte (standard `std::string::operator==` / `memcmp`).
- `Hex::operator<(const Signature&, const Signature&)` (`??MHex@@YA_NAEBVSignature@0@0@Z`): exported and matched byte-for-byte (standard `std::string::operator<` / `memcmp`).
- Gate: PASS, 113 checked (was 110 on `main`), 0 failures, 0 regressions, 0 unfinished among checked. Landed via `lane_merge.py` (`gate-il-c9103dc990ad.json`).

## Status against the original

- Exports: 99 matched on `main` before this batch; 102 matched now (113 total checked symbols including vtables and data).
- The remaining 280 exports are either parked, blocked on internal targets or blocked on std::map instantiations.

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
- `std::map<Signature, Value const*>` and `_Tree` instantiations (about 120 exports): template instantiations that depend on `DefaultEnvironment`'s unmanaged map layout.
