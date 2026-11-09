# Quire

Quire mirrors every Mathcad Prime 12.0.1.0 assembly. The goal is exact **Release IL** parity using the project's assembly and name mappings.

The former Quire worksheet application is legacy. Its custom UI, worksheet DTOs, `.qwx` format, import adapter, and sample worksheet have been retired. `src/Quire` remains a library project mirroring `MathcadPrime.dll`.

## Build and validate

```powershell
dotnet build Quire.slnx -c Release
python tools/validation/release_il.py --base main --project Quire.Core
```

Builds and reference inspection use the Mathcad Prime 12.0.1.0 installation; see [validation](docs/mirror/validation.md) for prerequisites and native build commands. Many mirrored members remain stubs. A successful build does not establish IL parity.

Follow the [five-step workflow](docs/mirror/PLAN.md) and [agent instructions](AGENTS.md). [WORK](docs/mirror/WORK.md) records ownership; the [assembly map](docs/mirror/assembly-map.md) records dependencies and completion order.

Members with Mathcad counterparts must remain, even when unfinished. Quire-only product code may be removed; shared mirror tooling, mappings, build infrastructure, and required resources remain.
