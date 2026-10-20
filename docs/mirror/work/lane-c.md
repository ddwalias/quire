# Lane C — Quire.Utils

- Scope: all managed members in `src/Quire.Utils`, including former C/D tier members (D18).
- Status: completed; full exact Mathcad Release IL parity achieved for all managed members in Quire.Utils.
- Worktree: `C:/Projects/Quire-wt/lane-c-utils`.
- Branch: `mirror/c-utils-20261021-quire-utils`.
- Original assembly: utilsmgd.
- Selected members: full assembly inventory:
  - `Quire.Utils.IBMPFormatter` (marker interface)
  - `Quire.Utils.IPNGFormatter` (marker interface)
  - `Quire.Utils.IImageConverter::ImageStreamToImageStream` (exact declaration)
  - `Quire.Utils.IJPEGFormatter::get_Quality` (exact declaration)
  - `Quire.Utils.IJPEGFormatter::set_Quality` (exact declaration)
  - `Quire.Utils.ITransformHelper::Base64JPGToBase64PNG` (exact declaration)
  - `Quire.Utils.BMPFormatter::get_MimeType` (exact Release IL)
  - `Quire.Utils.BMPFormatter::.ctor` (exact Release IL)
  - `Quire.Utils.PNGFormatter::get_MimeType` (exact Release IL)
  - `Quire.Utils.PNGFormatter::.ctor` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::get_MimeType` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::get_EncoderParams` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::get_Quality` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::set_Quality` (exact Release IL)
  - `Quire.Utils.JPEGFormatter::.ctor` (exact Release IL)
  - `Quire.Utils.ImageFormatter::get_Encoder` (exact Release IL)
  - `Quire.Utils.ImageFormatter::get_Format` (exact Release IL)
  - `Quire.Utils.ImageFormatter::get_EncoderParams` (exact Release IL)
  - `Quire.Utils.ImageFormatter::GetEncoderInfo` (exact Release IL)
  - `Quire.Utils.ImageFormatter::.ctor` (exact Release IL)
  - `Quire.Utils.TransformHelper::Base64JPGToBase64PNG` (exact Release IL)
  - `Quire.Utils.TransformHelper::.ctor` (exact Release IL)
  - `Quire.Utils.ImageConverter::SaveImageToStream` (exact Release IL)
  - `Quire.Utils.ImageConverter::GetStream` (exact Release IL)
  - `Quire.Utils.ImageConverter::ImageStreamToImageStream` (exact Release IL)
  - `Quire.Utils.ImageConverter::.ctor` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::get_CanRead` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::get_CanSeek` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::get_CanWrite` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::get_Length` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::get_Position` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::set_Position` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::.ctor` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::CheckDisposed` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Read` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Write` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Seek` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::SetLength` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Flush` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Dispose` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::CleanUp` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Commit` (exact Release IL)
  - `Quire.Utils.COMStreamAdaptor::Close` (exact Release IL)
- Release IL report: PASS; 37 checked; 37 exact bodies; 4 exact declarations; 0 unfinished remaining; 0 failures.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
All former reservations are released. Preserve previously exact methods, land passing
batches and continue independently around blockers. Only this assembly and checkpoint
belong to this lane; shared tools/mappings belong to the maintainer. Never inspect Debug IL.
Native/unreadable counterparts remain incomplete. Do not spawn agents or threads.
