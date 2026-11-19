# Assembly delegation prompt

Choose an assembly using the [assembly map](../assembly-map.md) and check active
assignments before delegating. Supply the selected assembly in this prompt.

```text
Work on exact Mathcad Release IL parity for <assembly>.
Read AGENTS.md, docs/mirror/PLAN.md and docs/mirror/assembly-map.md. Create your own
branch and worktree from current main. Check the assembly's existing checkpoints and
active ownership before selecting members. Edit only your assigned assembly and
checkpoint. Report shared tooling or mapping blockers to the maintainer and continue
independent work.

Read original Release IL, implement members, run the Release IL gate, commit and land
through python C:/Projects/Quire/tools/validation/lane_merge.py <branch>. Continue through
passing batches until the assignment is complete or remaining work is blocked.
```
