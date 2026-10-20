# Lane C — Quire.Utils

- Scope: all managed members in `src/Quire.Utils`, including former C/D tier members (D18).
- Status: Batch 1 landed; moving to Batch 2.
- Worktree: `C:/Projects/Quire-wt/lane-c-utils`.
- Branch: `mirror/c-utils-20261021-quire-utils`.
- Original assembly: utilsmgd.
- Selected members:
  - `Quire.Utils.IImageConverter::ImageStreamToImageStream` (exact declaration)
  - `Quire.Utils.IJPEGFormatter::get_Quality` (exact declaration)
  - `Quire.Utils.IJPEGFormatter::set_Quality` (exact declaration)
  - `Quire.Utils.ITransformHelper::Base64JPGToBase64PNG` (exact declaration)
  - `Quire.Utils.BMPFormatter::get_MimeType` (exact Release IL)
  - `Quire.Utils.PNGFormatter::get_MimeType` (exact Release IL)
- Release IL report: PASS; 11 checked; 7 exact bodies; 4 exact declarations; 30 unfinished remaining.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only this assembly and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
