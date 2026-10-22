#nullable disable
#pragma warning disable

namespace Quire.IO;

partial class FileUtils
{
    public static int FindHighestNumberedFileName(string folder, string baseFileName, bool numberIsLastExtension, out string foundFileName)
    {
        int num = -1;
        foundFileName = null;
        global::Quire.IO.IFileOperationsProvider fileOperationsProvider = new global::Quire.IO.FileOperationsProvider();
        global::Quire.DebugTools.DevAssert(fileOperationsProvider.DirectoryExists(folder), null);
        string text = numberIsLastExtension ? string.Empty : fileOperationsProvider.GetExtension(baseFileName);
        string filter = baseFileName.Remove(baseFileName.Length - text.Length) + "*" + text;
        try
        {
            string[] files = fileOperationsProvider.GetFiles(folder, filter);
            foreach (string file in files)
            {
                string path;
                if (numberIsLastExtension)
                {
                    path = file;
                }
                else
                {
                    path = global::System.IO.Path.GetFileNameWithoutExtension(file);
                }
                path = global::System.IO.Path.GetExtension(path).TrimStart('.');
                try
                {
                    int num2 = baseFileName.Equals(global::System.IO.Path.GetFileName(file)) ? 0 : int.Parse(path);
                    if (num2 > num)
                    {
                        num = num2;
                        foundFileName = file;
                    }
                }
                catch (global::System.FormatException)
                {
                }
                catch (global::System.OverflowException)
                {
                }
            }
        }
        catch (global::System.Exception)
        {
        }

        return num;
    }
}
