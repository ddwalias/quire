#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#pragma warning(disable: 4100)

namespace mslisp {
    namespace Error {
        __declspec(noinline) void report(System::String^ msg)
        {
            throw gcnew System::Exception(msg);
        }

        __declspec(noinline) void report(System::String^ msg, System::Object^ o)
        {
            report(System::String::Concat(msg, o));
        }
    }

    bool is_true(System::Object^ o)
    {
        if (o == nullptr)
        {
            return false;
        }
        bool^ b = dynamic_cast<bool^>(o);
        if (b != nullptr)
        {
            return *b;
        }
        return true;
    }

    array<System::Reflection::MethodInfo^>^ FindMethods(System::Type^ t, System::String^ name, short arity)
    {
        array<System::Reflection::MethodInfo^>^ methods = t->GetMethods(
            (System::Reflection::BindingFlags)65884);
        System::Collections::Generic::List<System::Reflection::MethodInfo^>^ list =
            gcnew System::Collections::Generic::List<System::Reflection::MethodInfo^>(methods->Length);
        for (short i = 0; i < methods->Length; i++)
        {
            array<System::Reflection::ParameterInfo^>^ parameters = methods[i]->GetParameters();
            if (name->Equals(methods[i]->Name))
            {
                int num = methods[i]->IsStatic ? arity : (arity - 1);
                if (arity == -1 || parameters->Length == (short)num)
                {
                    list->Add(methods[i]);
                }
            }
        }
        return list->ToArray();
    }
}

namespace Quire { namespace mslisp {

    [Quire::Mirror::MirrorOfAttribute("mslisp.Stream, mslisp")]
    public interface class Stream
    {
        System::String^ Write(System::String^ s);
        System::String^ WriteLine(System::String^ s);
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.ConsoleStream, mslisp")]
    public ref class ConsoleStream : Stream
    {
    public:
        ConsoleStream()
        {
        }

        virtual System::String^ Write(System::String^ s)
        {
            System::Console::Write(s);
            return s;
        }

        virtual System::String^ WriteLine(System::String^ s)
        {
            System::Console::WriteLine(s);
            return s;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.DebugStream, mslisp")]
    public ref class DebugStream : Stream
    {
    public:
        DebugStream()
        {
        }

        virtual System::String^ Write(System::String^ s)
        {
            System::Diagnostics::Trace::Write(s);
            return s;
        }

        virtual System::String^ WriteLine(System::String^ s)
        {
            System::Diagnostics::Trace::WriteLine(s);
            return s;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.FileStream, mslisp")]
    public ref class FileStream : Stream
    {
    private:
        static System::Collections::Generic::Dictionary<System::String^, Quire::utl_mgd::SafeMutex^>^ s_mutex_map =
            gcnew System::Collections::Generic::Dictionary<System::String^, Quire::utl_mgd::SafeMutex^>();
        static Quire::utl_mgd::SafeMutex^ s_map_mutex =
            gcnew Quire::utl_mgd::SafeMutex("FileStream");
        System::String^ m_file;
        Quire::utl_mgd::SafeMutex^ m_mutex;

        static Quire::utl_mgd::SafeMutex^ GetMutex(System::String^ path)
        {
            s_map_mutex->WaitOne();
            try
            {
                if (s_mutex_map->ContainsKey(path))
                {
                    return s_mutex_map[path];
                }
                Quire::utl_mgd::SafeMutex^ safeMutex = gcnew Quire::utl_mgd::SafeMutex(path, 1);
                s_mutex_map[path] = safeMutex;
                return safeMutex;
            }
            finally
            {
                s_map_mutex->ReleaseMutex();
            }
        }

        static bool TryDelete(System::String^ f)
        {
            Quire::utl_mgd::SafeMutex^ mutex = GetMutex(f);
            mutex->WaitOne();
            try
            {
                System::IO::File::Delete(f);
                return true;
            }
            catch (System::Exception^)
            {
            }
            finally
            {
                mutex->ReleaseMutex();
            }
            return false;
        }

    public:
        FileStream(System::String^ f)
        {
            System::String^ text = f;
            int num = 0;
            if (!TryDelete(f))
            {
                do
                {
                    num++;
                    System::ValueType^ valueType = num;
                    text = System::String::Concat(System::String::Concat(f, "."), valueType);
                }
                while (!TryDelete(text));
            }
            m_file = text;
            m_mutex = GetMutex(text);
            System::GC::KeepAlive(this);
        }

        virtual System::String^ Write(System::String^ s)
        {
            try
            {
                m_mutex->WaitOne();
                System::IO::StreamWriter^ streamWriter = System::IO::File::AppendText(m_file);
                streamWriter->Write(s);
                streamWriter->Close();
                return s;
            }
            finally
            {
                m_mutex->ReleaseMutex();
            }
        }

        virtual System::String^ WriteLine(System::String^ s)
        {
            try
            {
                m_mutex->WaitOne();
                System::IO::StreamWriter^ streamWriter = System::IO::File::AppendText(m_file);
                streamWriter->WriteLine(s);
                streamWriter->Close();
                return s;
            }
            finally
            {
                m_mutex->ReleaseMutex();
            }
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Condition, mslisp")]
    public ref class Condition : System::Exception
    {
    public:
        array<System::Object^>^ args;

        Condition(array<System::Object^>^ a) : args(a)
        {
        }

        Condition(System::String^ msg)
        {
            array<System::Object^>^ arr = gcnew array<System::Object^>(1);
            arr[0] = msg;
            args = arr;
        }

        Condition(System::String^ msg, System::Object^ handle)
        {
            array<System::Object^>^ arr = gcnew array<System::Object^>(2);
            arr[0] = msg;
            arr[1] = handle;
            args = arr;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func0, mslisp")]
    delegate System::Object^ Func0();

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func1, mslisp")]
    delegate System::Object^ Func1(System::Object^ A_0);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func2, mslisp")]
    delegate System::Object^ Func2(System::Object^ A_0, System::Object^ A_1);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func3, mslisp")]
    delegate System::Object^ Func3(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func4, mslisp")]
    delegate System::Object^ Func4(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2, System::Object^ A_3);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func5, mslisp")]
    delegate System::Object^ Func5(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2, System::Object^ A_3, System::Object^ A_4);

    [Quire::Mirror::MirrorOfAttribute("mslisp.Func6, mslisp")]
    delegate System::Object^ Func6(System::Object^ A_0, System::Object^ A_1, System::Object^ A_2, System::Object^ A_3, System::Object^ A_4, System::Object^ A_5);

    [Quire::Mirror::MirrorOfAttribute("mslisp.BuiltinDelegate, mslisp")]
    delegate System::Object^ BuiltinDelegate(array<System::Object^>^ args);

    [Quire::Mirror::MirrorOfAttribute("mslisp.GenCall, mslisp")]
    interface class GenCall
    {
        bool Emit(System::Reflection::Emit::ILGenerator^ g, short arity);
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Symbol, mslisp")]
    public interface class Symbol
    {
        System::String^ get_name();
        System::String^ get_fullname();
        System::String^ get_package();
        System::Object^ get_value();
        System::Object^ set_value(System::Object^ o);
        bool not_set();

        property Symbol^ Base
        {
            Symbol^ get();
        }

        property Symbol^ Tag
        {
            Symbol^ get();
        }

        property Quire::utl_mgd::LabelSet^ Labels
        {
            Quire::utl_mgd::LabelSet^ get();
        }

        property bool Wild
        {
            bool get();
        }

        Symbol^ WithWild(bool bWild);
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.TaggedSymbol, mslisp")]
    ref class TaggedSymbol : Symbol
    {
    private:
        Symbol^ m_base;
        Symbol^ m_tag;
        Quire::utl_mgd::LabelSet^ m_labels;

    public:
        TaggedSymbol(Symbol^ base, Symbol^ tag, Quire::utl_mgd::LabelSet^ labels)
            : m_base(base), m_tag(tag), m_labels(labels)
        {
        }

        virtual System::String^ ToString() override
        {
            System::String^ text = m_base->ToString();
            if (m_labels != nullptr && !m_labels->Empty())
            {
                text = System::String::Concat(text, m_labels->ToString());
            }
            if (m_tag != nullptr)
            {
                System::String^ text2 = m_tag->ToString();
                System::String^ text3 = ":";
                text = System::String::Concat(text, System::String::Concat(text3, text2));
            }
            System::GC::KeepAlive(this);
            return text;
        }

        virtual System::String^ get_name()
        {
            System::String^ name = m_base->get_name();
            System::GC::KeepAlive(this);
            return name;
        }

        virtual System::String^ get_fullname()
        {
            System::String^ fullname = m_base->get_fullname();
            System::GC::KeepAlive(this);
            return fullname;
        }

        virtual System::String^ get_package()
        {
            System::String^ package = m_base->get_package();
            System::GC::KeepAlive(this);
            return package;
        }

        virtual System::Object^ get_value()
        {
            System::Object^ value = m_base->get_value();
            System::GC::KeepAlive(this);
            return value;
        }

        virtual System::Object^ set_value(System::Object^ o)
        {
            System::Object^ result = m_base->set_value(o);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual bool not_set()
        {
            bool result = m_base->not_set();
            System::GC::KeepAlive(this);
            return result;
        }

        property Symbol^ Base
        {
            virtual Symbol^ get()
            {
                return m_base;
            }
        }

        property Symbol^ Tag
        {
            virtual Symbol^ get()
            {
                return m_tag;
            }
        }

        property Quire::utl_mgd::LabelSet^ Labels
        {
            virtual Quire::utl_mgd::LabelSet^ get()
            {
                return m_labels;
            }
        }

        property bool Wild
        {
            virtual bool get()
            {
                bool b = (m_labels != nullptr && m_labels->Wild());
                System::GC::KeepAlive(this);
                return b;
            }
        }

        virtual Symbol^ WithWild(bool bWild)
        {
            if (bWild != Wild)
            {
                TaggedSymbol^ result = gcnew TaggedSymbol(Base, Tag, Quire::utl_mgd::LabelTrie::WithWild(Labels, bWild));
                System::GC::KeepAlive(this);
                return result;
            }
            System::GC::KeepAlive(this);
            return this;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SymbolImp, mslisp")]
    ref class SymbolImp : Symbol, System::Runtime::Serialization::ISerializable
    {
    private:
        System::String^ m_name;
        System::String^ m_package;
        System::Object^ m_value;
        System::String^ m_fullname;
        bool m_not_set;

    public:
        property bool Wild
        {
            virtual bool get()
            {
                return false;
            }
        }

        property Quire::utl_mgd::LabelSet^ Labels
        {
            virtual Quire::utl_mgd::LabelSet^ get()
            {
                return nullptr;
            }
        }

        property Symbol^ Tag
        {
            virtual Symbol^ get()
            {
                return nullptr;
            }
        }

        property Symbol^ Base
        {
            virtual Symbol^ get()
            {
                return this;
            }
        }

        SymbolImp(System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context)
        {
            m_name = info->GetString("m_name");
            System::String^ sPkg = info->GetString("m_package");
            m_package = sPkg;
            array<System::Object^>^ arr = gcnew array<System::Object^>(2);
            arr[0] = sPkg;
            arr[1] = m_name;
            m_fullname = System::String::Format("{0}.{1}", arr);
            if (info->GetBoolean("is_value"))
            {
                System::Type^ typeFromHandle = System::Object::typeid;
                System::String^ name = "m_value";
                m_value = info->GetValue(name, typeFromHandle);
            }
            m_not_set = info->GetBoolean("m_not_set");
            System::GC::KeepAlive(this);
        }

        SymbolImp(System::String^ package, System::String^ name)
            : m_name(name), m_package(package), m_not_set(true)
        {
            array<System::Object^>^ arr = gcnew array<System::Object^>(2);
            arr[0] = m_package;
            arr[1] = m_name;
            m_fullname = System::String::Format("{0}.{1}", arr);
            System::GC::KeepAlive(this);
        }

        virtual void GetObjectData(System::Runtime::Serialization::SerializationInfo^ info, System::Runtime::Serialization::StreamingContext context)
        {
            info->AddValue("m_name", (System::Object^)m_name);
            info->AddValue("m_package", (System::Object^)m_package);
            if (m_value != nullptr)
            {
                if (m_value->GetType()->IsSerializable)
                {
                    info->AddValue("is_value", true);
                    info->AddValue("m_value", m_value);
                }
                else
                {
                    info->AddValue("is_value", false);
                    m_not_set = true;
                }
            }
            else
            {
                info->AddValue("is_value", false);
            }
            info->AddValue("m_not_set", m_not_set);
            System::GC::KeepAlive(this);
        }

        virtual System::String^ ToString() override
        {
            return m_name;
        }

        virtual System::String^ get_name()
        {
            return m_name;
        }

        virtual System::String^ get_fullname()
        {
            return m_fullname;
        }

        virtual System::String^ get_package()
        {
            return m_package;
        }

        virtual System::Object^ get_value()
        {
            return m_value;
        }

        virtual System::Object^ set_value(System::Object^ o)
        {
            m_not_set = false;
            m_value = o;
            return o;
        }

        virtual bool not_set()
        {
            return m_not_set;
        }

        virtual Symbol^ WithWild(bool bWild)
        {
            if (bWild != Wild)
            {
                TaggedSymbol^ result = gcnew TaggedSymbol(this, nullptr, Quire::utl_mgd::LabelTrie::WithWild(Quire::utl_mgd::LabelTrie::Empty(), bWild));
                System::GC::KeepAlive(this);
                return result;
            }
            System::GC::KeepAlive(this);
            return this;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.Package, mslisp")]
    ref class Package
    {
    private:
        System::String^ m_name;
        System::Collections::Generic::Dictionary<System::String^, Symbol^>^ m_internal;
        System::Collections::Generic::Dictionary<System::String^, Symbol^>^ m_external;

    public:
        Package(System::String^ name)
            : m_name(name),
              m_internal(gcnew System::Collections::Generic::Dictionary<System::String^, Symbol^>()),
              m_external(gcnew System::Collections::Generic::Dictionary<System::String^, Symbol^>())
        {
        }

        Symbol^ intern(System::String^ name)
        {
            Symbol^ sym = nullptr;
            if (m_external->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            if (m_internal->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            sym = gcnew SymbolImp(m_name, name);
            m_internal->Add(name, sym);
            System::GC::KeepAlive(this);
            return sym;
        }

        Symbol^ _export(Symbol^ s)
        {
            if (m_external->ContainsKey(s->get_name()))
            {
                m_external->Remove(s->get_name());
            }
            m_external->Add(s->get_name(), s);
            System::GC::KeepAlive(this);
            return s;
        }

        Symbol^ _export(System::String^ name)
        {
            Symbol^ sym = nullptr;
            if (m_external->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            sym = nullptr;
            if (m_internal->TryGetValue(name, sym))
            {
                m_internal->Remove(name);
            }
            else
            {
                sym = gcnew SymbolImp(m_name, name);
            }
            m_external->Add(name, sym);
            System::GC::KeepAlive(this);
            return sym;
        }

        Symbol^ find_export(System::String^ name)
        {
            Symbol^ sym = nullptr;
            if (m_external->TryGetValue(name, sym))
            {
                System::GC::KeepAlive(this);
                return sym;
            }
            return nullptr;
        }

        System::String^ get_name()
        {
            return m_name;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp._fcomparer, mslisp")]
    ref class _fcomparer : System::Collections::Generic::IComparer<System::Object^>
    {
    public:
        Quire::utl_mgd::Function^ f;

        _fcomparer(Quire::utl_mgd::Function^ _f) : f(_f)
        {
        }

        virtual int Compare(System::Object^ x, System::Object^ y)
        {
            System::Object^ obj = f->Invoke2(x, y);
            int^ val = dynamic_cast<int^>(obj);
            System::Diagnostics::Trace::Assert(val != nullptr, nullptr);
            System::GC::KeepAlive(this);
            return *val;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.InterpreterState, mslisp")]
    value struct InterpreterState
    {
    public:
        Package^ package;
        System::Collections::Generic::Stack<Package^>^ pstack;

        InterpreterState(Package^ p, System::Collections::Generic::Stack<Package^>^ ps)
            : package(p), pstack(ps)
        {
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.Env, mslisp")]
    ref class Env
    {
    public:
        Env(Env^ parent, array<System::Object^>^ vals)
            : m_parent(parent), m_vals(vals)
        {
        }

    private:
        Env^ m_parent;
        array<System::Object^>^ m_vals;
        int m_size;
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Expression, mslisp")]
    interface class Expression
    {
        System::Object^ execute(Env^ e);
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.Settable, mslisp")]
    interface class Settable
    {
        System::Object^ set(System::Object^ o, Env^ e);
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.StrcatFunction, mslisp")]
    ref class StrcatFunction : Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::String^ result = System::String::Concat(args);
            System::GC::KeepAlive(this);
            return result;
        }

        StrcatFunction()
        {
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SignalFunction, mslisp")]
    ref class SignalFunction : Quire::utl_mgd::FunctionBase
    {
    public:
        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            if (args->Length == 1)
            {
                Condition^ condition = dynamic_cast<Condition^>(args[0]);
                if (condition != nullptr)
                {
                    throw condition;
                }
            }
            throw gcnew Condition(args);
        }

        SignalFunction()
        {
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.SingleMethod, mslisp")]
    ref class SingleMethod : Quire::utl_mgd::FunctionBase
    {
    private:
        System::Reflection::MethodInfo^ m_info;

    public:
        SingleMethod(System::Reflection::MethodInfo^ m) : m_info(m)
        {
        }

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Object^ result = m_info->Invoke(nullptr, args);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.SingleMethod: ", m_info);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.DelegateFunction, mslisp")]
    ref class DelegateFunction : Quire::utl_mgd::FunctionBase
    {
    private:
        BuiltinDelegate^ m_delegate;

    public:
        DelegateFunction(BuiltinDelegate^ d) : m_delegate(d)
        {
        }

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Object^ result = m_delegate(args);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.DelegateFunction: ", m_delegate);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [Quire::Mirror::MirrorOfAttribute("mslisp.CodeGen, mslisp")]
    ref class CodeGen
    {
    public:
        static array<System::Type^>^ s_dtype = gcnew array<System::Type^>(7);

        static void EmitLdarg(System::Reflection::Emit::ILGenerator^ g, short i)
        {
            switch (i)
            {
            case 0:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_0);
                break;
            case 1:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_1);
                break;
            case 2:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_2);
                break;
            case 3:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg_3);
                break;
            default:
                g->Emit(System::Reflection::Emit::OpCodes::Ldarg, i);
                break;
            }
        }

        static void EmitLdargs(System::Reflection::Emit::ILGenerator^ g, short offset, short arity, array<System::Reflection::ParameterInfo^>^ p)
        {
            short num = offset;
            if (offset < arity)
            {
                do
                {
                    EmitLdarg(g, num);
                    if (p[num - offset]->ParameterType->IsValueType)
                    {
                        System::Type^ parameterType = p[num - offset]->ParameterType;
                        System::Reflection::Emit::OpCode unbox_Any = System::Reflection::Emit::OpCodes::Unbox_Any;
                        g->Emit(unbox_Any, parameterType);
                    }
                    num++;
                }
                while (num < arity);
            }
        }

        static void MaybeBox(System::Reflection::Emit::ILGenerator^ g, System::Type^ t)
        {
            if (t == System::Void::typeid)
            {
                g->Emit(System::Reflection::Emit::OpCodes::Ldnull);
            }
            else if (t->IsValueType)
            {
                g->Emit(System::Reflection::Emit::OpCodes::Box, t);
            }
        }

        static System::Object^ GenDelegate(System::Object^ f, int arity)
        {
            try
            {
                GenCall^ genCall = dynamic_cast<GenCall^>(f);
                if (genCall == nullptr)
                {
                    return nullptr;
                }
                array<System::Type^>^ arrayType = gcnew array<System::Type^>(arity);
                for (short num = 0; num < arity; num++)
                {
                    System::Type^ typeFromHandle = System::Object::typeid;
                    arrayType[num] = typeFromHandle;
                }
                System::Reflection::Emit::DynamicMethod^ dynamicMethod = gcnew System::Reflection::Emit::DynamicMethod(
                    f->ToString(),
                    System::Object::typeid,
                    arrayType,
                    CodeGen::typeid);
                System::Reflection::Emit::ILGenerator^ iLGenerator = dynamicMethod->GetILGenerator();
                if (!genCall->Emit(iLGenerator, (short)arity))
                {
                    return nullptr;
                }
                return dynamicMethod->CreateDelegate(s_dtype[arity]);
            }
            catch (System::Exception^)
            {
                return nullptr;
            }
        }

        CodeGen()
        {
        }

    private:
        static CodeGen()
        {
            s_dtype[0] = Func0::typeid;
            s_dtype[1] = Func1::typeid;
            s_dtype[2] = Func2::typeid;
            s_dtype[3] = Func3::typeid;
            s_dtype[4] = Func4::typeid;
            s_dtype[5] = Func5::typeid;
            s_dtype[6] = Func6::typeid;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.Constructor, mslisp")]
    ref class Constructor : Quire::utl_mgd::FunctionBase, GenCall
    {
    private:
        System::Type^ m_type;

    public:
        Constructor(System::Type^ t) : m_type(t)
        {
        }

        virtual System::Object^ Invoke(array<System::Object^>^ args) override
        {
            System::Object^ result = System::Activator::CreateInstance(m_type, args);
            System::GC::KeepAlive(this);
            return result;
        }

        virtual bool Emit(System::Reflection::Emit::ILGenerator^ g, short arity)
        {
            array<System::Reflection::ConstructorInfo^>^ constructors = m_type->GetConstructors();
            short num = 0;
            short num2 = -1;
            short num3 = 0;
            if (0 < constructors->Length)
            {
                do
                {
                    if (constructors[num3]->GetParameters()->Length == arity)
                    {
                        num++;
                        num2 = num3;
                    }
                    num3++;
                }
                while (num3 < constructors->Length);
                if (num == 1)
                {
                    CodeGen::EmitLdargs(g, 0, arity, constructors[num2]->GetParameters());
                    System::Reflection::ConstructorInfo^ con = constructors[num2];
                    g->Emit(System::Reflection::Emit::OpCodes::Newobj, con);
                    g->Emit(System::Reflection::Emit::OpCodes::Ret);
                    System::GC::KeepAlive(this);
                    return true;
                }
            }
            System::GC::KeepAlive(this);
            return false;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Concat("mslisp.Constructor: ", m_type);
            System::GC::KeepAlive(this);
            return result;
        }
    };

    [System::SerializableAttribute]
    [Quire::Mirror::MirrorOfAttribute("mslisp.QuoteExpr, mslisp")]
    ref class QuoteExpr : Expression
    {
    private:
        System::Object^ m_val;

    public:
        QuoteExpr(System::Object^ o) : m_val(o)
        {
        }

        virtual System::Object^ execute(Env^ e)
        {
            return m_val;
        }

        virtual System::String^ ToString() override
        {
            System::String^ result = System::String::Format("(quote {0})", m_val);
            System::GC::KeepAlive(this);
            return result;
        }
    };

}}
