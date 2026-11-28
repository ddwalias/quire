# Quire.Controls.EquationControl SpecTableControl layout

- Owner: luna-rtf; branch `feature/luna-equationcontrol-spectable-layout`.
- Members: moved `SpecTableControl` declarations into Mathcad member order as unmarked placeholders to stabilize closure ordinals.
- Release IL: PASS, 4121 checked (`.mirror-cache/release-il-4868872f7bc4/release-il.json`); lint passes.
- Remaining: implement closure-dependent members after this layout lands; other EquationControl types remain in scope.
- Reordered all declarations by Mathcad method-table order after the first lane gate exposed closure ordinals shifted by 73; _engineResults and _currentIdLabel are positioned with their properties to match Roslyn's closure anchors. MatrixResizeAdorner remains generated because its hand placeholder constructor changes IL and has no closure layout dependency.
