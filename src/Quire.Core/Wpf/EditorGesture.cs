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

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        if (this == obj)
        {
            return true;
        }
        if (obj.GetType() == typeof(EditorGesture))
        {
            return Equals((EditorGesture)obj);
        }
        return false;
    }

    public bool Equals(EditorGesture other)
    {
        if ((object)other == null)
        {
            return false;
        }
        if ((object)this == other)
        {
            return true;
        }
        if (object.Equals(other.CharsPressed, CharsPressed) && object.Equals(other.KeyPressed, KeyPressed))
        {
            return object.Equals(other.KeyGesture, KeyGesture);
        }
        return false;
    }

    public override string ToString()
    {
        string text = new EditorGestureConverter().ConvertTo(this, typeof(string)) as string;
        if (!DebugTools.AssertNotNull(text, null).Passed)
        {
            return string.Empty;
        }
        return text;
    }

    public override int GetHashCode()
    {
        return (((((CharsPressed != null) ? CharsPressed.GetHashCode() : 0) * 397) ^ KeyPressed.GetHashCode()) * 397)
            ^ ((KeyGesture != null) ? KeyGesture.GetHashCode() : 0);
    }

    [MirrorVerified("PtcWpfEditorGestureMechanicalDifferentialTests")]
    public static bool operator ==(EditorGesture left, EditorGesture right) => object.Equals(left, right);

    [MirrorVerified("Release IL")]
    public static bool operator !=(EditorGesture left, EditorGesture right) => !(left == right);
}
