#nullable disable
#pragma warning disable

namespace Quire.Services;

partial class ApplicationVariant
{
    public static global::System.Nullable<bool> IsEngNbPlus
    {
        get => _isEngNbPlus;
        private set
        {
            global::System.Nullable<bool> current = _isEngNbPlus;
            if (!current.HasValue)
            {
                _isEngNbPlus = value;
            }
        }
    }
    public static global::System.Nullable<bool> IsEngNb
    {
        get => _isEngNb;
        private set
        {
            global::System.Nullable<bool> current = _isEngNb;
            if (!current.HasValue)
            {
                _isEngNb = value;
            }
        }
    }
}
