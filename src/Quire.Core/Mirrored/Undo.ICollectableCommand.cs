#nullable disable
#pragma warning disable

namespace Quire.Undo;

partial interface ICollectableCommand
{
    bool CollectNewCommand(global::Quire.Undo.ICollectableCommand nextCommand);
}
