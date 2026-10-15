# Generated status files (not committed)

`docs/mirror/status/<Project>.txt` (per-member status) and `docs/mirror/coverage.md` (per-project counts) are
regenerated from the current build and ignored by git, so parallel branches never conflict on them. Status
definitions live in [../statuses.md](../statuses.md); the authoritative record is each member's attribute in source.

Regenerate after building:

```sh
dotnet build Quire.slnx
dotnet run --project tools/Quire.Tools.MirrorGen -- manifest
```

`manifest` also rewrites `docs/mirror/manifest/<Project>.txt`, which stays committed, is checked by the tests and is
maintainer-owned: agents restore it with `git checkout main -- docs/mirror/manifest` (PLAN.md, step 2). It refuses
to drop manifest lines unless given `--accept-removals`.
