#nullable disable
using System.Globalization;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class InvertToGridPlacementConverter : global::System.Windows.Data.IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool flag;
        int num;
        string text;
        if (value is bool)
        {
            flag = (bool)value;
        }
        else
        {
            return -1;
        }
        text = parameter as string;
        if (text != null)
        {
            switch (text)
            {
                case "MinHorizontal":
                    num = flag ? 2 : 0;
                    goto ReturnNumber;
                case "MaxHorizontal":
                    num = flag ? 0 : 2;
                    goto ReturnNumber;
                case "MinVertical":
                    num = flag ? 0 : 2;
                    goto ReturnNumber;
                case "MaxVertical":
                    num = flag ? 2 : 0;
                    goto ReturnNumber;
                default:
                    goto DefaultNumber;
            }
        }
    DefaultNumber:
        num = -1;
    ReturnNumber:
        return num;
    }

    [MirrorVerified("PtcWpfInvertToGridPlacementConverterMechanicalDifferentialTests")]
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    [MirrorVerified("PtcWpfInvertToGridPlacementConverterMechanicalDifferentialTests")]
    public InvertToGridPlacementConverter() { }
}
