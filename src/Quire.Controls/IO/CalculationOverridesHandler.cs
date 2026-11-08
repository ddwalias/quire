#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;
using Quire.Controls.Core.IORegions;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal static partial class CalculationOverridesHandler
{
    private static readonly Type _typeOfThis = typeof(CalculationOverridesHandler);

    [MirrorVerified("PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests")]
    public static DependencyProperty CalculationOverridesProperty = DependencyProperty.RegisterAttached("CalculationOverrides", typeof(ICalculationOverrideContainer), _typeOfThis);

    public static ICalculationOverrideContainer GetCalculationOverrides(DependencyObject region)
    {
        return (ICalculationOverrideContainer)region.GetValue(CalculationOverridesProperty);
    }

    [MirrorVerified("PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests")]
    public static void SetCalculationOverrides(DependencyObject region, ICalculationOverrideContainer container) => region.SetValue(CalculationOverridesProperty, container);

    [MirrorVerified("PtcControlsIOCalculationOverridesHandlerMechanicalDifferentialTests")]
    public static void ClearCalculationOverrides(DependencyObject region) => region.ClearValue(CalculationOverridesProperty);

    public static ISimpleCalculatable SetCalculationOverrideForSExpr(DependencyObject region, string sExprKey, CalculationOverridesTypes type, ISimpleCalculatable newOverride)
    {
        ICalculationOverrideContainer container = GetCalculationOverrides(region) ?? new CalculationOverrideContainer(IOItemsAliasManager.GetSExpressionsList(region, ItemOptions.InputsAndOutputs));
        ISimpleCalculatable previous = container.SetCalculationOverrideForSExpr(sExprKey, type, newOverride);
        SetCalculationOverrides(region, container);
        return previous;
    }

    public static ISimpleCalculatable GetCalculationOverrideForSExpr(DependencyObject region, string sExprKey, CalculationOverridesTypes type)
    {
        ICalculationOverrideContainer container = GetCalculationOverrides(region);
        if (container != null)
        {
            return container.GetCalculationOverrideForSExpr(sExprKey, type);
        }
        return null;
    }

    public static ISimpleCalculatable RemoveCalculationOverrideForSExpr(DependencyObject region, string sExprKey, CalculationOverridesTypes type)
    {
        ISimpleCalculatable removed = null;
        ICalculationOverrideContainer container = GetCalculationOverrides(region);
        if (container != null)
        {
            removed = container.RemoveCalculationOverrideForSExpr(sExprKey, type);
            if (container.CalculatableChildren.IsNullOrEmpty())
            {
                ClearCalculationOverrides(region);
            }
        }
        return removed;
    }

    public static IEnumerable<ICalculatable> RemoveCalculatableOverrides(DependencyObject region, CalculationOverridesTypes type)
    {
        ICalculationOverrideContainer container = GetCalculationOverrides(region);
        if (container != null)
        {
            IList<ICalculatable> removed = container.RemoveCalculationOverrides(type);
            if (container.CalculatableChildren.IsNullOrEmpty())
            {
                ClearCalculationOverrides(region);
            }
            return removed;
        }
        return new ICalculatable[0];
    }

    public static IEnumerable<ICalculatable> GetInputCalculationOverrides(DependencyObject region)
    {
        ICalculationOverrideContainer container = GetCalculationOverrides(region);
        if (container != null)
        {
            return container.GetInputCalculationOverrides();
        }
        return new ICalculatable[0];
    }

    public static IEnumerable<ICalculatable> GetOutputCalculationOverrides(DependencyObject region)
    {
        ICalculationOverrideContainer container = GetCalculationOverrides(region);
        if (container != null)
        {
            return container.GetOutputCalculationOverrides();
        }
        return new ICalculatable[0];
    }

    private partial class CalculationOverrideContainer
    {
        private readonly Dictionary<string, CalculationOverrides> _sExprToCalcOverridesMap;

        private readonly IList<string> _sExprOrderList;

        public CalculationOverrideContainer(IList<string> sExprOrderList)
        {
            _sExprOrderList = sExprOrderList;
            _sExprToCalcOverridesMap = new Dictionary<string, CalculationOverrides>();
        }

        public ISimpleCalculatable SetCalculationOverrideForSExpr(string sExprKey, CalculationOverridesTypes type, ISimpleCalculatable newOverride) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        public ISimpleCalculatable GetCalculationOverrideForSExpr(string sExprKey, CalculationOverridesTypes type)
        {
            if (_sExprToCalcOverridesMap != null && _sExprToCalcOverridesMap.ContainsKey(sExprKey))
            {
                return _sExprToCalcOverridesMap[sExprKey].GetCalculatableOfType(type);
            }
            return null;
        }

        public ISimpleCalculatable RemoveCalculationOverrideForSExpr(string sExprKey, CalculationOverridesTypes type) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

        public bool IsEmpty(string sExprKey) => !_sExprToCalcOverridesMap.ContainsKey(sExprKey);

        private IEnumerable<ISimpleCalculatable> GetCalculatableOverrides(Func<CalculationOverrides, IEnumerable<ISimpleCalculatable>> selector)
        {
            return _sExprOrderList.Where(sExpr => _sExprToCalcOverridesMap.ContainsKey(sExpr)).SelectMany(sExpr => selector(_sExprToCalcOverridesMap[sExpr]));
        }

        public IList<ICalculatable> RemoveCalculationOverrides(CalculationOverridesTypes type)
        {
            return _sExprToCalcOverridesMap.Keys.ToList().Select(sExpr => RemoveCalculationOverrideForSExpr(sExpr, type)).ToList<ICalculatable>();
        }

        public IEnumerable<ISimpleCalculatable> GetInputCalculationOverrides() => GetCalculatableOverrides(overrides => overrides.GetInputCalculatables());

        public IEnumerable<ISimpleCalculatable> GetOutputCalculationOverrides() => GetCalculatableOverrides(overrides => overrides.GetOutputCalculatables());

        public IEnumerable<ISimpleCalculatable> CalculatableChildren => GetCalculatableOverrides(overrides => overrides.GetAllCalculatables());
    }
}
