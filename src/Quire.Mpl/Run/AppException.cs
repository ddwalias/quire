#nullable disable
using System.Runtime.InteropServices;
using Quire.Mirror;

namespace Quire.Mpl.Run;

/// <summary>
/// A calculation error. <see cref="Argument"/> is the offending argument's index (<see cref="uint.MaxValue"/> for
/// none) and <see cref="Handle"/> packs a region (high 16 bits) and tag (low 16). Messages can carry them in text:
/// <c>tag&gt;message</c> wraps the error in one tagged with <c>tag</c>, and <c>message@argument</c> sets the argument.
/// Behaviour follows mplrun 12.0.1.0 (docs/mirror/evidence/Mpl.Run.AppException.md).
/// </summary>
[Serializable]
[ClassInterface(ClassInterfaceType.AutoDual)]
[Guid("7F1C7D54-0A1A-11E0-97F4-11F8E2A8C36C")]
public partial class AppException : ApplicationException
{
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static char errorTagDelimiter = '>';
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static char errorArgDelimiter = '@';

    [MirrorTested("Mpl.Run.AppException")]
    private uint m_argument;
    [MirrorTested("Mpl.Run.AppException")]
    private uint m_handle;

    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message) : this(message, null, uint.MaxValue, 0u) { }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message, Exception inner) : this(message, inner, uint.MaxValue, 0u) { }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message, uint argument) : this(message, null, argument, 0u) { }

    /// <summary>Wraps <paramref name="inner"/>, taking its message.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(Exception inner, uint handle) : this(inner.Message, inner, uint.MaxValue, handle) { }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message, Exception inner, uint handle) : this(message, inner, uint.MaxValue, handle) { }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message, uint argument, uint handle) : this(message, null, argument, handle) { }

    /// <summary>Sets <see cref="Tag"/>, then <see cref="Region"/>.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message, Exception inner, uint argument, uint tag, uint regionID) : this(message, inner, argument, 0u)
    {
        Tag = tag;
        Region = regionID;
    }

    /// <summary>Also resets the calling thread's Mpl stack budget (<see cref="Rts.ResetStackDepth"/>).</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public AppException(string message, Exception inner, uint argument, uint handle) : base(message, inner)
    {
        m_argument = argument;
        m_handle = handle;
        Rts.ResetStackDepth();
    }

    /// <summary>The high 16 bits of the handle. Setting adds the shifted value to the handle rather than replacing
    /// the high bits, so it is only exact on a handle whose region is 0.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public uint Region
    {
        get => m_handle >> 16;
        set => m_handle = (value << 16) + m_handle;
    }

    /// <summary>The low 16 bits of the handle. Setting adds the value to the cleared low half, so values past 16 bits
    /// carry into the region.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public uint Tag
    {
        get => m_handle & 0xFFFF;
        set => m_handle = (m_handle & 0xFFFF0000u) + value;
    }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public uint Handle => m_handle;

    [MirrorVerified("AppExceptionDifferentialTests")]
    public uint Argument => m_argument;

    /// <summary>The message, prefixed by <c>argument.</c> when there is an argument.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public string EfiMessage
    {
        get
        {
            if (m_argument == uint.MaxValue) return Message;
            return string.Format("{0}.{1}", m_argument, Message);
        }
    }

    /// <summary>The first <see cref="AppException"/> in the inner-exception chain, or a new untagged wrapper.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static AppException asAppException(Exception exception)
    {
        for (var e = exception; e != null; e = e.InnerException)
            if (e is AppException app) return app;
        return new AppException(exception, 0u);
    }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise() => throw new AppException("unknown mpl exception");

    /// <summary>The messages joined by tabs ("error" when there are none), raised with <paramref name="handle"/> as
    /// the argument.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(object[] msgs, uint handle)
    {
        if (msgs == null || msgs.Length == 0) return Raise("error", handle);
        if (msgs.Length == 1) return Raise(msgs[0].ToString(), handle);
        string text = msgs[0].ToString();
        for (int i = 1; i < msgs.Length; i++) text = string.Format("{0}\t{1}", text, msgs[i]);
        return Raise(text, handle);
    }

    /// <summary>
    /// For <c>tag&gt;rest</c> with a non-empty rest and a tag that parses as uint, the error for rest wrapped in one
    /// with that handle (so tags nest outward); otherwise a plain error with <paramref name="argument"/>.
    /// </summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static AppException ChainTaggedError(string message, uint argument)
    {
        int at = message.IndexOf(errorTagDelimiter);
        if (at > 0 && uint.TryParse(message.Substring(0, at), out uint tag) && at < message.Length - 1)
            return new AppException(ChainTaggedError(message.Substring(at + 1), argument), tag);
        return new AppException(message, argument);
    }

    /// <summary>Splits off a trailing <c>@argument</c> when it parses as uint, then <see cref="ChainTaggedError"/>.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static AppException ChainTagAndArgument(string message)
    {
        int at = message.IndexOf(errorArgDelimiter);
        if (at > 0 && at < message.Length - 1 && uint.TryParse(message.Substring(at + 1), out uint argument))
            return ChainTaggedError(message.Substring(0, at), argument);
        return ChainTaggedError(message, uint.MaxValue);
    }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(object thing) => Raise(Convert.ToString(thing));

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(string message) => throw ChainTagAndArgument(message);

    /// <summary>With no argument (<see cref="uint.MaxValue"/>) the message may carry one; otherwise
    /// <paramref name="argument"/> is used and only tags are parsed.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(string message, uint argument) =>
        throw (argument == uint.MaxValue ? ChainTagAndArgument(message) : ChainTaggedError(message, argument));

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(string message, uint argument, uint handle) => throw new AppException(message, argument, handle);

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(string message, Exception inner) => throw new AppException(message, inner);

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static object Raise(Exception inner, uint handle) => throw new AppException(inner, handle);

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static void FatalException(string message) => throw new AppException(message);

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static void StackOverflowException() => FatalException("stack overflow");

    /// <summary>Spends one level of the thread's stack budget ("stack overflow" once it goes negative), then
    /// throws <see cref="AbortRequestedException"/> if the thread's calculation was cancelled.</summary>
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static void ProbeStack()
    {
        if (--Rts.m_stack_depth < 0) FatalException("stack overflow");
        if (Rts.IsCancellationRequested()) throw new AbortRequestedException();
    }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public static void RestoreStack() => Rts.m_stack_depth++;
}

public partial class AbortRequestedException : AppException
{
    [MirrorVerified("AppExceptionDifferentialTests")]
    public AbortRequestedException() : base("abort requested") { }
}

/// <summary>An error the caller can recover from; the message is one of the static reason names.</summary>
public partial class NonCriticalException : ApplicationException
{
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static readonly string Default = "Default";
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static readonly string Unexpected = "Unexpected";
    [MirrorVerified("AppExceptionDifferentialTests")]
    public static readonly string ArgumentIsNull = "ArgumentIsNull";

    [MirrorVerified("AppExceptionDifferentialTests")]
    public NonCriticalException() : base(Default) { }

    [MirrorVerified("AppExceptionDifferentialTests")]
    public NonCriticalException(string message) : base(message) { }
}
