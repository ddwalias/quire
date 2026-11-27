#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Quire;

public partial class FindControl
{
    internal global::DevComponents.WpfRibbon.ButtonDropDown FindPreviousButton;
    internal global::DevComponents.WpfRibbon.ButtonDropDown FindNextButton;
    internal global::DevComponents.WpfRibbon.ButtonDropDown ReplaceButton;

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _this = (FindControl)target;
                return;
            case 2:
                FindRichBox = (FindReplaceRichTextBox)target;
                FindRichBox.TextChanged += FindRichText_TextChanged;
                FindRichBox.PropertyChanged += FindReplaceRichText_PropertyChanged;
                FindRichBox.GotKeyboardFocus += FindReplaceRichText_GotFocus;
                FindRichBox.LostKeyboardFocus += FindReplaceRichText_LostFocus;
                FindRichBox.PreviewKeyDown += FindReplaceRichText_PreviewKeyDown;
                return;
            case 3:
                FindPreviousButton = (global::DevComponents.WpfRibbon.ButtonDropDown)target;
                return;
            case 4:
                FindNextButton = (global::DevComponents.WpfRibbon.ButtonDropDown)target;
                return;
            case 5:
                ReplaceWithRichText = (FindReplaceRichTextBox)target;
                ReplaceWithRichText.TextChanged += ReplaceRichText_TextChanged;
                ReplaceWithRichText.PropertyChanged += FindReplaceRichText_PropertyChanged;
                ReplaceWithRichText.GotKeyboardFocus += FindReplaceRichText_GotFocus;
                ReplaceWithRichText.LostKeyboardFocus += FindReplaceRichText_LostFocus;
                ReplaceWithRichText.PreviewKeyDown += FindReplaceRichText_PreviewKeyDown;
                return;
            case 6:
                ReplaceButton = (global::DevComponents.WpfRibbon.ButtonDropDown)target;
                return;
            case 7:
                ReplaceButtonTooltipTextBlock = (TextBlock)target;
                return;
        }
        _contentLoaded = true;
    }
}
