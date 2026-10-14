using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Run;

public class ConsTests
{
    [Fact]
    public void Head_tail_length_and_format_follow_cons_links()
    {
        var last = new Cons(2, null);
        var first = new Cons(1, last);
        Assert.Equal(1, first.head);
        Assert.Same(last, first.tail);
        Assert.Equal(2, Cons.Length(first));
        Assert.Equal(0, Cons.Length(null));
        Assert.Equal("[1, 2]", first.ToString());
        Assert.Null(new Cons(1, "bad").tail);
        Assert.Equal(1, Cons.Length(new Cons(1, "bad")));
        Assert.Throws<NullReferenceException>(() => new Cons(1, "bad").ToString());
        Assert.Throws<NullReferenceException>(() => new Cons(null, null).ToString());
    }
    [Fact]
    public void Destructive_append_reuses_first_chain_and_links_second()
    {
        var first = new Cons(1, new Cons(2, null));
        var second = new Cons(3, null);
        Assert.Same(first, Cons.destructive_append(first, second));
        Assert.Same(second, first.tail.tail);
        Assert.Equal("[1, 2, 3]", first.ToString());
        Assert.Same(second, Cons.destructive_append(null, second));
        Assert.Throws<NullReferenceException>(() => Cons.destructive_append(new Cons(1, 7), second));
    }
}
