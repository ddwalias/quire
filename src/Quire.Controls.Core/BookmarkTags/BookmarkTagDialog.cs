#nullable disable
using System.Windows;
using System.Windows.Input;
using Quire.Core;

namespace Quire.Controls.Core.BookmarkTags;

public partial class BookmarkTagDialog
{
    private BookmarkTagViewModel ViewModel { get; }

    public static bool ShowBookmarkTagsDialog(BookmarkTagViewModel viewModel, Window owner)
    {
        BookmarkTagDialog bookmarkTagDialog = new BookmarkTagDialog(viewModel);
        if (owner != null)
        {
            bookmarkTagDialog.Owner = owner;
        }
        return bookmarkTagDialog.ShowDialog() == true;
    }

    private BookmarkTagDialog(BookmarkTagViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        if (viewModel.Tag.IsNotEmpty())
        {
            InsertTagTextBox.Text = ViewModel.Tag;
            InsertTagTextBox.Select(0, InsertTagTextBox.Text.Length);
        }
        FocusManager.SetFocusedElement(this, InsertTagTextBox);
        Loaded += (sender, e) =>
        {
            BookmarkTagDialogLoaded();
        };
    }

    private void BookmarkTagDialogLoaded()
    {
        DialogLoaded();
        InsertTagTextBox.ClearValue(FrameworkElement.MaxWidthProperty);
    }

    protected new void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        base.OnOkButtonClick(sender, e);
        ViewModel.Tag = InsertTagTextBox.Text;
    }
}
