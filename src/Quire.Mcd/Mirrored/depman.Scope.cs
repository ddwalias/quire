namespace Quire.depman;

public abstract partial class Scope
{
    public abstract int Id { get; }
    public abstract Scope Parent { get; }
}
