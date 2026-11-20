# Quire.Mceng checkpoint

- Owner: AGY (`ctx_8bb7f06c566c`); maintainer integration on `maintainer/mceng-audit-20261120`.
- Members: C++/CLI project split and `Engine.FactoryImp` members, including `VerifyIntegrity`.
- Release IL: 159 checked and passing; main gate receipt `gate-il-8fa200551291`. The `new` and `delete[]` toolchain entries verify the original name and size; their native bodies are not byte-compared (D23).
- Remaining: the gate reports 93 reference members with no Quire counterpart across the affected projects. The maintainer corrected the `VerifyIntegrity` failure message after the field-name mismatch was resolved.
