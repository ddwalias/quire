using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Quire.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class HistoryPopup : Popup, IHistoryPopup
{
    private ListBox _listBox;

    public event Delegate1<int> HistoryItemSelected;

    public HistoryPopup(IList<IVisualMathNode> historyList)
    {
        _listBox = new ListBox();
        _listBox.BeginInit();
        _listBox.ItemsSource = CreateListItems(historyList);
        _listBox.SelectionChanged += FireHistoryItemSelected;
        _listBox.EndInit();
        Child = _listBox;
        StaysOpen = false;
    }

    private static IList<ListBoxItem> CreateListItems(IList<IVisualMathNode> nodes)
    {
        IList<ListBoxItem> list = new List<ListBoxItem>(nodes.Count);
        foreach (IVisualMathNode node in nodes)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Content = node;
            list.Add(listBoxItem);
        }
        return list;
    }

    private void FireHistoryItemSelected(object sender, SelectionChangedEventArgs e)
    {
        e.Handled = true;
        if (HistoryItemSelected != null)
        {
            HistoryItemSelected(_listBox.SelectedIndex);
        }
    }

    public bool IsPopupOpen => IsOpen;

    public void Show()
    {
        IsOpen = true;
    }

    public void Hide()
    {
        IsOpen = false;
        if (Parent != null)
        {
            ((Panel)Parent).Children.Remove(this);
        }
    }
}
