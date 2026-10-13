using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;

namespace Quire.Controls;

/// <summary>A rich-text region backed by a WPF FlowDocument (stored as a Section XAML fragment).</summary>
public class TextRegionControl : RegionControl
{
    private readonly RichTextBox _box;
    private string _savedXaml = "";

    public TextRegionControl(TextRegionPersistentData? data = null)
    {
        data ??= new TextRegionPersistentData();
        RegionId = data.Id;
        _box = new RichTextBox
        {
            Width = data.Width,
            BorderThickness = new Thickness(0),
            Background = Brushes.Transparent,
            IsReadOnly = true,
            Focusable = false,
            IsHitTestVisible = false,
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 14.667,
            Padding = new Thickness(0),
            AcceptsTab = true,
        };
        _box.Document = new FlowDocument { PagePadding = new Thickness(0) };
        _box.PreviewKeyDown += HandleKeyDown;
        _box.LostKeyboardFocus += (_, e) =>
        {
            if (IsEditing && !(e.NewFocus is DependencyObject d && IsAncestorOf(d))) EndEdit(commit: true);
        };
        Body = _box;
        LoadXaml(data.SectionXaml);
        _savedXaml = SaveXaml();
    }

    public double TextWidth
    {
        get => _box.Width;
        set => _box.Width = value;
    }

    public override bool IsEmpty => string.IsNullOrWhiteSpace(new TextRange(_box.Document.ContentStart, _box.Document.ContentEnd).Text);

    public string PlainText => new TextRange(_box.Document.ContentStart, _box.Document.ContentEnd).Text.TrimEnd();

    private void LoadXaml(string xaml)
    {
        if (string.IsNullOrWhiteSpace(xaml)) return;
        var range = new TextRange(_box.Document.ContentStart, _box.Document.ContentEnd);
        try
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xaml));
            range.Load(ms, DataFormats.Xaml);
        }
        catch (Exception ex) when (ex is ArgumentException or System.Xml.XmlException or System.Windows.Markup.XamlParseException)
        {
            range.Text = xaml;
        }
    }

    private string SaveXaml()
    {
        var range = new TextRange(_box.Document.ContentStart, _box.Document.ContentEnd);
        using var ms = new MemoryStream();
        range.Save(ms, DataFormats.Xaml);
        return Encoding.UTF8.GetString(ms.ToArray());
    }

    public override void BeginEdit(string? initialText = null)
    {
        IsEditing = true;
        _box.IsReadOnly = false;
        _box.Focusable = true;
        _box.IsHitTestVisible = true;
        Dispatcher.BeginInvoke(() =>
        {
            _box.Focus();
            Keyboard.Focus(_box);
            if (!string.IsNullOrEmpty(initialText) && initialText != "\"") _box.CaretPosition.InsertTextInRun(initialText);
            _box.CaretPosition = _box.Document.ContentEnd;
        }, System.Windows.Threading.DispatcherPriority.Input);
    }

    public override void EndEdit(bool commit)
    {
        if (!IsEditing) return;
        IsEditing = false;
        _box.IsReadOnly = true;
        _box.Focusable = false;
        _box.IsHitTestVisible = false;
        var now = SaveXaml();
        if (now == _savedXaml) return;
        if (!commit)
        {
            _box.Document.Blocks.Clear();
            LoadXaml(_savedXaml);
            return;
        }
        _savedXaml = now;
        RaiseChanged();
    }

    public override void InsertText(string text)
    {
        if (IsEditing) _box.CaretPosition.InsertTextInRun(text);
    }

    private void HandleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            EndEdit(commit: true);
            RaiseLeave(LeaveDirection.Down);
            e.Handled = true;
        }
    }

    public override RegionPersistentData Save() =>
        new TextRegionPersistentData { Id = RegionId, Width = _box.Width, SectionXaml = SaveXaml() };
}
