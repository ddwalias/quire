#nullable disable

using System.Collections.ObjectModel;
using Quire.Controls;

namespace Quire.MruFiles;

internal partial interface IMruFilePersistanceManager
{
    ObservableCollection<MruFileItem> MruFileItemsCollection { get; set; }

    void Clear();

    void Save();

    void Reload();

    ObservableCollection<MruFileItem> GetUIVisibleItems();

    ObservableCollection<MruFileItem> GetAllItems();
}
