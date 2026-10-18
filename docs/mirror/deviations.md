# Intentional deviations ledger

This is the canonical reviewed ledger for intentional structural or behavioral differences between Quire and an
established Mathcad counterpart. A deviation is a deliberate, evidenced departure—not an unverified assumption,
implementation convenience, or accidental bug. Do not invent original behavior to justify one. Unknown or ambiguous
Mathcad behavior stays open/blocked in the work and evidence records in [`PLAN.md`](PLAN.md); it is not an accepted
deviation.

Before relying on any intentional difference, create a stable `DEV-####` entry here and record its evidence, reason,
compatibility impact, tests, revisit plan, and explicit decision. `proposed` does not authorize relying on the
difference. Only an explicit decision recorded with status `accepted-temporary` or `accepted-permanent` may be
treated as an intentional deviation; do not invent or infer named approvers. A suspected Quire bug or accidental
mismatch remains a bug to track and fix unless a deliberate exception is separately evidenced and explicitly
decided.

## Index

| ID | Scope / counterpart | Intentional difference | Status | Detail |
|---|---|---|---|---|
| — | — | No confirmed intentional deviations are recorded in this ledger yet. | — | — |

The scope cell names each covered Quire method as `Quire.Type::Member` (every overload of that name); the validator
lets an accepted id excuse a structural difference only on the members its row names.

No behavioral deviations were newly verified by this documentation edit. This empty index does not establish that no
deviations exist. Existing differences noted by the generator are not thereby reviewed, accepted, or classified as
behavioral deviations.

## Entry template

Copy this template for each proposed deviation. Keep its ID stable if the entry is revised or later superseded.

```markdown
### DEV-0001 — Short, specific title

- **Status:** proposed | accepted-temporary | accepted-permanent | rejected | superseded
- **Category:** structural | behavioral
- **Original counterpart:** assembly and version; type; member and full signature (or type/signature for a structural difference)
- **Original contract and evidence:** assembly metadata for structural shape; `ilspycmd` inspection where readable, documentation, and/or direct differential observations for behavior. Distinguish observed from inferred behavior; include confidence and limits. Unknown behavioral contract is not an intentional deviation.
- **Quire counterpart and behavior:** exact behavior and source path(s); link relevant callers/helpers when needed.
- **Difference:** concise statement of how Quire intentionally differs from the established original contract.
- **Reason / necessity:** evidence-backed reason this difference is required or accepted.
- **Compatibility and user impact:** affected callers, data, integrations, observable behavior, and scope/conditions.
- **Duration and revisit:** temporary or permanent; trigger/date/condition for review; planned resolution or reason no resolution is planned.
- **Verification:** behavior/regression test paths and cases; differential tests and exact cases if directly compared. Do not label inferred behavior as differential-verified.
- **Decision record:** explicit decision reference and date; identify the decision source only when documented. Do not fabricate a named approver.
- **Related records:** issue/PR/design/evidence links, or `none`.
```

## Reconciliation with generator notes

[`notes.txt`](notes.txt) documents differences in generated C# versus metadata, often caused by language or generator
constraints. It is not an approval record and does not, by itself, establish a behavioral deviation. Reconcile its
entries individually: link a reviewed intentional difference to a ledger ID; retain a generator-only mapping note in
`notes.txt`; track an unverified or accidental mismatch as open work/bug; or mark an obsolete note as such. Do not
bulk-import notes or silently treat them as accepted deviations. This reconciliation remains outstanding.
