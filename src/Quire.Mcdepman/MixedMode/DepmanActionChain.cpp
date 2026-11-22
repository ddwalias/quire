#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include "DepmanPosition.h"

// Defined in its own translation unit so that PositionBase::IsLocked calls it, as the original does.
#pragma comment(linker, "/include:?IsLocked@ActionChain@depman@@QEBA_NXZ")

namespace depman {

bool ActionChain::IsLocked() const
{
    return locked_;
}

} // namespace depman
