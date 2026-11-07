#nullable disable
#pragma warning disable

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quire.Controls.Text;

public partial class TextRegionModel
{
    public static readonly DependencyProperty TextProperty;
    public static readonly DependencyProperty DocumentProperty;
    public static readonly RoutedEvent TextChangedEvent;

    static TextRegionModel()
    {
        TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextRegionModel), new PropertyMetadata(string.Empty));
        DocumentProperty = DependencyProperty.Register("Document", typeof(MemoryStream), typeof(TextRegionModel), new PropertyMetadata(null, HandleDocumentChanged));
        TextChangedEvent = TextBoxBase.TextChangedEvent.AddOwner(typeof(TextRegionModel));
    }

    public string Text
    {
        get => GetValue(TextProperty).ToString();
        set => SetValue(TextProperty, value);
    }

    public MemoryStream Document
    {
        get => (MemoryStream)GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    private static void HandleDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue && d is ContentElement)
        {
            (d as ContentElement).RaiseEvent(new TextChangedEventArgs(TextChangedEvent, UndoAction.None));
        }
    }

    public TextRegionModel()
    {
    }
}
