# Quire.EquationEditor checkpoint (haiku, field/method clash fix)

- Owner: EquationEditor worker (task_a8c99dae6d11), branch `feature/haiku-eqed-obf`, rebased on origin/main ed4908164.
- Method: `python tools/Quire.Tools.MemberGen/accept.py --project Quire.EquationEditor` (94 kept project-wide). Kept members of
  other workers' types (EquationModel, MathNode, SpecTableModel, SpecTableColumn, MathNodeToRlTransformer, MathNodeToXmlTransform,
  MathNodeToSExpressionTransform) were reverted; those files stay with their owners.
- Mechanical parameter names (`A_n`) renamed by type in the changed files (IL-neutral).
- Release IL: `--project Quire.EquationEditor` PASS, 1519 checked, 0 failures. Lint: pass.
- Remaining: members of the reverted types; closure/ordinal layouts not started.
