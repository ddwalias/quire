#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.EquationEditor.Tree;
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

    public IOItem(DependencyObject region, string idsExpression, bool isIn)
    {
        _region = region;
        _idSExpression = idsExpression;
        _isIn = isIn;
        _idName = SExpressionHelperUtilites.ExtractIdOrFunctionDefinitionName(idsExpression);
    }

    public string Alias
    {
        get => IOManager.GetIOAlias(this);
        set
        {
            if (value != Alias)
            {
                if (!IOManager.SetIOAlias(this, value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Alias"));
                }
            }
        }
    }

    public void ShowInWorksheet()
    {
        if (!AttachedProperties.GetCanActivate(Region))
        {
            return;
        }
        if (IsInput)
        {
            ((IInputRegion)Region).ActivateAndScopeIdentifierPrivate(IdSexp);
        }
        else
        {
            ((IOutputRegion)Region).ActivateAndScopeResultPrivate();
        }
    }
}
