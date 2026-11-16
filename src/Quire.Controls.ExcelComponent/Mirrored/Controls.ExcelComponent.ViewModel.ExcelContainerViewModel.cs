using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Core.Ole;
using Quire.Controls.ExcelComponent.Model;
using Quire.PersistentData;
using Quire.Core;

namespace Quire.Controls.ExcelComponent.ViewModel;

public partial class ExcelContainerViewModel
{
    protected override bool IsStrictMinSize => false;

    protected override bool MustTakeLockOnUserDeactivation => false;

    public global::System.Nullable<bool> GetExcelVersionInstalled(bool forceRun)
    {
        return Model.GetExcelVersionInstalled(forceRun);
    }

    public bool Select(string rangeAddress, bool forceRun)
    {
        return Model.Select(rangeAddress, forceRun);
    }

    private Size _cachedSize = Size.Empty;

    private static Size _initialSize = Size.Empty;

    private Size? _cachedMinimalSize;

    public override bool IsResizing
    {
        get
        {
            return base.IsResizing;
        }
        protected internal set
        {
            if (value == IsResizing)
            {
                return;
            }
            base.IsResizing = value;
            if (value)
            {
                MinSize = RecalculateMinimalSize().GetValueOrDefault();
            }
        }
    }

    public bool? IsAsIcon
    {
        get
        {
            return Model.IsAsIcon;
        }
        set
        {
            Model.IsAsIcon = value;
        }
    }

    public string Selection => Model.GetSelection();

    public ExcelContainerViewModel(ExcelContainerModel model)
        : base(model, new Size(67.2, 19.2))
    {
    }

    protected override void PostCreationWindowless()
    {
        base.PostCreationWindowless();
        Model.HideOleObject();
    }

    public Size Measure(Size constraint)
    {
        Size val = EstablishInitialSize();
        if (val != Size.Empty)
        {
            if (_cachedSize == Size.Empty)
            {
                constraint = val;
            }
            else
            {
                if (double.IsInfinity(constraint.Width))
                {
                    constraint.Width = _cachedSize.Width;
                }
                if (double.IsInfinity(constraint.Height))
                {
                    constraint.Height = _cachedSize.Height;
                }
            }
            _cachedSize = constraint;
        }
        return constraint;
    }

    protected override void RunAndCheckAvailabilityOnResizing()
    {
        if (Model.CheckServerAvailable(forceRun: true, OleContainerMessageKeysDict.ResizeMessageKeys) && Model.IsExcelCalculationManualModel() != true && Model.HasAttachedInputsOrOutputs(checkOutputOnly: true))
        {
            Model.SetMessage(MessageCategory.UnsynchronizedResults, NodeMessageAdornerType.Warning, "server_was_started_warning");
        }
    }

    protected override void OnOleSavedSync()
    {
        _cachedMinimalSize = RecalculateMinimalSize();
        base.OnOleSavedSync();
    }

    protected override void OnOleSavedAsync()
    {
        if (DebugTools.AssertNotNull(_cachedMinimalSize, null).Passed)
        {
            if (_cachedMinimalSize.Value != Size.Empty)
            {
                MinSize = _cachedMinimalSize.Value;
            }
            _cachedMinimalSize = null;
        }
        base.OnOleSavedAsync();
    }

    public string GetExcelSelectionOrDefault(bool isInput)
    {
        string selection = Selection;
        string format = isInput ? "{0}" : "{0}:{0}";
        if (string.IsNullOrEmpty(selection))
        {
            return string.Format(format, "A1");
        }
        return selection;
    }
}
