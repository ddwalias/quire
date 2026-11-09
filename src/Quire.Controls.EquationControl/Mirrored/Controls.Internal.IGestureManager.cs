namespace Quire.Controls.Internal;

internal partial interface IGestureManager
{
    bool Execute(global::System.Windows.Input.KeyEventArgs args);
    bool Execute(string c);
    void AddBinding(global::System.Windows.Input.KeyGesture gesture, global::Quire.Controls.Internal.EditorCommand command);
    void AddBinding(string key, global::Quire.Controls.Internal.EditorCommand command);
}
