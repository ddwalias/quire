using System;
using Quire;
using Quire.Engine;
using Quire.Mpl.Run;
using Xunit;

namespace Quire.Engine.Tests;

public class ComponentHelpersTests
{
    private class FakeComponent : IMathcadComponent2
    {
        public bool LockResult { get; set; } = true;
        public bool UnlockCalled { get; set; }
        public string? LastSetPlace { get; set; }
        public IValue? LastSetValue { get; set; }
        public IValue? GetResult { get; set; }
        public Exception? SetException { get; set; }
        public Exception? GetException { get; set; }

        public bool LockComponent() => LockResult;
        public bool LockComponent(bool wait) => LockResult;
        public void UnlockComponent() => UnlockCalled = true;

        public void SetValue(string place, IValue value)
        {
            if (SetException != null) throw SetException;
            LastSetPlace = place;
            LastSetValue = value;
        }

        public IValue GetValue(string place)
        {
            if (GetException != null) throw GetException;
            return GetResult;
        }
    }

    [Fact]
    public void Lock_Success_Returns1()
    {
        var comp = new FakeComponent { LockResult = true };
        Assert.Equal(1, ComponentHelpers.Lock(comp));
    }

    [Fact]
    public void Lock_Failure_ThrowsAppException()
    {
        var comp = new FakeComponent { LockResult = false };
        var ex = Assert.Throws<AppException>(() => ComponentHelpers.Lock(comp));
        Assert.Equal("cannot_lock_component", ex.Message);
    }

    [Fact]
    public void Unlock_CallsUnlock_Returns0()
    {
        var comp = new FakeComponent();
        Assert.Equal(0, ComponentHelpers.Unlock(comp));
        Assert.True(comp.UnlockCalled);
    }

    [Fact]
    public void SetValue_Success_Returns0()
    {
        var comp = new FakeComponent();
        int res = ComponentHelpers.SetValue(comp, 10u, 20u, "in0", null);
        Assert.Equal(0, res);
        Assert.Equal("in0", comp.LastSetPlace);
    }

    [Fact]
    public void SetValue_Argument1_ThrowsWrappedInner()
    {
        var comp = new FakeComponent { SetException = new AppException("bad arg", 1u) };
        var ex = Assert.Throws<AppException>(() => ComponentHelpers.SetValue(comp, 100u, 200u, "in0", null));
        Assert.Equal(100u, ex.Handle);
        var inner = Assert.IsType<AppException>(ex.InnerException);
        Assert.Equal("bad arg", inner.Message);
        Assert.Equal(2u, inner.Argument);
    }

    [Fact]
    public void SetValue_OtherArgument_ThrowsWrappedOuter()
    {
        var comp = new FakeComponent { SetException = new AppException("other error", 5u) };
        var ex = Assert.Throws<AppException>(() => ComponentHelpers.SetValue(comp, 100u, 200u, "in0", null));
        Assert.Equal(200u, ex.Handle);
        Assert.Equal("other error", ex.Message);
    }

    [Fact]
    public void GetValue_Success_ReturnsValue()
    {
        var comp = new FakeComponent();
        var val = ComponentHelpers.GetValue(comp, "out0");
        Assert.Null(val);
    }

    [Fact]
    public void GetValue_Argument1_RewritesArgumentTo2()
    {
        var comp = new FakeComponent { GetException = new AppException("calc failed", 1u) };
        var ex = Assert.Throws<AppException>(() => ComponentHelpers.GetValue(comp, "out0"));
        Assert.Equal(2u, ex.Argument);
        Assert.Equal("calc failed", ex.Message);
    }

    [Fact]
    public void GetValue_OtherArgument_Rethrows()
    {
        var comp = new FakeComponent { GetException = new AppException("general fail", 9u) };
        var ex = Assert.Throws<AppException>(() => ComponentHelpers.GetValue(comp, "out0"));
        Assert.Equal(9u, ex.Argument);
    }
}
