#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    ICollection^ CollectionUtils::Concat(ICollection^ e1, ICollection^ e2)
    {
        if (e1 && e2)
        {
            ArrayList^ arrayList = gcnew ArrayList();
            for each (Object^ item in e1)
                arrayList->Add(item);
            for each (Object^ item in e2)
                arrayList->Add(item);
            return arrayList;
        }
        return e1 ? e1 : e2;
    }

    ArrayList^ CollectionUtils::Copy(ICollection^ c)
    {
        ArrayList^ arrayList = gcnew ArrayList();
        for each (Object^ item in c)
            arrayList->Add(item);
        return arrayList;
    }

    Hashtable^ CollectionUtils::CopyDict(IDictionary^ d)
    {
        ICollection^ keys = d->Keys;
        Hashtable^ hashtable = gcnew Hashtable(keys->Count);
        for each (Object^ item in keys)
            hashtable[item] = d[item];
        return hashtable;
    }

}}
