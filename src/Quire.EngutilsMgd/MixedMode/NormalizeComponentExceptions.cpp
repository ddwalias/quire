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
            Exception^ unwrappedException = Unwrap(ex);
            throw (unwrappedException != nullptr) ? unwrappedException : ex;
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
        Quire::Mpl::Run::AppException^ appException = dynamic_cast<Quire::Mpl::Run::AppException^>(e->InnerException);
        if (appException != nullptr && appException->Message != nullptr && appException->Handle != 0)
            return appException;
        return nullptr;
    }

}}
