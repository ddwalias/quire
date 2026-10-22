#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface ICanBlockUndoRedo
{
    bool BlockUndoRedo { get; }
}
