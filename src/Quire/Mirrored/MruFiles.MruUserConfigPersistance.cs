#nullable disable

using System.Collections.ObjectModel;
using System.Linq;
using Quire.Controls;
using Quire.Properties;

namespace Quire.MruFiles;

public partial class MruUserConfigPersistance
{
    private readonly Settings _settings;

    public MruUserConfigPersistance()
        : this(Settings.Default)
    {
    }

    private MruUserConfigPersistance(Settings settings)
    {
        _settings = settings;
    }

    public void Clear()
    {
        _settings.MyMRUItems = null;
    }

    public void Save()
    {
        _settings.Save();
    }

    public void Reload()
    {
        _settings.Reload();
    }

    public ObservableCollection<MruFileItem> MruFileItemsCollection
    {
        get => _settings.MyMRUItems;
        set => _settings.MyMRUItems = (MruObservableCollection)value;
    }

    public ObservableCollection<MruFileItem> GetUIVisibleItems()
    {
        return new ObservableCollection<MruFileItem>(_settings.MyMRUItems.Take(15));
    }

    public ObservableCollection<MruFileItem> GetAllItems()
    {
        return new ObservableCollection<MruFileItem>(_settings.MyMRUItems);
    }
}
