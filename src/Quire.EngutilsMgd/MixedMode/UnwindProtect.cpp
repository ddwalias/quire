#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    UnwindProtect::UnwindProtect()
    {
        s_args[0] = nullptr;
    }

    Object^ UnwindProtect::Evaluate(Quire::Mpl::Run::Function^ expr, Quire::Mpl::Run::Function^ cleanup)
    {
        try
        {
            return expr->Call(s_args);
        }
        finally
        {
            try
            {
                cleanup->Call(s_args);
            }
            catch (Exception^)
            {
                cleanup->Call(s_args);
            }
        }
        return nullptr;
    }

}}
