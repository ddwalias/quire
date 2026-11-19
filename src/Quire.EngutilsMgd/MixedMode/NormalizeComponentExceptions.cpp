#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    NormalizeComponentExceptions::NormalizeComponentExceptions()
    {
        s_args[0] = nullptr;
    }

    Object^ NormalizeComponentExceptions::Evaluate(Quire::Mpl::Run::Function^ expr)
    {
        try
        {
            return expr->Call(s_args);
        }
        catch (Exception^ ex)
        {
            Exception^ ex2 = Unwrap(ex);
            throw (ex2 != nullptr) ? ex2 : ex;
        }
        return nullptr;
    }

    Exception^ NormalizeComponentExceptions::Unwrap(Exception^ e)
    {
        if (e->InnerException != nullptr)
        {
            Exception^ ex = Unwrap(e->InnerException);
            if (ex != nullptr)
                return ex;
        }
        Quire::Mpl::Run::AppException^ ex2 = dynamic_cast<Quire::Mpl::Run::AppException^>(e->InnerException);
        if (ex2 != nullptr && ex2->Message != nullptr && ex2->Handle != 0)
            return ex2;
        return nullptr;
    }

}}
