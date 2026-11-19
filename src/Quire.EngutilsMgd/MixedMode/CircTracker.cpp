#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    String^ CircTracker::Seen(Object^ o)
    {
        if (seen->ContainsKey(o))
        {
            String^ result = seen[o];
            GC::KeepAlive(this);
            return result;
        }

        String^ text = o->GetType()->ToString();
        if (counts->ContainsKey(text))
            counts[text] = counts[text] + 1;
        else
            counts[text] = 0;

        String^ text2 = ">";
        ValueType^ valueType = counts[text];
        String^ text3 = " ";
        String^ value2 = String::Concat(String::Concat(String::Concat("<" + text, text3), valueType), text2);
        seen[o] = value2;
        GC::KeepAlive(this);
        return nullptr;
    }

}}
