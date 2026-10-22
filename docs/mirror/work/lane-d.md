# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI spike adds `LispUtils`, `LinePartition`, `FileUtils`, `Mailbox<T>`, `MessageQueue<Owner,Data,Msg>`, and `TaskQueue`; consumer exercises worker queues, scanner, file helpers, and Win32 synchronization.
- Release IL: C# PASS, 129/129 (`.mirror-cache/release-il-3d280dfb40eb/release-il.json`); C++/CLI candidate FAIL, 378 checked / 230 exact / 215 failures (`.mirror-cache/release-il-694cde9897cc/release-il.json`).
- Remaining: 20 methods removed, 117 generated global methods and 34 other methods unbound, and 44 mapped method differences. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
