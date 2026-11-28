using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Documents;
using Quire.Mirror;

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

    private static readonly Dictionary<Type, char> e;

    // Mathcad clears initlocals on this static constructor.
    [System.Runtime.CompilerServices.SkipLocalsInit]
    static FormattedString()
    {
        e = new Dictionary<Type, char>
        {
            { typeof(Bold), 'B' },
            { typeof(Italic), 'I' },
            { typeof(Underline), 'U' },
            { typeof(Quire.Wpf.Subscript), '₊' }
        };
    }

    public char this[int position]
    {
        get
        {
            if (position < 0 || position >= Length)
            {
                throw new IndexOutOfRangeException("Position was out of range. Must be non-negative and less than the length of the FormattedString.");
            }
            a(WpfTextBlock.Inlines.FirstInline, position, out Run run, out int offset);
            return run.Text[offset];
        }
        set
        {
            if (position < 0 || position >= Length)
            {
                throw new IndexOutOfRangeException("Position was out of range. Must be non-negative and less than the length of the FormattedString.");
            }
            a(WpfTextBlock.Inlines.FirstInline, position, out Run run, out int offset);
            StringBuilder text = new StringBuilder(run.Text);
            text[offset] = value;
            run.Text = text.ToString();
        }
    }

    public void Insert(int insertAt, string stringToInsert, int subscriptDepth)
    {
        if (string.IsNullOrEmpty(stringToInsert))
        {
            return;
        }
        a(WpfTextBlock.Inlines.FirstInline, insertAt, out Run run, out int offset);
        if (run == null)
        {
            if (insertAt > offset)
            {
                throw new ArgumentOutOfRangeException("insertAt", insertAt, "Logic Error: insertAt was greater than the length of the FormattedString");
            }
            Span root = (Span)WpfTextBlock.Inlines.FirstInline;
            if (root.Inlines.Count == 0)
            {
                root.Inlines.Add(new Run());
            }
            run = a(WpfTextBlock.Inlines.FirstInline);
            offset = run.Text.Length;
        }
        int depth = b(run);
        if (depth > subscriptDepth)
        {
            Span span = (Span)run.Parent;
            for (int level = depth; level > subscriptDepth; level--)
            {
                span = (Span)span.Parent;
            }
            Inline previousSibling = a(span, run, offset);
            run = new Run();
            offset = 0;
            span.Inlines.InsertAfter(previousSibling, run);
        }
        else if (depth < subscriptDepth)
        {
            run = a(run, offset, subscriptDepth - depth);
            offset = 0;
        }
        string text = run.Text.Insert(offset, stringToInsert);
        run.Text = text;
        b((Span)WpfTextBlock.Inlines.FirstInline);
        j += stringToInsert.Length;
    }

    public void Append(FormattedString stringToAppend)
    {
        if (stringToAppend.IsSimpleString)
        {
            Append(stringToAppend.Text);
            return;
        }
        FormattedString copy = stringToAppend.Clone();
        int length = copy.Length;
        Span span = (Span)WpfTextBlock.Inlines.FirstInline;
        foreach (Inline inline in (IEnumerable<Inline>)new List<Inline>(((Span)copy.WpfTextBlock.Inlines.FirstInline).Inlines))
        {
            span.Inlines.Add(inline);
        }
        j += length;
        b(span);
    }

    public void Append(string stringToAppend)
    {
        Span span = (Span)WpfTextBlock.Inlines.FirstInline;
        if (span.Inlines.LastInline is Run run)
        {
            run.Text += stringToAppend;
        }
        else
        {
            span.Inlines.Add(new Run(stringToAppend));
        }
        j += stringToAppend.Length;
    }

    public void RemoveRange(int startIndex, int endIndex)
    {
        if (startIndex < 0 || startIndex >= Length)
        {
            throw new ArgumentOutOfRangeException("startIndex");
        }
        if (endIndex < 0 || endIndex >= Length)
        {
            throw new ArgumentOutOfRangeException("endIndex");
        }
        if (startIndex > endIndex)
        {
            int swap = endIndex;
            endIndex = startIndex;
            startIndex = swap;
        }
        a(WpfTextBlock.Inlines.FirstInline, startIndex, out Run run, out int offset);
        if (run == null)
        {
            throw new InvalidOperationException("Logic Error: we should have thrown an ArgumentOutOfRangeException if this condition existed.");
        }
        int remaining = endIndex - startIndex + 1;
        int removed = Math.Min(remaining, run.Text.Length - offset);
        string text = run.Text.Remove(offset, removed);
        run.Text = text;
        for (remaining -= removed; remaining != 0; remaining -= removed)
        {
            run = a(run);
            removed = Math.Min(remaining, run.Text.Length);
            text = run.Text.Remove(0, removed);
            run.Text = text;
        }
        b((Span)WpfTextBlock.Inlines.FirstInline);
        j -= endIndex - startIndex + 1;
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    public FormattedString Substring(int startIndex, int length)
    {
        if (startIndex < 0 || startIndex > Length)
        {
            throw new ArgumentOutOfRangeException("startIndex");
        }
        if (length < 0 || startIndex + length > Length)
        {
            throw new ArgumentOutOfRangeException("length");
        }
        if (length == 0)
        {
            if (!IsEditable)
            {
                return CreateReadOnly();
            }
            return CreateEditable();
        }
        FormattedString result = Clone();
        result.Remove(startIndex + length, Length - (startIndex + length));
        result.Remove(0, startIndex);
        return result;
    }

    public int GetSubscriptDepthAtPosition(int position)
    {
        if (WpfTextBlock.Inlines.Count == 0)
        {
            return -1;
        }
        if (WpfTextBlock.Inlines.Count > 1)
        {
            throw new InvalidOperationException("Logic Error: The FormattedString's internal TextBlock is only expected to contain a single Inline");
        }
        a(WpfTextBlock.Inlines.FirstInline, position, out Run run, out int unused);
        if (run == null)
        {
            return -1;
        }
        return b(run);
    }

    public System.Windows.Documents.TextRange SelectCharacterAtPosition(int position)
    {
        if (WpfTextBlock.Inlines.Count == 0)
        {
            return new System.Windows.Documents.TextRange(WpfTextBlock.ContentStart, WpfTextBlock.ContentEnd);
        }
        if (WpfTextBlock.Inlines.Count > 1)
        {
            throw new InvalidOperationException("Logic Error: The FormattedString's internal TextBlock is only expected to contain a single Inline");
        }
        a(WpfTextBlock.Inlines.FirstInline, position, out Run run, out int offset);
        TextPointer start = run.ContentStart.GetPositionAtOffset(offset);
        TextPointer end = start.GetPositionAtOffset(1);
        return new System.Windows.Documents.TextRange(start, end);
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    public FormattedString Clone()
    {
        if (!IsEditable)
        {
            FormattedString copy = CreateReadOnly();
            copy.g = g;
            copy.h = h;
            copy.i = i;
            copy.f = f;
            copy.j = j;
            return copy;
        }
        return CreateEditable((Span)((System.Windows.Controls.TextBlock)System.Windows.Markup.XamlReader.Parse(System.Windows.Markup.XamlWriter.Save(WpfTextBlock).Replace("{}{", "{"))).Inlines.FirstInline);
    }

    private void a(Inline A_0, StringBuilder A_1, bool A_2)
    {
        char? marker = b(A_0);
        if (A_2 && marker.HasValue)
        {
            A_1.Append(c);
            A_1.Append(marker.Value);
        }
        if (A_0 is Run run)
        {
            A_1.Append(run.Text);
        }
        else if (A_0 is Span span)
        {
            foreach (Inline inline in span.Inlines)
            {
                a(inline, A_1, A_2);
            }
        }
        if (A_2 && marker.HasValue)
        {
            A_1.Append(d);
            A_1.Append(marker.Value);
        }
    }

    private Inline b(string A_0)
    {
        int markerIndex = A_0.IndexOf(c);
        if (markerIndex == -1)
        {
            return new Run(A_0);
        }
        bool wrapped = markerIndex == 0 && a(A_0) == A_0;
        Span span = wrapped ? a(A_0[markerIndex + 1]) : new Span();
        int trim = wrapped ? 2 : 0;
        int position = trim;
        A_0 = A_0.Substring(0, A_0.Length - trim);
        while (position < A_0.Length)
        {
            string piece = a(A_0.Substring(position));
            position += piece.Length;
            span.Inlines.Add(b(piece));
        }
        return span;
    }

    private Span a(char A_0)
    {
        Type type = typeof(Span);
        foreach (KeyValuePair<Type, char> entry in e)
        {
            if (A_0 == entry.Value)
            {
                type = entry.Key;
            }
        }
        return (Span)Activator.CreateInstance(type);
    }

    private int a(string A_0, int A_1)
    {
        if (A_0[A_1] != c)
        {
            throw new ArgumentException("Logic error! Need to start on a format marker to find a match.");
        }
        int depth = 1;
        char marker = A_0[A_1 + 1];
        for (int index = A_1 + 1; index < A_0.Length; index++)
        {
            if (A_0[index] == d && A_0[index + 1] == marker)
            {
                depth--;
            }
            else if (A_0[index] == c && A_0[index + 1] == marker)
            {
                depth++;
            }
            if (depth == 0)
            {
                return index + 1;
            }
        }
        throw new ArgumentException("Logic error! Unable to find a matching format marker, which implies a malformed string.");
    }

    internal static void b(Span A_0)
    {
        IList<Inline> children = new List<Inline>(A_0.Inlines);
        for (int i = 0; i < children.Count; i++)
        {
            Inline inline = children[i];
            Span span = inline as Span;
            if (span != null)
            {
                b(span);
                if (span.Inlines.Count == 0)
                {
                    A_0.Inlines.Remove(span);
                    children.RemoveAt(i--);
                    continue;
                }
            }
            Run run = inline as Run;
            if (run != null && run.Text.Length == 0)
            {
                A_0.Inlines.Remove(run);
                children.RemoveAt(i--);
            }
            else
            {
                if (i <= 0 || !a(inline, children[i - 1]))
                {
                    continue;
                }
                if (run != null)
                {
                    ((Run)children[i - 1]).Text += run.Text;
                    A_0.Inlines.Remove(run);
                    children.RemoveAt(i--);
                }
                else
                {
                    if (span == null)
                    {
                        continue;
                    }
                    List<Inline> moved = new List<Inline>(span.Inlines);
                    Span previous = (Span)children[i - 1];
                    foreach (Inline item in (IEnumerable<Inline>)moved)
                    {
                        previous.Inlines.Add(item);
                    }
                    A_0.Inlines.Remove(span);
                    children.RemoveAt(i--);
                    b(previous);
                }
            }
        }
    }

    private static void a(Inline A_0, int A_1, out Run A_2, out int A_3)
    {
        if (A_0 is Run run)
        {
            if (run.Text.Length > A_1)
            {
                A_2 = run;
                A_3 = A_1;
            }
            else
            {
                A_2 = null;
                A_3 = run.Text.Length;
            }
            return;
        }
        if (A_0 is Span span)
        {
            int length = 0;
            foreach (Inline inline in span.Inlines)
            {
                a(inline, A_1 - length, out A_2, out A_3);
                if (A_2 != null)
                {
                    return;
                }
                length += A_3;
            }
            A_2 = null;
            A_3 = length;
            return;
        }
        throw new ArgumentException("Inline was an unexpected type (not Run or Span)", "inlineToSearch");
    }

    private static Run a(Run A_0)
    {
        Inline next = null;
        for (Inline inline = A_0; inline != null; inline = inline.Parent as Inline)
        {
            next = inline.NextInline;
            if (next != null)
            {
                break;
            }
        }
        if (next == null)
        {
            return null;
        }
        a(next, 0, out Run run, out int unused);
        return run;
    }

    private Inline a(Span A_0, Run A_1, int A_2)
    {
        Run tail = new Run
        {
            Text = A_1.Text.Substring(A_2)
        };
        A_1.Text = A_1.Text.Substring(0, A_2);
        Inline left = A_1;
        Inline right = tail;
        for (Span span = (Span)left.Parent; span != A_0; span = (Span)left.Parent)
        {
            Span clone = a(span);
            clone.Inlines.Add(right);
            bool found = false;
            foreach (Inline item in (IEnumerable<Inline>)new List<Inline>(span.Inlines))
            {
                if (found || item == left)
                {
                    if (item == left)
                    {
                        found = true;
                    }
                    else
                    {
                        clone.Inlines.Add(item);
                    }
                }
            }
            left = span;
            right = clone;
        }
        A_0.Inlines.InsertAfter(left, right);
        return left;
    }

    private static Run a(Run A_0, int A_1, int A_2)
    {
        if (A_2 <= 0)
        {
            throw new ArgumentOutOfRangeException("subscriptDepth", "The subscript depth must be greater than 0.");
        }
        Run tail = new Run();
        tail.Text = A_0.Text.Substring(A_1);
        A_0.Text = A_0.Text.Substring(0, A_1);
        Span span = (Span)A_0.Parent;
        span.Inlines.InsertAfter(A_0, tail);
        Quire.Wpf.Subscript subscript;
        Run created;
        if (A_2 > 0)
        {
            subscript = new Quire.Wpf.Subscript();
            span.Inlines.InsertAfter(A_0, subscript);
            span = subscript;
        }
        for (int i = 1; i < A_2; i++)
        {
            subscript = new Quire.Wpf.Subscript();
            span.Inlines.Add(subscript);
            span = subscript;
        }
        created = new Run();
        span.Inlines.Add(created);
        return created;
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    private static Span a(Span A_0)
    {
        if (A_0.GetType() == typeof(Span))
        {
            return new Span();
        }
        if (A_0.GetType() == typeof(Quire.Wpf.Subscript))
        {
            return new Quire.Wpf.Subscript();
        }
        if (A_0.GetType() == typeof(Bold))
        {
            return new Bold();
        }
        if (A_0.GetType() == typeof(Italic))
        {
            return new Italic();
        }
        if (A_0.GetType() == typeof(Underline))
        {
            return new Underline();
        }
        throw new NotImplementedException(string.Format("Logic Error: {0} is unexpected, and couldn't be cloned.", A_0.GetType()));
    }

    private bool? a(a_ A_0, InlineCollection A_1, Func<string, int> A_2, bool A_3)
    {
        bool? found = false;
        bool atStart = A_3 && !(A_1.FirstInline is Quire.Wpf.Subscript);
        foreach (Inline inline in A_1)
        {
            found = a(A_0, inline, A_2, atStart);
            atStart = false;
            if (found != false)
            {
                break;
            }
        }
        return found;
    }

    private bool? a(a_ A_0, Inline A_1, Func<string, int> A_2, bool A_3)
    {
        if (DebugTools.AssertNotNull(A_1, null).Failed)
        {
            return null;
        }
        bool isSearchScope = Quire.Wpf.InlineExtensions.GetIsSearchScope(A_1);
        if (isSearchScope)
        {
            A_0.a();
        }
        bool? result;
        switch (A_1)
        {
            case Run run:
                result = A_0.a(run, A_2, A_3);
                break;
            case Span span:
                result = a(A_0, span.Inlines, A_2, A_3);
                break;
            default:
                result = null;
                DebugTools.FailAssertion(A_1.GetType().Name);
                break;
        }
        if (isSearchScope && result != true)
        {
            A_0.a();
        }
        return result;
    }

    private bool a(Func<string, int> A_0, bool A_1, out a_ A_2)
    {
        A_2 = new a_();
        return a(A_2, WpfTextBlock.Inlines, A_0, A_1) == true;
    }

    public bool SearchValueInFormattedStringScopes(Func<string, int> searchFunc)
    {
        a_ scopes;
        return a(searchFunc, false, out scopes);
    }

    private static void a(IEnumerable<Run> A_0, string A_1, string A_2, in int A_3)
    {
        if (DebugTools.DevAssert(A_0.IsNotEmpty(out IList<Run> runs), null).Failed)
        {
            return;
        }
        int replaced = 0;
        int overlap = Math.Min(A_1.Length, A_2.Length);
        int removed = 0;
        int surplus = Math.Max(0, A_1.Length - A_2.Length);
        for (int i = 0; i < runs.Count; i++)
        {
            Run run = runs[i];
            StringBuilder text = new StringBuilder(run.Text);
            int index = i == 0 ? A_3 : 0;
            while (index < text.Length && replaced < overlap)
            {
                text[index] = A_2[replaced];
                index++;
                replaced++;
            }
            if (replaced == overlap)
            {
                int toRemove = surplus - removed;
                if (toRemove > 0 && index < text.Length)
                {
                    int count = Math.Min(toRemove, text.Length - index);
                    text = text.Remove(index, count);
                    removed += count;
                }
                else if (toRemove == 0 && A_2.Length > A_1.Length)
                {
                    text.Insert(index, A_2.Substring(A_1.Length));
                    DebugTools.DevAssert(i == runs.Count - 1, null);
                }
            }
            run.Text = text.ToString();
        }
        DebugTools.DevAssert(replaced == overlap && removed == surplus, null);
    }

    private static FormattedString a(FormattedString A_0, string A_1, string A_2, Func<string, int> A_3, bool A_4, out int A_5)
    {
        A_5 = -1;
        if (DebugTools.AssertNotNull(A_1, null).Failed)
        {
            return null;
        }
        FormattedString result = A_0.Clone();
        if (result.a(A_3, A_4, out a_ scopes))
        {
            a(scopes.a(out int start, out A_5), A_1, A_2, in start);
            b(result.WpfTextBlock.Inlines.FirstInline as Span);
            result.j = A_0.j + A_2.Length - A_1.Length;
            DebugTools.DevAssert(result.j == result.Text.Length, "Wrong new string length");
        }
        else
        {
            result = null;
        }
        return result;
    }

    public static FormattedString ReplaceStringInFormattedString(FormattedString formattedString, Quire.Controls.Core.FindReplaceQuery findReplaceQuery, out int position)
    {
        bool searchSubscripts = !SExpressionHelperUtilites.IsIdentifierSExpr(findReplaceQuery.ValueToReplaceWith);
        bool matchCase = findReplaceQuery.FindReplaceOption.HasFlag(Quire.Controls.Core.FindOptions.MatchCase);
        string valueToFind = findReplaceQuery.ValueToFind;
        string valueToReplaceWith = findReplaceQuery.ValueToReplaceWith;
        Func<string, int> search;
        if (valueToFind.Equals(valueToReplaceWith, StringComparison.OrdinalIgnoreCase) && !matchCase)
        {
            search = Quire.Controls.Core.FindUtilities.CreateCaseInsensitiveSearchFunctionForGivenString(valueToFind, valueToReplaceWith, findReplaceQuery.FindReplaceOption.HasFlag(Quire.Controls.Core.FindOptions.WholeWordOnly));
        }
        else
        {
            search = Quire.Controls.Core.FindUtilities.CreateSearchFunctionForGivenString(valueToFind, matchCase, findReplaceQuery.FindReplaceOption.HasFlag(Quire.Controls.Core.FindOptions.WholeWordOnly));
        }
        return a(formattedString, valueToFind, valueToReplaceWith, search, searchSubscripts, out position);
    }

    public static FormattedString ReplaceStringInFormattedStringWithSubscripts(FormattedString formattedString, Quire.Controls.Core.FindReplaceQuery findReplaceQuery, out int position)
    {
        position = -1;
        if (formattedString == null)
        {
            return null;
        }
        FormattedString find = CreateFormattedStringFromSubscriptsList(findReplaceQuery.ValueToFind, findReplaceQuery.FindSubscriptPositions);
        FormattedString replace = CreateFormattedStringFromSubscriptsList(findReplaceQuery.ValueToReplaceWith, findReplaceQuery.ReplaceSubscriptPositions);
        position = a(formattedString, findReplaceQuery, find, replace);
        FormattedString result = null;
        if (position >= 0)
        {
            result = formattedString.Substring(0, position);
            result.Append(replace);
            int end = position + findReplaceQuery.ValueToFind.Length;
            if (end < formattedString.Length)
            {
                result.Append(formattedString.Substring(end));
            }
            DebugTools.DevAssert(result.j == result.Text.Length, "Wrong new string length");
        }
        return result;
    }

    public static FormattedString CreateFormattedStringFromSubscriptsList(string text, List<Quire.Pair<int, int>> subscriptPositions)
    {
        FormattedString result = CreateEditable();
        string plain;
        string subscripted;
        int textIndex = 0;
        int insertAt = 0;
        foreach (Quire.Pair<int, int> subscript in subscriptPositions)
        {
            plain = text.Substring(textIndex, subscript.First - textIndex);
            subscripted = text.Substring(subscript.First, subscript.Second - subscript.First + 1);
            result.Insert(insertAt, plain, 0);
            insertAt += plain.Length;
            result.Insert(insertAt, subscripted, 1);
            insertAt += subscripted.Length;
            textIndex = subscript.Second + 1;
        }
        if (textIndex < text.Length)
        {
            result.Insert(insertAt, text.Substring(textIndex), 0);
        }
        return result;
    }

    private static int a(FormattedString A_0, Quire.Controls.Core.FindReplaceQuery A_1, FormattedString A_2, FormattedString A_3)
    {
        bool wholeWord = A_1.FindReplaceOption.HasFlag(Quire.Controls.Core.FindOptions.WholeWordOnly);
        StringComparison comparison = A_1.FindReplaceOption.HasFlag(Quire.Controls.Core.FindOptions.MatchCase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        if (wholeWord && A_1.IsFindContainSubscript && !A_1.FindIsFullySubscript && A_2.Equals(A_0, comparison))
        {
            return 0;
        }
        for (int i = 0; i <= A_0.Length - A_2.Length; i++)
        {
            FormattedString candidate = A_0.Substring(i, A_2.Length);
            bool isReplacement = candidate.Equals(A_3, StringComparison.Ordinal);
            if (!A_2.Text.Equals(candidate.Text, comparison) || isReplacement)
            {
                continue;
            }
            bool notWholeWord = wholeWord && !a(A_0, i, candidate.Length);
            if (A_2.Equals(candidate, comparison) && !notWholeWord)
            {
                return i;
            }
            FormattedString subscripted = CreateEditable();
            subscripted.Insert(0, A_2.Text, 1);
            if (subscripted.Equals(candidate, comparison) && !notWholeWord)
            {
                if (A_1.ReplaceIsFullySubscript)
                {
                    return i;
                }
                A_1.Status = Quire.Controls.Core.FindReplaceStatus.InvalidOperation;
                return -1;
            }
        }
        return -1;
    }

    private static bool a(FormattedString A_0, int A_1, int A_2)
    {
        int startDepth = A_0.GetSubscriptDepthAtPosition(A_1);
        int endDepth = A_0.GetSubscriptDepthAtPosition(A_1 + A_2 - 1);
        if (A_1 <= 0 || A_0.GetSubscriptDepthAtPosition(A_1 - 1) != startDepth)
        {
            if (A_1 + A_2 < A_0.Length)
            {
                return A_0.GetSubscriptDepthAtPosition(A_1 + A_2) != endDepth;
            }
            return true;
        }
        return false;
    }

    public bool Contains(FormattedString formattedString, StringComparison comparison, bool wholeWordOnly)
    {
        for (int i = 0; i <= Length - formattedString.Length; i++)
        {
            FormattedString candidate = Substring(i, formattedString.Length);
            if (formattedString.Equals(candidate, comparison))
            {
                if (wholeWordOnly)
                {
                    return a(this, i, candidate.Length);
                }
                return true;
            }
        }
        return false;
    }

    partial class a_
    {
        [MirrorName("a")]
        private readonly List<Run> a__ = new List<Run>();

        [MirrorName("b")]
        private readonly StringBuilder b_ = new StringBuilder();

        private int c;

        [MirrorVerified("Release IL")]
        private int d = -1;

        public IEnumerable<Run> a(out int A_0, out int A_1)
        {
            A_0 = b();
            A_1 = c - b_.Length + b();
            if (b() == -1)
            {
                return Enumerable.Empty<Run>();
            }
            int i;
            for (i = 0; i < a__.Count; i++)
            {
                if (a__[i].Text.Length > A_0)
                {
                    break;
                }
                A_0 -= a__[i].Text.Length;
            }
            c = 0;
            if (!DebugTools.DevAssert(i < a__.Count, null).Passed)
            {
                return Enumerable.Empty<Run>();
            }
            return a__.Skip(i);
        }

        public bool a(Run A_0, Func<string, int> A_1, bool A_2)
        {
            DebugTools.DevAssert(b() == -1, null);
            a__.Add(A_0);
            b_.Append(A_0.Text);
            c += A_0.Text.Length;
            if (DebugTools.AssertNotNull(A_1, null).Passed)
            {
                string text = b_.ToString();
                if (A_2)
                {
                    int index = text.IsNullOrEmpty() ? -1 : A_1(text.Substring(1));
                    a(index == -1 ? -1 : index + 1);
                }
                else
                {
                    a(A_1(text));
                }
            }
            return b() != -1;
        }

        public void a()
        {
            b_.Clear();
            a__.Clear();
            a(-1);
        }

        [MirrorVerified("Release IL")]
        private int b() => d;

        [MirrorVerified("Release IL")]
        private void a(int index) => d = index;
    }
}
