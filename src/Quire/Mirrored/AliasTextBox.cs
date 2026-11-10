#nullable disable

using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Quire;

public partial class AliasTextBox
{
    public event Action<TextBox> PreMouseDown;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (PreMouseDown != null)
        {
            PreMouseDown(this);
        }

        base.OnMouseDown(e);
    }
}
