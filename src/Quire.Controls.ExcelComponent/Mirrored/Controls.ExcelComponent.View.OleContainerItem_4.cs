using System;
using System.Globalization;
using System.Windows.Data;

namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleContainerItem<TViewModel, TModel, TIPublicModel, TPersistentData>
{
    protected override IValueConverter GetDimensionsConverter()
    {
        return new DimensionsConverter(this);
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.View.OleContainerItem`4+DimensionsConverter, Ptc.Controls.ExcelComponent")]
    public partial class DimensionsConverter : IValueConverter
    {
        private readonly OleContainerItem<TViewModel, TModel, TIPublicModel, TPersistentData> _container;

        public DimensionsConverter(OleContainerItem<TViewModel, TModel, TIPublicModel, TPersistentData> oleContainer)
        {
            _container = oleContainer;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)parameter)
            {
                _container._expectedSize.Width = (double)value;
            }
            else
            {
                _container._expectedSize.Height = (double)value;
            }
            return value;
        }
    }
}
