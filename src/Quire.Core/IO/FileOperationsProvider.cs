#nullable disable
using System.IO;
using Quire.Mirror;

namespace Quire.IO;

public partial class FileOperationsProvider
{
    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public bool FileExists(string fileName) => File.Exists(fileName);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public bool DirectoryExists(string path) => Directory.Exists(path);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public TextReader GetFileReader(string fileName) => new StreamReader(fileName);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public TextWriter GetFileWriter(string fileName) => new StreamWriter(fileName);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public TextWriter GetFileWriter(string path, bool appendFlag) => new StreamWriter(path, appendFlag);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string[] GetFiles(string path) => Directory.GetFiles(path);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string[] GetFiles(string path, string filter) => Directory.GetFiles(path, filter);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string[] GetDirectories(string path) => Directory.GetDirectories(path);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string[] GetDirectories(string path, string filter) => Directory.GetDirectories(path, filter);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string GetFullPath(string relativePath) => Path.GetFullPath(relativePath);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string GetExtension(string fileName) => Path.GetExtension(fileName);

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public FileOperationsProvider() { }

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public string CurrentDirectory { get => Environment.CurrentDirectory; set => Environment.CurrentDirectory = value; }

    [MirrorVerified("PtcIOFileOperationsProviderMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }
}
