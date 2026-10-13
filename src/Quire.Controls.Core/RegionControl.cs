using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Core;

public enum LeaveDirection
{
    Down,
    Right,
    Cancel,
}

public sealed class RegionLeaveEventArgs(LeaveDirection direction) : EventArgs
{
    public LeaveDirection Direction { get; } = direction;
}

/// <summary>
/// Base class of every worksheet region. The worksheet positions regions on its canvas; a region
/// owns its content, editing behaviour and persistence.
/// </summary>
public abstract class RegionControl : ContentControl
{
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected), typeof(bool), typeof(RegionControl), new PropertyMetadata(false, (d, _) => ((RegionControl)d).UpdateChrome()));

    public static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(
        nameof(HasError), typeof(bool), typeof(RegionControl), new PropertyMetadata(false, (d, _) => ((RegionControl)d).UpdateChrome()));

    protected static readonly Brush SelectionBrush = Frozen(Color.FromRgb(0x3C, 0x8C, 0xDC));
    protected static readonly Brush EditBrush = Frozen(Color.FromRgb(0x9A, 0xB8, 0xD8));
    protected static readonly Brush ErrorBrush = Frozen(Color.FromRgb(0xD0, 0x30, 0x30));

    private readonly Border _chrome;
    private bool _isEditing;

    protected RegionControl()
    {
        _chrome = new Border { BorderThickness = new Thickness(1), Padding = new Thickness(2), BorderBrush = Brushes.Transparent };
        base.Content = _chrome;
        Focusable = false;
        SnapsToDevicePixels = true;
    }

    protected static Brush Frozen(Color c)
    {
        var b = new SolidColorBrush(c);
        b.Freeze();
        return b;
    }

    public string RegionId { get; set; } = RegionPersistentData.NewId();

    /// <summary>The region's visual content (inside the selection chrome).</summary>
    protected UIElement? Body
    {
        get => _chrome.Child;
        set => _chrome.Child = value;
    }

    public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool HasError { get => (bool)GetValue(HasErrorProperty); set => SetValue(HasErrorProperty, value); }

    public bool IsEditing
    {
        get => _isEditing;
        protected set
        {
            if (_isEditing == value) return;
            _isEditing = value;
            UpdateChrome();
            EditingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Worksheet number display settings; regions showing results re-render when it changes.</summary>
    public ResultFormatOptions ResultFormat { get; private set; } = ResultFormatOptions.Default;

    public void ApplyResultFormat(ResultFormatOptions format)
    {
        ResultFormat = format;
        OnResultFormatChanged();
    }

    protected virtual void OnResultFormatChanged() { }

    /// <summary>Content changed (for undo snapshots and the dirty flag).</summary>
    public event EventHandler? Changed;

    public event EventHandler? EditingChanged;

    /// <summary>The user finished editing (Enter, Tab or Esc) and wants the cursor placed elsewhere.</summary>
    public event EventHandler<RegionLeaveEventArgs>? RequestLeave;

    protected void RaiseChanged() => Changed?.Invoke(this, EventArgs.Empty);
    protected void RaiseLeave(LeaveDirection direction) => RequestLeave?.Invoke(this, new RegionLeaveEventArgs(direction));

    public virtual bool CanEdit => true;

    /// <summary>Starts editing; <paramref name="initialText"/> is typed text that created the region.</summary>
    public abstract void BeginEdit(string? initialText = null);

    /// <summary>Stops editing, keeping changes when <paramref name="commit"/> is true.</summary>
    public abstract void EndEdit(bool commit);

    /// <summary>Inserts text at the caret while editing (palettes use this).</summary>
    public virtual void InsertText(string text) { }

    /// <summary>True when the region holds nothing worth keeping (empty regions are removed after editing).</summary>
    public virtual bool IsEmpty => false;

    public abstract RegionPersistentData Save();

    private void UpdateChrome()
    {
        _chrome.BorderBrush = HasError ? ErrorBrush : IsSelected ? SelectionBrush : IsEditing ? EditBrush : Brushes.Transparent;
        _chrome.Background = IsSelected ? Frozen(Color.FromArgb(0x18, 0x3C, 0x8C, 0xDC)) : Brushes.Transparent;
    }
}

/// <summary>A region the calculation engine evaluates.</summary>
public abstract class CalculatableRegionControl : RegionControl, ICalculatable
{
    private string _rl = "(noop)";

    public string Rl
    {
        get => _rl;
        protected set
        {
            if (_rl == value) return;
            _rl = value;
            RlChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? RlChanged;

    public SynchronizationStatus SynchronizationStatus { get; private set; } = SynchronizationStatus.Pending;

    public CalculationResult? Result { get; private set; }

    public void SetSynchronizationStatus(SynchronizationStatus status)
    {
        SynchronizationStatus = status;
        Opacity = status == SynchronizationStatus.Pending ? 0.75 : 1;
    }

    public void SetResult(CalculationResult result)
    {
        Result = result;
        OnResult(result);
    }

    protected abstract void OnResult(CalculationResult result);

    /// <summary>Re-apply the last result, e.g. after the display format changed.</summary>
    protected override void OnResultFormatChanged()
    {
        if (Result != null) OnResult(Result);
    }
}
