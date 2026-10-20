# Lane C — Quire.Utils

- Scope: all managed members in `src/Quire.Utils`, including former C/D tier members (D18).
- Status: Batch 2 ready to land; moving to Batch 3.
- Worktree: `C:/Projects/Quire-wt/lane-c-utils`.
- Branch: `mirror/c-utils-20261021-quire-utils`.
- Original assembly: utilsmgd.
- Selected members:
  - `Quire.Utils.JPEGFormatter::get_MimeType` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::get_EncoderParams` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::get_Quality` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::set_Quality` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::.ctor` (exact Release IL)
  - `Quire.Utils.ImageFormatter::get_Encoder` (exact Release IL)
  - `Quire.Utils.ImageFormatter::get_Format` (exact Release IL)
  - `Quire.Utils.ImageFormatter::get_EncoderParams` (exact Release IL)
  - `Quire.Utils.ImageFormatter::GetEncoderInfo` (exact Release IL)
  - `Quire.Utils.TransformHelper::Base64JPGToBase64PNG` (exact Release IL)
- Release IL report: PASS; 17 checked; 17 exact bodies; 4 exact declarations; 20 unfinished remaining.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only this assembly and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
