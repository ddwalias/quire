#nullable disable
using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Core.Serialization;

public static partial class SerializationHelperUtils
{
    public static bool IsConvertedFromMathcad14(DependencyObject region)
    {
        object value = region.GetValue(SerializationHelper.ConvertedFromProperty);
        if (!(value is applicationVersionType))
        {
            return false;
        }
        return (applicationVersionType)value == applicationVersionType.Mathcad14;
    }

    public static void SetConvertedFromMathcad14(DependencyObject region)
    {
        region.SetValue(SerializationHelper.ConvertedFromProperty, applicationVersionType.Mathcad14);
    }

    public static void SaveToPartUsingTemporaryFile(Action<string> saveDocumentToTempFile, PackagePart part, string originalFileName = null)
    {
        bool flag = true;
        if (originalFileName == null)
        {
            flag = false;
        }
        else if (originalFileName.Equals(string.Empty))
        {
            return;
        }
        string text = string.Empty;
        string fileName;
        try
        {
            if (flag)
            {
                fileName = Path.GetFileName(originalFileName);
                if (!string.IsNullOrEmpty(fileName))
                {
                    text = Path.Combine(Path.GetTempPath(), fileName);
                }
            }
            else
            {
                text = Path.GetTempFileName();
            }
            if (!string.IsNullOrEmpty(text))
            {
                saveDocumentToTempFile(text);
                using FileStream fileStream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read);
                using Stream destination = part.GetStream(FileMode.Create);
                fileStream.CopyTo(destination);
            }
        }
        finally
        {
            TryDeleteTemporaryFile(text);
        }
    }

    public static void TryDeleteTemporaryFile(string filename)
    {
        if (File.Exists(filename))
        {
            if ((File.GetAttributes(filename) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(filename, FileAttributes.Normal);
            }
            try
            {
                File.Delete(filename);
            }
            catch (Exception)
            {
            }
        }
    }

    public static void CopyObfuscatedStream(Stream source, Stream target, int maxBufferSize = 16384)
    {
        Stream seekableStream;
        Stream seekableStream2;
        string tempFilePath = InitializeSeekableFileStream(source, out seekableStream);
        string tempFilePath2 = InitializeSeekableFileStream(target, out seekableStream2, false);
        long length = seekableStream.Length;
        int num = (int)Math.Min((long)maxBufferSize, length);
        byte[] array = new byte[num];
        seekableStream2.SetLength(length);
        seekableStream.Position = 0L;
        int num3;
        long num2 = 0L;
        while ((num3 = seekableStream.Read(array, 0, num)) > 0)
        {
            Obfuscate(array, num3);
            num2 += num3;
            seekableStream2.Position = length - num2;
            seekableStream2.Write(array, 0, num3);
            seekableStream2.Flush();
        }
        seekableStream2.Position = 0L;
        DebugTools.DevAssert(length == num2, null);
        FinalizeSeekableStream(seekableStream, tempFilePath);
        FinalizeSeekableStream(seekableStream2, tempFilePath2, target);
    }

    private static void FinalizeSeekableStream(Stream seekableStream, string tempFilePath, Stream originalStream = null)
    {
        if (tempFilePath == null || DebugTools.DevAssert(seekableStream != originalStream, null).Failed)
        {
            return;
        }
        if (originalStream != null)
        {
            seekableStream.CopyTo(originalStream);
        }
        seekableStream.Close();
        File.Delete(tempFilePath);
    }

    private static string InitializeSeekableFileStream(Stream originalStream, out Stream seekableStream, bool makeCopy = true)
    {
        string text;
        if (originalStream.CanSeek)
        {
            text = null;
            seekableStream = originalStream;
        }
        else
        {
            text = Path.GetTempFileName();
            seekableStream = File.Open(text, FileMode.Create);
            if (makeCopy)
            {
                originalStream.CopyTo(seekableStream);
            }
        }
        return text;
    }

    public static void Obfuscate(byte[] arrData, int length = int.MaxValue)
    {
        int num = 0;
        int num2 = Math.Min(arrData.Length, length) - 1;
        while (num <= num2)
        {
            byte data = arrData[num];
            arrData[num] = Mask(arrData[num2]);
            arrData[num2] = Mask(data);
            num++;
            num2--;
        }
    }

    private static byte Mask(byte data)
    {
        return (byte)(data ^ 170);
    }
}
