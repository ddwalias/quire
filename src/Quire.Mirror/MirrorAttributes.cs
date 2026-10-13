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
