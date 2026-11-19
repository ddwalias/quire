#include "EngutilsMgd.h"

namespace Quire { namespace Native { namespace EngutilsMgd {

    String^ PathCache::AddPath(String^ path)
    {
        if (!m_pathset->ContainsKey(path))
        {
            m_paths->Add(path);
            m_pathset[path] = true;
        }
        GC::KeepAlive(this);
        return path;
    }

    String^ PathCache::AddParentDir(String^ path)
    {
        AddPath((gcnew DirectoryInfo(path))->Parent->FullName);
        GC::KeepAlive(this);
        return path;
    }

    PathCache^ PathCache::AddPaths(String^ s)
    {
        int num = s->IndexOf(Path::PathSeparator);
        if (num != -1)
        {
            do
            {
                AddPath(s->Substring(0, num));
                s = s->Substring(num + 1);
                num = s->IndexOf(Path::PathSeparator);
            } while (num != -1);
        }
        AddPath(s);
        return this;
    }

    String^ PathCache::FindFile(String^ name)
    {
        if (Path::IsPathRooted(name))
        {
            GC::KeepAlive(this);
            AddPath((gcnew DirectoryInfo(name))->Parent->FullName);
            GC::KeepAlive(this);
            return name;
        }

        IEnumerator^ enumerator = m_paths->GetEnumerator();
        try
        {
            while (enumerator->MoveNext())
            {
                String^ text = Path::Combine((String^)enumerator->Current, name);
                if (File::Exists(text))
                {
                    return text;
                }
            }
        }
        finally
        {
            IDisposable^ disposable = dynamic_cast<IDisposable^>(enumerator);
            delete disposable;
        }
        GC::KeepAlive(this);
        return nullptr;
    }

    String^ PathCache::ToString()
    {
        String^ result = m_paths->ToString();
        GC::KeepAlive(this);
        return result;
    }

}}}
