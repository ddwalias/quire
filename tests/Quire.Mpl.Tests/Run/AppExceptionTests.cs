using Quire.Mpl.Run;

namespace Quire.Mpl.Tests.Run;

/// <summary>
/// Behaviour of <see cref="AppException"/> and its relatives against docs/mirror/evidence/Mpl.Run.AppException.md;
/// AppExceptionDifferentialTests compares with Mathcad.
/// </summary>
public class AppExceptionTests
{
    [Fact]
    public void Tags_nest_outward_and_the_argument_applies_innermost()
    {
        var e = AppException.ChainTagAndArgument("12>34>msg@5");
        Assert.Equal((12u, uint.MaxValue, "msg"), (e.Handle, e.Argument, e.Message));
        var middle = Assert.IsType<AppException>(e.InnerException);
        Assert.Equal(34u, middle.Handle);
        var inner = Assert.IsType<AppException>(middle.InnerException);
        Assert.Equal((5u, "5.msg"), (inner.Argument, inner.EfiMessage));
        Assert.Same(inner, AppException.asAppException(new InvalidOperationException("x", inner)));
    }

    [Fact]
    public void Malformed_tags_and_arguments_stay_in_the_message()
    {
        Assert.Equal("x>msg", AppException.ChainTagAndArgument("x>msg").Message);
        Assert.Equal("12>", AppException.ChainTagAndArgument("12>").Message);
        Assert.Equal("msg@", AppException.ChainTagAndArgument("msg@").Message);
        Assert.Equal("@5", AppException.ChainTagAndArgument("@5").Message);
        var e = Assert.Throws<AppException>(() => AppException.Raise("a@1", 3u));
        Assert.Equal(("a@1", 3u), (e.Message, e.Argument));
    }

    [Fact]
    public void Region_and_tag_setters_add()
    {
        var e = new AppException("m", null, 0, 0x1_0002);
        e.Region = 3;
        Assert.Equal(4u, e.Region);                 // added to the existing high half
        e.Tag = 0x1_0005;
        Assert.Equal((5u, 5u), (e.Region, e.Tag));  // the carry lands in the region
        var f = new AppException("m", null, 1, 7, 2);
        Assert.Equal((2u, 7u, 0x2_0007u), (f.Region, f.Tag, f.Handle));
    }

    [Fact]
    public void Raise_joins_messages_with_tabs()
    {
        Assert.Equal("a\tb", Assert.Throws<AppException>(() => AppException.Raise(["a", "b"], 3)).Message);
        Assert.Equal("error", Assert.Throws<AppException>(() => AppException.Raise([], 3)).Message);
        Assert.Equal("unknown mpl exception", Assert.Throws<AppException>(() => AppException.Raise()).Message);
        Assert.Equal("stack overflow", Assert.Throws<AppException>(AppException.StackOverflowException).Message);
    }

    [Fact]
    public void Probing_spends_the_stack_budget_and_checks_cancellation()
    {
        Exception? overflow = null, abort = null;
        int afterError = 0;
        var t = new Thread(() =>
        {
            for (int i = 0; i < 250; i++) AppException.ProbeStack();
            overflow = Record.Exception(AppException.ProbeStack);
            afterError = Rts.m_stack_depth;     // constructing the error reset the budget
            var src = new CancellationTokenSource();
            src.Cancel();
            Rts.RegisterThreadForCancellationChecking(src);
            abort = Record.Exception(AppException.ProbeStack);
            AppException.RestoreStack();
        });
        t.Start();
        t.Join();
        Assert.Equal("stack overflow", overflow?.Message);
        Assert.Equal(250, afterError);
        Assert.Equal("abort requested", Assert.IsType<AbortRequestedException>(abort).Message);
        Assert.Equal("Default", new NonCriticalException().Message);
    }
}
