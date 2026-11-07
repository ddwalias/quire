#nullable disable
namespace Quire.Controls.Core;

public partial interface ISpellCheckRegion
{
    bool CanAddToDictionary();
    string GetWordToAddToDictionary();
    void RefreshDictionary();
    void DisableSpellCheck();
}
