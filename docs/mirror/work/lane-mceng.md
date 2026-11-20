# Quire.Mceng checkpoint

- Owner: AGY (`ctx_8bb7f06c566c`); maintainer integration on `maintainer/mceng-audit-20261120`.
- Members: C++/CLI project split and `Engine.FactoryImp` members, including `VerifyIntegrity`.
- Release IL: 159 checked, passing with the candidate validator and mceng CRT manifest. The `new` and `delete[]` toolchain entries verify the original name and size; their native bodies are not byte-compared (D23).
- Remaining: land the shared validator and CRT entries, then rerun the main gate. The maintainer corrected the `VerifyIntegrity` failure message after the field-name mismatch was resolved.
