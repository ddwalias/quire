using System.Runtime.CompilerServices;

namespace Quire.Mirror;

/// <summary>Marks a Quire type as the counterpart of a Mathcad Prime type, named as "Namespace.Type, Assembly".</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Delegate,
    Inherited = false)]
public sealed class MirrorOfAttribute(string mathcadType) : Attribute
{
    public string MathcadType { get; } = mathcadType;
}

/// <summary>Marks a generated member that has no implementation yet. The coverage report counts these.</summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorStubAttribute : Attribute;

public static class MirrorStub
{
    /// <summary>The exception every stub member throws.</summary>
    public static NotImplementedException NotImplemented([CallerFilePath] string file = "", [CallerMemberName] string member = "") =>
        new($"{Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file))}.{member} is a mirror stub and is not implemented yet.");
}

/// <summary>
/// Marks a mirrored member whose behaviour can't be established yet (PLAN.md §3c rule 4). It stays unimplemented and
/// throws, like a stub, but the reason is recorded and the member counts as settled for the phase.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorBlockedAttribute(string reason) : Attribute
{
    public string Reason { get; } = reason;
}

/// <summary>
/// Marks a mirrored member whose behaviour a differential test against the installed Mathcad assembly confirms.
/// <paramref name="test"/> names the test class in Quire.Differential.Tests; a test there checks that it exists.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorVerifiedAttribute(string test) : Attribute
{
    public string Test { get; } = test;
}

/// <summary>Marks a mirrored member implemented from documentation or declared metadata, not differentially tested.</summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorDocumentedAttribute(string source) : Attribute
{
    public string Source { get; } = source;
}

/// <summary>Marks a member of a mirrored type that has no Mathcad counterpart.</summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class QuireOnlyAttribute : Attribute;
