using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Plots;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class RegionFactory
{
    public static readonly IRegionFactory Instance = new RegionFactory();

    public IRegionPersistentData CreateFailedRegionPersistentData()
    {
        BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Ptc.Controls.Core;component/images/deserialization_failed.png"));
        ((Freezable)bitmapImage).Freeze();
        ImageRegionPersistentData imageRegionPersistentData = new ImageRegionPersistentData(new ImageControl
        {
            Source = bitmapImage
        });
        MC14ConversionAnnotation.AddConversionAnnotation(((IRegionPersistentData)imageRegionPersistentData).Control, PersistentDataObjects.MC14ConversionAnnotationType.CorruptedRegion);
        return imageRegionPersistentData;
    }

    private static IRegionPersistentData CreateTemplatedTextBoxTypedPersistentData(fieldType fieldType)
    {
        switch (fieldType.ItemElementName)
        {
            case ItemChoiceType1.fileName:
                return new TemplatedTextBoxPersistentData(new FileNameControl());
            case ItemChoiceType1.filePath:
                return new TemplatedTextBoxPersistentData(new FilePathControl());
            case ItemChoiceType1.pageNumber:
                return new TemplatedTextBoxPersistentData(new PageNumberControl());
            case ItemChoiceType1.dateSaved:
                return new TemplatedTextBoxPersistentData(new SavedDateControl());
            default:
                return null;
        }
    }
}
