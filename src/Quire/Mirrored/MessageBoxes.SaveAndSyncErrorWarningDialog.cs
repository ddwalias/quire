#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core.Dialogs;

namespace Quire.MessageBoxes;

public partial class SaveAndSyncErrorWarningDialog
{
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        ((DependencyObject)this).ClearValue(Window.SizeToContentProperty);
        ((DependencyObject)this).ClearValue(FrameworkElement.MaxHeightProperty);
    }

    protected override TextBlock Message => null;

    protected override Image Image => _image;

    protected override ItemsControl Buttons => _buttons;

    public void InitializeComponent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    void global::System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
