#nullable disable
#pragma warning disable

namespace Quire.Undo;

partial interface IUndoableCommand
{
    string Name { get; }
    bool Do();
    void Undo();
    void Redo();
}
