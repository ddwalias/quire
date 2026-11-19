using System.Collections.Generic;

namespace Quire.AutomationService;

public partial class AutomationSpiritAliasesList
{
    private readonly List<string> _aliases;

    public int Count
    {
        get
        {
            if (_aliases == null)
            {
                return 0;
            }

            return _aliases.Count;
        }
    }

    public AutomationSpiritAliasesList(List<string> aliasesList)
    {
        _aliases = new List<string>();
        if (aliasesList != null)
        {
            _aliases = aliasesList;
        }
    }

    public AutomationSpiritAliasesList(global::Quire.Controls.Core.IORegions.IIOItemCollection inputs)
    {
        _aliases = new List<string>();
        if (inputs != null)
        {
            foreach (global::Quire.Controls.IO.IOItem item in inputs)
            {
                _aliases.Add(item.Alias);
            }
        }
    }

    public string GetAliasByIndex(int index)
    {
        // Mathcad's compiler tests the bounds as 0 <= index and Count > index.
        if (0 <= index && Count > index)
        {
            return _aliases[index];
        }

        return null;
    }
}
