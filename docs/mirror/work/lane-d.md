# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-f5`.
- Members: C++/CLI mixed-mode proof; exact `Logging::.ctor` and `Logging.IsLoggingEnabled` port, with `Logging.Log`/`LogLine` still differing.
- Release IL: VCX candidate FAIL, 394 checked / 282 exact / 165 failures (`.mirror-cache/release-il-65bd10a0f829/release-il.json`).
- Remaining: 118 unbound global methods, 34 methods without counterparts, five removals, eight mapped differences. Production C# inventory remains 129 exact / 154 different / 27 unresolved.
