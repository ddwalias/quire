# QUI-18 — retire the legacy worksheet application

- Owner: Codex.
- Branch: `contact/qui-18-persistentdataobjects-pdo-parked-members-and`.
- Members: remove custom worksheet/settings/region DTOs, legacy application shell, worksheet package/import paths, region controls, calculation worker, prototype interface additions and their shims, and the three obsolete app test files. Retain mapped assemblies and declarations; restore original implementations needed to preserve existing exact matches, including their closure ordinals. `Quire` now builds as a library mirror of `MathcadPrime.dll`.
- Release IL: `.mirror-cache/qui-18-release-final/release-il.json` — PASS, 12,751 checked; base `24b6610d3`.
- Rebased check: `.mirror-cache/qui18-compare.json` — PASS, 12,842 checked. Solve-block registration uses the original field-initializer form, preserving `beforefieldinit`.
- Remaining mismatches in this change: none. Original PDO/PersistentData/XmlWorksheetData stubs remain unfinished; removing the custom DTOs does not implement them.
