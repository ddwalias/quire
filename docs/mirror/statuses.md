# Member statuses

Each member of a mirrored type has one status (PLAN.md §3c), from its attributes:

- **Stub** `[MirrorStub]`: generated, throws.
- **Unclassified** (no attribute): hand-written before §3c; still to be analyzed, or marked Quire-only.
- **Blocked** `[MirrorBlocked(reason)]`: analysis couldn't expose or settle the original; throws, reason recorded.
- **Documented** `[MirrorDocumented(source)]`: documentation-only; not sufficient for completion.
- **Analyzed** `[MirrorAnalyzed(evidence)]`: analysis recorded in [`evidence/`](evidence/); no passing behaviour test yet.
- **Tested** `[MirrorTested(evidence)]`: analyzed, reconstructed from the original, behaviour tests pass.
- **Verified** `[MirrorVerified(test)]`: differential-verified against the installed Mathcad, for the recorded cases only.
- **Deviation** `[MirrorDeviation(id)]`: an accepted entry in [`deviations.md`](deviations.md); a divergence, never a match.
- **Abstract** (no attribute, no body): an abstract or interface member without a default body. Mathcad's has
  no body either, so the declaration, which the manifest checks, is the whole contract.
- **Quire-only** `[QuireOnly]`: no Mathcad counterpart.
- **Generated** (Tier B, [D11](decisions.md); the attribute arrives with the member generator): emitted
  by the generator and accepted on an exact Release-IL match with every callee settled. Until the gate supports
  it, generated members are reported as Stub.

*Settled* is Tested + Verified + Deviation + Abstract + Quire-only (§3c rule 7). Blocked, documented and analyzed members
are open. Settled says nothing about untested inputs, and deviations are counted apart from matches.
Explicit interface implementations, enum values and delegate signatures are not counted.

Live counts per project (`coverage.md`) and per-member detail (`status/<Project>.txt`) are generated, not
committed; see [status/README.md](status/README.md) to regenerate them.
