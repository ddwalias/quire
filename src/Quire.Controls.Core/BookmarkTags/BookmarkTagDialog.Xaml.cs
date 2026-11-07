// Keeps XAML generated members separate so BookmarkTagDialog .ctor closure ordinal is b__4_0.
#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Quire.Mirror;

namespace Quire.Controls.Core.BookmarkTags;

public partial class BookmarkTagDialog
{
    internal TextBox InsertTagTextBox;

    internal Button OkButton;

    private bool _contentLoaded;

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Core;component/bookmarktags/bookmarktagdialog.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [MirrorVerified("PtcControlsCoreBookmarkTagsBookmarkTagDialogMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                InsertTagTextBox = (TextBox)target;
                break;
            case 2:
                OkButton = (Button)target;
                OkButton.Click += OnOkButtonClick;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
