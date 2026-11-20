# Quire.Native.Creo3dfit (creo3dfit.dll) checkpoint

- Owner: AGY, Orca task `task_52171e9f530c`, dispatch `ctx_a2d9d026d99b`.
- Branch: `feature/creo3dfit-agy`, from `main`.
- Project: `src/Quire.Native.Creo3dfit/Quire.Native.Creo3dfit.vcxproj`.
- Gate: `python tools/validation/native_gate.py --base main --project Quire.Native.Creo3dfit`.

## Members matched and verified

- `mathcad_fitsurface_`: entry point at RVA `0x26530` (5 bytes: `jmp fit_surface_impl` at `0x264A0`). Matched exactly (5 bytes, 1 relocation site masked).
- `fit_surface_impl` (at internal RVA `0x264A0`): 13-parameter wrapper constructing the 14-argument stack frame with null 14th argument, matching RVA `0x264A0` byte-for-byte (132 bytes, 1 relocation site masked).
- `internal_fit_surface` (at internal RVA `0x26540`): internal surface fitting implementation parked under Decision D29 / D31.
- Gate: PASS (3 checked: `mathcad_fitsurface_`, `fit_surface_impl`, `internal_fit_surface`), 0 failures, 0 regressions, 0 unfinished.

## Status against the original

- Across all Mathcad assemblies, only `efigraphics_.dll` imports `creo3dfit.dll`, importing exclusively `mathcad_fitsurface_`.
- `mathcad_fitsurface_` is fully verified and exported via `Quire.Native.Creo3dfit.def`.
- The remaining 4 exports of `creo3dfit.dll` (`btkEvent::CurrentKind`, `btkProcess::StdStream`, `btkMBStrFunc::mbsMode`, and `storageclient_debug_bind_hack`) are internal PTC Creo platform infrastructure symbols. Per Decision D31 and `docs/mirror/scope.md`, separate Creo platform dependencies remain deferred.
