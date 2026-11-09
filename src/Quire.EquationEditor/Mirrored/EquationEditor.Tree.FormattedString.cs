using System.Text;
using System.Windows.Documents;

namespace Quire.EquationEditor.Tree;

public partial class FormattedString
{
    private System.Windows.Controls.TextBlock f;
    private string g;
    private string h;
    private Span i;
    private int j;
    private bool k;

    public FormattedString()
    {
    }

    public static FormattedString CreateReadOnly()
    {
        FormattedString formatted = new FormattedString();
        DebugTools.DevAssert(!formatted.IsEditable, a__);
        return formatted;
    }

    public static FormattedString CreateEditable()
    {
        FormattedString formatted = CreateReadOnly();
        formatted.MakeEditable();
        return formatted;
    }

    public static FormattedString CreateReadOnlyFromXmlString(string inline)
    {
        FormattedString formatted = new FormattedString { h = inline };
        DebugTools.DevAssert(!formatted.IsEditable, a__);
        return formatted;
    }

    public static FormattedString CreateEditable(Span inline)
    {
        FormattedString formatted = new FormattedString { i = inline };
        DebugTools.DevAssert(!formatted.IsEditable, a__);
        formatted.MakeEditable();
        return formatted;
    }

    public static FormattedString CreateReadOnly(string source)
    {
        FormattedString formatted = new FormattedString { g = source, j = source.Length };
        DebugTools.DevAssert(!formatted.IsEditable, a__);
        return formatted;
    }

    public static FormattedString CreateEditable(string source)
    {
        FormattedString formatted = CreateReadOnly(source);
        formatted.MakeEditable();
        return formatted;
    }

    public static implicit operator FormattedString(string s) => CreateEditable(s);

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (obj == this)
            return true;
        if (obj.GetType() != typeof(FormattedString))
            return false;
        return Equals((FormattedString)obj);
    }

    public bool Equals(FormattedString other)
    {
        if (other == null)
            return false;
        if (other == this)
            return true;
        return object.Equals(other.ToString(), ToString());
    }

    public bool Equals(FormattedString other, StringComparison comparisonType)
    {
        if (other == null)
            return false;
        if (other == this)
            return true;
        return string.Equals(other.ToString(), ToString(), comparisonType);
    }

    public System.Windows.Controls.TextBlock WpfTextBlock
    {
        get
        {
            if (!DebugTools.DevAssert(IsEditable, "Cannot access TextBlock property while in ReadOnlyState.").Failed)
                return f;
            return null;
        }
    }

    public string XmlString
    {
        get
        {
            if (IsEditable)
                h = System.Windows.Markup.XamlWriter.Save(WpfTextBlock.Inlines.FirstInline);
            return h;
        }
    }

    public Span ContentForXaml
    {
        set
        {
            MakeEditable();
            WpfTextBlock.Inlines.Clear();
            WpfTextBlock.Inlines.Add(value);
            j = Text.Length;
        }
    }

    public string Text
    {
        get
        {
            if (!IsEditable)
                return g;
            StringBuilder text = new StringBuilder();
            DebugTools.DevAssert(WpfTextBlock.Inlines.Count < 2, null);
            a(WpfTextBlock.Inlines.FirstInline, text, false);
            return text.ToString();
        }
    }

    public bool IsEditable
    {
        get => k;
        // Mathcad names this private setter a(bool); its mapping remains unresolved.
        [Quire.Mirror.MirrorStub]
        private set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }


    public void Remove(int startIndex, int count)
    {
        if (count <= 0)
            return;
        RemoveRange(startIndex, startIndex + count - 1);
    }

    public FormattedString Substring(int startIndex) => Substring(startIndex, Length - startIndex);

    object System.ICloneable.Clone() => Clone();

    private string a(string text)
    {
        int subscriptStart = text.IndexOf(c);
        if (subscriptStart == -1)
            return text;
        if (subscriptStart != 0)
            return text.Substring(0, subscriptStart);
        return text.Substring(0, a(text, 0) + 1);
    }

    private char? b(Inline inline)
    {
        // Release IL returns the found marker before the empty nullable.
        if (e.TryGetValue(inline.GetType(), out char marker))
            return marker;
        return null;
    }

    private static int b(Run run)
    {
        if (run == null)
            throw new ArgumentNullException(nameof(run));
        int depth = 0;
        Inline inline = run;
        while (inline != null)
        {
            if (inline is Quire.Wpf.Subscript)
                depth++;
            inline = inline.Parent as Inline;
        }
        return depth;
    }

    private static bool a(Inline left, Inline right) => left.GetType() == right.GetType();

    private static Run a(Inline inlineToSearch)
    {
        Run run = inlineToSearch as Run;
        if (run != null)
            return run;
        // Release IL retains the span on the stack across the null check.
        Span span = inlineToSearch as Span;
        if (span == null)
            throw new ArgumentException("Inline was an unexpected type (not Run or Span)", nameof(inlineToSearch));
        return a(span.Inlines.LastInline);
    }

    private void a()
    {
        g = null;
        h = null;
        i = null;
    }


    public int Length
    {
        // Mathcad clears initlocals on this getter despite its fat method header.
        [System.Runtime.CompilerServices.SkipLocalsInit]
        get
        {
            if (!IsEditable)
                return j;
            if (j == 0 && ((Span)WpfTextBlock.Inlines.FirstInline).Inlines.Count != 0)
                j = Text.Length;
            return j;
        }
    }

    public bool IsSimpleString
    {
        get
        {
            if (!IsEditable)
                return g != null && h == null;
            Span content = (Span)WpfTextBlock.Inlines.FirstInline;
            return content.Inlines.Count == 0 ||
                (content.Inlines.Count == 1 && content.Inlines.FirstInline.GetType() == typeof(Run));
        }
    }

    public override int GetHashCode() => ToString().GetHashCode();

    public override string ToString()
    {
        if (!IsEditable)
        {
            if (!DebugTools.AssertNotNull(g, "ToString() method shouldn't be used on read-only, non-simple formatted strings, as the result isn't defined.").Passed)
                return string.Empty;
            return g;
        }
        StringBuilder text = new StringBuilder();
        DebugTools.DevAssert(WpfTextBlock.Inlines.Count < 2, null);
        a(WpfTextBlock.Inlines.FirstInline, text, true);
        return text.ToString();
    }
}
