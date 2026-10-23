#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>

using namespace System;

namespace Quire { namespace Mpl {

    [Quire::Mirror::MirrorOfAttribute("Mpl.IRegion, msclr")]
    public ref class IRegion abstract
    {
    public:
        [Quire::Mirror::MirrorOfAttribute("Mpl.IRegion+Symbol, msclr")]
        value struct Symbol
        {
            String^ name;
            String^ type;
            UInt64 ordinal;
        };

        virtual array<Symbol>^ GetImports() = 0;
        virtual array<Symbol>^ GetExports() = 0;
        virtual void Evaluate(array<Object^>^ i, array<Object^>^ e) = 0;
    };
}}
