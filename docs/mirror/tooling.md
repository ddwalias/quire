# Mirror tooling

Use the [five-step workflow](PLAN.md). All IL inspection and comparison uses Release builds.
Never inspect Debug IL.

| Tool | Purpose |
|---|---|
| `tools/reference/reference.py` | Cache original Mathcad C#, IL and metadata identities locally |
| `tools/validation/release_il.py` | Build production Release assemblies and enforce exact IL parity |
| `tools/validation/il_diff.py` | Print the first differing Release instructions |
| `tools/validation/lane_merge.py` | Refresh, rebase, compare, fast-forward and push |
| `tools/validation/assembly_map.py` | Generate assembly mappings, prerequisites and completion order for planning |
| `tools/Quire.Tools.MethodAudit` | Read Release metadata and IL without loading/executing the target |
| `tools/Quire.Tools.MirrorGen` | Generate metadata skeletons, mappings and manifests |
| `tools/Quire.Tools.MemberGen` | Reconstruct mechanical method bodies from Mathcad IL |

## MemberGen

MemberGen handles mechanical stubs: accessors, field-storing constructors, forwarding calls,
constant returns and straight-line bodies. It moves reconstructed members and their fields
out of `Generated/Mirror` into partial source files. It uses the existing tier lists and
name mappings. `--include-tier-a` adds Tier A/A2 members owned by the agent.
For the four assemblies assigned in [WORK](WORK.md), it considers every mapped mechanical
stub automatically, including former C/D members (D18). Non-mechanical bodies still require
manual reconstruction; unreadable native bodies remain incomplete.

```powershell
python tools/Quire.Tools.MemberGen/accept.py --project Quire.Mpl
python tools/Quire.Tools.MemberGen/accept.py --project Quire.Mpl --include-tier-a
```

Start with clean committed source for that project. Each round restores that source,
generates the remaining candidates, builds the production project in Release and audits
it against Mathcad. Compilation failures and IL mismatches reject the responsible member;
the next round excludes it. Stop when a round rejects nothing. On failure, restore the
project. No tests, coverage, mutation or per-member contract writeups are generated or run.

The local report includes original tokens and kept/rejected members; output defaults to
`.mirror-cache/membergen/<project>-<time>`. Run the standard Release IL check before landing
to retain the existing exact matches as well. `emit` is internal to `accept.py`; its
`MirrorVerified("Release IL")` annotations are provisional until the audit passes.

For a survey, first build the relevant production projects with `-c Release`, then:

```powershell
dotnet run -c Release --project tools/Quire.Tools.MemberGen -- survey --repo . --install '<Mathcad directory>' --project Quire.Mpl --output .mirror-cache/membergen-survey.md
```

## MirrorGen

MirrorGen reads metadata to generate type/member skeletons and the manifest. It preserves
visibility, nested types and signatures using `Rules.cs`, `Mapper.cs` and `Identifier.cs`.
It does not establish IL parity.

Regeneration reads the normal Release build: every member MirrorGen writes carries
`[MirrorStub]`, so the members without it are hand-written. Hand-written `[MirrorOf]` bindings,
type declarations and `: this(...)` constructor chains are read from sources outside `Generated/`.

```powershell
dotnet build Quire.slnx -c Release
dotnet run -c Release --project tools/Quire.Tools.MirrorGen -- emit
dotnet build Quire.slnx -c Release
python tools/validation/release_il.py --base main
```

Emit drops stubs for members hand-written code now provides (on the type, or a hand-written
base for methods); the gate does not count removing a never-exact stub, replacing one stub
with another, or adding a stub declared as its Mathcad counterpart, as a change. Keep
original binaries and decompiled code in the ignored cache; never commit them.

The retired runtime validators, mutation/coverage tooling and stored baselines are removed.
Only the Release IL tools participate in batch acceptance and merging.
