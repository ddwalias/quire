#pragma once

using namespace System;
using namespace System::Collections;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

namespace Quire { namespace Engine {

    [Quire::Mirror::MirrorOfAttribute("Engine.Tags, engutils-mgd")]
    [StructLayout(LayoutKind::Sequential, Size = 1)]
    public value struct Tags
    {
    public:
        static String^ StripTag(String^ s);
        static String^ GetTag(String^ s);
        static bool EqualsNoTags(Object^ x, Object^ y);
        static String^ StripFontTag(String^ s);
        static String^ EncodeRegionID(int id, String^ tag);

    private:
        static wchar_t to_hex(wchar_t c);
        static wchar_t to_hex(int d);
        static bool is_hex(wchar_t c);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.StringUtils, engutils-mgd")]
    [StructLayout(LayoutKind::Sequential, Size = 1)]
    public value struct StringUtils
    {
    public:
        static int^ ParseInt(String^ s);
        static String^ MangleForXAML(String^ s);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.CollectionUtils, engutils-mgd")]
    public ref class CollectionUtils
    {
    public:
        static ICollection^ Concat(ICollection^ e1, ICollection^ e2);
        static ArrayList^ Copy(ICollection^ c);
        static Hashtable^ CopyDict(IDictionary^ d);
        CollectionUtils() {}
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.Reflection, engutils-mgd")]
    [StructLayout(LayoutKind::Sequential, Size = 1)]
    public value struct Reflection
    {
    public:
        static Type^ FindType(String^ tname);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.CircTracker, engutils-mgd")]
    ref class CircTracker
    {
    private:
        System::Collections::Generic::Dictionary<Object^, String^>^ seen = gcnew System::Collections::Generic::Dictionary<Object^, String^>();
        System::Collections::Generic::Dictionary<String^, int>^ counts = gcnew System::Collections::Generic::Dictionary<String^, int>();

    public:
        CircTracker() {}
        String^ Seen(Object^ o);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.UnwindProtect, engutils-mgd")]
    [StructLayout(LayoutKind::Sequential, Size = 1)]
    public value struct UnwindProtect
    {
    private:
        static UnwindProtect();
    public:
        static array<Object^>^ s_args = gcnew array<Object^>(1);
        static Object^ Evaluate(Quire::Mpl::Run::Function^ expr, Quire::Mpl::Run::Function^ cleanup);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.NormalizeComponentExceptions, engutils-mgd")]
    [StructLayout(LayoutKind::Sequential, Size = 1)]
    public value struct NormalizeComponentExceptions
    {
    private:
        static NormalizeComponentExceptions();
    public:
        static array<Object^>^ s_args = gcnew array<Object^>(1);
        static Object^ Evaluate(Quire::Mpl::Run::Function^ expr);
        static Exception^ Unwrap(Exception^ e);
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.UnmanagedMemoryStream, engutils-mgd")]
    public ref class UnmanagedMemoryStream : Stream
    {
    private:
        // Int32::MaxValue prevents C++ constant-folding so Seek emits ldsfld MemStreamMaxLength.
        static const long MemStreamMaxLength = Int32::MaxValue;
        unsigned char* _mem;
        int _length;
        int _capacity;
        int _position;
        bool _writable;
        bool _isOpen;

    public:
        UnmanagedMemoryStream(unsigned char* memory, int length, int capacity, bool canWrite);
        virtual property bool CanRead { bool get() override; }
        virtual property bool CanSeek { bool get() override; }
        virtual property bool CanWrite { bool get() override; }
        virtual void Close() override;
        virtual void Flush() override;
        virtual property long long Length { long long get() override; }
        virtual property long long Position { long long get() override; void set(long long value) override; }
        unsigned char* GetBytePtr();
        virtual int Read(array<unsigned char>^ buffer, int offset, int count) override;
        virtual int ReadByte() override;
        virtual long long Seek(long long offset, SeekOrigin loc) override;
        virtual void SetLength(long long value) override;
        virtual void Write(array<unsigned char>^ buffer, int offset, int count) override;
        virtual void WriteByte(unsigned char value) override;
        static void memcpy(array<unsigned char>^ src, int srcIndex, unsigned char* dest, int destIndex, int len);
        static void memcpy(unsigned char* src, int srcIndex, array<unsigned char>^ dest, int destIndex, int len);
    };

}}

namespace Quire { namespace Native { namespace EngutilsMgd {

    [Quire::Mirror::MirrorOfAttribute("PathCache, engutils-mgd")]
    [Serializable]
    public ref class PathCache
    {
    private:
        ArrayList^ m_paths = gcnew ArrayList();
        Hashtable^ m_pathset = gcnew Hashtable();

        String^ AddPath(String^ path);
        String^ AddParentDir(String^ path);

    public:
        PathCache() {}
        PathCache^ AddPaths(String^ s);
        String^ FindFile(String^ name);
        virtual String^ ToString() override;
    };

    [Quire::Mirror::MirrorOfAttribute("PushbackReader, engutils-mgd")]
    public ref class PushbackReader
    {
    private:
        Stack^ m_stack = gcnew Stack();
        TextReader^ m_reader;

    public:
        PushbackReader(TextReader^ r) : m_reader(r) {}
        void Unread(int c);
        int Read();
    };

}}}
