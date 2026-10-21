#include <cstdint>

#pragma managed(push, off)
extern "C" __declspec(dllexport) __declspec(noinline) int __cdecl NativeAdd(int left, int right)
{
    return left + right;
}
#pragma managed(pop)

using namespace System;

namespace UtlMixedModeProof
{
    public ref class NativeBridge abstract sealed
    {
    public:
        static int Add(int left, int right)
        {
            return NativeAdd(left, right);
        }
    };
}
