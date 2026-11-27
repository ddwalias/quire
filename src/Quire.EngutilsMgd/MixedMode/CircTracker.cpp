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

        String^ closingBracket = ">";
        ValueType^ valueType = counts[text];
        String^ countSeparator = " ";
        String^ description = String::Concat(String::Concat(String::Concat("<" + text, countSeparator), valueType), closingBracket);
        seen[o] = description;
        GC::KeepAlive(this);
        return nullptr;
    }

}}
