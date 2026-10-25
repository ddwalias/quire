#nullable disable
using System;
using System.IO;
using System.Windows.Media.Imaging;
using Quire;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class TemporaryStorageHelper
{
    public string StorageFileName { get; set; }

    public void SaveIfFileNameNullOrEmpty(Func<BitmapSource> getImageSource)
    {
        SaveIfFileNameNullOrEmpty(delegate(string filename)
        {
            SaveBitmap(filename, getImageSource);
        });
    }

    private static void SaveBitmap(string filename, Func<BitmapSource> getImageSource)
    {
        using FileStream target = File.Open(filename, FileMode.Create);
        ImageUtils.EncodePngImage(getImageSource(), target);
    }

    public BitmapSource LoadBitmap(string filename)
    {
        DebugTools.DevAssert(StorageFileName == null, null);
        BitmapImage bitmapImage = new BitmapImage();
        using (FileStream streamSource = File.Open(filename, FileMode.Open))
        {
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = streamSource;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
        }
        return bitmapImage;
    }

    public void CleanStorage(bool deleteFile)
    {
        if (deleteFile && !string.IsNullOrEmpty(StorageFileName))
        {
            try
            {
                File.Delete(StorageFileName);
            }
            catch
            {
            }
        }
        StorageFileName = null;
    }

    public void SaveIfFileNameNullOrEmpty(Action<string> save)
    {
        if (string.IsNullOrEmpty(StorageFileName))
        {
            StorageFileName = Path.GetTempFileName();
            save(StorageFileName);
        }
    }

    public TemporaryStorageHelper()
    {
    }
}
