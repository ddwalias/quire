#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include "DepmanPosition.h"

// Uncalled native member functions are stripped by /OPT:REF unless referenced.
// When referenced by callers in Depman, they will be emitted with exact IL.
// #pragma comment(linker, "/include:?previous@Position@depman@@$$FQEAAPEAV12@XZ")
// #pragma comment(linker, "/include:?next@Position@depman@@$$FQEAAPEAV12@XZ")

namespace depman {

Position* Position::previous()
{
    return dynamic_cast<Position*>(prev_);
}

Position* Position::next()
{
    return dynamic_cast<Position*>(next_);
}

} // namespace depman
