namespace Quire;

public partial class TabManager
{
    private bool IsIndexInCollectionRange(int targetId)
    {
        return 0 <= targetId && targetId < ItemsCollection.Count;
    }
}
