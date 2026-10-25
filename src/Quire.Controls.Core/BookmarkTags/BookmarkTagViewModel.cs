#nullable disable
using System;
using Quire;

namespace Quire.Controls.Core.BookmarkTags;

public partial class BookmarkTagViewModel : ViewModelBase
{
    private string _tag;

    public string Tag
    {
        get
        {
            return _tag;
        }
        set
        {
            if (value == null)
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
            _tag = value;
            OnPropertyChanged("Tag");
        }
    }

    public BookmarkTagViewModel(string currentTag)
    {
        Tag = currentTag;
    }
}
