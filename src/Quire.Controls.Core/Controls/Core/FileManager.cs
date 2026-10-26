#nullable disable
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Quire.IO;

namespace Quire.Controls.Core;

public partial class FileManager
{
    public partial class Extension
    {
        public const string Xmcd = ".xmcd";

        public const string Mcdx = ".mcdx";

        public const string Mctx = ".mctx";

        public const string Rtf = ".rtf";

        public const string Xps = ".xps";

        public const string Dita = ".dita";

        public const string Pdf = ".pdf";

        public const string All = ".*";

        public Extension()
        {
        }
    }

    public static void EnsureStreamRelease(FileStream stream)
    {
        if (stream != null)
        {
            stream.Close();
            stream.Dispose();
        }
    }

    public static bool IsPackage(string path, string extension)
    {
        return IsPackage(path, extension, false);
    }

    public static bool IsPackage(string path, string extension, bool mcdxOnly)
    {
        FileStream fileStream = null;
        bool result = false;
        try
        {
            byte[] array = new byte[2];
            fileStream = PackageOperationsProvider.OpenFileStreamForReadWithFullShare(path);
            if (fileStream.Length == 0L && IsPrimeExtension(extension))
            {
                result = true;
            }
            else
            {
                fileStream.Read(array, 0, 2);
                string text = Encoding.ASCII.GetString(array);
                if (text != null && text == "PK" && (!mcdxOnly || IsPrimeExtension(extension)))
                {
                    result = true;
                }
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            EnsureStreamRelease(fileStream);
        }
        return result;
    }

    public static bool IsWorksheet(string path)
    {
        bool flag;
        FileStream fileStream = null;
        try
        {
            fileStream = PackageOperationsProvider.OpenFileStreamForReadWithFullShare(path);
            XmlReader xmlReader = XmlReader.Create(fileStream);
            try
            {
                flag = xmlReader.IsStartElement("worksheet") || xmlReader.IsStartElement("header") || xmlReader.IsStartElement("footer") || xmlReader.IsStartElement("first-page-header") || xmlReader.IsStartElement("first-page-footer");
            }
            finally
            {
                if (xmlReader != null)
                {
                    ((IDisposable)xmlReader).Dispose();
                }
            }
        }
        catch (Exception)
        {
            flag = false;
        }
        finally
        {
            EnsureStreamRelease(fileStream);
        }
        return flag;
    }

    public static void WaitFor(int seconds, WaitHandle handle)
    {
        WaitHandle[] waitHandles = new WaitHandle[1] { handle };
        int millisecondsTimeout = ((seconds < 0) ? seconds : (seconds * 1000));
        if (!WaitHandle.WaitAll(waitHandles, millisecondsTimeout, false))
        {
            throw new Exception("The wait for save has timed out");
        }
    }

    public static bool IsPrimeExtension(string extension)
    {
        if (!string.Equals(extension, ".mcdx", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(extension, ".mctx", StringComparison.OrdinalIgnoreCase);
        }
        return true;
    }

    public FileManager()
    {
    }
}
