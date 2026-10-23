#nullable disable
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class EditorGestureConverter
{
    [MirrorVerified("PtcWpfEditorGestureConverterMechanicalDifferentialTests")]
    public EditorGestureConverter() { }

    [MirrorVerified("Release IL")]
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

    [MirrorVerified("Release IL")]
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        KeyGestureConverter keyGestureConverter = new KeyGestureConverter();
        try
        {
            if (keyGestureConverter.ConvertFrom(context, culture, value) is KeyGesture keyGesture && (int)keyGesture.Key != 0)
            {
                return new EditorGesture(keyGesture);
            }
        }
        catch
        {
        }
        string text = value.ToString();
        if (!DebugTools.DevAssert(text.IsNotEmpty(), null).Passed)
        {
            return null;
        }
        return new EditorGesture(text);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        EditorGesture editorGesture = value as EditorGesture;
        if (editorGesture == null)
        {
            return null;
        }
        if (editorGesture.KeyGesture != null)
        {
            return new KeyGestureConverter().ConvertTo(context, culture, editorGesture.KeyGesture, destinationType);
        }
        return editorGesture.CharsPressed;
    }
}
