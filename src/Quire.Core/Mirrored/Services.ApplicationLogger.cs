#nullable disable
#pragma warning disable

namespace Quire.Services;

partial class ApplicationLogger
{
    public string SubProduct { get => _subProduct; set { _subProduct = value; FullProductName = string.IsNullOrEmpty(value) ? _product : string.Format("{0}_{1}", _product, value); } }
    protected string FullProductName { get => _fullProductName; private set => _fullProductName = string.IsNullOrEmpty(value) ? _product : value; }
    public string LogFolder { get => _logFolder; set => _logFolder = string.IsNullOrEmpty(value) ? _defaultApplicationLogFolderPath : value; }
}
