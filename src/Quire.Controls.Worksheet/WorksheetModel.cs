using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Worksheet;

/// <summary>Orders worksheet items the way they are calculated: top to bottom, then left to right.</summary>
public sealed partial class WorksheetItemPositionComparer : Whiteboard.PositionComparer, IComparer<Point>
{
    public static readonly WorksheetItemPositionComparer Instance = new();
}

public partial interface IWorksheetModel : ICalculatableContext
{
    IReadOnlyList<RegionControl> Items { get; }
    Point GetItemWorksheetLocation(RegionControl item);
    void AddItemAtLocation(RegionControl item, Point location);
    void RemoveItem(RegionControl item);
    void MoveItem(RegionControl item, Point location);
}

/// <summary>
/// The worksheet's regions in calculation order, exposed to the engine as an <see cref="ICalculatableContext"/>.
/// </summary>
public sealed partial class WorksheetModel : IWorksheetModel
{
    private readonly List<RegionControl> _items = [];
    private readonly Dictionary<RegionControl, Point> _locations = [];

    /// <summary>Declared explicitly: the mirrored part adds Mathcad's constructors, which would hide the implicit one.</summary>
    public WorksheetModel() { }

    public IReadOnlyList<RegionControl> Items => _items;

    public IReadOnlyList<ICalculatable> Calculatables => _items.OfType<ICalculatable>().ToArray();

    public event EventHandler<CalculatableEventArgs>? CalculatableAdded;
    public event EventHandler<CalculatableEventArgs>? CalculatableRemoved;
    public event EventHandler<CalculatableEventArgs>? CalculatableMoved;
    public event EventHandler? RecalculateAllRequested;
    public event EventHandler? CalculateNowRequested;
    public event EventHandler? AutoCalcChanged;
    public event EventHandler? UnitSystemUpdated;

    public bool IsAutoCalcOn { get; private set; } = true;
    public string UnitSystem { get; private set; } = "si";
    public ResultFormatOptions ResultFormat { get; private set; } = ResultFormatOptions.Default;

    public WorksheetSettings Settings => new()
    {
        UnitSystem = UnitSystem, AutoCalc = IsAutoCalcOn, Decimals = ResultFormat.Decimals,
        ExponentialThreshold = ResultFormat.ExponentialThreshold, ShowTrailingZeros = ResultFormat.ShowTrailingZeros,
    };

    public Point GetItemWorksheetLocation(RegionControl item) => _locations[item];

    private int InsertionIndex(Point p)
    {
        int i = 0;
        while (i < _items.Count && WorksheetItemPositionComparer.Instance.Compare(_locations[_items[i]], p) <= 0) i++;
        return i;
    }

    private ICalculatable? PreviousCalculatable(RegionControl item)
    {
        int i = _items.IndexOf(item);
        for (int j = i - 1; j >= 0; j--)
            if (_items[j] is ICalculatable c) return c;
        return null;
    }

    public void AddItemAtLocation(RegionControl item, Point location)
    {
        _locations[item] = location;
        _items.Insert(InsertionIndex(location), item);
        item.ApplyResultFormat(ResultFormat);
        if (item is ICalculatable c) CalculatableAdded?.Invoke(this, new CalculatableEventArgs(c, PreviousCalculatable(item)));
    }

    public void RemoveItem(RegionControl item)
    {
        if (!_items.Contains(item)) return;
        var prev = PreviousCalculatable(item);
        _items.Remove(item);
        _locations.Remove(item);
        if (item is ICalculatable c) CalculatableRemoved?.Invoke(this, new CalculatableEventArgs(c, prev));
    }

    public void MoveItem(RegionControl item, Point location)
    {
        var oldPrev = PreviousCalculatable(item);
        int oldIndex = _items.IndexOf(item);
        _items.RemoveAt(oldIndex);
        _locations[item] = location;
        _items.Insert(InsertionIndex(location), item);
        if (item is ICalculatable c)
        {
            var newPrev = PreviousCalculatable(item);
            if (!ReferenceEquals(oldPrev, newPrev)) CalculatableMoved?.Invoke(this, new CalculatableEventArgs(c, newPrev));
        }
    }

    public void SetUnitSystem(string name)
    {
        if (UnitSystem == name) return;
        UnitSystem = name;
        UnitSystemUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void SetAutoCalc(bool on)
    {
        if (IsAutoCalcOn == on) return;
        IsAutoCalcOn = on;
        AutoCalcChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetResultFormat(ResultFormatOptions format)
    {
        ResultFormat = format;
        foreach (var item in _items) item.ApplyResultFormat(format);
    }

    public void CalculateNow() => CalculateNowRequested?.Invoke(this, EventArgs.Empty);

    public void RecalculateAll() => RecalculateAllRequested?.Invoke(this, EventArgs.Empty);
}
