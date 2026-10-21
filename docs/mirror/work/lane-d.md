# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-20261021-e1`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-e1`.
- Original assemblies: utl-mgd and mcmisc.
- Selected members: complete `utl-mgd` surface in `Quire.Engine.Utl`: labels and dictionaries, safe handles and synchronization, queues and interruption, string/hex helpers, function wrappers and logging; retain the earlier `mcmisc` helper batch.
- Release IL report: PASS against `main`; 286 checked; 129 exact managed bodies; 163 engine-native classifications; 0 unfinished; `.mirror-cache/release-il-0bf2344789dc/release-il.json`.
- Remaining mismatches: none in checked Release IL. Native behavior is ported through Windows APIs and UTL/MSVC exports but remains unverified at runtime; C++ stream-formatting parity and process-exit cleanup need a Windows integration pass.
