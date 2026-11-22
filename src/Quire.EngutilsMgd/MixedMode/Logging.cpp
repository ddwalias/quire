#include "EngutilsMgd.h"
#include "EngutilsNative.h"
#include <iostream>
#include <ostream>

namespace utl
{
    __declspec(dllimport) std::ostream& __cdecl Log(Substring<const char> name);
}

namespace Engine
{
    __declspec(noinline) String^ GetString(Object^ o, Quire::Engine::CircTracker^ seen)
    {
        String^ s = dynamic_cast<String^>(o);
        if ((Object^)s != nullptr)
        {
            return s;
        }
        Type^ t = o->GetType();
        if (t->IsValueType || t->IsPrimitive)
        {
            return o->ToString();
        }
        return seen->Seen(o);
    }
}

namespace Quire { namespace Engine {

    [Quire::Mirror::MirrorOfAttribute("Engine.Logging, engutils-mgd")]
    public ref class Logging
    {
    private:
        static void LogProperties(Object^ o, std::ostream& out, int level, CircTracker^ seen)
        {
            String^ text = ::Engine::GetString(o, seen);
            if ((Object^)text != nullptr)
            {
                if (text->Length > 0)
                {
                    out << utl::ToStrng(text);
                }
                out << std::endl;
                return;
            }
            out << std::endl;
            for (int i = 0; i < level; i++)
            {
                out << " ";
            }
            out << utl::ToStrng(seen->Seen(o)) << ": " << std::endl;
            array<PropertyInfo^>^ props = o->GetType()->GetProperties(
                BindingFlags::Instance | BindingFlags::Static | BindingFlags::Public | BindingFlags::FlattenHierarchy);
            for (int i = 0; i < props->Length; i++)
            {
                array<ParameterInfo^>^ indexParams = props[i]->GetIndexParameters();
                if (indexParams != nullptr && indexParams->Length > 0)
                {
                    continue;
                }
                Object^ val = nullptr;
                if (props[i]->CanRead)
                {
                    try
                    {
                        val = props[i]->GetValue(o, nullptr);
                    }
                    catch (Exception^)
                    {
                        val = "<UNAVAILABLE>";
                    }
                }
                else
                {
                    val = "<UNAVAILABLE>";
                }
                for (int j = 0; j < level; j++)
                {
                    out << " ";
                }
                out << utl::ToStrng(props[i]->Name) << ": ";
                LogProperties(val, out, level + 1, seen);
            }
        }

    public:
        static void LogProperties(Object^ o, String^ logname)
        {
            LogProperties(o, utl::Log(to_string(utl::ToStrng(logname)).c_str()), 0, gcnew CircTracker());
        }

        Logging() {}
    };

}}
