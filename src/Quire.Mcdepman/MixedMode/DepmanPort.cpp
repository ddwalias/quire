#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>

namespace Quire { namespace depman {

    [Quire::Mirror::MirrorOfAttribute("depman.IDepman, mcdepman")]
    public interface class IDepman : System::IDisposable
    {
        void Remove(long h);
        void SetAutoCalc(bool b);
    };
}}
