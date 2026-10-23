#nullable disable
using System.Windows.Input;
using System.Windows.Input;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EditorGesture
{
    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public readonly string CharsPressed;

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public EditorGesture(string charsPressed)
    {
        CharsPressed = charsPressed;
    }

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public readonly Key KeyPressed;

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public EditorGesture(Key keyPressed)
    {
        KeyPressed = keyPressed;
    }

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public readonly KeyGesture KeyGesture;

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public EditorGesture(KeyGesture keyGesture)
    {
        KeyGesture = keyGesture;
    }

    public bool Matches(KeyEventArgs args)
    {
        if (KeyGesture != null && KeyGesture.Matches(null, args))
        {
            return true;
        }
        if (KeyPressed == args.Key)
        {
            return true;
        }
        return false;
    }

    public bool Matches(string charsPressed)
    {
        if (!string.IsNullOrEmpty(charsPressed))
        {
            return CharsPressed == charsPressed;
        }
        return false;
    }

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public static bool operator ==(EditorGesture left, EditorGesture right) => object.Equals(left, right);

    [MirrorVerified("Release IL")]
    public static bool operator !=(EditorGesture left, EditorGesture right) => !(left == right);
}
