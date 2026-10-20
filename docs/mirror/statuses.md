# Source annotation reference

Existing source retains `MirrorStub`, `MirrorBlocked`, `MirrorDocumented`, `MirrorAnalyzed`,
`MirrorTested`, `MirrorVerified`, `MirrorDeviation` and `QuireOnly` attributes for compatibility
with the metadata reader. Their strings describe the evidence used when that source was written.
They do not establish current IL parity or impose work on an agent.

MemberGen writes `MirrorVerified("Release IL")` provisionally and retains the member only after
its Release IL matches. The generated [Release IL report](validation.md), bound to the original
artifact identity and method token, is the completion record. No status ladder, evidence-file
lookup, test-class lookup or annotation update is required.
