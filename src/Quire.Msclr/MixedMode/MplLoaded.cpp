#include "Msclr.h"

namespace Mpl {

    Loaded* GetModuleState(const Hex::Module& module)
    {
        return static_cast<Loaded*>(module.GetState());
    }
}
