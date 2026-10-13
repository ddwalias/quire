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

// Per-member status (PLAN.md §3c rule 6). A member carries at most one of these; one without any is "unclassified":
// hand-written before §3c and still to be analyzed. The evidence argument of the REA-based statuses names a record
// in docs/mirror/evidence/ (file stem, e.g. "Mcd.Rational"); Quire.Mirror.Tests checks that every referenced record,
// differential test class and deviation entry exists.

/// <summary>
/// Marks a mirrored member whose original behaviour can't be established yet: REA couldn't expose it, or it stayed
/// ambiguous. It throws, like a stub, with the reason recorded. Blocked members are unresolved work, not settled.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorBlockedAttribute(string reason) : Attribute
{
    public string Reason { get; } = reason;
}

/// <summary>
/// REA-analyzed: the original counterpart's analysis is recorded in docs/mirror/evidence/<paramref name="evidence"/>.md,
/// but no behaviour test for Quire's implementation passes yet. Not settled.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorAnalyzedAttribute(string evidence) : Attribute
{
    public string Evidence { get; } = evidence;
}

/// <summary>
/// REA-analyzed + behaviour-tested: implemented independently from the analysis recorded in
/// docs/mirror/evidence/<paramref name="evidence"/>.md, whose tests pass. No direct comparison with Mathcad is claimed.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorTestedAttribute(string evidence) : Attribute
{
    public string Evidence { get; } = evidence;
}

/// <summary>
/// Differential-verified: the test class <paramref name="test"/> in Quire.Differential.Tests compares this member
/// directly with the installed Mathcad assembly, and passes. This covers the recorded cases only.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorVerifiedAttribute(string test) : Attribute
{
    public string Test { get; } = test;
}

/// <summary>
/// An accepted intentional deviation: <paramref name="id"/> is an accepted <c>DEV-####</c> entry in
/// docs/mirror/deviations.md. Settled as a recorded divergence, never as a match.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Field,
    Inherited = false)]
public sealed class MirrorDeviationAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}

/// <summary>
/// Documentation-only: implemented from documentation or declared metadata, without REA analysis of the body. Not
/// sufficient for completion.
/// </summary>
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
