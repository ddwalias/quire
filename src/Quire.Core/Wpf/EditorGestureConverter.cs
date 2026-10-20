#nullable disable
using System.ComponentModel;
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
}
