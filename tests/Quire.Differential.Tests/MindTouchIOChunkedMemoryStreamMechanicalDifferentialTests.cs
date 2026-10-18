namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>MindTouch.IO.ChunkedMemoryStream</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/MindTouch.IO.ChunkedMemoryStream.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MindTouchIOChunkedMemoryStreamMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060000f4() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x060000f4);

    [SkippableFact]
    public void ctor_060000f6() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x060000f6);

    [SkippableFact]
    public void ctor_060000f8() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x060000f8);

    [SkippableFact]
    public void ctor_060000f9() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x060000f9);

    [SkippableFact]
    public void Flush_06000103() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x06000103);

    [SkippableFact]
    public void ChunkSize_get_060000f1() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x060000f1);

    [SkippableFact]
    public void ChunkSize_set_060000f2() => Mechanical.Compare(typeof(global::Quire.MindTouch.IO.ChunkedMemoryStream), 0x060000f2);
}
