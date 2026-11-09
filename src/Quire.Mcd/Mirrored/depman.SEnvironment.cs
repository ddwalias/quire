namespace Quire.depman;

internal partial interface SEnvironment
{
    unsafe Position* Lookup(Scope s, DSym sym);

    SharedThreadContext GetSharedThreadContext();

    int GetRegionId();
}
