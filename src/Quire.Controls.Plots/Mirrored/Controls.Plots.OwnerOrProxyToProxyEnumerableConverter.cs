using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls.Plots;

public partial class OwnerOrProxyToProxyEnumerableConverter : IValueConverter
{
    private static readonly OwnerOrProxyToProxyEnumerableConverter _instance = new OwnerOrProxyToProxyEnumerableConverter();

    public static OwnerOrProxyToProxyEnumerableConverter Instance => _instance;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return GetItems(GetProxy(value) ?? GetProxy(parameter));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static IEquationProxy GetProxy(object proxyOrOwner)
    {
        IEquationProxy equationProxy = (proxyOrOwner is IEquationProxyOwner) ? (proxyOrOwner as IEquationProxyOwner).EquationProxy : null;
        return equationProxy ?? (proxyOrOwner as IEquationProxy);
    }

    private static IEnumerable<IEquationProxy> GetItems(IEquationProxy proxy)
    {
        yield return proxy;
    }
}
