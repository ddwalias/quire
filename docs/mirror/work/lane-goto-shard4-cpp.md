# Goto cleanup checkpoint

- Owner: luna, branch `feature/luna-goto4-clean`.
- Members: `SafeMutex::EnsureReleased`, `Reader::read_tag`, `Reader::eat_white`, and `Front::Move` in `Quire.Engine.Utl`, `Quire.MspconsMgd`, and `Quire.Mcfront12`.
- Release IL: Utl PASS (352 checked, `release-il-e6527a69a091`), MspconsMgd PASS (109 checked, `release-il-f45e85580c36`), Mcfront12 PASS (251 checked, `release-il-0e96058fa007`).
- Remaining: Mcdepman `DepTrackingEnv::Lookup` is being implemented on `feature/luna-mcdepman`; its 12 gotos will be handled in a follow-up landing after that work settles.
