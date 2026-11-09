namespace Quire.Controls.Internal;

internal partial interface IHistoryPopup
{
    bool IsPopupOpen { get; }
    event global::Quire.Delegate1<int> HistoryItemSelected;
    void Show();
    void Hide();
}
