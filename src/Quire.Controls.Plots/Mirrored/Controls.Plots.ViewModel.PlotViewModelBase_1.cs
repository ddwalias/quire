#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using Quire;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Undo;
using Quire.Controls.Plots.Model;

namespace Quire.Controls.Plots.ViewModel;

public partial class PlotViewModelBase<TModel>
{
    protected TModel Model { get; set; }

    private bool _showControlLevelErrorFrameForNestedError;

    private bool _areAxisExpressionsVisible;

    private bool _calcIsDisabled;

    private bool _isPendingActivation;

    private bool _isActiveRegion;

    private global::Quire.Calculation.Core.Support.CalculatableChildHider _childHider;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableAdded;

    public event global::System.Action<global::Quire.Calculation.Core.ICalculatable> CalculatableRemoved;

    internal PlotViewModelBase() { }

    private void OnModelDeserializationEnded()
    {
        DelegateModelEvent(DeserializationEnded);
    }

    private void OnModelChanged()
    {
        DelegateModelEvent(ModelChanged);
    }

    private static void DelegateModelEvent(Action handler)
    {
        if (handler != null)
        {
            handler();
        }
    }

    protected virtual void BindCommandsAndEvents()
    {
        if (DebugTools.AssertNotNull(Model, null, Array.Empty<object>()).Failed)
        {
            return;
        }
        Model.PropertyChanged += ModelPropertyChangedInner;
        Model.Changed += OnModelChanged;
        Model.DeserializationEnded += OnModelDeserializationEnded;
        CreateCommandBinding(ShowAxisExpressionsCommand, ShowAxisExpressionsCommandExecute);
        _childHider = new CalculatableChildHider(CollectHiddenAndVisibleCalculatables(), FireCalculatableAdded, FireCalculatableRemoved, null, false);
    }

    protected override void ModelPropertyChanged(string propertyName) => base.ModelPropertyChanged(propertyName);

    public bool ShowControlLevelErrorFrameForNestedError
    {
        get
        {
            return _showControlLevelErrorFrameForNestedError;
        }
        private set
        {
            if (value != _showControlLevelErrorFrameForNestedError)
            {
                _showControlLevelErrorFrameForNestedError = value;
                OnPropertyChanged("ShowControlLevelErrorFrameForNestedError");
            }
        }
    }

    public bool AreAxisExpressionsVisible
    {
        get
        {
            return _areAxisExpressionsVisible;
        }
        private set
        {
            if (_areAxisExpressionsVisible != value)
            {
                _areAxisExpressionsVisible = value;
                OnAxisExpressionsVisibilityChanged();
                OnPropertyChanged("AreAxisExpressionsVisible");
            }
        }
    }

    protected virtual void OnAxisExpressionsVisibilityChanged()
    {
        UpdateShowControlLevelErrorFrameForNestedError();
    }

    public bool CalcIsDisabled
    {
        get
        {
            return _calcIsDisabled;
        }
        set
        {
            if (_calcIsDisabled != value)
            {
                _calcIsDisabled = value;
                OnPropertyChanged("CalcIsDisabled");
            }
        }
    }

    internal IEnumerable<ICalculatable> CollectHiddenAndVisibleCalculatables()
    {
        return Model.GetHiddenAndVisibleCalculatables();
    }

    public bool HideFromCalculationContext
    {
        get
        {
            return CalcIsDisabled;
        }
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public bool PendingActivation
    {
        private get
        {
            return _isPendingActivation;
        }
        set
        {
            _isPendingActivation = value;
            ComputeAreAxisExpresionsVisible();
        }
    }

    public virtual bool IsActiveRegion
    {
        get
        {
            return _isActiveRegion;
        }
        set
        {
            PendingActivation = false;
            if (_isActiveRegion != value)
            {
                _isActiveRegion = value;
                OnPropertyChanged("IsActiveRegion");
            }
            ComputeAreAxisExpresionsVisible();
        }
    }

    protected void UpdateShowControlLevelErrorFrameForNestedError()
    {
        ShowControlLevelErrorFrameForNestedError = Model.HasError && !AreAxisExpressionsVisible;
    }

    private void ComputeAreAxisExpresionsVisible()
    {
        AreAxisExpressionsVisible = CalculateAreAxisExpressionsVisible;
    }

    private bool CalculateAreAxisExpressionsVisible => PendingActivation || IsActiveRegion || ShowAxisExpressions;

    public bool ShowAxisExpressions
    {
        get
        {
            return Model.ShowAxisExpressions;
        }
        set
        {
            Model.ShowAxisExpressions = value;
        }
    }

    public bool IsCalculating => Model != null && Model.IsCalculating;

    public global::Quire.Controls.Core.BackgroundTypeE PlotBackgroundType
    {
        get
        {
            return Model.PlotBackgroundType;
        }
        set
        {
            Model.PlotBackgroundType = value;
        }
    }

    public double GraphWidth
    {
        get
        {
            return Model.GraphWidth;
        }
        set
        {
            Model.GraphWidth = Math.Max(value, CurrentMinimalGraphWidth);
        }
    }

    public double GraphHeight
    {
        get
        {
            return Model.GraphHeight;
        }
        set
        {
            Model.GraphHeight = value;
        }
    }

    public static readonly global::System.Windows.Input.RoutedCommand ShowAxisExpressionsCommand;

    internal event global::System.Action ModelChanged;

    internal event global::System.Action DeserializationEnded;

    private void ShowAxisExpressionsCommandExecute(object arg)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(arg is bool, null, Array.Empty<object>()))
        {
            return;
        }
        bool oldState = ShowAxisExpressions;
        CommandBuffer.Execute(new UndoableCommand(
            "Set ShowAxisExpressions",
            () =>
            {
                ShowAxisExpressions = GetNewValue(IsActiveRegion, ShowAxisExpressions, arg);
                return true;
            },
            () =>
            {
                ShowAxisExpressions = oldState;
            }));
    }

    internal void OnGraphSizeChanged(Size newSize)
    {
        GraphHeight = newSize.Height;
        GraphWidth = newSize.Width;
    }

    public void SetDataObject(global::Quire.Serialization.IDeS11NProvider de11NProvider, global::Quire.PersistentDataObjects.plotType dataObject)
    {
        HideFromCalculationContext = Model.SetDataObject(de11NProvider, dataObject, ResultManager.Instance);
    }

    public global::Quire.PersistentDataObjects.plotType GetDataObject(global::Quire.Serialization.IS11NProvider s11NProvider)
    {
        return Model.GetDataObject(s11NProvider, ResultManager.Instance, HideFromCalculationContext);
    }

    private void FireCalculatableAdded(ICalculatable calc)
    {
        Action<ICalculatable> calculatableAdded = CalculatableAdded;
        if (calculatableAdded != null)
        {
            calculatableAdded(calc);
        }
    }

    private void FireCalculatableRemoved(ICalculatable calc)
    {
        Action<ICalculatable> calculatableRemoved = CalculatableRemoved;
        if (calculatableRemoved != null)
        {
            calculatableRemoved(calc);
        }
    }

    public IEnumerable<ICalculatable> CalculatableChildren => _childHider.CalculatableChildren;

    public global::Quire.Calculation.Core.EngineError PlotCalculatableError { get; private set; }

    private double _currentMinimalGraphWidth;

    public double CurrentMinimalGraphWidth
    {
        get
        {
            return _currentMinimalGraphWidth;
        }
        set
        {
            _currentMinimalGraphWidth = value;
            if (_currentMinimalGraphWidth > GraphWidth)
            {
                GraphWidth = _currentMinimalGraphWidth;
            }
        }
    }

    protected void OnCalculatableAdded(ICalculatable calculatable)
    {
        ICalculationHideableChild hideableChild = calculatable as ICalculationHideableChild;
        if (hideableChild != null)
        {
            hideableChild.HideFromCalculationContext = HideFromCalculationContext;
        }
        _childHider.ChildAdded(calculatable);
    }

    protected void OnCalculatableRemoved(ICalculatable calculatable)
    {
        _childHider.ChildRemoved(calculatable);
    }

    public void Commit()
    {
        CollectHiddenAndVisibleCalculatables().Each(calculatable => calculatable.Commit());
    }

    protected void ModelPlotCalculatableError(object sender, EngineErrorEventArgs e)
    {
        PlotCalculatableError = e.EngineError;
        OnPropertyChanged("PlotCalculatableError");
    }
}
