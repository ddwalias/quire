#nullable disable
using System.Xml;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Native.PtcEquationEditor;

internal partial class af
{
    [MirrorVerified("Release IL")]
    public int b() => a("rows");

    [MirrorVerified("Release IL")]
    public void b(int index) => a("rows", index);

    [MirrorVerified("Release IL")]
    public int a() => a("cols");

    [MirrorVerified("Release IL")]
    public void a(int index) => a("cols", index);

    [MirrorVerified("Release IL")]
    public af(XmlElement xmlElement)
    {
        a_ = xmlElement;
    }
}
