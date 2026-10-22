#nullable disable
#pragma warning disable

namespace Quire.IO;

partial interface IFileOperationsProvider
{
    string CurrentDirectory { get; set; }
    bool FileExists(string fileName) ;
    bool DirectoryExists(string path) ;
    global::System.IO.TextReader GetFileReader(string fileName) ;
    global::System.IO.TextWriter GetFileWriter(string fileName) ;
    global::System.IO.TextWriter GetFileWriter(string path, bool appendFlag) ;
    string[] GetFiles(string path) ;
    string[] GetFiles(string path, string filter) ;
    string[] GetDirectories(string path) ;
    string[] GetDirectories(string path, string filter) ;
    global::System.Exception CreateDirectory(string directory) ;
    string GetFullPath(string relativePath) ;
    string GetExtension(string fileName) ;
}
