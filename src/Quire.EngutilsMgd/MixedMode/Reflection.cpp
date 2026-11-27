#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    Type^ Reflection::FindType(String^ tname)
    {
        Type^ type = Type::GetType(tname);
        if ((Object^)type != nullptr)
            return type;

        array<Assembly^>^ assemblies = AppDomain::CurrentDomain->GetAssemblies();
        for (int i = 0; i < assemblies->Length; i++)
        {
            Type^ assemblyType = assemblies[i]->GetType(tname);
            if ((Object^)assemblyType != nullptr)
                return assemblyType;
        }
        return nullptr;
    }

}}
