#nullable disable
using System.ComponentModel;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.IO;

public partial class IOItem
{
    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    private readonly DependencyObject _region;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    public DependencyObject Region => _region;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    private readonly string _idSExpression;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    public string IdSexp => _idSExpression;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    private readonly string _idName;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    public string IdName => _idName;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    private readonly bool _isIn;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    public bool IsInput => _isIn;

    [MirrorVerified("PtcControlsIOIOItemMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;
}
